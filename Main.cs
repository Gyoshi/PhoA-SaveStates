using HarmonyLib;
using Rewired;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;

namespace SaveStates
{
#if DEBUG
    [EnableReloading]
#endif
    static class Main
    {        
        public static QuickSaveData data;
        public static string dataPath;
        public static int currentSlot = 1;
        public static bool loadAvailable = false;
        public static bool CAM_HELD = false;
        public static bool CAM_RELEASED = false;

        public static Harmony harmony;
        public static UnityModManager.ModEntry.ModLogger logger;
        public static Settings settings;

        public static Player player;

        static void Load(UnityModManager.ModEntry modEntry)
        {
            logger = modEntry.Logger;

            dataPath = Path.Combine(modEntry.Path, "Quick save files");
            Directory.CreateDirectory(dataPath);
            QuickSaveData.readFiles();
            if (QuickSaveData.slots.ContainsKey(currentSlot)) {
                loadAvailable = true;
                data = QuickSaveData.slots[currentSlot];
            }
            else
            {
                data = new QuickSaveData();
            }

            modEntry.OnUpdate = OnUpdate;

            settings = Settings.Load<Settings>(modEntry);
            modEntry.OnGUI = OnGUI;
            modEntry.OnSaveGUI = OnSaveGUI;
#if DEBUG
            modEntry.OnUnload = Unload;
            #endif
            harmony = new Harmony(modEntry.Info.Id);
            harmony.PatchAll();

        }
        #if DEBUG
        static bool Unload(UnityModManager.ModEntry modEntry)
        {
            harmony.UnpatchAll();

            return true;
        }
        #endif

        static void OnUpdate(UnityModManager.ModEntry modEntry, float dt)
        {
            player = (Player)typeof(ControlAdapter)
                .GetField("player", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(PT2.director.control);
            CAM_RELEASED = CAM_HELD && !player.GetButton("Camera");
            CAM_HELD = player.GetButton("Camera");

            if (PT2.director.control.CAM_PRESSED && settings.freeze)
            {
                PT2.screen_covers.HazeScreen("9999ff", 0.6f, 0f, float.PositiveInfinity);
                Time.timeScale = 0f; //on cam down instead?
            }
            else if (CAM_RELEASED && settings.freeze)
            {
                PT2.screen_covers.CancelHazeScreen();
                if (!PT2.game_paused)
                    Time.timeScale = 1f;
            }

            if (CAM_HELD)
            {
                // Save
                if (PT2.director.control.RIGHT_STICK_CLICK && PT2.director.control.IsControlStickDeadZone(0.4f, false) || Input.GetKeyDown(KeyCode.Home))
                {
                    // Do the quicksave
                    QuickSave();
                    QuickSaveData.slots[currentSlot] = data;

                    // Save quicksave data to disk
                    data.Save();
                    loadAvailable = true; //should be readonly property

                    PT2.display_messages.DisplayMessage("Saved to Slot " + currentSlot, DisplayMessagesLogic.MSG_TYPE.GALE_MINUS_STATUS);
                }
                // Load
                bool loadRequested = PT2.director.control.GRAB_PRESSED || Input.GetKeyDown(KeyCode.End);
                if (loadRequested && loadAvailable)
                {
                    QuickLoad();

                    PT2.display_messages.DisplayMessage("Loaded Slot " + currentSlot, DisplayMessagesLogic.MSG_TYPE.GALE_PLUS_STATUS);
                }
                if (loadRequested && !loadAvailable)
                {
                    PT2.display_messages.DisplayMessage("Slot " + currentSlot + " is empty!", DisplayMessagesLogic.MSG_TYPE.INVENTORY_FULL);
                }
                // Swap slots
                if (PT2.director.control.SPRINT_PRESSED || Input.GetKeyDown(KeyCode.PageUp)) { currentSlot--; }
                else if (PT2.director.control.CROUCH_PRESSED || Input.GetKeyDown(KeyCode.PageDown)) { currentSlot++; }
                else { goto NOSWAP; }
                SwapSlots();
                NOSWAP: { };

                PT2.director.control.SilenceAllInputsThisFrame();
            }
            if (Main.CAM_HELD && Main.player.GetButtonDown("Alt Tool"))
                PT2.camera_control.ZoomSimple();
        }

        static void OnGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Draw(modEntry);
        }

        static void OnSaveGUI(UnityModManager.ModEntry modEntry)
        {
            settings.Save(modEntry);
        }

