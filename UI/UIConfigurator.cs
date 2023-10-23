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

namespace Yurand.Timberborn.Achievements.UI
{
    [Configurator(SceneEntrypoint.InGame | SceneEntrypoint.MainMenu)]
    public class UIConfigurator : IConfigurator
    {
        public void Configure(IContainerDefinition containerDefinition)
        {
            containerDefinition.Bind<AchievementsMenu>().AsSingleton();
            containerDefinition.Bind<AchievementBoxFactory>().AsSingleton();
        }
    }
}
