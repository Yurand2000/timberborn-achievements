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

        protected AchievementDefinitionBase() {
            this.uniqueId = "mockAchievement";
            this.imageFile = "mockImage";
            this.localizedTitle = "mockTitle";
            this.localizedDescription = "mockDescription";
        }
    }
}