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

namespace Yurand.Timberborn.Achievements.NoCampingAllowed
{
    public class GameLogic : AchievementLogicBase
    {
        private int current_campfires = 0;
        public GameLogic(EventBus eventBus, IConsoleWriter console, AchievementManager manager)
            : base(eventBus, console, manager) { }
        
        [OnEvent]
        public void OnCampfireBuilt(ConstructibleEnteredFinishedStateEvent constructibleEvent) {
            if (IsCampfireConstructible(constructibleEvent.Constructible)) {
                current_campfires += 1;
            }

            if (current_campfires >= 3) {
                manager.UpdateLocalAchievement(noCampingAllowedId, new AchievementSimple.Updater{ completed = true });
                SetAchievementCompleted();
                debug_console.LogInfo($"Completed noCampingAllowed achievement");
            }
        }
        
        [OnEvent]
        public void OnCampfireDestroyed(ConstructibleExitedFinishedStateEvent constructibleEvent) {
            if (IsCampfireConstructible(constructibleEvent.Constructible)) {
                current_campfires -= 1;
            }
        }

        private bool IsCampfireConstructible(Constructible constructible) {
            var prefab = constructible.GetComponentFast<Prefab>();
            return prefab is not null && IsCampfirePrefab(prefab);
        }

        private bool IsCampfirePrefab(Prefab prefab) {
            return prefab.PrefabName.Contains(campfirePrefabSubName, StringComparison.OrdinalIgnoreCase);
        }

        private const string campfirePrefabSubName = "Campfire";
        public const string noCampingAllowedId = "a010.noCampingAllowed";
    }

    public class Generator : IAchievementGenerator
    {
        public IEnumerable<AchievementDefinitionBase> Generate()
        {
            var definition = new AchievementSimpleDefinition(
                GameLogic.noCampingAllowedId,
                noCampingAllowedIdImage,
                noCampingAllowedIdTitle,
                noCampingAllowedIdDescription
            );
        
            yield return definition;
        }

        private const string noCampingAllowedIdTitle = "yurand.achievements.a010.noCampingAllowed.title";
        private const string noCampingAllowedIdDescription = "yurand.achievements.a010.noCampingAllowed.description";
        private const string noCampingAllowedIdImage = "yurand.achievements.a010.noCampingAllowed.image";
    }
}
