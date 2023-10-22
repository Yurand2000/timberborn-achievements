using Bindito.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimberApi.ConfiguratorSystem;
using TimberApi.SceneSystem;
using Timberborn.EntityPanelSystem;

namespace Yurand.Timberborn.Achievements
{
    [Configurator(SceneEntrypoint.InGame)]
    public class InGameConfigurator : IConfigurator
    {
        public void Configure(IContainerDefinition containerDefinition)
        {
            containerDefinition.Bind<AchievementManagerInGame>().AsSingleton();
            containerDefinition.Bind<DebugConsoleMethods>().AsSingleton();
            containerDefinition.Bind<SetAchievementStateMenu>().AsSingleton();
        }
    }
    
    [Configurator(SceneEntrypoint.All)]
    public class ManagerConfigurator : IConfigurator
    {
        public void Configure(IContainerDefinition containerDefinition)
        {
            containerDefinition.Bind<AchievementManager>().AsSingleton();
        }
    }
}
