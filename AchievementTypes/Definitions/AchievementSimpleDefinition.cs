using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.Common;

namespace Yurand.Timberborn.Achievements
{
    [Serializable]
    public class AchievementSimpleDefinition : AchievementDefinitionBase {
        public AchievementSimpleDefinition(string uniqueId, string imageFile, string localizedTitle, string localizedDescription)
            :base(uniqueId, imageFile, localizedTitle, localizedDescription) { }

        protected AchievementSimpleDefinition() : base() { }

        public override string type => nameof(AchievementSimpleDefinition);
    }
}