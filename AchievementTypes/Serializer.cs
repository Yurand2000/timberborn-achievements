using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;
using Timberborn.Persistence;

namespace Yurand.Timberborn.Achievements
{
    public class AchievementSerializer
    {
        public static AchievementBase Default(AchievementDefinitionBase definitionBase) {
            switch (definitionBase) {
                case AchievementHiddenDefinition achievement:
                    return new AchievementHidden(achievement);
                case AchievementSimpleDefinition achievement:
                    return new AchievementSimple(achievement);
                case AchievementFailableDefinition achievement:
                    return new AchievementFailable(achievement);
                case AchievementWithCompletitionDefinition achievement:
                    return new AchievementWithCompletition(achievement);
                case AchievementWithCompletitionTieredDefinition achievement:
                    return new AchievementWithCompletitionTiered(achievement);
                default:
                    throw new ArgumentException($"Cannot provide default achievement for definition of type {definitionBase.GetType().Name}");
            }
        }
        public static AchievementBase Deserialize(IDictionary<string, AchievementDefinitionBase> definitions, SerializableAchievementBase value) {
            if (!definitions.ContainsKey(value.achievementId)) {
                PluginEntryPoint.console.LogInfo($"achievement {value.achievementId} not found when deserializing...");
                return null;
            }

            var definition = definitions[value.achievementId];
            switch (value) {
                case SerializableAchievementHidden serialized:
                    return new AchievementHidden(
                        (AchievementHiddenDefinition)definition,
                        serialized.completed
                    );
                case SerializableAchievementSimple serialized:
                    return new AchievementSimple(
                        (AchievementSimpleDefinition)definition,
                        serialized.completed
                    );
                case SerializableAchievementFailable serialized:
                    var state = AchievementFailable.AchievementState.NotCompleted;
                    if (serialized.completed) state = AchievementFailable.AchievementState.Completed;
                    if (serialized.failed) state = AchievementFailable.AchievementState.Failed;

                    return new AchievementFailable(
                        (AchievementFailableDefinition)definition,
                        state
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
                case AchievementHidden achievement:
                    return new SerializableAchievementHidden(achievement);
                case AchievementSimple achievement:
                    return new SerializableAchievementSimple(achievement);
                case AchievementFailable achievement:
                    return new SerializableAchievementFailable(achievement);
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
            if ( !objectLoader.Has(achievementTypeKey) )
                return new Obsoletable<SerializableAchievementBase>();

            var type = objectLoader.Get(achievementTypeKey);
            var baseAch = DeserializeBase(objectLoader);
            switch (type) {
                case nameof(SerializableAchievementHidden):
                    return new SerializableAchievementHidden(baseAch);
                case nameof(SerializableAchievementSimple):
                    return new SerializableAchievementSimple(baseAch);
                case nameof(SerializableAchievementFailable):
                    return new SerializableAchievementFailable(baseAch, objectLoader.Get(failedKey));
                case nameof(SerializableAchievementWithCompletition):
                    return new SerializableAchievementWithCompletition(baseAch, objectLoader.Get(currentStateKey));
                case nameof(SerializableAchievementWithCompletitionTiered):
                    return new SerializableAchievementWithCompletitionTiered(baseAch, objectLoader.Get(currentStateKey));
                default:
                    throw new ArgumentException($"Cannot deserialize achievement of type {type}");
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
                case SerializableAchievementHidden achievement:
                    objectSaver.Set(achievementTypeKey, nameof(SerializableAchievementHidden));
                    break;
                case SerializableAchievementSimple achievement:
                    objectSaver.Set(achievementTypeKey, nameof(SerializableAchievementSimple));
                    break;
                case SerializableAchievementFailable achievement:
                    objectSaver.Set(achievementTypeKey, nameof(SerializableAchievementFailable));
                    objectSaver.Set(failedKey, achievement.failed);
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
        private static readonly PropertyKey<bool> failedKey = new PropertyKey<bool>("failedKey");
        private static readonly PropertyKey<float> currentStateKey = new PropertyKey<float>("currentStateKey");
    }

    [Serializable]
    [XmlRoot("Achievements")]
    public class SerializableAchievements {
        
        [XmlElement(ElementName = "AchievementBase")]
        [XmlElement(Type = typeof(SerializableAchievementSimple), ElementName = "AchievementSimple")]
        [XmlElement(Type = typeof(SerializableAchievementFailable), ElementName = "AchievementFailable")]
        [XmlElement(Type = typeof(SerializableAchievementHidden), ElementName = "AchievementHidden")]
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
            if ( !objectLoader.Has(achievementsListKey) )
                return new Obsoletable<SerializableAchievements>();

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