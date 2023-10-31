using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Timberborn.Persistence;

namespace Yurand.Timberborn.Achievements
{
    public abstract class AchievementBase {
        public AchievementDefinitionBase definition { get; private set; }
        public bool completed;

        public AchievementBase(AchievementDefinitionBase definition, bool completed = false) {
            this.definition = definition;
            this.completed = completed;
        }

        public abstract void Update(Updater updater);
        public abstract void UpdateFromLocal(AchievementBase achievement);
        public class Updater {}

        public virtual bool IsSame(AchievementBase other) {
            return this.definition == other.definition &&
                this.completed == other.completed;
        }
    }

    [Serializable]
    public class SerializableAchievementBase {
        [XmlAttribute("id")] public string achievementId;
        [XmlAttribute("completed")] public bool completed;
        public SerializableAchievementBase() { achievementId = null; completed = false; }
        public SerializableAchievementBase(string achievementId, bool completed)
        {
            this.achievementId = achievementId;
            this.completed = completed;
        }
        public SerializableAchievementBase(AchievementBase achievementBase)
            : this(achievementBase.definition.uniqueId, achievementBase.completed) {}

        public SerializableAchievementBase(SerializableAchievementBase other)
        {
            this.achievementId = other.achievementId;
            this.completed = other.completed;
        }

        public virtual bool IsSame(SerializableAchievementBase other) {
            return this.achievementId == other.achievementId &&
                this.completed == other.completed;
        }
    }
}