using Bindito.Core;
using TimberApi.ConfiguratorSystem;
using TimberApi.SceneSystem;

namespace Yurand.Timberborn.Achievements.BeaverPopulationAchievement
{
    [Configurator(SceneEntrypoint.InGame)]
    public class Configurator : IConfigurator
    {
        public void Configure(IContainerDefinition containerDefinition)
        {
            containerDefinition.Bind<GameLogic>().AsSingleton();
        }
    }
}
