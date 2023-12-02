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

namespace Yurand.Timberborn.Achievements.UI
{
    [Configurator(SceneEntrypoint.All)]
    public class UIConfigurator : IConfigurator
    {
        public void Configure(IContainerDefinition containerDefinition)
        {
            containerDefinition.Bind<AchievementsMenu>().AsSingleton();
            containerDefinition.Bind<AchievementBoxFactory>().AsSingleton();
            containerDefinition.Bind<ImageLoader>().AsSingleton();
        }
    }
    
    [Configurator(SceneEntrypoint.InGame)]
    public class GameUIConfigurator : IConfigurator
    {
        public void Configure(IContainerDefinition containerDefinition)
        {
            containerDefinition.Bind<AchievementCompletedAlert>().AsSingleton();
		    containerDefinition.MultiBind<AlertPanelModule>().ToProvider<AchievementCompletedAlertProvider>().AsSingleton();
        }
        private class AchievementCompletedAlertProvider : IProvider<AlertPanelModule>
        {
            private readonly AchievementCompletedAlert achievementCompletedAlert;

            public AchievementCompletedAlertProvider(AchievementCompletedAlert achievementCompletedAlert)
            {
                this.achievementCompletedAlert = achievementCompletedAlert;
            }

            public AlertPanelModule Get()
            {
                AlertPanelModule.Builder builder = new AlertPanelModule.Builder();
                builder.AddAlertFragment(achievementCompletedAlert, 141);
                return builder.Build();
            }
        }
    }
}
