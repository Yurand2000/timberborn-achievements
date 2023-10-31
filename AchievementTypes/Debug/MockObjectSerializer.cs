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
        private class MockObjectSerializer : IObjectSaver, IObjectLoader
        {
            private Dictionary<(Type, string), object> propertyKeys = new Dictionary<(Type, string), object>();

            private (Type, string) GetKey<T>(PropertyKey<T> key) {
                return (key.GetType().GenericTypeArguments[0], key.Name);
            }
            private (Type, string) GetListBaseKey<T>(ListKey<T> key) {
                return (key.GetType().GenericTypeArguments[0], key.Name);
            }
            private (Type, string) GetListLengthKey<T>(ListKey<T> key) {
                return (typeof(int), key.Name + "L");
            }
            private (Type, string) GetListItemKey((Type, string) base_key, int index) {
                return (base_key.Item1, base_key.Item2 + index.ToString());
            }
            private T GetInner<T>(PropertyKey<T> key) {
                var keys = GetKey(key);
                if (propertyKeys.ContainsKey(keys)) return (T)propertyKeys[keys];
                else return default;
            }
            private List<T> GetInnerList<T>(ListKey<T> key) {
                var keys_base = GetListBaseKey(key);
                var index_keys = GetListLengthKey(key);
                if (!propertyKeys.ContainsKey(index_keys)) return null;

                var list = new List<T>();
                for (int i = 0; i < (int)propertyKeys[index_keys]; i++) {
                    var keys = GetListItemKey(keys_base, i);
                    if (!propertyKeys.ContainsKey(keys)) return null;
                    list.Add((T)propertyKeys[keys]);
                }

                return list;
            }

            private void SetInner<T>(PropertyKey<T> key, T value) {
                propertyKeys.Add(GetKey(key), value);
            }
            private void SetInnerList<T>(ListKey<T> key, IReadOnlyCollection<T> values) {
                var keys_base = GetListBaseKey(key);
                var index_keys = GetListLengthKey(key);
                propertyKeys.Add(index_keys, values.Count);
                foreach(var pair in values.Select((value, index) => (value, index))) {
                    var keys = GetListItemKey(keys_base, pair.index);
                    propertyKeys.Add(keys, pair.value);
                }
            }

            public T Get<T>(PropertyKey<T> key) where T : Component { return GetInner(key); }
            public int Get(PropertyKey<int> key) { return GetInner(key); }
            public float Get(PropertyKey<float> key) { return GetInner(key); }
            public bool Get(PropertyKey<bool> key) { return GetInner(key); }
            public string Get(PropertyKey<string> key) { return GetInner(key); }
            public char Get(PropertyKey<char> key) { return GetInner(key); }
            public Quaternion Get(PropertyKey<Quaternion> key) { return GetInner(key); }
            public Vector3 Get(PropertyKey<Vector3> key) { return GetInner(key); }
            public Vector3Int Get(PropertyKey<Vector3Int> key) { return GetInner(key); }
            public Vector2 Get(PropertyKey<Vector2> key) { return GetInner(key); }
            public Vector2Int Get(PropertyKey<Vector2Int> key) { return GetInner(key); }
            public Color Get(PropertyKey<Color> key) { return GetInner(key); }
            public Guid Get(PropertyKey<Guid> key) { return GetInner(key); }
            public T Get<T>(PropertyKey<T> key, IObjectSerializer<T> serializer) {
                var keys = GetKey(key);
                if (propertyKeys.ContainsKey(keys)) return (T)serializer.Deserialize((IObjectLoader)propertyKeys[keys]);
                else return default;
            }

            public List<T> Get<T>(ListKey<T> key) where T : Component { return GetInnerList(key); }
            public List<int> Get(ListKey<int> key) { return GetInnerList(key); }
            public List<float> Get(ListKey<float> key) { return GetInnerList(key); }
            public List<bool> Get(ListKey<bool> key) { return GetInnerList(key); }
            public List<string> Get(ListKey<string> key) { return GetInnerList(key); }
            public List<char> Get(ListKey<char> key) { return GetInnerList(key); }
            public List<Quaternion> Get(ListKey<Quaternion> key) { return GetInnerList(key); }
            public List<Vector3> Get(ListKey<Vector3> key) { return GetInnerList(key); }
            public List<Vector3Int> Get(ListKey<Vector3Int> key) { return GetInnerList(key); }
            public List<Vector2> Get(ListKey<Vector2> key) { return GetInnerList(key); }
            public List<Vector2Int> Get(ListKey<Vector2Int> key) { return GetInnerList(key); }
            public List<Color> Get(ListKey<Color> key) { return GetInnerList(key); }
            public List<Guid> Get(ListKey<Guid> key) { return GetInnerList(key); }
            public List<T> Get<T>(ListKey<T> key, IObjectSerializer<T> serializer) {
                var keys_base = GetListBaseKey(key);
                var index_keys = GetListLengthKey(key);
                if (!propertyKeys.ContainsKey(index_keys)) return null;

                var list = new List<T>();
                for (int i = 0; i < (int)propertyKeys[index_keys]; i++) {
                    var keys = GetListItemKey(keys_base, i);
                    if (!propertyKeys.ContainsKey(keys)) return null;                    
                    list.Add(serializer.Deserialize((IObjectLoader)propertyKeys[keys]).Value);
                }

                return list;
            }

            public bool GetObsoletable<T>(PropertyKey<T> key, out T value) where T : Component {
                value = GetInner(key);
                return value is not null;
            }

            public bool GetObsoletable<T>(PropertyKey<T> key, IObjectSerializer<T> serializer, out T value) {
                value = default;
                var keys = GetKey(key);
                if (propertyKeys.ContainsKey(keys)) {
                    var obsoletable = serializer.Deserialize((IObjectLoader)propertyKeys[keys]);
                    if(obsoletable.Obsolete)
                        return false;
                    else {
                        value = obsoletable.Value;
                        return true;
                    }
                }
                else return false;
            }

            public bool Has<T>(PropertyKey<T> key) {
                return propertyKeys.ContainsKey(GetKey(key));
            }

            public bool Has<T>(ListKey<T> key) {
                return propertyKeys.ContainsKey(GetListLengthKey(key));
            }

            public void Set<T>(PropertyKey<T> key, T value) where T : BaseComponent { SetInner(key, value); }
            public void Set(PropertyKey<int> key, int value) { SetInner(key, value); }
            public void Set(PropertyKey<float> key, float value) { SetInner(key, value); }
            public void Set(PropertyKey<bool> key, bool value) { SetInner(key, value); }
            public void Set(PropertyKey<string> key, string value) { SetInner(key, value); }
            public void Set(PropertyKey<char> key, char value) { SetInner(key, value); }
            public void Set(PropertyKey<Quaternion> key, Quaternion value) { SetInner(key, value); }
            public void Set(PropertyKey<Vector3> key, Vector3 value) { SetInner(key, value); }
            public void Set(PropertyKey<Vector3Int> key, Vector3Int value) { SetInner(key, value); }
            public void Set(PropertyKey<Vector2> key, Vector2 value) { SetInner(key, value); }
            public void Set(PropertyKey<Vector2Int> key, Vector2Int value) { SetInner(key, value); }
            public void Set(PropertyKey<Color> key, Color value) { SetInner(key, value); }
            public void Set(PropertyKey<Guid> key, Guid value) { SetInner(key, value); }
            public void Set<T>(PropertyKey<T> key, T value, IObjectSerializer<T> serializer) {
                var keys = GetKey(key);
                var mockSerializer = new MockObjectSerializer();
                serializer.Serialize(value, mockSerializer);
                propertyKeys.Add(keys, mockSerializer);
            }

            public void Set<T>(ListKey<T> key, IReadOnlyCollection<T> values) where T : BaseComponent { SetInnerList(key, values); }
            public void Set(ListKey<int> key, IReadOnlyCollection<int> values) { SetInnerList(key, values); }
            public void Set(ListKey<float> key, IReadOnlyCollection<float> values) { SetInnerList(key, values); }
            public void Set(ListKey<bool> key, IReadOnlyCollection<bool> values) { SetInnerList(key, values); }
            public void Set(ListKey<string> key, IReadOnlyCollection<string> values) { SetInnerList(key, values); }
            public void Set(ListKey<char> key, IReadOnlyCollection<char> values) { SetInnerList(key, values); }
            public void Set(ListKey<Quaternion> key, IReadOnlyCollection<Quaternion> values) { SetInnerList(key, values); }
            public void Set(ListKey<Vector3> key, IReadOnlyCollection<Vector3> values) { SetInnerList(key, values); }
            public void Set(ListKey<Vector3Int> key, IReadOnlyCollection<Vector3Int> values) { SetInnerList(key, values); }
            public void Set(ListKey<Vector2> key, IReadOnlyCollection<Vector2> values) { SetInnerList(key, values); }
            public void Set(ListKey<Vector2Int> key, IReadOnlyCollection<Vector2Int> values) { SetInnerList(key, values); }
            public void Set(ListKey<Color> key, IReadOnlyCollection<Color> values) { SetInnerList(key, values); }
            public void Set(ListKey<Guid> key, IReadOnlyCollection<Guid> values) { SetInnerList(key, values); }
            public void Set<T>(ListKey<T> key, IReadOnlyCollection<T> values, IObjectSerializer<T> serializer) {
                var keys_base = GetListBaseKey(key);
                var index_keys = GetListLengthKey(key);
                propertyKeys.Add(index_keys, values.Count);
                foreach(var pair in values.Select((value, index) => (value, index))) {
                    var keys = GetListItemKey(keys_base, pair.index);
                    var mockSerializer = new MockObjectSerializer();
                    serializer.Serialize(pair.value, mockSerializer);
                    propertyKeys.Add(keys, mockSerializer);
                }
            }
        }
    }
}