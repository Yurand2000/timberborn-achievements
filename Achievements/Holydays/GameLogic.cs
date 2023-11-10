using System.Collections.Generic;
using HarmonyLib;
using TimberApi.ConsoleSystem;
using Timberborn.Beavers;
using Timberborn.Characters;
using Timberborn.SingletonSystem;

namespace Yurand.Timberborn.Achievements.Holydays
{
    public class GameLogic : ILoadableSingleton
    {
        private IConsoleWriter console;
        private AchievementManager manager;
        public GameLogic(IConsoleWriter console, AchievementManager manager) {
            this.console = console;
            this.manager = manager;
        }

        public void Load() {
            var today = System.DateTime.Now;
            if (today.Day == 31 && today.Month == 10) {
                manager.UpdateLocalAchievement(halloweenId, new AchievementHidden.Updater{ completed = true });
                if (PluginEntryPoint.debugLogging) console.LogInfo($"Completed Achievement {halloweenId}");
            }
            else if (today.Day == 1 && today.Month == 11) {
                manager.UpdateLocalAchievement(dayOfTheDeadId, new AchievementHidden.Updater{ completed = true });
                if (PluginEntryPoint.debugLogging) console.LogInfo($"Completed Achievement {dayOfTheDeadId}");
            }
            else if (today.Day == 25 && today.Month == 12) {
                manager.UpdateLocalAchievement(christmasId, new AchievementHidden.Updater{ completed = true });
                if (PluginEntryPoint.debugLogging) console.LogInfo($"Completed Achievement {christmasId}");
            }
            else if (today.Day == 6 && today.Month == 1) {
                manager.UpdateLocalAchievement(epiphanyId, new AchievementHidden.Updater{ completed = true });
                if (PluginEntryPoint.debugLogging) console.LogInfo($"Completed Achievement {epiphanyId}");
            }
        }

        public const string halloweenId = "a058.0.holydays.halloween";
        public const string dayOfTheDeadId = "a058.1.holydays.dayOfTheDead";
        public const string christmasId = "a058.2.holydays.christmas";
        public const string epiphanyId = "a058.3.holydays.epiphany";
    }

    [HarmonyPatch]
    public class Patcher {
        [HarmonyPostfix]
        [HarmonyPatch(typeof(AchievementManager), "LoadDefinitions")]
        public static void PatchLoadAchievementsDefinitions(ref List<AchievementDefinitionBase> __result) {
            __result.Add(new AchievementHiddenDefinition(GameLogic.halloweenId, halloweenImage, halloweenTitle, halloweenDescription));
            __result.Add(new AchievementHiddenDefinition(GameLogic.dayOfTheDeadId, dayOfTheDeadImage, dayOfTheDeadTitle, dayOfTheDeadDescription));
            __result.Add(new AchievementHiddenDefinition(GameLogic.christmasId, christmasImage, christmasTitle, christmasDescription));
            __result.Add(new AchievementHiddenDefinition(GameLogic.epiphanyId, epiphanyImage, epiphanyTitle, epiphanyDescription));
        }
        private const string halloweenTitle = "yurand.achievements.a058.0.holydays.halloween.title";
        private const string halloweenDescription = "yurand.achievements.a058.0.holydays.halloween.description";
        private const string halloweenImage = "yurand.achievements.a058.0.holydays.halloween.image";
        private const string dayOfTheDeadTitle = "yurand.achievements.a058.1.holydays.dayOfTheDead.title";
        private const string dayOfTheDeadDescription = "yurand.achievements.a058.1.holydays.dayOfTheDead.description";
        private const string dayOfTheDeadImage = "yurand.achievements.a058.1.holydays.dayOfTheDead.image";
        private const string christmasTitle = "yurand.achievements.a058.2.holydays.christmas.title";
        private const string christmasDescription = "yurand.achievements.a058.2.holydays.christmas.description";
        private const string christmasImage = "yurand.achievements.a058.2.holydays.christmas.image";
        private const string epiphanyTitle = "yurand.achievements.a058.3.holydays.epiphany.title";
        private const string epiphanyDescription = "yurand.achievements.a058.3.holydays.epiphany.description";
        private const string epiphanyImage = "yurand.achievements.a058.3.holydays.epiphany.image";
    }
}
