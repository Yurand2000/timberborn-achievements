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

        public abstract bool Update(Updater updater);
        public abstract void UpdateFromLocal(AchievementBase achievement);
        public class Updater {}
    }

    public class AchievementSimple : AchievementBase {
        public AchievementSimple(AchievementSimpleDefinition definition, bool completed = false)
            :base(definition, completed) { }

        public AchievementSimpleDefinition GetDefinition() { return (AchievementSimpleDefinition)definition; }

        public override bool Update(AchievementBase.Updater updaterBase) {
            var updater = (Updater)updaterBase;
            if (updater is null) return false;

            this.completed = updater.completed;
            return true;
        }

        public override void UpdateFromLocal(AchievementBase achievementBase) {
            var achievement = (AchievementSimple)achievementBase;
            if (achievement is null) return;
            if (achievement.completed) this.completed = true;
        }

        public new class Updater : AchievementBase.Updater {
            public bool completed;
        }
    }

    public class AchievementWithCompletition : AchievementBase {
        public float current_state;
        public AchievementWithCompletition(AchievementWithCompletitionDefinition definition, bool completed = false, float current_state = 0f)
            :base(definition, completed)
        {
            this.current_state = current_state;
        }

        public AchievementWithCompletitionDefinition GetDefinition() { return (AchievementWithCompletitionDefinition)definition; }

        public override bool Update(AchievementBase.Updater updaterBase) {
            var updater = (Updater)updaterBase;
            if (updater is null) return false;

            var max = GetDefinition().max_completition;
            this.current_state = Math.Clamp(updater.next_state, 0, max);
            if (this.current_state == max) this.completed = true;

            this.completed = updater.force_complete ?? this.completed;
            return true;
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
    }

    public class AchievementWithCompletitionTiered : AchievementBase {
        public float current_state;
        public AchievementWithCompletitionTiered(AchievementWithCompletitionTieredDefinition definition, bool completed = false, float current_state = 0f)
            :base(definition, completed)
        {
            this.current_state = current_state;
        }

        public AchievementWithCompletitionTieredDefinition GetDefinition() { return (AchievementWithCompletitionTieredDefinition)definition; }

        public override bool Update(AchievementBase.Updater updaterBase) {
            var updater = (Updater)updaterBase;
            if (updater is null) return false;

            var max = GetMaxCompletition();
            this.current_state = Math.Clamp(updater.next_state, 0, max);
            if (this.current_state == max) this.completed = true;


            this.completed = updater.force_complete ?? this.completed;
            return true;
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
    }

    [Serializable]
    public class SerializableAchievementBase {
        [XmlAttribute("id")] public string achievementId;
        [XmlAttribute("completed")] public bool completed;

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
    }

    [Serializable]
    public class SerializableAchievementSimple : SerializableAchievementBase
    {
        public SerializableAchievementSimple(SerializableAchievementBase other) : base(other) {}
        public SerializableAchievementSimple(AchievementSimple achievement) : base(achievement) { }
    }

    [Serializable]
    public class SerializableAchievementWithCompletition : SerializableAchievementBase
    {
        [XmlElement("current_state")] public float current_state;
        public SerializableAchievementWithCompletition(SerializableAchievementBase other, float current_state) : base(other)
        {
            this.current_state = current_state;
        }
        public SerializableAchievementWithCompletition(AchievementWithCompletition achievement) : base(achievement)
        {
            this.current_state = achievement.current_state;
        }
    }

    [Serializable]
    public class SerializableAchievementWithCompletitionTiered : SerializableAchievementBase
    {
        [XmlElement("current_state")] public float current_state;
        public SerializableAchievementWithCompletitionTiered(SerializableAchievementBase other, float current_state) : base(other)
        {
            this.current_state = current_state;
        }
        public SerializableAchievementWithCompletitionTiered(AchievementWithCompletitionTiered achievement) : base(achievement)
        {
            this.current_state = achievement.current_state;
        }
    }

    public class AchievementSerializer
    {
        public static AchievementBase Default(AchievementDefinitionBase definition) {
            return null;
        }
        public static AchievementBase Deserialize(IDictionary<string, AchievementDefinitionBase> definitions, SerializableAchievementBase value) {
            return null;
        }

        public static SerializableAchievementBase Serialize(AchievementBase value) {
            return null;
        }
    }

    public class SerializableAchievementSerializer : IObjectSerializer<SerializableAchievementBase>
    {
        public Obsoletable<SerializableAchievementBase> Deserialize(IObjectLoader objectLoader)
        {
            var type = objectLoader.Get(achievementTypeKey);
            var baseAch = DeserializeBase(objectLoader);
            switch (type) {
                case nameof(SerializableAchievementSimple):
                    return new SerializableAchievementSimple(baseAch);
                case nameof(SerializableAchievementWithCompletition):
                    return new SerializableAchievementWithCompletition(baseAch, objectLoader.Get(currentStateKey));
                case nameof(SerializableAchievementWithCompletitionTiered):
                    return new SerializableAchievementWithCompletitionTiered(baseAch, objectLoader.Get(currentStateKey));
                default:
                    throw new ArgumentException($"Cannot serialize achievement of type {type}");
            }
        }

        private SerializableAchievementBase DeserializeBase(IObjectLoader objectLoader) {
            return new SerializableAchievementBase(
                objectLoader.Get(achievementIdKey),
                objectLoader.Get(completedKey)
            );
        }

        public void Serialize(SerializableAchievementBase value, IObjectSaver objectSaver)
        {
            objectSaver.Set(achievementIdKey, value.achievementId);
            objectSaver.Set(completedKey, value.completed);
            switch (value) {
                case SerializableAchievementSimple achievement:
                    objectSaver.Set(achievementTypeKey, nameof(SerializableAchievementSimple));
                    break;
                case SerializableAchievementWithCompletition achievement:
                    objectSaver.Set(achievementTypeKey, nameof(SerializableAchievementWithCompletition));
                    objectSaver.Set(currentStateKey, achievement.current_state);
                    break;
                case SerializableAchievementWithCompletitionTiered achievement:
                    objectSaver.Set(achievementTypeKey, nameof(SerializableAchievementWithCompletitionTiered));
                    objectSaver.Set(currentStateKey, achievement.current_state);
                    break;
                default:
                    throw new ArgumentException($"Cannot serialize achievement of type {value.GetType().Name}");
                case null:
                    throw new ArgumentNullException();
            }
        }

        private static readonly PropertyKey<string> achievementIdKey = new PropertyKey<string>("achievementIdKey");
        private static readonly PropertyKey<string> achievementTypeKey = new PropertyKey<string>("achievementTypeKey");
        private static readonly PropertyKey<bool> completedKey = new PropertyKey<bool>("completedKey");
        private static readonly PropertyKey<float> currentStateKey = new PropertyKey<float>("currentStateKey");
    }

    [Serializable]
    [XmlRoot("Achievements")]
    public class SerializableAchievements {
        [XmlElement(ElementName = "Achievement")] public SerializableAchievementBase[] achievements;
        public SerializableAchievements(SerializableAchievementBase[] achievements) {
            this.achievements = achievements;
        }
    }

    public class SerializableAchievementsSerializer : IObjectSerializer<SerializableAchievements>
    {
        public Obsoletable<SerializableAchievements> Deserialize(IObjectLoader objectLoader)
        {
            return new SerializableAchievements(
                objectLoader.Get(achievementsListKey, new SerializableAchievementSerializer()).ToArray()
            );
        }

        public void Serialize(SerializableAchievements value, IObjectSaver objectSaver)
        {
            objectSaver.Set(achievementsListKey, value.achievements.ToList(), new SerializableAchievementSerializer());
        }

        private static readonly ListKey<SerializableAchievementBase> achievementsListKey = new ListKey<SerializableAchievementBase>("achievementsListKey");
    }
}