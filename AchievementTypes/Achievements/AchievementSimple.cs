using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Timberborn.Persistence;

namespace Yurand.Timberborn.Achievements
{
    public class AchievementSimple : AchievementBase {
        public AchievementSimple(AchievementSimpleDefinition definition, bool completed = false)
            :base(definition, completed) { }

        public AchievementSimpleDefinition GetDefinition() { return (AchievementSimpleDefinition)definition; }

        public override void Update(AchievementBase.Updater updaterBase) {
            var updater = (Updater)updaterBase;
            if (updater is null) return;

            this.completed = updater.completed;
        }

        public override void UpdateFromLocal(AchievementBase achievementBase) {
            var achievement = (AchievementSimple)achievementBase;
            if (achievement is null) return;
            if (achievement.completed) this.completed = true;
        }

        public new class Updater : AchievementBase.Updater {
            public bool completed;
        }
        public override bool IsSame(AchievementBase other)
        {
            var c_other = (AchievementSimple) other;            
            if (c_other is null) return false;

            return base.IsSame(c_other);
        }
    }
    
    [Serializable]
    public class SerializableAchievementSimple : SerializableAchievementBase
    {
        public SerializableAchievementSimple() : base() {}
        public SerializableAchievementSimple(SerializableAchievementBase other) : base(other) {}
        public SerializableAchievementSimple(AchievementSimple achievement) : base(achievement) { }

        public override bool IsSame(SerializableAchievementBase other)
        {
            var c_other = (SerializableAchievementSimple) other;            
            if (c_other is null) return false;

            return base.IsSame(c_other);
        }
    }
}