using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Timberborn.Persistence;

namespace Yurand.Timberborn.Achievements
{
    public class AchievementHidden : AchievementSimple {
        public AchievementHidden(AchievementHiddenDefinition definition, bool completed = false)
            :base(definition, completed) { }

        public new AchievementHiddenDefinition GetDefinition() { return (AchievementHiddenDefinition)definition; }

        public new class Updater : AchievementSimple.Updater {}
        public override bool IsSame(AchievementBase other)
        {
            var c_other = (AchievementHidden) other;            
            if (c_other is null) return false;

            return base.IsSame(c_other);
        }
    }
    
    [Serializable]
    public class SerializableAchievementHidden : SerializableAchievementSimple
    {
        public SerializableAchievementHidden() : base() {}
        public SerializableAchievementHidden(SerializableAchievementBase other) : base(other) {}
        public SerializableAchievementHidden(AchievementHidden achievement) : base(achievement) { }

        public override bool IsSame(SerializableAchievementBase other)
        {
            var c_other = (SerializableAchievementHidden) other;            
            if (c_other is null) return false;

            return base.IsSame(c_other);
        }
    }
}