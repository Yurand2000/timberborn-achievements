using System;
using System.Collections.Generic;
using System.Linq;
using TimberApi.ConsoleSystem;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace Yurand.Timberborn.Achievements
{
    public class AchievementManager : ILoadableSingleton, ISaveableSingleton
    {
        private const string achievementFile = "achievements.json";

        private IConsoleWriter console;
        private Dictionary<string, AchievementDefinition> achievementDefinitions = new Dictionary<string, AchievementDefinition>();
        private Dictionary<string, Achievement> achievements = new Dictionary<string, Achievement>();

        public AchievementManager(IConsoleWriter console) {
            this.console = console;
        }

        public void Load() {
            foreach (var definition in makeAchievements())
                achievementDefinitions.Add(definition.uniqueId, definition);

            try {
                var local_acks_data = System.IO.File.ReadAllText(PluginEntryPoint.directory + "/" + achievementFile);
                var local_acks = JsonHelper.FromListJson<SerializableAchievement>(local_acks_data);
                foreach (var achievement in local_acks) {
                    if (!achievementDefinitions.ContainsKey(achievement.achievementId)) continue;

                    var definition = achievementDefinitions[achievement.achievementId];

                    achievements.Add(
                        achievement.achievementId,
                        new Achievement {
                            definition = achievementDefinitions[achievement.achievementId],
                            completed = achievement.completed,
                            current_value = definition.statusDefinition.HasValue ? achievement.current_value : null
                        }
                    );
                }
            } catch (System.IO.FileNotFoundException) { }

            foreach (var key in achievementDefinitions.Keys.Except(achievements.Keys)) {
                var definition = achievementDefinitions[key];

                achievements.Add(
                    key, new Achievement {
                        definition = definition,
                        completed = false,
                        current_value = definition.statusDefinition.HasValue ? 0 : null,
                    }
                );
            }
            
            if (PluginEntryPoint.debugLogging) {
                console.LogInfo("Correctly loaded achievements manager.");
                console.LogInfo($"Loaded {achievementDefinitions.Count} achievements");
            }
        }

        public void Save() { Save(null); }
        public void Save(ISingletonSaver singletonSaver) {
            try {
                var local_acks = achievements.Values.Select(ack => ack.ToSerializable()).ToList();
                var json = JsonHelper.ToListJson(local_acks, true);
                console.LogInfo($"acks: {local_acks.Count}\n{json}");
                System.IO.File.WriteAllText(PluginEntryPoint.directory + "/" + achievementFile, json);

                if (PluginEntryPoint.debugLogging) {
                    console.LogInfo("Updated local achievements file.");
                }
            } catch (Exception e) {
                console.LogError($"Failed to save achievements, with error: {e}");
            }
        }

        public List<Achievement> GetLocalAchievements() {
            return new List<Achievement>(achievements.Values);
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
}