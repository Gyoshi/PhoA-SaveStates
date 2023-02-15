using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace SaveStates
{
    [Serializable]
    public class QuickSaveData
    {
        // Data --
        public string saveFileString = null;
        public string[] objectCodes = { };
        public string[] persistentObjectCodes = { };
        public string[] extremelyPersistentObjectCodes = { };

        public int doorId = 0;
        public Vector3 position = Vector3.zero;
        public Vector3 encounterPosition = Vector3.zero;
        public int camera = -1;
        public bool mapMode = false;

        [NonSerialized]
        public GaleStats galeStats = new GaleStats();
        public int _hp;
        public int _max_hp;
        public float _stamina;
        public float _max_stamina;
        public float _stamina_buff;
        public float _attack_buff;

        public float staminaStun = 0f;
        public bool grounded = false;
        // End Data --

        public static Dictionary<int, QuickSaveData> slots = new Dictionary<int, QuickSaveData>();
        public static int maxSlot = 16;

        public string room { get { return saveFileString.Split(',')[0]; } }

        public void Save()
        {
            foreach (string file in Directory.GetFiles(Main.dataPath))
            {
                int slotNumber = int.Parse(Path.GetFileName(file).Remove(2));
                Main.logger.Log("File slotNumber: " + slotNumber);
                if (Main.currentSlot == slotNumber)
                {
                    File.Delete(file);
                }
            }
            string filename = getSaveFilename();

            this.UpdateFields();
            string jsonString = JsonUtility.ToJson(this, true);
            File.WriteAllText(filename, jsonString);
        }

        public static QuickSaveData Load(string filename)
        {
            QuickSaveData data;

            try
            {
                string fileContents = File.ReadAllText(filename);
                data = JsonUtility.FromJson<QuickSaveData>(fileContents);
            }
            catch (Exception e)
            {
                Main.logger.Log("Failed to read savedata at slot " + Main.currentSlot + ". Remove the potentially corrupted savedata file from the mod folder if you wish to continue on a blank slate.");
                throw e;
            }
            data.UpdateStats();
            return data;
        }

        private string getSaveFilename()
        {
            string filename = room;
            if (room.ToLower().StartsWith("p1_"))
            {
                filename = room.Substring(3);
            }
            filename = char.ToUpper(filename[0]) + filename.Substring(1);
            filename = Main.currentSlot.ToString("00") + "_" + filename + ".json";
            return Path.Combine(Main.dataPath, filename);
        }
        public static void readFiles()
        {
            foreach (string file in Directory.GetFiles(Main.dataPath))
            {
                int slotNumber = int.Parse(Path.GetFileName(file).Remove(2));
                if (slotNumber < 1 || slotNumber > QuickSaveData.maxSlot)
                {
                    continue;
                }
                slots[slotNumber] = Load(file);
            }
        }
        public void UpdateStats()
        {
            this.galeStats.hp = _hp;
            this.galeStats.max_hp = _max_hp;
            this.galeStats.stamina = _stamina;
            this.galeStats.max_stamina = _max_stamina;
            this.galeStats.stamina_buff = _stamina_buff;
            this.galeStats.attack_buff = _attack_buff;
        }
        public void UpdateFields()
        {
            this._hp = galeStats.hp;
            this._max_hp = galeStats.max_hp;
            this._stamina = galeStats.stamina;
            this._max_stamina = galeStats.max_stamina;
            this._stamina_buff = galeStats.stamina_buff;
            this._attack_buff = galeStats.attack_buff;
        }
    }
}