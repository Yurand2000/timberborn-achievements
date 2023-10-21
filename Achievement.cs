using System;

namespace Yurand.Timberborn.Achievements
{
    public struct Achievement {
        public AchievementDefinition definition;
        public bool completed;
        public float? current_value;

        public SerializableAchievement ToSerializable() {
            return new SerializableAchievement(definition.uniqueId, completed, current_value);
        }
    }

    [Serializable]
    public class SerializableAchievement {
        public string achievementId;        
        public bool completed;
        public float current_value;

        public SerializableAchievement(string achievementId, bool completed, float? current_value) {
            this.achievementId = achievementId;
            this.completed = completed;
            this.current_value = current_value ?? 0;
        }
    }
}