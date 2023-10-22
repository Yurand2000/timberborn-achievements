using System.Collections.Generic;
using HarmonyLib;
using TimberApi.ConsoleSystem;
using Timberborn.SingletonSystem;

namespace Yurand.Timberborn.Achievements.ExampleAchievement
{
    public class GameLogic : ILoadableSingleton
    {
        private EventBus eventBus;
        private IConsoleWriter console;
        private AchievementManager manager;
        public GameLogic(EventBus eventBus, IConsoleWriter console, AchievementManager manager) {
            this.eventBus = eventBus;
            this.console = console;
            this.manager = manager;
        }

        public void Load() {
            if (PluginEntryPoint.debugLogging) {
                console.LogInfo("ExampleAchievement logic loaded.");
            }
        }
    }

    [HarmonyPatch]
    public class Patcher {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AchievementManager), "LoadDefinitions")]
        public static void PatchLoadAchievementsDefinitions(ref List<AchievementDefinition> __result) {
            if (PluginEntryPoint.debugLogging) {
                PluginEntryPoint.console.LogInfo("ExampleAchievement definition loaded.");
            }
        }
    }
}
