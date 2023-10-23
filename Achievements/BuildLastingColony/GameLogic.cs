using System.Collections.Generic;
using System.Linq.Expressions;
using HarmonyLib;
using TimberApi.ConsoleSystem;
using Timberborn.SingletonSystem;
using Timberborn.TimeSystem;
using Timberborn.WeatherSystem;

namespace Yurand.Timberborn.Achievements.BuildLastingColonyAchievement
{
    public class GameLogic : ILoadableSingleton
    {
        private EventBus eventBus;
        private IConsoleWriter console;
        private AchievementManager manager;
        private WeatherService weatherService;
        public GameLogic(EventBus eventBus, IConsoleWriter console, AchievementManager manager, WeatherService weatherService) {
            this.eventBus = eventBus;
            this.console = console;
            this.manager = manager;
            this.weatherService = weatherService;
        }

        public void Load() {
            eventBus.Register(this);            
            OnNewDayEvent(null);
        }

        [OnEvent]
        public void OnNewDayEvent(DaytimeStartEvent daytimeStartEvent) {
            var current_cycle = weatherService.Cycle;
            var updater = new AchievementWithCompletition.Updater(){ next_state = current_cycle };

            manager.TryUpdateLocalAchievement(buildColony5Id, updater);
            manager.TryUpdateLocalAchievement(buildColony10Id, updater);
            manager.TryUpdateLocalAchievement(buildColony15Id, updater);
            manager.TryUpdateLocalAchievement(buildColony20Id, updater);
            manager.TryUpdateLocalAchievement(buildColony30Id, updater);
            manager.TryUpdateLocalAchievement(buildColony50Id, updater);
            manager.TryUpdateLocalAchievement(buildColony100Id, updater);

            if (PluginEntryPoint.debugLogging) {
                console.LogInfo("Updated buildColonyX achievements.");
            }
        }

        public const string buildColony5Id = "buildColony5";
        public const string buildColony10Id = "buildColony10";
        public const string buildColony15Id = "buildColony15";
        public const string buildColony20Id = "buildColony20";
        public const string buildColony30Id = "buildColony30";
        public const string buildColony50Id = "buildColony50";
        public const string buildColony100Id = "buildColony100";
    }

    [HarmonyPatch]
    public class Patcher {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AchievementManager), "LoadDefinitions")]
        public static void PatchLoadAchievementsDefinitions(ref List<AchievementDefinitionBase> __result) {
            __result.Add(new AchievementWithCompletitionDefinition(GameLogic.buildColony5Id, buildColony5Image, buildColony5Title, buildColony5Description, 5, true));
            __result.Add(new AchievementWithCompletitionDefinition(GameLogic.buildColony10Id, buildColony10Image, buildColony10Title, buildColony10Description, 10, true));
            __result.Add(new AchievementWithCompletitionDefinition(GameLogic.buildColony15Id, buildColony15Image, buildColony15Title, buildColony15Description, 15, true));
            __result.Add(new AchievementWithCompletitionDefinition(GameLogic.buildColony20Id, buildColony20Image, buildColony20Title, buildColony20Description, 20, true));
            __result.Add(new AchievementWithCompletitionDefinition(GameLogic.buildColony30Id, buildColony30Image, buildColony30Title, buildColony30Description, 30, true));
            __result.Add(new AchievementWithCompletitionDefinition(GameLogic.buildColony50Id, buildColony50Image, buildColony50Title, buildColony50Description, 50, true));
            __result.Add(new AchievementWithCompletitionDefinition(GameLogic.buildColony100Id, buildColony100Image, buildColony100Title, buildColony100Description, 100, true));
        }
        private const string buildColony5Title = "yurand.achievements.buildColony5.title";
        private const string buildColony5Description = "yurand.achievements.buildColony5.description";
        private const string buildColony5Image = null;
        private const string buildColony10Title = "yurand.achievements.buildColony10.title";
        private const string buildColony10Description = "yurand.achievements.buildColony10.description";
        private const string buildColony10Image = null;
        private const string buildColony15Title = "yurand.achievements.buildColony15.title";
        private const string buildColony15Description = "yurand.achievements.buildColony15.description";
        private const string buildColony15Image = null;
        private const string buildColony20Title = "yurand.achievements.buildColony20.title";
        private const string buildColony20Description = "yurand.achievements.buildColony20.description";
        private const string buildColony20Image = null;
        private const string buildColony30Title = "yurand.achievements.buildColony30.title";
        private const string buildColony30Description = "yurand.achievements.buildColony30.description";
        private const string buildColony30Image = null;
        private const string buildColony50Title = "yurand.achievements.buildColony50.title";
        private const string buildColony50Description = "yurand.achievements.buildColony50.description";
        private const string buildColony50Image = null;
        private const string buildColony100Title = "yurand.achievements.buildColony100.title";
        private const string buildColony100Description = "yurand.achievements.buildColony100.description";
        private const string buildColony100Image = null;
    }
}
