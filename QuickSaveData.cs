using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.Experimental.Director;
using UnityModManagerNet;

namespace SaveStates
{
    [Serializable]
    public class QuickSaveData
    {
        public string room = null;
        public int doorId = 0;
        public Vector3 position = Vector3.zero;
        public Vector3 encounterPosition = Vector3.zero;
        public int camera = -1;
        public bool mapMode = false;

        public void SaveToJson(string filename)
        {
            string jsonString = JsonUtility.ToJson(this, true);
            File.WriteAllText(filename, jsonString);
        }

        public static QuickSaveData LoadFromJson(string filename)
        {
            if (!File.Exists(filename))
            {
                return new QuickSaveData();
            }
            try
            {
                string fileContents = File.ReadAllText(filename);
                return JsonUtility.FromJson<QuickSaveData>(fileContents);
            }
            catch (Exception e)
            {
                Main.logger.Log("Failed to read savedata with exception:\n" + e + "\nRemove the file \'savedata.xml\' from the mod folder if you wish to continue on a blank slate.");
                throw;
            }
        }
    }
}