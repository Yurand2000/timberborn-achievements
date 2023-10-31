using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.Common;

namespace Yurand.Timberborn.Achievements
{
    [Serializable]
    public class AchievementWithCompletitionDefinition : AchievementDefinitionBase {
        public float max_completition {get; private set; }
        public bool as_integer { get; private set; }
        public AchievementWithCompletitionDefinition(string uniqueId, string imageFile, string localizedTitle, string localizedDescription, float max_completition, bool as_integer)
            :base(uniqueId, imageFile, localizedTitle, localizedDescription)
        {
            this.max_completition = max_completition;
            this.as_integer = as_integer;
        }

        protected AchievementWithCompletitionDefinition() : base() { this.max_completition = 0; this.as_integer = true; }

        public override string type => nameof(AchievementWithCompletitionDefinition);
    }
}