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

namespace Yurand.Timberborn.Achievements.FlyBy
{
    public class GameLogic : AchievementLogicBase
    {
        public GameLogic(EventBus eventBus, IConsoleWriter console, AchievementManager manager)
            : base(eventBus, console, manager) { }
        
        [OnEvent]
        public void OnWindtunnelConstructed(ConstructibleEnteredFinishedStateEvent constructibleEvent) {
            if (IsWindtunnelConstructible(constructibleEvent.Constructible)) {
                manager.UpdateLocalAchievement(flyById, new AchievementSimple.Updater{ completed = true });
                SetAchievementCompleted();
                debug_console.LogInfo($"Completed flyBy achievement");
            }
        }
        private bool IsWindtunnelConstructible(Constructible constructible) {
            var prefab = constructible.GetComponentFast<Prefab>();
            return prefab is not null && IsWindtunnelPrefab(prefab);
        }

        private bool IsWindtunnelPrefab(Prefab prefab) {
            return prefab.PrefabName.Contains(windtunnelPrefabSubName, StringComparison.OrdinalIgnoreCase);
        }

        private const string windtunnelPrefabSubName = "Windtunnel";
        public const string flyById = "a011.flyBy";
    }

    public class Generator : IAchievementGenerator
    {
        public IEnumerable<AchievementDefinitionBase> Generate()
        {
            var definition = new AchievementSimpleDefinition(
                GameLogic.flyById,
                flyByImage,
                flyByTitle,
                flyByDescription
            );
        
            yield return definition;
        }

        private const string flyByTitle = "yurand.achievements.a011.flyBy.title";
        private const string flyByDescription = "yurand.achievements.a011.flyBy.description";
        private const string flyByImage = "yurand.achievements.a011.flyBy.image";
    }
}