        private static void QuickSave() // should be instance method of QuickSaveData
        {
            // Save SaveFile data
            data.saveFileString = PT2.save_file._NS_CompactSaveDataAsString();
            SaveObjectCodes(ref data.objectCodes, "_object_codes");
            SaveObjectCodes(ref data.persistentObjectCodes, "_persistent_object_codes");
            SaveObjectCodes(ref data.extremelyPersistentObjectCodes, "_xtreme_object_codes");

            // Save room data
            data.doorId = LevelBuildLogic.door_end_id;

            // Save position
            data.position = PT2.gale_interacter.GetGaleTransform().position;
            data.encounterPosition = new Vector3(WorldMapFoeLogic.X_WHERE_BATTLE_OCCURRED, WorldMapFoeLogic.Y_WHERE_BATTLE_OCCURRED, 0f);
            data.camera = PT2.camera_control._curr_camera_config;
            //checkpoint = { PT2.gale_interacter._checkpoint_location, ...}

            // Save more general mode
            FieldInfo field = typeof(GaleLogicOne).GetField("_gale_state_on_level_load", BindingFlags.NonPublic | BindingFlags.Instance);
            data.mapMode = (GALE_MODE)field.GetValue(PT2.gale_script) == GALE_MODE.MAP_MODE;

            // Save stats
            data.galeStats = PT2.gale_interacter.stats;

            // Save Gale Logic
            if (PT2.gale_script is GaleLogicOne galeLogicOne)
            {
                data.staminaStun = galeLogicOne.stamina_stun;
                data.grounded = galeLogicOne._mover2.collision_info.below;
            }
        }

        private static void QuickLoad()
        {
            // Clear stuff like PT2.Initialize()
            //PT2.level_load_in_progress = false;
            PT2.sound_g.ForceStopOcarina();
            PT2.director.CloseAllDialoguers();
            PT2.gale_interacter.NoInteractionsCurrently();

            // From death
            PT2.screen_covers.CancelBlackBars();
            SpriteRenderer menuGaleSprite = (SpriteRenderer)typeof(ScreenCoversLogic)
                .GetField("_menu_gale_sprite", BindingFlags.NonPublic | BindingFlags.Instance)
                .GetValue(PT2.screen_covers);
            menuGaleSprite.gameObject.SetActive(false);

            // Load SaveFile data
            PT2.save_file._NS_ProcessSaveDataString(data.saveFileString); // also calls LoadLevel :/
            LoadObjectCodes(data.objectCodes, "_object_codes");
            LoadObjectCodes(data.persistentObjectCodes, "_persistent_object_codes");
            LoadObjectCodes(data.extremelyPersistentObjectCodes, "_xtreme_object_codes");

            // Load room
            PT2.LoadLevel(data.room, data.doorId, Vector3.zero, false, 0f, false, true);
            if (data.mapMode)
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
            PT2.camera_control.SwitchCameraConfig(data.camera, 0, true);
            PT2.gale_interacter.GetGaleTransform().position = data.position;
            WorldMapFoeLogic.X_WHERE_BATTLE_OCCURRED = data.encounterPosition.x;
            WorldMapFoeLogic.Y_WHERE_BATTLE_OCCURRED = data.encounterPosition.y;
            PT2.gale_interacter.ScanForInteractSigns();

            // Load stats
            PT2.gale_interacter.stats = data.galeStats;
            PT2.hud_heart.J_UpdateHealth(data.galeStats.hp, data.galeStats.max_hp, false, false);
            PT2.hud_stamina.J_InitializeStaminaHud(data.galeStats.max_stamina); //superfluous after savefile data?
            PT2.hud_stamina.J_SetCurrentStamina(data.galeStats.stamina);

            // Load Gale Logic
            if (PT2.gale_script is GaleLogicOne galeLogicOne)
            {
                galeLogicOne.stamina_stun = data.staminaStun;
                galeLogicOne._mover2.collision_info.below = data.grounded;
            }

            #if DEBUG
            logger.Log("ロード済み");
            #endif
        }

        private static void SwapSlots()
        {
            // Adding max because % does negative values wrong
            currentSlot = (currentSlot + QuickSaveData.maxSlot - 1) % QuickSaveData.maxSlot + 1;

            string message = "Slot " + currentSlot;

            if (QuickSaveData.slots.ContainsKey(currentSlot))
            {
                loadAvailable = true;
                data = QuickSaveData.slots[currentSlot];
                message += "<sprite=30>";
            }
            else
            {
                loadAvailable = false;
                data = new QuickSaveData();
            }
            PT2.display_messages.DisplayMessage(message, DisplayMessagesLogic.MSG_TYPE.SMALL_ITEM_GET);
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

    [HarmonyPatch(typeof(CameraLogic), "ZoomSimple")]
    public class Zoom_Patch //does nothing with time pause / input silence
    {
        public static bool Prefix()
        {
            if (Main.CAM_HELD && Main.player.GetButtonDown("Alt Tool"))
                return true;
            return false;
        }
    }
}