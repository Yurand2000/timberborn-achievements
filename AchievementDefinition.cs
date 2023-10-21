using System;

namespace Yurand.Timberborn.Achievements
{
    [Serializable]
    public struct AchievementDefinition {
        public string uniqueId;
        public string imageFile;
        public string localizedTitle;
        public string localizedDescription;
        public AchievementStatusDefinition? statusDefinition;

        public AchievementDefinition(string uniqueId, string imageFile, string localizedTitle, string localizedDescription) {
            this.uniqueId = uniqueId;
            this.imageFile = imageFile;
            this.localizedTitle = localizedTitle;
            this.localizedDescription = localizedDescription;
            this.statusDefinition = null;
        }
        
        public AchievementDefinition(string uniqueId, string imageFile, string localizedTitle, string localizedDescription, float max_value, bool is_integer)
            : this(uniqueId, imageFile, localizedTitle, localizedDescription) {
            this.statusDefinition = new AchievementStatusDefinition {
                max_value = max_value,
                is_integer = is_integer
            };
        }

        [Serializable]
        public struct AchievementStatusDefinition {
            public float max_value;
            public bool is_integer;
        }
    }
}