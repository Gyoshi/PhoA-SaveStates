using HarmonyLib;
using System;
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

        public static Harmony harmony;
        public static UnityModManager.ModEntry.ModLogger logger;

        // Compiled without dependencies on UnityModManagerNet
        static void Load(UnityModManager.ModEntry modEntry)
        {
            logger = modEntry.Logger;

            dataPath = Path.Combine(modEntry.Path, "savedata.json");
            data = QuickSaveData.LoadFromJson(dataPath);

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
            //display save slot
            //PT2.gale_interacter.DisplayNumAboveHead(10, DamageNumberLogic.DISPLAY_STYLE.BLINK_IN_PLACE2, true);
            if (PT2.director.control.GRAB_HELD && PT2.director.control.CAM_PRESSED)
            {
                //Save
                PT2.gale_interacter.DisplayNumAboveHead(10, DamageNumberLogic.DISPLAY_STYLE.HOVER_AND_FLASH_RED, false);

                QuickSave();

                data.SaveToJson(dataPath);
            }
            if (!PT2.director.control.GRAB_HELD && PT2.director.control.CAM_PRESSED)
            {
                //Load
                QuickLoad();

                PT2.gale_interacter.DisplayNumAboveHead(10, DamageNumberLogic.DISPLAY_STYLE.HOVER_AND_FLASH_GREEN, true);
            }
        }
        private static void QuickSave()
        {
            // Save room
            data.room = Traverse.Create(typeof(PT2)).Field("_room_to_load").GetValue() as string;
            data.doorId = LevelBuildLogic.door_end_id;
            Main.logger.Log("Saved room : " + data.room);

            // Save position
            data.position = PT2.gale_interacter.GetGaleTransform().position;
            data.encounterPosition = new Vector3(WorldMapFoeLogic.X_WHERE_BATTLE_OCCURRED, WorldMapFoeLogic.Y_WHERE_BATTLE_OCCURRED, 0f);
            data.camera = PT2.camera_control._curr_camera_config;
            //checkpoint = { PT2.gale_interacter._checkpoint_location, ...}
            Main.logger.Log("Saved position : " + data.position);

            // Save more general mode
            FieldInfo field = typeof(GaleLogicOne).GetField("_gale_state_on_level_load", BindingFlags.NonPublic | BindingFlags.Instance);
            data.mapMode = (GALE_MODE)field.GetValue(PT2.gale_script) == GALE_MODE.MAP_MODE;

            // Save stats
            data.galeStats = PT2.gale_interacter.stats;
            Main.logger.Log("Saved stats: hp-" + data.galeStats.hp + " stamina-"+data.galeStats.stamina);

            // Save savefile data
            data.saveFileString = PT2.save_file._NS_CompactSaveDataAsString();
        }

        private static void QuickLoad()
        {
            // Clear stuff like PT2.Initialize()
            PT2.sound_g.ForceStopOcarina();
            PT2.director.CloseAllDialoguers();
            PT2.gale_interacter.NoInteractionsCurrently();

            // Load room
            PT2.LoadLevel(data.room, data.doorId, Vector3.zero, false, 0.1f, false, true);
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
            OpeningMenuLogic.EnableGameplayElements();

            // Load stats
            PT2.gale_interacter.stats = data.galeStats;
            PT2.hud_heart.J_UpdateHealth(data.galeStats.hp, data.galeStats.max_hp, false, false);
            PT2.hud_stamina.J_InitializeStaminaHud(data.galeStats.max_stamina);
            PT2.hud_stamina.J_SetCurrentStamina(data.galeStats.stamina);

            // Load inventory
            //PT2.save_file.Debug_EmptyInventory();

            Main.logger.Log("ロード済み");
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