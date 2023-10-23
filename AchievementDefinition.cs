using System;
using System.Collections.Generic;
using System.Linq;
using Timberborn.Common;

namespace Yurand.Timberborn.Achievements
{
    [Serializable]
    public abstract class AchievementDefinitionBase {
        public abstract string type { get; }
        public string uniqueId { get; private set; }
        public string imageFile { get; private set; }
        public string localizedTitle { get; private set; }
        public string localizedDescription { get; private set; }

        public AchievementDefinitionBase(string uniqueId, string imageFile, string localizedTitle, string localizedDescription) {
            this.uniqueId = uniqueId;
            this.imageFile = imageFile;
            this.localizedTitle = localizedTitle;
            this.localizedDescription = localizedDescription;
        }
    }

    [Serializable]
    public class AchievementSimpleDefinition : AchievementDefinitionBase {
        public AchievementSimpleDefinition(string uniqueId, string imageFile, string localizedTitle, string localizedDescription)
            :base(uniqueId, imageFile, localizedTitle, localizedDescription) { }

        public override string type => nameof(AchievementSimpleDefinition);
    }

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

        public override string type => nameof(AchievementWithCompletitionDefinition);
    }

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

        public override string type => nameof(AchievementWithCompletitionTieredDefinition);
    }
}