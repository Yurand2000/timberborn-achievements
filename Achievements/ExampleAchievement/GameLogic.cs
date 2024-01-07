using System.Collections.Generic;
using HarmonyLib;
using TimberApi.ConsoleSystem;
using Timberborn.SingletonSystem;

namespace Yurand.Timberborn.Achievements.ExampleAchievement
{
    public class GameLogic : AchievementLogicBase
    {
        public GameLogic(EventBus eventBus, IConsoleWriter console, AchievementManager manager)
            : base(eventBus, console, manager) {}

        protected override void OnLoad() {
            debug_console.LogInfo("ExampleAchievement logic loaded.");
            SetAchievementCompleted();
        }

        public const string exampleAchievementId = "a000.example";
    }

    public class Generator : IAchievementGenerator
    {
        public IEnumerable<AchievementDefinitionBase> Generate()
        {
            var definition = new AchievementHiddenDefinition(
                GameLogic.exampleAchievementId,
                exampleAchievementTitle,
                exampleAchievementDescription,
                exampleAchievementImage
            );

            if (PluginEntryPoint.debugLogging) {
                PluginEntryPoint.console.LogInfo("ExampleAchievement definition loaded.");
            }
        
            yield return definition;
        }
        private const string exampleAchievementTitle = "a000.example.title";
        private const string exampleAchievementDescription = "a000.example.description";
        private const string exampleAchievementImage = "a000.example.image";
    }
}
