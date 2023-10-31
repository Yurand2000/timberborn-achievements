using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Timberborn.Persistence;

namespace Yurand.Timberborn.Achievements
{
    public class AchievementWithCompletitionTiered : AchievementBase {
        public float current_state;
        public AchievementWithCompletitionTiered(AchievementWithCompletitionTieredDefinition definition, bool completed = false, float current_state = 0f)
            :base(definition, completed)
        {
            this.current_state = current_state;
        }

        public AchievementWithCompletitionTieredDefinition GetDefinition() { return (AchievementWithCompletitionTieredDefinition)definition; }

        public override void Update(AchievementBase.Updater updaterBase) {
            var updater = (Updater)updaterBase;
            if (updater is null) return;

            var max = GetMaxCompletition();
            this.current_state = Math.Clamp(updater.next_state, 0, max);
            if (this.current_state == max) this.completed = true;

            this.completed = updater.force_complete ?? this.completed;
        }

        public override void UpdateFromLocal(AchievementBase achievementBase) {
            var achievement = (AchievementWithCompletitionTiered)achievementBase;
            if (achievement is null) return;
            if (achievement.current_state > this.current_state) this.current_state = achievement.current_state;
            if (achievement.completed) this.completed = true;
        }
        
        public new class Updater : AchievementBase.Updater {
            public float next_state;
            public bool? force_complete;
        }

        public int? getLastCompletedTier() {
            var tiers = GetDefinition().max_completition_tiers.Count;

            if (current_state < GetDefinition().max_completition_tiers[0]) return null;
            for (int tier = 1; tier < tiers; tier++) {
                if (current_state < GetDefinition().max_completition_tiers[tier]) return tier - 1;
            }

            return tiers;
        }

        private float GetMaxCompletition() {
            return GetDefinition().max_completition_tiers.Last();
        }

        public override bool IsSame(AchievementBase other)
        {
            var c_other = (AchievementWithCompletitionTiered) other;            
            if (c_other is null) return false;

            return this.current_state == c_other.current_state && base.IsSame(c_other);
        }
    }

    [Serializable]
    public class SerializableAchievementWithCompletitionTiered : SerializableAchievementBase
    {
        [XmlElement("current_state")] public float current_state;
        public SerializableAchievementWithCompletitionTiered() : base() { current_state = 0; }
        public SerializableAchievementWithCompletitionTiered(SerializableAchievementBase other, float current_state) : base(other)
        {
            this.current_state = current_state;
        }
        public SerializableAchievementWithCompletitionTiered(AchievementWithCompletitionTiered achievement) : base(achievement)
        {
            this.current_state = achievement.current_state;
        }

        public override bool IsSame(SerializableAchievementBase other)
        {
            var c_other = (SerializableAchievementWithCompletitionTiered) other;            
            if (c_other is null) return false;

            return this.current_state == c_other.current_state && base.IsSame(c_other);
        }
    }
}