using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using UnityModManagerNet;

namespace SaveStates
{
    [Serializable]
    public static class QuickSaveData
    {
        public static string room = "";
        public static int doorId = 0;
        public static Vector3 position = Vector3.zero;
        public static Vector3 encounterPosition = Vector3.zero;
        public static int camera = -1;
        public static bool mapMode = false;

        public static void SaveToXml(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SerializableDictionary<string, object>));
            using (FileStream stream = File.Create(filename))
            {
                serializer.Serialize(stream, dictionary);
            }
        }

        public static void LoadFromXml(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(SerializableDictionary<string, object>));

            try
            {
                using (FileStream stream = File.OpenRead(filename))
                {
                    dictionary = (SerializableDictionary<string, object>)serializer.Deserialize(stream);
                }
            }
            catch (Exception e)
            {
                Main.logger.Log("Failed to read savedata with exception:\n" + e + "\nRemove the file \'savedata.xml\' from the mod folder if you wish to continue on a blank slate.");
                throw;
            }
}
    }

    [Serializable]
    public sealed class SerializableDictionary<TKey, TValue>
    {
        [XmlIgnore]
        public Dictionary<TKey, TValue> dictionary = new Dictionary<TKey, TValue>();

        [XmlArray("Items"), XmlArrayItem("Item")]
        public KeyValuePair<TKey, TValue>[] Items
        {
            get
            {
                return dictionary.ToArray();
            }
            set
            {
                dictionary = value.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
        }
    }
    [System.Xml.Serialization.XmlTypeAttribute("Vector3")]
    public class SerializableVector3
    {
        [System.Xml.Serialization.XmlElementAttribute("X")]
        public float X { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("Y")]
        public float Y { get; set; }

        [System.Xml.Serialization.XmlElementAttribute("Z")]
        public float Z { get; set; }

        // Conversion from Vector3 to SerializableVector3
        public static implicit operator SerializableVector3(Vector3 vector)
        {
            return new SerializableVector3 { X = vector.x, Y = vector.y, Z = vector.z };
        }

        // Conversion from SerializableVector3 to Vector3
        public static implicit operator Vector3(SerializableVector3 serializableVector)
        {
            return new Vector3(serializableVector.X, serializableVector.Y, serializableVector.Z);
        }
    }
}