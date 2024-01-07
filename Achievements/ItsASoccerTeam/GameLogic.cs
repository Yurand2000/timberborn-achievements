using System.Collections.Generic;
using HarmonyLib;
using TimberApi.ConsoleSystem;
using Timberborn.Beavers;
using Timberborn.Characters;
using Timberborn.SingletonSystem;

namespace Yurand.Timberborn.Achievements.ItsASoccerTeam
{
    public class GameLogic : AchievementLogicBase
    {
        private int current_children = 0;
        public GameLogic(EventBus eventBus, IConsoleWriter console, AchievementManager manager)
            : base(eventBus, console, manager) { }

        [OnEvent]
        public void OnCharacterCreated(CharacterCreatedEvent characterCreatedEvent)
        {
            Child componentFast = characterCreatedEvent.Character.GetComponentFast<Child>();
            if ((object)componentFast != null) {
                current_children += 1;

                if (current_children >= 11) {
                    manager.UpdateLocalAchievement(itsASoccerTeamId, new AchievementHidden.Updater{ completed = true });
                    SetAchievementCompleted();

                    debug_console.LogInfo("Completed itsASoccerTeam achievement.");
                }
            }
        }

        [OnEvent]
        public void OnCharacterKilled(CharacterKilledEvent characterKilledEvent)
        {
            Child componentFast = characterKilledEvent.Character.GetComponentFast<Child>();
            if ((object)componentFast != null) {
                current_children -= 1;
            }
        }

        public const string itsASoccerTeamId = "a049.itsASoccerTeam";
    }

    public class Generator : IAchievementGenerator
    {
        public IEnumerable<AchievementDefinitionBase> Generate()
        {
            var definition = new AchievementHiddenDefinition(
                GameLogic.itsASoccerTeamId,
                itsASoccerTeamImage,
                itsASoccerTeamTitle,
                itsASoccerTeamDescription
            );        
        
            yield return definition;
        }
        private const string itsASoccerTeamTitle = "yurand.achievements.a049.itsASoccerTeam.title";
        private const string itsASoccerTeamDescription = "yurand.achievements.a049.itsASoccerTeam.description";
        private const string itsASoccerTeamImage = "yurand.achievements.a049.itsASoccerTeam.image";
    }
}
