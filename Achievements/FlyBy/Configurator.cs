using Bindito.Core;
using TimberApi.ConfiguratorSystem;
using TimberApi.SceneSystem;

namespace Yurand.Timberborn.Achievements.FlyBy
{
    [Configurator(SceneEntrypoint.InGame)]
    public class LogicConfigurator : IConfigurator
    {
        public void Configure(IContainerDefinition containerDefinition)
        {
            containerDefinition.Bind<GameLogic>().AsSingleton();
        }
    }
    
    [Configurator(SceneEntrypoint.All)]
    public class DefinitionConfigurator : IConfigurator
    {
        public void Configure(IContainerDefinition containerDefinition)
        {
            containerDefinition.MultiBind<IAchievementGenerator>().To<Generator>().AsSingleton();
        }
    }
}
