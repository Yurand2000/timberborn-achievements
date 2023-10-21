using Bindito.Core;
using HarmonyLib;
using System;
using System.Collections.Generic;
using TimberApi.ConfiguratorSystem;
using TimberApi.ConsoleSystem;
using TimberApi.DependencyContainerSystem;
using TimberApi.SceneSystem;
using Timberborn.Localization;
using UnityEngine.UIElements;
using Yurand.Timberborn.Achievements.UI;

namespace Yurand.Timberborn.Achievements
{
    [Configurator(SceneEntrypoint.MainMenu)]
    public class Patches : IConfigurator
    {
        public void Configure(IContainerDefinition containerDefinition) {
            var harmony = PluginEntryPoint.harmony;
            harmony.Patch(
                AccessTools.Method(mainMenuPanelClass + ":GetPanel"),
                postfix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(MainMenuPanelPostfix)))
            );

            harmony.Patch(
                AccessTools.Method(ingameOptionsPanelClass + ":GetPanel"),
                postfix: new HarmonyMethod(AccessTools.Method(GetType(), nameof(IngameMenuPanelPostfix)))
            );            
        }
        
        private static void MainMenuPanelPostfix(ref VisualElement __result)
        {
            var loc = DependencyContainer.GetInstance<ILoc>();
            VisualElement root = __result.Query("MainMenuPanel");
            Button button = new Button();
            button.AddToClassList("menu-button");
            button.text = loc.T(achievementsPanelMenuOptionLoc);
            button.clicked += AchievementsMenu.OpenOptionsDelegate;
            root.Insert(6, button);
        }
        
        private static void IngameMenuPanelPostfix(ref VisualElement __result)
        {
            var loc = DependencyContainer.GetInstance<ILoc>();
            VisualElement root = __result.Query("OptionsBox");
            Button button = new Button();
            button.AddToClassList("menu-button");
            button.text = loc.T(achievementsPanelMenuOptionLoc);
            button.clicked += AchievementsMenu.OpenOptionsDelegate;
            root.Insert(6, button);
        }

        private const string achievementsPanelMenuOptionLoc = "yurand.achievements.menu_option";

        private const string mainMenuPanelClass = "Timberborn.MainMenuScene.MainMenuPanel";
        private const string ingameOptionsPanelClass = "Timberborn.OptionsGame.GameOptionsBox";
    }
}