using HarmonyLib;
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

        public static Harmony harmony;
        public static UnityModManager.ModEntry.ModLogger logger;

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

            // TODO: figure this out (easy fix if camera disabled?)
            //modEntry.OnUpdate = OnUpdate;
            modEntry.OnLateUpdate = OnUpdate;
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
            //Save
            if (PT2.director.control.RIGHT_STICK_CLICK && PT2.director.control.IsControlStickDeadZone(0.4f, false) || Input.GetKeyDown(KeyCode.Home))
            {
                // Do the quicksave
                QuickSave();
                QuickSaveData.slots[currentSlot] = data;

                // Save quicksave data to disk
                data.Save();
                loadAvailable = true;
                
                PT2.gale_interacter.DisplayNumAboveHead(currentSlot, DamageNumberLogic.DISPLAY_STYLE.HOVER_AND_FLASH_RED, false);
            }
            //Load
            bool loadRequested = PT2.director.control.CAM_PRESSED && PT2.director.control.GRAB_HELD || Input.GetKeyDown(KeyCode.End);
            if (loadAvailable && loadRequested)
            {
                QuickLoad();

                PT2.gale_interacter.DisplayNumAboveHead(currentSlot, DamageNumberLogic.DISPLAY_STYLE.HOVER_AND_FLASH_GREEN, true);
            }
            // Swap slots
            if (Input.GetKey(KeyCode.RightShift))
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow)) { currentSlot--; }
                else if (Input.GetKeyDown(KeyCode.RightArrow)) { currentSlot++; }
                else { goto NOSWAP; }
                // Adding max because % does negative values wrong
                currentSlot = (currentSlot + QuickSaveData.maxSlot - 1) % QuickSaveData.maxSlot + 1;

                if (QuickSaveData.slots.ContainsKey(currentSlot))
                {
                    loadAvailable = true;
                    data = QuickSaveData.slots[currentSlot];
                }
                else
                {
                    loadAvailable = false;
                    data = new QuickSaveData();
                }

                PT2.gale_interacter.DisplayNumAboveHead(currentSlot, DamageNumberLogic.DISPLAY_STYLE.BLINK_IN_PLACE2, true);
            }
            NOSWAP: { };
        }
        private static void QuickSave() // should be instance method
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
            PT2.sound_g.ForceStopOcarina();
            PT2.director.CloseAllDialoguers();
            PT2.gale_interacter.NoInteractionsCurrently();

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
        private static void SaveObjectCodes(ref string[] objectCodesArray, string fieldName)
        {
            HashSet<string> codesSet = (HashSet<string>)typeof(SaveFile).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance).GetValue(PT2.save_file);
            objectCodesArray = new string[codesSet.Count];
            codesSet.CopyTo(objectCodesArray, 0);
        }
        private static void LoadObjectCodes(string[] objectCodesArray, string fieldName)
        {
            HashSet<string> codesSet = new HashSet<string>(objectCodesArray);
            typeof(SaveFile).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance).SetValue(PT2.save_file, codesSet);
        }
    }
    //[HarmonyPatch(typeof(GaleLogicOne), "Update")]
    //public class GaleLogicOne_Patch
    //{
    //    private static ControlAdapter control;
    //    public static bool Postfix(GaleLogicOne __instance)
    //    {
    //        control = Traverse.Create(__instance).Field("_control").GetValue<ControlAdapter>();
    //        if (control.CAM_PRESSED)
    //        {
    //            //UnityModManager.ModEntry.Logger.Log(":o");
    //            //Traverse.Create(__instance).Method("_IncurLimitBreakCost").GetValue();
    //        }
    //        return true;
    //    }
    //}
}