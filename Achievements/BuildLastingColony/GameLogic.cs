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

            manager.UpdateLocalAchievement(buildColony5Id, updater);
            manager.UpdateLocalAchievement(buildColony10Id, updater);
            manager.UpdateLocalAchievement(buildColony15Id, updater);
            manager.UpdateLocalAchievement(buildColony20Id, updater);
            manager.UpdateLocalAchievement(buildColony30Id, updater);
            manager.UpdateLocalAchievement(buildColony50Id, updater);
            manager.UpdateLocalAchievement(buildColony100Id, updater);

            if (PluginEntryPoint.debugLogging) {
                console.LogInfo("Updated buildColonyX achievements.");
            }
        }

        public const string buildColony5Id = "a001.0.buildColony5";
        public const string buildColony10Id = "a001.1.buildColony10";
        public const string buildColony15Id = "a001.2.buildColony15";
        public const string buildColony20Id = "a001.3.buildColony20";
        public const string buildColony30Id = "a001.4.buildColony30";
        public const string buildColony50Id = "a001.5.buildColony50";
        public const string buildColony100Id = "a001.6.buildColony100";
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
        private const string buildColony5Title = "yurand.achievements.a001.0.buildColony5.title";
        private const string buildColony5Description = "yurand.achievements.a001.0.buildColony5.description";
        private const string buildColony5Image = "yurand.achievements.a001.0.buildColony5.image";
        private const string buildColony10Title = "yurand.achievements.a001.1.buildColony10.title";
        private const string buildColony10Description = "yurand.achievements.a001.1.buildColony10.description";
        private const string buildColony10Image = "yurand.achievements.a001.1.buildColony10.image";
        private const string buildColony15Title = "yurand.achievements.a001.2.buildColony15.title";
        private const string buildColony15Description = "yurand.achievements.a001.2.buildColony15.description";
        private const string buildColony15Image = "yurand.achievements.a001.2.buildColony15.image";
        private const string buildColony20Title = "yurand.achievements.a001.3.buildColony20.title";
        private const string buildColony20Description = "yurand.achievements.a001.3.buildColony20.description";
        private const string buildColony20Image = "yurand.achievements.a001.3.buildColony20.image";
        private const string buildColony30Title = "yurand.achievements.a001.4.buildColony30.title";
        private const string buildColony30Description = "yurand.achievements.a001.4.buildColony30.description";
        private const string buildColony30Image = "yurand.achievements.a001.4.buildColony30.image";
        private const string buildColony50Title = "yurand.achievements.a001.5.buildColony50.title";
        private const string buildColony50Description = "yurand.achievements.a001.5.buildColony50.description";
        private const string buildColony50Image = "yurand.achievements.a001.5.buildColony50.image";
        private const string buildColony100Title = "yurand.achievements.a001.6.buildColony100.title";
        private const string buildColony100Description = "yurand.achievements.a001.6.buildColony100.description";
        private const string buildColony100Image = "yurand.achievements.a001.6.buildColony100.image";
    }
}
