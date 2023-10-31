using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Timberborn.Persistence;

namespace Yurand.Timberborn.Achievements
{
    public class AchievementWithCompletition : AchievementBase {
        public float current_state;
        public AchievementWithCompletition(AchievementWithCompletitionDefinition definition, bool completed = false, float current_state = 0f)
            :base(definition, completed)
        {
            this.current_state = current_state;
        }

        public AchievementWithCompletitionDefinition GetDefinition() { return (AchievementWithCompletitionDefinition)definition; }

        public override void Update(AchievementBase.Updater updaterBase) {
            var updater = (Updater)updaterBase;
            if (updater is null) return;

            var max = GetDefinition().max_completition;
            this.current_state = Math.Clamp(updater.next_state, 0, max);
            if (this.current_state == max) this.completed = true;

            this.completed = updater.force_complete ?? this.completed;
        }

        public override void UpdateFromLocal(AchievementBase achievementBase) {
            var achievement = (AchievementWithCompletition)achievementBase;
            if (achievement is null) return;
            if (achievement.current_state > this.current_state) this.current_state = achievement.current_state;
            if (achievement.completed) this.completed = true;
        }
        
        public new class Updater : AchievementBase.Updater {
            public float next_state;
            public bool? force_complete;
        }
        public override bool IsSame(AchievementBase other)
        {
            var c_other = (AchievementWithCompletition) other;            
            if (c_other is null) return false;

            return this.current_state == c_other.current_state && base.IsSame(c_other);
        }
    }

    [Serializable]
    public class SerializableAchievementWithCompletition : SerializableAchievementBase
    {
        [XmlElement("current_state")] public float current_state;
        public SerializableAchievementWithCompletition() : base() { current_state = 0; }
        public SerializableAchievementWithCompletition(SerializableAchievementBase other, float current_state) : base(other)
        {
            this.current_state = current_state;
        }
        public SerializableAchievementWithCompletition(AchievementWithCompletition achievement) : base(achievement)
        {
            this.current_state = achievement.current_state;
        }

        public override bool IsSame(SerializableAchievementBase other)
        {
            var c_other = (SerializableAchievementWithCompletition) other;            
            if (c_other is null) return false;

            return this.current_state == c_other.current_state && base.IsSame(c_other);
        }
    }
}