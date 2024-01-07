using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using TimberApi.ConsoleSystem;
using Timberborn.BaseComponentSystem;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.AI;
using Yurand.Timberborn.Achievements.UI;

namespace Yurand.Timberborn.Achievements
{
    public partial class AchievementManager : ILoadableSingleton, IUnloadableSingleton
    {
        private const string achievementFile = "achievements.xml";
        public const bool loadTestAchievements = false;

        private readonly IConsoleWriter console;
        private readonly EventBus eventBus;
        private Dictionary<string, AchievementDefinitionBase> achievementDefinitions = new Dictionary<string, AchievementDefinitionBase>();
        private Dictionary<string, AchievementBase> global_achievements = new Dictionary<string, AchievementBase>();
        private Dictionary<string, AchievementBase> local_achievements = null;
        private IEnumerable<IAchievementGenerator> definitionGenerators;
        private bool isInGame = false;

        public AchievementManager(
            IConsoleWriter console,
            EventBus eventBus,
            IEnumerable<IAchievementGenerator> achievementGenerators
        ) {
            this.console = console;
            this.eventBus = eventBus;
            this.definitionGenerators = achievementGenerators;
        }

        public void Load() {
            foreach (var definitionGenerator in definitionGenerators) {
                AddAchievementDefinitions(definitionGenerator.Generate());
            }

            if (loadTestAchievements)
                AddAchievementDefinitions(LoadDebugDefinitions());

            LoadGlobal();

            if (PluginEntryPoint.debugLogging) {
                console.LogInfo("Correctly loaded achievements manager.");
                TestAchievementTypeConsistency();
            }

            console.LogInfo($"Loaded {achievementDefinitions.Count} achievements");
        }

        private void LoadGlobal() {
            try {
                var global_acks_data = System.IO.File.ReadAllText(PluginEntryPoint.directory + "/" + achievementFile);
                var global_acks = XmlHelper.FromString<SerializableAchievements>(global_acks_data)?.achievements ?? new SerializableAchievementBase[]{};
                foreach (var achievement in global_acks) {
                    if (!achievementDefinitions.ContainsKey(achievement.achievementId)) continue;
                    var definition = achievementDefinitions[achievement.achievementId];

                    var deserialized = AchievementSerializer.Deserialize(achievementDefinitions, achievement);

                    if (deserialized is not null)
                        global_achievements.Add(achievement.achievementId, deserialized);
                }
            } catch (System.IO.FileNotFoundException) { }
        }

        public void Unload() {
            SaveGlobal();
        }

        public void SaveGlobal() {
            try {
                var xml = XmlHelper.ToString(SerializeAchievements(global_achievements));
                System.IO.File.WriteAllText(PluginEntryPoint.directory + "/" + achievementFile, xml);

                if (PluginEntryPoint.debugLogging) {
                    console.LogInfo("Updated local achievements file.");
                }
            } catch (Exception e) {
                console.LogError($"Failed to save achievements, with error: {e}");
            }
        }

        public static SerializableAchievements SerializeAchievements(Dictionary<string, AchievementBase> achievements) {
            var achievementsArray = achievements.Values.Select(ack => AchievementSerializer.Serialize(ack)).ToArray();
            return new SerializableAchievements(achievementsArray);
        }

        public IDictionary<string, AchievementDefinitionBase> GetAchievementDefinitions() {
            return new ReadOnlyDictionary<string, AchievementDefinitionBase>(achievementDefinitions);
        }

        public void AddAchievementDefinition(AchievementDefinitionBase definition) {
            achievementDefinitions.Add(definition.uniqueId, definition);
        }

        public void AddAchievementDefinitions(IEnumerable<AchievementDefinitionBase> definitions) {
            foreach(var definition in definitions) {
                achievementDefinitions.Add(definition.uniqueId, definition);
            }
        }
        public void UpdateLocalAchievement(string achievementId, AchievementBase.Updater updater, bool update_global = true) {
            if (!achievementDefinitions.ContainsKey(achievementId)) return;
            CreateLocalAchievementIfEmpty(achievementId);

            local_achievements[achievementId].Update(updater);
            if(update_global)
                UpdateGlobalAchievementFromLocal(achievementId);
        }

