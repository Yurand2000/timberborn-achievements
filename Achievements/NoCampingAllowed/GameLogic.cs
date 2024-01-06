using System;
using System.Collections.Generic;
using HarmonyLib;
using TimberApi.ConsoleSystem;
using Timberborn.BlockSystem;
using Timberborn.Buildings;
using Timberborn.ConstructibleSystem;
using Timberborn.ConstructionSites;
using Timberborn.EntitySystem;
using Timberborn.PrefabSystem;
using Timberborn.SingletonSystem;

namespace Yurand.Timberborn.Achievements.NoCampingAllowed
{
    public class GameLogic : ILoadableSingleton
    {
        private EventBus eventBus;
        private IConsoleWriter console;
        private AchievementManager manager;
        private int current_campfires = 0;
        public GameLogic(EventBus eventBus, IConsoleWriter console, AchievementManager manager) {
            this.eventBus = eventBus;
            this.console = console;
            this.manager = manager;
        }

        public void Load() {
            eventBus.Register(this);
        }
        
        [OnEvent]
        public void OnCampfireBuilt(ConstructibleEnteredFinishedStateEvent constructibleEvent) {
            if (IsCampfireConstructible(constructibleEvent.Constructible)) {
                current_campfires += 1;
            }

            if (current_campfires >= 3) {
                manager.UpdateLocalAchievement(noCampingAllowedId, new AchievementSimple.Updater{ completed = true });
                eventBus.Unregister(this);

                if (PluginEntryPoint.debugLogging) {
                    console.LogInfo($"Completed noCampingAllowed achievement");
                }
            }
        }
        
        [OnEvent]
        public void OnCampfireDestroyed(ConstructibleExitedFinishedStateEvent constructibleEvent) {
            if (IsCampfireConstructible(constructibleEvent.Constructible)) {
                current_campfires -= 1;
            }
        }

        private bool IsCampfireConstructible(Constructible constructible) {
            var prefab = constructible.GetComponentFast<Prefab>();
            return prefab is not null && IsCampfirePrefab(prefab);
        }

        private bool IsCampfirePrefab(Prefab prefab) {
            return prefab.PrefabName.Contains(campfirePrefabSubName, StringComparison.OrdinalIgnoreCase);
        }

        private const string campfirePrefabSubName = "Campfire";
        public const string noCampingAllowedId = "a010.noCampingAllowed";
    }

    [HarmonyPatch]
    public class Patcher {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AchievementManager), "LoadDefinitions")]
        public static void PatchLoadAchievementsDefinitions(ref List<AchievementDefinitionBase> __result) {
            __result.Add(new AchievementSimpleDefinition(GameLogic.noCampingAllowedId, noCampingAllowedIdImage, noCampingAllowedIdTitle, noCampingAllowedIdDescription));
        }
        private const string noCampingAllowedIdTitle = "yurand.achievements.a010.noCampingAllowed.title";
        private const string noCampingAllowedIdDescription = "yurand.achievements.a010.noCampingAllowed.description";
        private const string noCampingAllowedIdImage = "yurand.achievements.a010.noCampingAllowed.image";
    }
}
