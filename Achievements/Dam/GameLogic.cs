using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using TimberApi.ConsoleSystem;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.Common;
using Timberborn.ConstructibleSystem;
using Timberborn.ConstructionSites;
using Timberborn.EntitySystem;
using Timberborn.Persistence;
using Timberborn.PrefabSystem;
using Timberborn.SingletonSystem;
using UnityEngine;

namespace Yurand.Timberborn.Achievements.Dam
{
    public class GameLogic : SaveableAchievementLogicBase
    {
        private enum DamHeight { One, Two, Three };
        private HashSet<HashSet<Vector3Int>> logical_dams;
        private int biggest_dam;
        public GameLogic(EventBus eventBus, IConsoleWriter console, AchievementManager manager, ISingletonLoader loader)
            : base(eventBus, console, manager, loader)
        {
            this.logical_dams = new HashSet<HashSet<Vector3Int>>();
            this.biggest_dam = 0;
        }

        protected override void NoSingletonLoad() {
            biggest_dam = 0;
        }

        protected override void SingletonLoad(IObjectLoader loader) {
            if (!loader.Has(propertyBiggestDam))
                biggest_dam = 0;
            else
                biggest_dam = loader.Get(propertyBiggestDam);
        }

        protected override void SingletonSave(IObjectSaver saver) {
            saver.Set(propertyBiggestDam, biggest_dam);
        }

        [OnEvent]
        public void OnConstructedEvent(ConstructibleEnteredFinishedStateEvent constructibleEvent) {
            var prefab = constructibleEvent.Constructible.GetComponentFast<Prefab>();
            var block_object = constructibleEvent.Constructible.GetComponentFast<BlockObject>();
            if (prefab is null || block_object is null) return;

            var position = block_object.CoordinatesAtBaseZ;

            if (IsPrefab(prefab, damPrefabSubName) || IsPrefab(prefab, leveePrefabSubName) || IsFloodgate1Prefab(prefab))
                AddBlockInDams(position, DamHeight.One);
            else if (IsPrefab(prefab, floodgate2PrefabSubName))
                AddBlockInDams(position, DamHeight.Two);
            else if (IsPrefab(prefab, floodgate3PrefabSubName))
                AddBlockInDams(position, DamHeight.Three);
            else return;

            biggest_dam = Math.Max(biggest_dam, getBiggestDamSize());
            var updater = new AchievementWithCompletition.Updater(){ next_state = biggest_dam };
            manager.UpdateLocalAchievement(damId, updater);
            manager.UpdateLocalAchievement(realCivilEngineerId, updater);
            if (biggest_dam >= 40) SetAchievementCompleted();

            debug_console.LogInfo("Updated dam building achievements.");
        }

        private void AddBlockInDams(Vector3Int new_dam_block, DamHeight height) {
            var adjacent_dams = GetAdjacentDams(new_dam_block, height);
            var new_blocks = NewDamBlockToSet(new_dam_block, height);
            if (adjacent_dams.IsEmpty()) {
                logical_dams.Add(new_blocks);
            } else if (adjacent_dams.Count == 1) {
                adjacent_dams[0].AddRange(new_blocks);
            } else {
                var new_dam = MergeDams(adjacent_dams);
                new_dam.AddRange(new_blocks);
            }
        }

        private HashSet<Vector3Int> MergeDams(List<HashSet<Vector3Int>> dams) {
            var new_dam = new HashSet<Vector3Int>();
            foreach (var dam in dams) {
                new_dam.AddRange(dam);
                logical_dams.Remove(dam);
            }

            logical_dams.Add(new_dam);
            return new_dam;
        }

        [OnEvent]
        public void OnDestroyedEvent(ConstructibleExitedFinishedStateEvent constructibleEvent) {
            var prefab = constructibleEvent.Constructible.GetComponentFast<Prefab>();
            var block_object = constructibleEvent.Constructible.GetComponentFast<BlockObject>();
            if (prefab is null || block_object is null) return;

            var position = block_object.CoordinatesAtBaseZ;

            if (IsPrefab(prefab, damPrefabSubName) || IsPrefab(prefab, leveePrefabSubName) || IsFloodgate1Prefab(prefab))
                RemoveBlockInDams(position, DamHeight.One);
            else if (IsPrefab(prefab, floodgate2PrefabSubName))
                RemoveBlockInDams(position, DamHeight.Two);
            else if (IsPrefab(prefab, floodgate3PrefabSubName))
                RemoveBlockInDams(position, DamHeight.Three);
            else return;
        }

        private void RemoveBlockInDams(Vector3Int removed_dam_block, DamHeight height) {
            var dam = GetDam(removed_dam_block);
            var removed_blocks = NewDamBlockToSet(removed_dam_block, height);
            if (dam is null) {
                console.LogError("When dismantling dams it must belong to only one dam set.");
                console.LogError("The dam building achievement logic will forcibly stopped.");
                SetAchievementCompleted();
            } else {
                //remove the big dam
                logical_dams.Remove(dam);
                foreach (var removed_block in removed_blocks)
                    dam.Remove(removed_block);

                //add each block individually to reconstruct the split dams
                foreach (var block in dam) {
                    AddBlockInDams(block, DamHeight.One);
                }
            }
        }

        private bool IsPrefab(Prefab prefab, string subname) {
            return prefab.PrefabName.Contains(subname, StringComparison.OrdinalIgnoreCase);
        }

        private bool IsFloodgate1Prefab(Prefab prefab) {
            return IsPrefab(prefab, floodgate1PrefabSubName) &&
                !IsPrefab(prefab, floodgate2PrefabSubName) &&
                !IsPrefab(prefab, floodgate3PrefabSubName);
        }

