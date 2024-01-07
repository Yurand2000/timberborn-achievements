using System;
using System.Collections.Generic;
using System.Linq;
using TimberApi.ModSystem;
using Timberborn.Common;

namespace Yurand.Timberborn.Achievements
{
    [Serializable]
    public class AchievementWithCompletitionDefinition : AchievementDefinitionBase {
        public float max_completition {get; private set; }
        public bool as_integer { get; private set; }

        private void Constructor(float max_completition, bool as_integer) {
            this.max_completition = max_completition;
            this.as_integer = as_integer;
        }
        public AchievementWithCompletitionDefinition(string uniqueId, string imageFile, string localizedTitle, string localizedDescription, float max_completition, bool as_integer)
            :base(uniqueId, imageFile, localizedTitle, localizedDescription)
        {
            Constructor(max_completition, as_integer);
        }
        public AchievementWithCompletitionDefinition(string uniqueId, string imageFile, bool imageFromDefaultDirectory, string localizedTitle, string localizedDescription, IMod mod, float max_completition, bool as_integer)
            :base(uniqueId, imageFile, imageFromDefaultDirectory, localizedTitle, localizedDescription, mod)
        {
            Constructor(max_completition, as_integer);
        }

        protected AchievementWithCompletitionDefinition() : base() { this.max_completition = 0; this.as_integer = true; }

        public override string type => nameof(AchievementWithCompletitionDefinition);
    }
}