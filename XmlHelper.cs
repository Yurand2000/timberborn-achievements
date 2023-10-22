using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

namespace Yurand.Timberborn.Achievements
{
    //https://stackoverflow.com/questions/36239705/serialize-and-deserialize-json-and-json-array-in-unity
    //modified
    public static class XmlHelper
    {
        public static string ToString<T>(T value) {
            using (var writer = new System.IO.StringWriter()) {
                var serializer = new XmlSerializer(typeof(T));
                serializer.Serialize(writer, value);
                return writer.ToString();
            }
        }

        public static T FromString<T>(string value) {
            using (var reader = new System.IO.StringReader(value)) {
                var serializer = new XmlSerializer(typeof(T));
                return (T) serializer.Deserialize(reader);
            }
        }
    }
}