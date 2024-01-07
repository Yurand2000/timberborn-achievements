using System.Collections.Generic;

namespace Yurand.Timberborn.Achievements
{
    public interface IAchievementGenerator
    {
        IEnumerable<AchievementDefinitionBase> Generate();
    }
}