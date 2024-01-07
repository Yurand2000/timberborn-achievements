using System;
using System.Collections.Generic;
using System.Linq;
using TimberApi.ModSystem;
using Timberborn.Common;

namespace Yurand.Timberborn.Achievements
{
    [Serializable]
    public class AchievementHiddenDefinition : AchievementSimpleDefinition {
        public AchievementHiddenDefinition(string uniqueId, string imageFile, string localizedTitle, string localizedDescription)
            :base(uniqueId, imageFile, localizedTitle, localizedDescription) { }
        public AchievementHiddenDefinition(string uniqueId, string imageFile, bool imageFromDefaultDirectory, string localizedTitle, string localizedDescription, IMod mod)
            :base(uniqueId, imageFile, imageFromDefaultDirectory, localizedTitle, localizedDescription, mod) { }

        protected AchievementHiddenDefinition() : base() { }

        public override string type => nameof(AchievementHiddenDefinition);
    }
}