        private HashSet<Vector3Int> GetDam(Vector3Int dam_block) {
            foreach (var dam in logical_dams) {
                foreach (var block in dam) {
                    if (block == dam_block) {
                        return dam;
                    }
                }
            }

            return null;
        }

        private List<HashSet<Vector3Int>> GetAdjacentDams(Vector3Int new_dam_block, DamHeight height) {
            var dam0 = GetAdjacentDams(new_dam_block);
            if (height != DamHeight.One) {
                var dam1 = GetAdjacentDams(new_dam_block + new Vector3Int(0,0,1));
                AppendListUnique(dam0, dam1);

                if (height != DamHeight.Two) {
                    var dam2 = GetAdjacentDams(new_dam_block + new Vector3Int(0,0,2));
                    AppendListUnique(dam0, dam2);
                }
            }

            return dam0;
        }
        private List<HashSet<Vector3Int>> GetAdjacentDams(Vector3Int new_dam_block) {
            var dams = new List<HashSet<Vector3Int>>();
            foreach (var dam in logical_dams) {
                foreach (var block in dam) {
                    if (isNeighbor(block, new_dam_block)) {
                        dams.Add(dam);
                    }
                }
            }

            return dams;
        }

        private void AppendListUnique(List<HashSet<Vector3Int>> list, List<HashSet<Vector3Int>> list2) {
            foreach(var obj in list2) {
                if (!list.Contains(obj))
                    list.Add(obj);
            }
        }

        private HashSet<Vector3Int> NewDamBlockToSet(Vector3Int new_dam, DamHeight height) {
            var set = new HashSet<Vector3Int>();
            set.Add(new_dam);
            if (height != DamHeight.One) {
                set.Add(new_dam + new Vector3Int(0,0,1));
                if (height != DamHeight.Two)
                    set.Add(new_dam + new Vector3Int(0,0,2));
            }

            return set;
        }

        private int getBiggestDamSize() {
            var max_size = 0;
            foreach (var dam in logical_dams) {
                max_size = Math.Max(max_size, dam.Count);
            }

            return max_size;
        }

        private bool isNeighbor(Vector3Int left, Vector3Int right) {
            var distance = Vector3Int.Distance(left, right);
            return distance >= 0.9999 && distance <= 1.0001;
        }

        private const string leveePrefabSubName = "Levee";
        private const string damPrefabSubName = "Dam";
        private const string floodgate1PrefabSubName = "Floodgate";
        private const string floodgate2PrefabSubName = "DoubleFloodgate";
        private const string floodgate3PrefabSubName = "TripleFloodgate";
        public const string damId = "a005.0.dam";
        public const string realCivilEngineerId = "a005.1.realCivilEngineer";
        private readonly PropertyKey<int> propertyBiggestDam = new PropertyKey<int>("biggest_dam");
    }

    public class GameLogicBridge4x1 : AchievementLogicBase
    {
        public GameLogicBridge4x1(EventBus eventBus, IConsoleWriter console, AchievementManager manager)
            : base(eventBus, console, manager) { }
        
        [OnEvent]
        public void OnWindtunnelConstructed(ConstructibleEnteredFinishedStateEvent constructibleEvent) {
            if (IsBridgeConstructible(constructibleEvent.Constructible)) {
                manager.UpdateLocalAchievement(bridgeReviewId, new AchievementSimple.Updater{ completed = true });
                SetAchievementCompleted();
                debug_console.LogInfo($"Completed bridgeReviewId achievement");
            }
        }
        private bool IsBridgeConstructible(Constructible constructible) {
            var prefab = constructible.GetComponentFast<Prefab>();
            return prefab is not null && IsBridgePrefab(prefab);
        }

        private bool IsBridgePrefab(Prefab prefab) {
            return prefab.PrefabName.Contains(bridge4x1PrefabSubName, StringComparison.OrdinalIgnoreCase);
        }


        private const string bridge4x1PrefabSubName = "SuspensionBridge4x1";
        public const string bridgeReviewId = "a005.2.bridgeReview";
    }

    public class Generator : IAchievementGenerator
    {
        public IEnumerable<AchievementDefinitionBase> Generate()
        {
            var definitions = new List<AchievementDefinitionBase> {
                new AchievementWithCompletitionDefinition(
                    GameLogic.damId,
                    damImage,
                    damTitle,
                    damDescription,
                    20, true
                ),
                new AchievementWithCompletitionDefinition(
                    GameLogic.realCivilEngineerId,
                    realCivilEngineerImage,
                    realCivilEngineerTitle,
                    realCivilEngineerDescription,
                    40, true
                ),
                new AchievementSimpleDefinition(
                    GameLogicBridge4x1.bridgeReviewId,
                    bridgeReviewImage,
                    bridgeReviewTitle,
                    bridgeReviewDescription
                )
            };
        
            return definitions;
        }

        private const string damTitle = "yurand.achievements.a005.0.dam.title";
        private const string damDescription = "yurand.achievements.a005.0.dam.description";
        private const string damImage = "yurand.achievements.a005.0.dam.image";

        private const string realCivilEngineerTitle = "yurand.achievements.a005.1.realCivilEngineer.title";
        private const string realCivilEngineerDescription = "yurand.achievements.a005.1.realCivilEngineer.description";
        private const string realCivilEngineerImage = "yurand.achievements.a005.1.realCivilEngineer.image";

        private const string bridgeReviewTitle = "yurand.achievements.a005.2.bridgeReview.title";
        private const string bridgeReviewDescription = "yurand.achievements.a005.2.bridgeReview.description";
        private const string bridgeReviewImage = "yurand.achievements.a005.2.bridgeReview.image";
    }
}
