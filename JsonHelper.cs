using System;
using System.Collections.Generic;
using UnityEngine;

namespace Yurand.Timberborn.Achievements
{
    //https://stackoverflow.com/questions/36239705/serialize-and-deserialize-json-and-json-array-in-unity
    //modified
    public static class JsonHelper
    {
        public static List<T> FromListJson<T>(string json)
        {
            Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
            return wrapper.Items;
        }

        public static string ToListJson<T>(List<T> array)
        {
            Wrapper<T> wrapper = new Wrapper<T> { Items = array };
            return JsonUtility.ToJson(wrapper);
        }

        public static string ToListJson<T>(List<T> array, bool prettyPrint)
        {
            Wrapper<T> wrapper = new Wrapper<T> { Items = array };
            return JsonUtility.ToJson(wrapper, prettyPrint);
        }

        [Serializable]
        public class Wrapper<T> {
            [SerializeField] public List<T> Items;
        }
    }
}