        private void UpdateGlobalAchievementFromLocal(string achievementId) {
            CreateGlobalAchievementIfEmpty(achievementId);
            var was_completed = global_achievements[achievementId].completed;
            global_achievements[achievementId].UpdateFromLocal(local_achievements[achievementId]);

            if (!was_completed && global_achievements[achievementId].completed) {
                var definition = achievementDefinitions[achievementId];
                eventBus.Post(new AchievementCompletedEvent(definition.localizedTitle));
            }
        }

        public void ResetLocalAchievements() {
            local_achievements.Clear();
        }

        public void ResetGlobalAchievements() {
            global_achievements.Clear();
        }

        private void CreateGlobalAchievementIfEmpty(string achievementId) {
            CreateAchievementIfEmptyInDictionary(achievementId, global_achievements);
        }

        private void CreateLocalAchievementIfEmpty(string achievementId) {
            CreateAchievementIfEmptyInDictionary(achievementId, local_achievements);
        }

        public AchievementBase GetGlobalAchievement(string achievementId) {
            CreateGlobalAchievementIfEmpty(achievementId);
            return global_achievements[achievementId];
        }

        public AchievementBase GetLocalAchievement(string achievementId) {
            if (isInGame) {
                CreateLocalAchievementIfEmpty(achievementId);
                return local_achievements[achievementId];
            } else {
                return null;
            }
        }

        public bool IsAchievementCompleted(string achievementId) {
            return GetGlobalAchievement(achievementId).completed;
        }

        private void CreateAchievementIfEmptyInDictionary(string achievementId, IDictionary<string, AchievementBase> dictionary) {
            if (!dictionary.ContainsKey(achievementId)) {
                var definition = achievementDefinitions[achievementId];
                dictionary.Add(achievementId, AchievementSerializer.Default(definition));
            }
        }

        public void SetInGame(bool isInGame, Dictionary<string, AchievementBase> local_achievements) {
            this.isInGame = isInGame;
            this.local_achievements = local_achievements;

            if (PluginEntryPoint.debugLogging) {
                if (this.isInGame)
                    console.LogInfo("Achievement Manager set to in game");
                else
                    console.LogInfo("Achievement Manager set to NOT in game");
            }
        }
    }

    public partial class AchievementManagerInGame : ILoadableSingleton, ISaveableSingleton, IUnloadableSingleton
    {
        private AchievementManager manager;
        private IConsoleWriter console;
        private ISingletonLoader singletonLoader;
        private Dictionary<string, AchievementBase> local_achievements = new Dictionary<string, AchievementBase>();
        public AchievementManagerInGame(AchievementManager manager, ISingletonLoader singletonLoader, IConsoleWriter console) {
            this.manager = manager;
            this.singletonLoader = singletonLoader;
            this.console = console;
        }

        public void Load() {
            TryLoadLocalAchievements();
            manager.SetInGame(true, local_achievements);            
            if (AchievementManager.loadTestAchievements) {
                SetDebugAchievementGlobalState();
            }

            if (PluginEntryPoint.debugLogging) {
                console.LogInfo("Correctly loaded achievements manager (ingame).");
            }
        }

        private void TryLoadLocalAchievements() {
            var definitions = manager.GetAchievementDefinitions();

            if (singletonLoader.HasSingleton(singletonKey)) {
                var loader = singletonLoader.GetSingleton(singletonKey);
                if( loader.Has(localAchievementsKey) ) {
                    var local_acks_data = loader.Get(localAchievementsKey, new SerializableAchievementsSerializer());
                    foreach(var local_ack in local_acks_data.achievements) {
                        var deserialized = AchievementSerializer.Deserialize(definitions, local_ack);

                        if (deserialized is not null)
                            local_achievements.Add(local_ack.achievementId, deserialized);
                    }
                }
            }
        }

        public void Save(ISingletonSaver singletonSaver) {
            var saver = singletonSaver.GetSingleton(singletonKey);
            saver.Set(
                localAchievementsKey,
                AchievementManager.SerializeAchievements(local_achievements),
                new SerializableAchievementsSerializer()
            );
        }

        public void Unload() {
            manager.SetInGame(false, null);

            if (PluginEntryPoint.debugLogging) {
                console.LogInfo("Correctly unloaded achievements manager (ingame).");
            }
        }

        private static readonly SingletonKey singletonKey = new SingletonKey(nameof(AchievementManagerInGame));
        private static readonly PropertyKey<SerializableAchievements> localAchievementsKey = new PropertyKey<SerializableAchievements>("localAchievements");
    }
}