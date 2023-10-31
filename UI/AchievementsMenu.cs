using System;
using System.IO;
using TimberApi.AssetSystem;
using TimberApi.AssetSystem.Exceptions;
using TimberApi.ConsoleSystem;
using TimberApi.UiBuilderSystem;
using TimberApi.UiBuilderSystem.ElementSystem;
using Timberborn.Common;
using Timberborn.CoreUI;
using Timberborn.Localization;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.UIElements.LengthUnit;

namespace Yurand.Timberborn.Achievements.UI
{
    public class AchievementsMenu : IPanelController
    {
        public static Action OpenOptionsDelegate;
        private UIBuilder uiBuilder;
        private PanelStack panelStack;
        private AchievementBoxFactory achievementBoxFactory;
        private AchievementManager achievementManager;
        private IConsoleWriter console;

        public AchievementsMenu(
            UIBuilder uiBuilder,
            PanelStack panelStack,
            AchievementBoxFactory achievementBoxFactory,
            AchievementManager achievementManager,
            IConsoleWriter console
        ) {
            this.uiBuilder = uiBuilder;
            this.panelStack = panelStack;
            this.achievementBoxFactory = achievementBoxFactory;
            this.achievementManager = achievementManager;
            this.console = console;
            OpenOptionsDelegate = OpenOptionsPanel;

            if (PluginEntryPoint.debugLogging) {
                console.LogInfo("Anchievements Menu Initialized Successfully");
            }
        }

        private void OpenOptionsPanel() {
            panelStack.HideAndPush(this);

            if (PluginEntryPoint.debugLogging) {
                console.LogInfo("Achievements Menu Opened");
            }
        }

        public VisualElement GetPanel()
        {
            UIBoxBuilder panelBuilder = uiBuilder.CreateBoxBuilder()
                .SetWidth(new Length(800, Pixel))
                .SetHeight(new Length(600, Pixel))
                .ModifyScrollView(builder => builder
                    .SetJustifyContent(Justify.Center)
                    .SetAlignItems(Align.Center)
                );

            var definitions = achievementManager.GetAchievementDefinitions();

            foreach(var key in definitions.Keys) {
                var global_achievement = achievementManager.GetGlobalAchievement(key);
                var local_achievement = achievementManager.GetLocalAchievement(key);
                
                var achievementBox = achievementBoxFactory.MakeAchievementBox(global_achievement, local_achievement);
                if (achievementBox is not null)
                    panelBuilder.AddComponent(achievementBox.Build());
            }

            VisualElement root = panelBuilder
                .AddCloseButton("CloseButton")
                .SetBoxInCenter()
                .AddHeader(achievementsPanelHeaderLoc)
                .BuildAndInitialize();

            root.Q<Button>("CloseButton").clicked += OnUICancelled;

            return root;
        }

        public void OnUICancelled() {
            panelStack.Pop(this);
            achievementManager.SaveGlobal();

            if (PluginEntryPoint.debugLogging) {
                console.LogInfo("Achievements Menu Closed");
            }
        }

        public bool OnUIConfirmed() {
            return false;
        }

        private const string achievementsPanelHeaderLoc = "yurand.achievements.panel_header";
    }
}