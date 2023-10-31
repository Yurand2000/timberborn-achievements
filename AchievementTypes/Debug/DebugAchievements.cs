using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using TimberApi.ConsoleSystem;
using Timberborn.BaseComponentSystem;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.AI;

namespace Yurand.Timberborn.Achievements
{
    public partial class AchievementManager
    {
        private List<AchievementDefinitionBase> LoadDebugDefinitions() {
            var list = new List<AchievementDefinitionBase>{
                new AchievementSimpleDefinition("achievementSimple", "null", "achievementSimpleTitle", "achievementSimpleDescription"),
                new AchievementSimpleDefinition("achievementSimple2", "null", "achievementSimpleTitle2", "achievementSimpleDescription"),
                new AchievementHiddenDefinition("achievementHidden", "null", "achievementHiddenTitle", "achievementHiddenDescription"),
                new AchievementHiddenDefinition("achievementHidden2", "null", "achievementHiddenTitle2", "achievementHiddenDescription"),
                new AchievementFailableDefinition("achievementFailable", "null", "achievementFailableTitle", "achievementFailableDescription"),
                new AchievementFailableDefinition("achievementFailable2", "null", "achievementFailableTitle2", "achievementFailableDescription"),
                new AchievementFailableDefinition("achievementFailable3", "null", "achievementFailableTitle3", "achievementFailableDescription"),
                new AchievementWithCompletitionDefinition("achievementWithCompletition", "null", "achievementWithCompletitionTitle", "achievementWithCompletitionDescription", 10f, true),
                new AchievementWithCompletitionDefinition("achievementWithCompletition2", "null", "achievementWithCompletitionTitle", "achievementWithCompletitionDescription", 10f, true),
                new AchievementWithCompletitionTieredDefinition("achievementWithCompletitionTiered", "null", "achievementWithCompletitionTieredTitle", "achievementWithCompletitionTieredDescription", new List<float>{10f, 20f}, false),
                new AchievementWithCompletitionTieredDefinition("achievementWithCompletitionTiered2", "null", "achievementWithCompletitionTieredTitle2", "achievementWithCompletitionTieredDescription", new List<float>{10f, 20f}, false),
                new AchievementWithCompletitionTieredDefinition("achievementWithCompletitionTiered3", "null", "achievementWithCompletitionTieredTitle3", "achievementWithCompletitionTieredDescription", new List<float>{10f, 20f}, false),
            };

            return list;
        }
    }
    
    public partial class AchievementManagerInGame
    {
        private void SetDebugAchievementGlobalState() {
            manager.UpdateLocalAchievement("achievementSimple2", new AchievementSimple.Updater(){ completed = true });
            manager.UpdateLocalAchievement("achievementHidden2", new AchievementHidden.Updater(){ completed = true });
            manager.UpdateLocalAchievement("achievementFailable2", new AchievementFailable.Updater(){ state = AchievementFailable.AchievementState.Completed });
            manager.UpdateLocalAchievement("achievementFailable3", new AchievementFailable.Updater(){ state = AchievementFailable.AchievementState.Failed });
            manager.UpdateLocalAchievement("achievementWithCompletition2", new AchievementWithCompletition.Updater(){ next_state = 10f });
            manager.UpdateLocalAchievement("achievementWithCompletitionTiered2", new AchievementWithCompletitionTiered.Updater(){ next_state = 10f });
            manager.UpdateLocalAchievement("achievementWithCompletitionTiered3", new AchievementWithCompletitionTiered.Updater(){ next_state = 20f });
        }
    }
}