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
    }

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
    }

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
    }

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
    }

    [Serializable]
    public class SerializableAchievementSimple : SerializableAchievementBase
    {
        public SerializableAchievementSimple() : base() {}
        public SerializableAchievementSimple(SerializableAchievementBase other) : base(other) {}
        public SerializableAchievementSimple(AchievementSimple achievement) : base(achievement) { }
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
    }

    public class AchievementSerializer
    {
        public static AchievementBase Default(AchievementDefinitionBase definitionBase) {
            switch (definitionBase) {
                case AchievementSimpleDefinition achievement:
                    return new AchievementSimple(achievement);
                case AchievementWithCompletitionDefinition achievement:
                    return new AchievementWithCompletition(achievement);
                case AchievementWithCompletitionTieredDefinition achievement:
                    return new AchievementWithCompletitionTiered(achievement);
                default:
                    throw new ArgumentException($"Cannot provide default achievement for definition of type {definitionBase.GetType().Name}");
            }
        }
        public static AchievementBase Deserialize(IDictionary<string, AchievementDefinitionBase> definitions, SerializableAchievementBase value) {
            var definition = definitions[value.achievementId];
            switch (value) {
                case SerializableAchievementSimple serialized:
                    return new AchievementSimple(
                        (AchievementSimpleDefinition)definition,
                        serialized.completed
                    );
                case SerializableAchievementWithCompletition serialized:
                    return new AchievementWithCompletition((
                        AchievementWithCompletitionDefinition)definition,
                        serialized.completed, serialized.current_state
                    );
                case SerializableAchievementWithCompletitionTiered serialized:
                    return new AchievementWithCompletitionTiered(
                        (AchievementWithCompletitionTieredDefinition) definition,
                        serialized.completed, serialized.current_state
                    );
                default:
                    throw new ArgumentException($"Cannot provide serializable achievement for achievement type {value.GetType().Name}");
            }
        }

        public static SerializableAchievementBase Serialize(AchievementBase value) {
            switch (value) {
                case AchievementSimple achievement:
                    return new SerializableAchievementSimple(achievement);
                case AchievementWithCompletition achievement:
                    return new SerializableAchievementWithCompletition(achievement);
                case AchievementWithCompletitionTiered achievement:
                    return new SerializableAchievementWithCompletitionTiered(achievement);
                default:
                    throw new ArgumentException($"Cannot provide serializable achievement for achievement type {value.GetType().Name}");
            }
        }
    }

    public class SerializableAchievementSerializer : IObjectSerializer<SerializableAchievementBase>
    {
        public Obsoletable<SerializableAchievementBase> Deserialize(IObjectLoader objectLoader) {
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

        public void Serialize(SerializableAchievementBase value, IObjectSaver objectSaver) {
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
        
        [XmlElement(ElementName = "AchievementBase")]
        [XmlElement(Type = typeof(SerializableAchievementSimple), ElementName = "AchievementSimple")]
        [XmlElement(Type = typeof(SerializableAchievementWithCompletition), ElementName = "AchievementWithCompletition")]
        [XmlElement(Type = typeof(SerializableAchievementWithCompletitionTiered), ElementName = "AchievementWithCompletitionTiered")]
        public SerializableAchievementBase[] achievements;
        public SerializableAchievements() { achievements = null; }
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