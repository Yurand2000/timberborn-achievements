using System.Collections.Generic;
using HarmonyLib;
using TimberApi.ConsoleSystem;
using Timberborn.Beavers;
using Timberborn.Characters;
using Timberborn.SingletonSystem;

namespace Yurand.Timberborn.Achievements.ItsASoccerTeam
{
    public class GameLogic : ILoadableSingleton
    {
        private EventBus eventBus;
        private IConsoleWriter console;
        private AchievementManager manager;
        private int current_children;
        public GameLogic(EventBus eventBus, IConsoleWriter console, AchievementManager manager) {
            this.eventBus = eventBus;
            this.console = console;
            this.manager = manager;
            this.current_children = 0;
        }

        public void Load() {
            eventBus.Register(this);
        }

        [OnEvent]
        public void OnCharacterCreated(CharacterCreatedEvent characterCreatedEvent)
        {
            Child componentFast = characterCreatedEvent.Character.GetComponentFast<Child>();
            if ((object)componentFast != null) {
                current_children += 1;

                if (current_children >= 11) {
                    manager.UpdateLocalAchievement(itsASoccerTeamId, new AchievementHidden.Updater{ completed = true });
                    eventBus.Unregister(this);

                    if (PluginEntryPoint.debugLogging) {
                        console.LogInfo("Completed itsASoccerTeam achievement.");
                    }
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

    [HarmonyPatch]
    public class Patcher {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AchievementManager), "LoadDefinitions")]
        public static void PatchLoadAchievementsDefinitions(ref List<AchievementDefinitionBase> __result) {
            __result.Add(new AchievementHiddenDefinition(GameLogic.itsASoccerTeamId, itsASoccerTeamImage, itsASoccerTeamTitle, itsASoccerTeamDescription));
        }
        private const string itsASoccerTeamTitle = "yurand.achievements.a049.itsASoccerTeam.title";
        private const string itsASoccerTeamDescription = "yurand.achievements.a049.itsASoccerTeam.description";
        private const string itsASoccerTeamImage = "yurand.achievements.a049.itsASoccerTeam.image";
    }
}
