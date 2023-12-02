using Bindito.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimberApi.ConfiguratorSystem;
using TimberApi.SceneSystem;
using TimberApi.DependencyContainerSystem;
using Timberborn.Localization;
using UnityEngine.UIElements;
using Timberborn.EntityPanelSystem;
using HarmonyLib;
using Timberborn.AlertPanelSystem;
using Timberborn.CoreUI;
using Timberborn.SingletonSystem;
using Timberborn.GameSound;
using Timberborn.Beavers;
using TimberApi.UiBuilderSystem.CustomElements;

namespace Yurand.Timberborn.Achievements.UI
{
    public class AchievementCompletedEvent {
        public readonly string achievement_name;
        public AchievementCompletedEvent(string achievement_name) {
            this.achievement_name = achievement_name;
        }
    }
}