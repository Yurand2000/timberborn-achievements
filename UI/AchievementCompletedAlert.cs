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
    public class AchievementCompletedAlert : IAlertFragment
    {
	    private readonly VisualElementLoader visualElementLoader;
        private readonly EventBus eventBus;
        private readonly GameUISoundController gameUISoundController;
        private readonly ILoc loc;
        private VisualElement root;
        private Label info_label;

        public AchievementCompletedAlert(VisualElementLoader visualElementLoader, EventBus eventBus, GameUISoundController gameUISoundController, ILoc loc)
        {
            this.visualElementLoader = visualElementLoader;
            this.eventBus = eventBus;
            this.gameUISoundController = gameUISoundController;
            this.loc = loc;
        }
        public VisualElement InitializeAlertFragment()
        {
            root = visualElementLoader.LoadVisualElement("Game/AlertPanel/WellbeingHighscoreAlertFragment");
            root.Q<Button>("Goals").clicked += AchievementsMenu.OpenOptionsDelegate;
            root.Q<Label>("Text").text = loc.T(achievementButtonText);
            info_label = root.Q<Label>("Info");
            info_label.text = loc.T(achievementCompletedInfo) + "achievement_name_here";
            root.Q("WellbeingScore").ToggleDisplayStyle(false); //hide wellbeing score button.
            root.Q<Button>("HideButton").clicked += delegate { Hide(); };
            eventBus.Register(this);
            Hide();
            return root;
        }

        [OnEvent]
        public void UpdateAlert(AchievementCompletedEvent ev) {
            info_label.text = loc.T(achievementCompletedInfo) + loc.T(ev.achievement_name);
            root.ToggleDisplayStyle(true);
            gameUISoundController.PlayWellbeingHighscoreSound();
        }

        private void PrintChildren(VisualElement elem, string depth) {
            foreach (var child in elem.Children()) {
                PluginEntryPoint.console.LogInfo($"{depth}- {child.name} : {child.GetType().Name}");
                PrintChildren(child, depth + "  ");
            }
        }

        private void Hide() {
            root.ToggleDisplayStyle(false);
        }

        public void UpdateAlertFragment() { }

        private const string achievementCompletedInfo = "yurand.achievements.achievementCompleted.info";
        private const string achievementButtonText = "yurand.achievements.achievementCompleted.button";
    }
}
