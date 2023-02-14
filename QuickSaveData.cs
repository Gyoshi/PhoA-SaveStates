using System;
using System.IO;
using UnityEngine;

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

        [NonSerialized]
        public GaleStats galeStats = new GaleStats();

        public int hp;
        public int max_hp;
        public float stamina;
        public float max_stamina;
        public float stamina_buff;
        public float attack_buff;

        public string saveFileString = null;

        public void SaveToJson(string filename)
        {
            this.UpdateFields();
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
                QuickSaveData data = JsonUtility.FromJson<QuickSaveData>(fileContents);
                data.UpdateStats();
                return data;
            }
            catch (Exception e)
            {
                Main.logger.Log("Failed to read savedata with exception:\n" + e + "\nRemove the file \'savedata.xml\' from the mod folder if you wish to continue on a blank slate.");
                throw;
            }
        }
        public void UpdateStats()
        {
            this.galeStats.hp = hp;
            this.galeStats.max_hp = max_hp;
            this.galeStats.stamina = stamina;
            this.galeStats.max_stamina = max_stamina;
            this.galeStats.stamina_buff = stamina_buff;
            this.galeStats.attack_buff = attack_buff;
        }
        public void UpdateFields()
        {   
            this.hp = galeStats.hp;
            this.max_hp = galeStats.max_hp;
            this.stamina = galeStats.stamina;
            this.max_stamina = galeStats.max_stamina;
            this.stamina_buff = galeStats.stamina_buff;
            this.attack_buff = galeStats.attack_buff;
        }
    }
}