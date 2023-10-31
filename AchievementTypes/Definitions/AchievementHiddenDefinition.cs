using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.Common;

namespace Yurand.Timberborn.Achievements
{
    [Serializable]
    public class AchievementHiddenDefinition : AchievementSimpleDefinition {
        public AchievementHiddenDefinition(string uniqueId, string imageFile, string localizedTitle, string localizedDescription)
            :base(uniqueId, imageFile, localizedTitle, localizedDescription) { }

        protected AchievementHiddenDefinition() : base() { }

        public override string type => nameof(AchievementHiddenDefinition);
    }
}