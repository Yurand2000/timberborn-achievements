using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Transactions;
using TimberApi.ConsoleSystem;
using Timberborn.Common;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.UIElements;

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
            LoadDefinitions();
            LoadGlobal();            
            
            if (PluginEntryPoint.debugLogging) {
                console.LogInfo("Correctly loaded achievements manager.");
                console.LogInfo($"Loaded {achievementDefinitions.Count} achievements");
            }
        }

        private void LoadDefinitions() {
            foreach (var definition in makeAchievements())
                achievementDefinitions.Add(definition.uniqueId, definition);
        }

        private void LoadGlobal() {
            try {
                var global_acks_data = System.IO.File.ReadAllText(PluginEntryPoint.directory + "/" + achievementFile);
                var global_acks = XmlHelper.FromString<SerializableAchievements>(global_acks_data).achievements;
                foreach (var achievement in global_acks) {
                    if (!achievementDefinitions.ContainsKey(achievement.achievementId)) continue;

                    var definition = achievementDefinitions[achievement.achievementId];

                    global_achievements.Add(
                        achievement.achievementId,
                        new Achievement(achievementDefinitions, achievement)
                    );
                }
            } catch (System.IO.FileNotFoundException) { }

            foreach (var key in achievementDefinitions.Keys.Except(global_achievements.Keys)) {
                var definition = achievementDefinitions[key];

                global_achievements.Add(
                    key, new Achievement {
                        definition = definition,
                        completed = false,
                        current_value = definition.statusDefinition.HasValue ? 0 : null,
                    }
                );
            }
        }

        public void Unload() {
            SaveGlobal();
        }

        public void SaveGlobal() {
            try {
                var xml = XmlHelper.ToString(SerializeAchievements(global_achievements));
                console.LogInfo($"acks: {xml}");
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

        public IDictionary<string, Achievement> GetGlobalAchievements() {
            return new ReadOnlyDictionary<string, Achievement>(global_achievements);
        }

        public IDictionary<string, Achievement> GetLocalAchievements() {
            if (isInGame)
                return new ReadOnlyDictionary<string, Achievement>(local_achievements);
            else
                return null;
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
            var loader = singletonLoader.GetSingleton(singletonKey);
            var local_acks_data = loader.Get(localAchievementsKey, new SerializableAchievementsSerializer());
            var definitions = manager.GetAchievementDefinitions();
            foreach(var local_ack in local_acks_data.achievements) {
                local_achievements.Add(
                    local_ack.achievementId,
                    new Achievement(definitions, local_ack)
                );
            }

            manager.SetInGame(true, local_achievements);

            if (PluginEntryPoint.debugLogging) {
                console.LogInfo("Correctly loaded achievements manager (ingame).");
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

        private static readonly SingletonKey singletonKey = new SingletonKey(nameof(AchievementManager));
        private static readonly PropertyKey<SerializableAchievements> localAchievementsKey = new PropertyKey<SerializableAchievements>("localAchievements");
    }
}