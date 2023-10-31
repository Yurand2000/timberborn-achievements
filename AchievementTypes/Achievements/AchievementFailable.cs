using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Timberborn.Persistence;

namespace Yurand.Timberborn.Achievements
{
    public class AchievementFailable : AchievementBase {
        public bool failed;
        public AchievementFailable(AchievementFailableDefinition definition, AchievementState state = AchievementState.NotCompleted)
            :base(definition, state == AchievementState.Completed)
        {
            this.failed = state == AchievementState.Failed;
        }

        public AchievementFailableDefinition GetDefinition() { return (AchievementFailableDefinition)definition; }

        public override void Update(AchievementBase.Updater updaterBase) {
            var updater = (Updater)updaterBase;
            if (updater is null) return;

            this.completed = updater.state == AchievementState.Completed;
            this.failed = updater.state == AchievementState.Failed;
        }

        public override void UpdateFromLocal(AchievementBase achievementBase) {
            var achievement = (AchievementFailable)achievementBase;
            if (achievement is null) return;
            if (achievement.completed) this.completed = true;
        }

        public enum AchievementState { NotCompleted, Failed, Completed }
        public new class Updater : AchievementBase.Updater {
            public AchievementState state;
        }
        public override bool IsSame(AchievementBase other)
        {
            var c_other = (AchievementFailable) other;            
            if (c_other is null) return false;

            return this.failed == c_other.failed && base.IsSame(c_other);
        }
    }

    [Serializable]
    public class SerializableAchievementFailable : SerializableAchievementBase
    {
        public bool failed = false;
        public SerializableAchievementFailable() : base() {}
        public SerializableAchievementFailable(SerializableAchievementBase other, bool failed) : base(other)
        {
            this.failed = failed;
        }
        public SerializableAchievementFailable(AchievementFailable achievement) : base(achievement)
        {
            this.failed = achievement.failed;
        }

        public override bool IsSame(SerializableAchievementBase other)
        {
            var c_other = (SerializableAchievementFailable) other;            
            if (c_other is null) return false;

            return this.failed == c_other.failed && base.IsSame(c_other);
        }
    }
}