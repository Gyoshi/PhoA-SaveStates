using System;
using System.IO;
using UnityEngine;

namespace SaveStates
{
    [Serializable]
    public class QuickSaveData
    {
        public string saveFileString = null;

        public string room = null;
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
                Main.logger.Log("Failed to read savedata. Remove the savedata file from the mod folder if you wish to continue on a blank slate.");
                throw e;
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