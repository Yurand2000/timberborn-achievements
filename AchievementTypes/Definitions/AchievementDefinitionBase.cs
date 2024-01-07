using System;
using System.Collections.Generic;
using System.Linq;
using TimberApi.ModSystem;
using Timberborn.Common;

namespace Yurand.Timberborn.Achievements
{
    [Serializable]
    public abstract class AchievementDefinitionBase {
        public abstract string type { get; }
        public string uniqueId { get; private set; }
        public string imageFile { get; private set; }
        public bool imageDefaultDirectory { get; private set; }
        public string localizedTitle { get; private set; }
        public string localizedDescription { get; private set; }
        public IMod mod { get; private set; }

        public AchievementDefinitionBase(
            string uniqueId,
            string imageFile,
            string localizedTitle,
            string localizedDescription
        ) {
            this.uniqueId = uniqueId;
            this.imageFile = imageFile;
            this.localizedTitle = localizedTitle;
            this.imageDefaultDirectory = true;
            this.localizedDescription = localizedDescription;
            this.mod = null;
        }

        public AchievementDefinitionBase(
            string uniqueId,
            string imageFile,
            bool imageFromDefaultDirectory,
            string localizedTitle,
            string localizedDescription,
            IMod mod
        ) {
            this.uniqueId = uniqueId;
            this.imageFile = imageFile;
            this.localizedTitle = localizedTitle;
            this.imageDefaultDirectory = imageFromDefaultDirectory;
            this.localizedDescription = localizedDescription;
            this.mod = mod;
        }

        protected AchievementDefinitionBase() {
            this.uniqueId = "mockAchievement";
            this.imageFile = "mockImage";
            this.localizedTitle = "mockTitle";
            this.localizedDescription = "mockDescription";
            this.imageDefaultDirectory = true;
            this.mod = null;
        }
    }
}