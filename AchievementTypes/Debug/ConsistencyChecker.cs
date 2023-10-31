using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Text;
using TimberApi.ConsoleSystem;
using Timberborn.BaseComponentSystem;
using Timberborn.Persistence;
using Timberborn.SingletonSystem;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.AI;

namespace Yurand.Timberborn.Achievements
{
    public partial class AchievementManager
    {
        private void TestAchievementTypeConsistency() {
            var achievementTypes = FindAllDerivedTypes<AchievementDefinitionBase>();
            var dictionary = new Dictionary<string, AchievementDefinitionBase>();
            foreach(var type in achievementTypes) {
                var definitionConstructor = type.GetConstructor(BindingFlags.NonPublic|BindingFlags.Instance, null, new Type[0], null);
                if (definitionConstructor is null) {
                    console.LogError($"Achivement of type {type.Name} consistency error: no default constructor provided");
                    continue;
                }

                AchievementDefinitionBase definition = (AchievementDefinitionBase) definitionConstructor.Invoke(new object[]{});
                dictionary.Clear(); dictionary.Add(definition.uniqueId, definition);
                try {
                    var achievement = AchievementSerializer.Default(definition);
                    var serialized = AchievementSerializer.Serialize(achievement);
                    var deserialized = AchievementSerializer.Deserialize(dictionary, serialized);
                    if (!deserialized.IsSame(achievement)) {
                        console.LogError($"Achivement of type {achievement.GetType().Name} consistency error: cannot be correctly serialized [1]");
                        console.LogError($"{PropertyList(achievement)}\n!=\n{PropertyList(deserialized)}");
                    }

                    var serializer = new SerializableAchievementSerializer();
                    var mockSerializer = new MockObjectSerializer();
                    serializer.Serialize(serialized, mockSerializer);
                    var deserialized2 = serializer.Deserialize(mockSerializer).Value;
                    if (!serialized.IsSame(deserialized2)) {
                        console.LogError($"Achivement of type {achievement.GetType().Name} consistency error: cannot be correctly serialized [2]");
                        console.LogError($"{PropertyList(serialized)}\n!=\n{PropertyList(deserialized2)}");
                    }
                } catch (Exception e) {
                    console.LogError($"Achievement of type {definition.GetType().Name} consistency error: {e}");
                }
            }

            console.LogInfo("Achievements type consistency check successful");
        }

        public static string PropertyList(object obj)
        {
            var builder = new StringBuilder();

            var props = obj.GetType().GetProperties(BindingFlags.Public|BindingFlags.Instance).ToList();
            props.AddRange(obj.GetType().GetProperties(BindingFlags.NonPublic|BindingFlags.Instance));
            foreach (var property in props) {
                builder.AppendLine(property.Name + ": " + property.GetValue(obj, null));
            }

            var fields = obj.GetType().GetFields(BindingFlags.Public|BindingFlags.Instance).ToList();
            fields.AddRange(obj.GetType().GetFields(BindingFlags.NonPublic|BindingFlags.Instance));
            foreach (var field in fields) {
                builder.AppendLine(field.Name + ": " + field.GetValue(obj));
            }
            return builder.ToString();
        }

        private static List<Type> FindAllDerivedTypes<T>() {
            return FindAllDerivedTypes<T>(Assembly.GetAssembly(typeof(T)));
        }

        private static List<Type> FindAllDerivedTypes<T>(Assembly assembly) {
            var baseType = typeof(T);
            return assembly
                .GetTypes()
                .Where(type =>
                    type != baseType &&
                    baseType.IsAssignableFrom(type)
                ).ToList();
        }
    }
}