using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;
using static UnityModManagerNet.UnityModManager;

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
        public float _lift_power = 4f;
        public int _hp;
        public int _max_hp;
        public float _stamina;
        public float _max_stamina;
        public float _stamina_buff;
        public float _attack_buff;

        public float staminaStun = 0f;
        public bool grounded = false;
        // End Data --

        private bool _loadAvailable = false;

        public string room { get { return saveFileString.Split(',')[0]; } }
        public bool loadAvailable { get { return _loadAvailable; } }

        public void Write(string dataPath)
        {
            foreach (string file in Directory.GetFiles(dataPath))
            {
                int slotNumber = int.Parse(Path.GetFileName(file).Remove(2));
                if (Main.currentSlot == slotNumber)
                {
                    File.Delete(file);
                }
            }
            string filename = getSaveFilename(dataPath);

            this.UpdateFields();
            string jsonString = JsonUtility.ToJson(this, true);
            File.WriteAllText(filename, jsonString);
        }

        public static QuickSaveData Read(string filename)
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
            data._loadAvailable = true;
            return data;
        }

        private string getSaveFilename(string dataPath)
        {
            string filename = room;
            if (room.ToLower().StartsWith("p1_"))
            {
                filename = room.Substring(3);
            }
            filename = char.ToUpper(filename[0]) + filename.Substring(1);
            filename = Main.currentSlot.ToString("00") + "_" + filename + ".json";
            return Path.Combine(dataPath, filename);
        }
        private void UpdateStats()
        {
            this.galeStats.lift_power = _lift_power;
            this.galeStats.hp = _hp;
            this.galeStats.max_hp = _max_hp;
            this.galeStats.stamina = _stamina;
            this.galeStats.max_stamina = _max_stamina;
            this.galeStats.stamina_buff = _stamina_buff;
            this.galeStats.attack_buff = _attack_buff;
        }
        private void UpdateFields()
        {
            this._lift_power = galeStats.lift_power;
            this._hp = galeStats.hp;
            this._max_hp = galeStats.max_hp;
            this._stamina = galeStats.stamina;
            this._max_stamina = galeStats.max_stamina;
            this._stamina_buff = galeStats.stamina_buff;
            this._attack_buff = galeStats.attack_buff;
        }
        public void QuickSave() 
        {
            // Save SaveFile data
            this.saveFileString = PT2.save_file._NS_CompactSaveDataAsString();
            SaveObjectCodes(ref this.objectCodes, "_object_codes");
            SaveObjectCodes(ref this.persistentObjectCodes, "_persistent_object_codes");
            SaveObjectCodes(ref this.extremelyPersistentObjectCodes, "_xtreme_object_codes");

            // Save room data
            this.doorId = LevelBuildLogic.door_end_id;

            // Save position
            this.position = PT2.gale_interacter.GetGaleTransform().position;
            this.encounterPosition = new Vector3(WorldMapFoeLogic.X_WHERE_BATTLE_OCCURRED, WorldMapFoeLogic.Y_WHERE_BATTLE_OCCURRED, 0f);
            this.camera = PT2.camera_control._curr_camera_config;
            //checkpoint = { PT2.gale_interacter._checkpoint_location, ...}

            // Save more general mode
            FieldInfo field = typeof(GaleLogicOne).GetField("_gale_state_on_level_load", BindingFlags.NonPublic | BindingFlags.Instance);
            this.mapMode = (GALE_MODE)field.GetValue(PT2.gale_script) == GALE_MODE.MAP_MODE;

            // Save stats
            this.galeStats = PT2.gale_interacter.stats;

            // Save Gale Logic
            if (PT2.gale_script is GaleLogicOne galeLogicOne)
            {
                this.staminaStun = galeLogicOne.stamina_stun;
                this.grounded = galeLogicOne._mover2.collision_info.below;
            }

            this._loadAvailable = true;
        }
        public void QuickLoad()
        {
            // Clear stuff like PT2.Initialize()
            //PT2.level_load_in_progress = false;
            PT2.sound_g.ForceStopOcarina();
            PT2.director.CloseAllDialoguers();
            PT2.gale_interacter.NoInteractionsCurrently();
            
            // From opening menu
            if (LevelBuildLogic.level_name == "game_start")
            {
                PT2.coming_from_opening_menu = true;
                PT2.sound_g.AdjustMusicVolume(null, 0f, 0.5f, false, true);
                PT2.director.current_opening_menu.GameStart(); // Is this ok?
                PT2.director.current_opening_menu = null;
            }

            // From death
            PT2.screen_covers.CancelBlackBars();
            SpriteRenderer menuGaleSprite = (SpriteRenderer)typeof(ScreenCoversLogic)
                .GetField("_menu_gale_sprite", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(PT2.screen_covers);
            menuGaleSprite.gameObject.SetActive(false);

            // Load SaveFile data
            PT2.save_file._NS_ProcessSaveDataString(this.saveFileString); // also calls LoadLevel :/
            LoadObjectCodes(this.objectCodes, "_object_codes");
            LoadObjectCodes(this.persistentObjectCodes, "_persistent_object_codes");
            LoadObjectCodes(this.extremelyPersistentObjectCodes, "_xtreme_object_codes");

            // Load room
            PT2.LoadLevel(this.room, this.doorId, Vector3.zero, false, 0f, false, true);
            if (this.mapMode)
            {
                PT2.gale_script.SetGaleModeOnLevelLoad(GALE_MODE.MAP_MODE);
            }
            else
            {
                PT2.gale_script.SetGaleModeOnLevelLoad(GALE_MODE.DEFAULT);
            }
            PT2.gale_script.SendGaleCommand(GALE_CMD.SET_GALE_MODE);
            PT2.gale_script.SendGaleCommand(GALE_CMD.RESET); //idk what this does but it removes at least 1 weird bug so

            // Load position
            PT2.camera_control.SwitchCameraConfig(this.camera, 0, true);
            PT2.gale_interacter.GetGaleTransform().position = this.position;
            WorldMapFoeLogic.X_WHERE_BATTLE_OCCURRED = this.encounterPosition.x;
            WorldMapFoeLogic.Y_WHERE_BATTLE_OCCURRED = this.encounterPosition.y;
            PT2.gale_interacter.ScanForInteractSigns();
            PT2.gale_script.SendGaleCommand(GALE_CMD.PREVENT_DOOR_UP_SPAM, 0f);

            // Load stats
            PT2.gale_interacter.stats = this.galeStats;
            PT2.hud_heart.J_UpdateHealth(this.galeStats.hp, this.galeStats.max_hp, false, false);
            PT2.hud_heart.ForceCancelBlareSfx();
            PT2.hud_stamina.J_InitializeStaminaHud(this.galeStats.max_stamina); //superfluous after savefile data?
            PT2.hud_stamina.J_SetCurrentStamina(this.galeStats.stamina);

            // Load Gale Logic
            if (PT2.gale_script is GaleLogicOne galeLogicOne)
            {
                galeLogicOne.stamina_stun = this.staminaStun;
                galeLogicOne._mover2.collision_info.below = this.grounded;
            }
        }

        private static void SaveObjectCodes(ref string[] objectCodesArray, string fieldName)
        {
            HashSet<string> codesSet = (HashSet<string>)typeof(SaveFile)
                .GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(PT2.save_file);
            objectCodesArray = new string[codesSet.Count];
            codesSet.CopyTo(objectCodesArray, 0);
        }
        private static void LoadObjectCodes(string[] objectCodesArray, string fieldName)
        {
            HashSet<string> codesSet = new HashSet<string>(objectCodesArray);
            typeof(SaveFile)
                .GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(PT2.save_file, codesSet);
        }
    }
}