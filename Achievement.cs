using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Timberborn.Persistence;

namespace Yurand.Timberborn.Achievements
{
    public struct Achievement {
        public AchievementDefinition definition;
        public bool completed;
        public float? current_value;

        public Achievement(IDictionary<string, AchievementDefinition> definitions, SerializableAchievement achievement) {
            definition = definitions[achievement.achievementId];
            completed = achievement.completed;
            current_value = definition.statusDefinition.HasValue ? achievement.current_value : null;
        }

        public SerializableAchievement ToSerializable() {
            return new SerializableAchievement(definition.uniqueId, completed, current_value);
        }
    }

    [Serializable]
    public struct SerializableAchievement {
        [XmlAttribute("id")] public string achievementId;    
        [XmlAttribute("completed")] public bool completed;
        [XmlAttribute("max_completition")] public float current_value;

        public SerializableAchievement(string achievementId, bool completed, float? current_value) {
            this.achievementId = achievementId;
            this.completed = completed;
            this.current_value = current_value ?? 0;
        }
    }

    public struct SerializableAchievementSerializer : IObjectSerializer<SerializableAchievement>
    {
        public Obsoletable<SerializableAchievement> Deserialize(IObjectLoader objectLoader)
        {
            var achievement = new SerializableAchievement();
            achievement.achievementId = objectLoader.GetValueOrNull(achievementIdKey);
            achievement.completed = objectLoader.GetValueOrNullable(completedKey) ?? false;
            achievement.current_value = objectLoader.GetValueOrNullable(currentValueKey) ?? 0;
            if (achievement.achievementId == null) {
                return new Obsoletable<SerializableAchievement>();
            } else {
                return achievement;
            }
        }

        public void Serialize(SerializableAchievement value, IObjectSaver objectSaver)
        {
            objectSaver.Set(achievementIdKey, value.achievementId);
            objectSaver.Set(completedKey, value.completed);
            objectSaver.Set(currentValueKey, value.current_value);
        }

        private static readonly PropertyKey<string> achievementIdKey = new PropertyKey<string>("achievementIdKey");
        private static readonly PropertyKey<bool> completedKey = new PropertyKey<bool>("completedKey");
        private static readonly PropertyKey<float> currentValueKey = new PropertyKey<float>("currentValueKey");
    }

    [Serializable]
    [XmlRoot("Achievements")]
    public struct SerializableAchievements {
        [XmlElement(ElementName = "Achievement")] public SerializableAchievement[] achievements;
        public SerializableAchievements(SerializableAchievement[] achievements) {
            this.achievements = achievements;
        }
    }
    
    public struct SerializableAchievementsSerializer : IObjectSerializer<SerializableAchievements>
    {
        public Obsoletable<SerializableAchievements> Deserialize(IObjectLoader objectLoader)
        {
            var achievements = new SerializableAchievements();
            achievements.achievements = objectLoader.Get(achievementsListKey, new SerializableAchievementSerializer()).ToArray();
            return achievements;
        }

        public void Serialize(SerializableAchievements value, IObjectSaver objectSaver)
        {
            objectSaver.Set(achievementsListKey, value.achievements.ToList().AsReadOnly(), new SerializableAchievementSerializer());
        }

        private static readonly ListKey<SerializableAchievement> achievementsListKey = new ListKey<SerializableAchievement>("achievementsListKey");
    }
}