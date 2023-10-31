using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.Common;

namespace Yurand.Timberborn.Achievements
{
    [Serializable]
    public class AchievementFailableDefinition : AchievementDefinitionBase {
        public AchievementFailableDefinition(string uniqueId, string imageFile, string localizedTitle, string localizedDescription)
            :base(uniqueId, imageFile, localizedTitle, localizedDescription) { }

        protected AchievementFailableDefinition() : base() { }

        public override string type => nameof(AchievementFailableDefinition);
    }
}