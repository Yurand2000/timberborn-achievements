using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.Common;

namespace Yurand.Timberborn.Achievements
{
    [Serializable]
    public class AchievementWithCompletitionTieredDefinition : AchievementDefinitionBase {
        public IReadOnlyList<float> max_completition_tiers { get; private set; }
        public bool as_integer { get; private set; }
        public AchievementWithCompletitionTieredDefinition(string uniqueId, string imageFile, string localizedTitle, string localizedDescription, List<float> max_completition_tiers, bool as_integer)
            :base(uniqueId, imageFile, localizedTitle, localizedDescription)
        {
            if ( max_completition_tiers.Count == 0 ) throw new ArgumentException("Max Completition List must not be empty.");

            var is_strict_sorted = max_completition_tiers
                .Zip(max_completition_tiers.Skip(1), (a, b) => (a, b))
                .All((pair) => { var (a, b) = pair; return a < b; });
            if ( !is_strict_sorted ) throw new ArgumentException("Max Completition List must be sorted, with no duplicates.");

            this.max_completition_tiers = max_completition_tiers;
            this.as_integer = as_integer;
        }

        protected AchievementWithCompletitionTieredDefinition() : base() { this.max_completition_tiers = new List<float>{ 0, 1 }; this.as_integer = true; }

        public override string type => nameof(AchievementWithCompletitionTieredDefinition);
    }
}