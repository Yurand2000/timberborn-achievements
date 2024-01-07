using Bindito.Core;
using TimberApi.ConfiguratorSystem;
using TimberApi.SceneSystem;

namespace Yurand.Timberborn.Achievements.ExampleAchievement
{
    [Configurator(SceneEntrypoint.InGame)]
    public class Configurator : IConfigurator
    {
        public void Configure(IContainerDefinition containerDefinition)
        {
            if (AchievementManager.loadTestAchievements)
                containerDefinition.Bind<GameLogic>().AsSingleton();
        }
    }
    
    [Configurator(SceneEntrypoint.All)]
    public class DefinitionConfigurator : IConfigurator
    {
        public void Configure(IContainerDefinition containerDefinition)
        {
            if (AchievementManager.loadTestAchievements)
                containerDefinition.MultiBind<IAchievementGenerator>().To<Generator>().AsSingleton();
        }
    }
}
