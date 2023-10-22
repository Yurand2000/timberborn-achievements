using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TimberApi.ConsoleSystem;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using UnityEngine.AI;

namespace Yurand.Timberborn.Achievements
{
    public class AchievementManager : ILoadableSingleton, IUnloadableSingleton
    {
        private const string achievementFile = "achievements.xml";

        private IConsoleWriter console;
        private Dictionary<string, AchievementDefinition> achievementDefinitions = new Dictionary<string, AchievementDefinition>();
        private Dictionary<string, Achievement> global_achievements = new Dictionary<string, Achievement>();
        private Dictionary<string, Achievement> local_achievements = null;
        private bool isInGame = false;

        public AchievementManager(IConsoleWriter console) {
            this.console = console;
        }

        public void Load() {
            AddAchievementDefinitions(LoadDefinitions());
            LoadGlobal();

            if (PluginEntryPoint.debugLogging) {
                console.LogInfo("Correctly loaded achievements manager.");
                console.LogInfo($"Loaded {achievementDefinitions.Count} achievements");
            }
        }

        private List<AchievementDefinition> LoadDefinitions() {
            return makeAchievements().ToList();
        }

        private void LoadGlobal() {
            try {
                var global_acks_data = System.IO.File.ReadAllText(PluginEntryPoint.directory + "/" + achievementFile);
                var global_acks = XmlHelper.FromString<SerializableAchievements>(global_acks_data).achievements ?? new SerializableAchievement[]{};
                foreach (var achievement in global_acks) {
                    if (!achievementDefinitions.ContainsKey(achievement.achievementId)) continue;
                    var definition = achievementDefinitions[achievement.achievementId];

                    global_achievements.Add(
                        achievement.achievementId,
                        new Achievement(achievementDefinitions, achievement)
                    );
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

        public static SerializableAchievements SerializeAchievements(Dictionary<string, Achievement> achievements) {
            var achievementsArray = achievements.Values.Select(ack => ack.ToSerializable()).ToArray();
            return new SerializableAchievements(achievementsArray);
        }

        public IDictionary<string, AchievementDefinition> GetAchievementDefinitions() {
            return new ReadOnlyDictionary<string, AchievementDefinition>(achievementDefinitions);
        }

        public void AddAchievementDefinition(AchievementDefinition definition) {
            achievementDefinitions.Add(definition.uniqueId, definition);
        }
        
        public void AddAchievementDefinitions(IEnumerable<AchievementDefinition> definitions) {
            foreach(var definition in definitions) {
                achievementDefinitions.Add(definition.uniqueId, definition);
            }
        }

        public IDictionary<string, Achievement> GetGlobalAchievements() {
            return new ReadOnlyDictionary<string, Achievement>(global_achievements);
        }

        public IDictionary<string, Achievement> GetLocalAchievements() {
            if (isInGame)
                return new ReadOnlyDictionary<string, Achievement>(local_achievements);
            else
                return null;
        }

        public bool TryUpdateLocalAchievement(string achievementId, bool completed) {
            if (!achievementDefinitions.ContainsKey(achievementId)) return false;
            CreateLocalAchievementIfEmpty(achievementId);

            local_achievements[achievementId].completed = completed;
            UpdateGlobalAchievementFromLocal(achievementId);
            return true;
        }

        public bool TryUpdateLocalAchievement(string achievementId, float completition) {
            if (!achievementDefinitions.ContainsKey(achievementId)) return false;
            CreateLocalAchievementIfEmpty(achievementId);
            var definition = achievementDefinitions[achievementId];

            if (completition >= local_achievements[achievementId].current_value) {
                local_achievements[achievementId].current_value = completition;
            }

            var max_value = definition.statusDefinition?.max_value ?? float.MaxValue;
            if (completition >= max_value) {
                local_achievements[achievementId].current_value = max_value;
                local_achievements[achievementId].completed = true;
            }

            UpdateGlobalAchievementFromLocal(achievementId);
            return true;
        }

        public bool TryForceUpdateLocalAchievement(string achievementId, bool completed, float completition, bool update_global) {
            if (!achievementDefinitions.ContainsKey(achievementId)) return false;
            CreateLocalAchievementIfEmpty(achievementId);
            var definition = achievementDefinitions[achievementId];

            local_achievements[achievementId].completed = completed;
            local_achievements[achievementId].current_value = completition;
             var max_value = definition.statusDefinition?.max_value ?? float.MaxValue;
            if (completition >= max_value) {
                local_achievements[achievementId].current_value = max_value;
            }

            if(update_global) UpdateGlobalAchievementFromLocal(achievementId);
            return true;
        }

        private void UpdateGlobalAchievementFromLocal(string achievementId) {
            CreateGlobalAchievementIfEmpty(achievementId);
            global_achievements[achievementId].completed = local_achievements[achievementId].completed;
            global_achievements[achievementId].current_value = local_achievements[achievementId].current_value;
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

        private void CreateAchievementIfEmptyInDictionary(string achievementId, Dictionary<string, Achievement> dictionary) {
            if (!dictionary.ContainsKey(achievementId)) {
                var definition = achievementDefinitions[achievementId];
                dictionary.Add(achievementId, new Achievement(definition));
            }
        }

        public void SetInGame(bool isInGame, Dictionary<string, Achievement> local_achievements) {
            this.isInGame = isInGame;
            this.local_achievements = local_achievements;

            if (PluginEntryPoint.debugLogging) {
                if (this.isInGame)
                    console.LogInfo("Achievement Manager set to in game");
                else
                    console.LogInfo("Achievement Manager set to NOT in game");
            }
        }

        private AchievementDefinition[] makeAchievements() {
            return new AchievementDefinition[] {
                new AchievementDefinition(
                    "testAch00",
                    "AchievementsBase/Resources/testicon.png",
                    "achievement 00 title",
                    "achievement 00 description"),
                new AchievementDefinition(
                    "testAch01",
                    "AchievementsBase/Resources/testicon.png",
                    "achievement 01 title",
                    "achievement 01 description",
                    100.15f, false),
                new AchievementDefinition(
                    "testAch02",
                    "AchievementsBase/Resources/testicon.png",
                    "achievement 02 title",
                    "achievement 02 description",
                    1000, true),
            };
        }
    }

    public class AchievementManagerInGame : ILoadableSingleton, ISaveableSingleton, IUnloadableSingleton
    {
        private AchievementManager manager;
        private IConsoleWriter console;
        private ISingletonLoader singletonLoader;
        private Dictionary<string, Achievement> local_achievements = new Dictionary<string, Achievement>();
        public AchievementManagerInGame(AchievementManager manager, ISingletonLoader singletonLoader, IConsoleWriter console) {
            this.manager = manager;
            this.singletonLoader = singletonLoader;
            this.console = console;
        }

        public void Load() {
            TryLoadLocalAchievements();
            manager.SetInGame(true, local_achievements);

            if (PluginEntryPoint.debugLogging) {
                console.LogInfo("Correctly loaded achievements manager (ingame).");
            }
        }

        private void TryLoadLocalAchievements() {
            var definitions = manager.GetAchievementDefinitions();

            if (singletonLoader.HasSingleton(singletonKey)) {
                var loader = singletonLoader.GetSingleton(singletonKey);
                var local_acks_data = loader.Get(localAchievementsKey, new SerializableAchievementsSerializer());
                foreach(var local_ack in local_acks_data.achievements) {
                    local_achievements.Add(
                        local_ack.achievementId,
                        new Achievement(definitions, local_ack)
                    );
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