using System.Collections.Generic;
using HarmonyLib;
using TimberApi.ConsoleSystem;
using Timberborn.SingletonSystem;
using Timberborn.Beavers;

namespace Yurand.Timberborn.Achievements.BeaverPopulationAchievement
{
    [HarmonyPatch]
    public class GameLogic : ILoadableSingleton
    {
        private static GameLogic logic;
        private EventBus eventBus;
        private IConsoleWriter console;
        private AchievementManager manager;
        private BeaverPopulation beaverPopulation;
        public GameLogic(EventBus eventBus, IConsoleWriter console, AchievementManager manager, BeaverPopulation beaverPopulation) {
            this.eventBus = eventBus;
            this.console = console;
            this.manager = manager;
            this.beaverPopulation = beaverPopulation;
            logic = this;
        }

        public void Load() {
            eventBus.Register(this);
            UpdateBeaverCounters();
        }

        public void UpdateBeaverCounters() {
            var beavers = beaverPopulation.NumberOfBeavers;

            manager.TryUpdateLocalAchievement(beaverPopulation50Id, beavers);
            manager.TryUpdateLocalAchievement(beaverPopulation100Id, beavers);
            manager.TryUpdateLocalAchievement(beaverPopulation200Id, beavers);
            manager.TryUpdateLocalAchievement(beaverPopulation300Id, beavers);
            manager.TryUpdateLocalAchievement(beaverPopulation500Id, beavers);

            if (PluginEntryPoint.debugLogging) {
                console.LogInfo("Updated beaverPopulationX achievements.");
            }
        }

        [HarmonyPatch(typeof(BeaverPopulation), "OnCharacterCreated")]
        [HarmonyPostfix]
        public static void OnCharactedCreatedPostfix() {
            if (logic is null) return;
            logic.UpdateBeaverCounters();
        }

        public const string beaverPopulation50Id = "beaverPopulation50";
        public const string beaverPopulation100Id = "beaverPopulation100";
        public const string beaverPopulation200Id = "beaverPopulation200";
        public const string beaverPopulation300Id = "beaverPopulation300";
        public const string beaverPopulation500Id = "beaverPopulation500";
    }

    [HarmonyPatch]
    public class Patcher {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AchievementManager), "LoadDefinitions")]
        public static void PatchLoadAchievementsDefinitions(ref List<AchievementDefinition> __result) {
            __result.Add(new AchievementDefinition(GameLogic.beaverPopulation50Id, beaverPopulation50Image, beaverPopulation50Title, beaverPopulation50Description, 50, true));
            __result.Add(new AchievementDefinition(GameLogic.beaverPopulation100Id, beaverPopulation100Image, beaverPopulation100Title, beaverPopulation100Description, 100, true));
            __result.Add(new AchievementDefinition(GameLogic.beaverPopulation200Id, beaverPopulation200Image, beaverPopulation200Title, beaverPopulation200Description, 200, true));
            __result.Add(new AchievementDefinition(GameLogic.beaverPopulation300Id, beaverPopulation300Image, beaverPopulation300Title, beaverPopulation300Description, 300, true));
            __result.Add(new AchievementDefinition(GameLogic.beaverPopulation500Id, beaverPopulation500Image, beaverPopulation500Title, beaverPopulation500Description, 500, true));
        }
        
        private const string beaverPopulation50Title = "yurand.achievements.beaverPopulation50.title";
        private const string beaverPopulation50Description = "yurand.achievements.beaverPopulation50.description";
        private const string beaverPopulation50Image = null;
        private const string beaverPopulation100Title = "yurand.achievements.beaverPopulation100.title";
        private const string beaverPopulation100Description = "yurand.achievements.beaverPopulation100.description";
        private const string beaverPopulation100Image = null;
        private const string beaverPopulation200Title = "yurand.achievements.beaverPopulation200.title";
        private const string beaverPopulation200Description = "yurand.achievements.beaverPopulation200.description";
        private const string beaverPopulation200Image = null;
        private const string beaverPopulation300Title = "yurand.achievements.beaverPopulation300.title";
        private const string beaverPopulation300Description = "yurand.achievements.beaverPopulation300.description";
        private const string beaverPopulation300Image = null;
        private const string beaverPopulation500Title = "yurand.achievements.beaverPopulation500.title";
        private const string beaverPopulation500Description = "yurand.achievements.beaverPopulation500.description";
        private const string beaverPopulation500Image = null;
    }
}
