using HarmonyLib;
using System;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityModManagerNet;
using static UnityEngine.Random;

namespace SaveStates
{
#if DEBUG
    [EnableReloading]
#endif
    static class Main
    {        
        public static string dataPath;

        public static Harmony harmony;
        public static UnityModManager.ModEntry.ModLogger logger;

        // Compiled without dependencies on UnityModManagerNet
        static void Load(UnityModManager.ModEntry modEntry)
        {
            logger = modEntry.Logger;
            dataPath = Path.Combine(modEntry.Path, "savedata.xml");
            if (File.Exists(dataPath))
            {
                QuickSaveData.LoadFromXml(dataPath);
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
            //display save slot
            //PT2.gale_interacter.DisplayNumAboveHead(10, DamageNumberLogic.DISPLAY_STYLE.BLINK_IN_PLACE2, true);
            if (PT2.director.control.GRAB_HELD && PT2.director.control.CAM_PRESSED)
            {
                //Save
                PT2.gale_interacter.DisplayNumAboveHead(10, DamageNumberLogic.DISPLAY_STYLE.HOVER_AND_FLASH_RED, false);

                QuickSave();

                QuickSaveData.SaveToXml(dataPath);
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
            QuickSaveData.room = Traverse.Create(typeof(PT2)).Field("_room_to_load").GetValue() as string;
            QuickSaveData.doorId = LevelBuildLogic.door_end_id;
            Main.logger.Log("Saved room : " + room);

            // Save position
            QuickSaveData.position = PT2.gale_interacter.GetGaleTransform().position;
            QuickSaveData.encounterPosition = new Vector3(WorldMapFoeLogic.X_WHERE_BATTLE_OCCURRED, WorldMapFoeLogic.Y_WHERE_BATTLE_OCCURRED, 0f);
            QuickSaveData.camera = PT2.camera_control._curr_camera_config;
            //checkpoint = { PT2.gale_interacter._checkpoint_location, ...}
            Main.logger.Log("Saved position : " + position);

            // Saving Gail's exact state (rolling, dying, climbing, etc.) is probly not worth the effort
            //galeState = Traverse.Create(typeof(GaleLogicOne)).Field("StateFn").GetValue<Action>(PT2.gale_script);
            // Save more general mode
            FieldInfo field = typeof(GaleLogicOne).GetField("_gale_state_on_level_load", BindingFlags.NonPublic | BindingFlags.Instance);
            QuickSaveData.mapMode = (GALE_MODE)field.GetValue(PT2.gale_script) == GALE_MODE.MAP_MODE;
            Main.logger.Log("Saved map mode : " + mapMode);

        }

        private static void QuickLoad()
        {
            // Clear stuff like PT2.Initialize()
            PT2.sound_g.ForceStopOcarina();
            PT2.director.CloseAllDialoguers();
            PT2.gale_interacter.NoInteractionsCurrently();

            // Load room
            PT2.LoadLevel(QuickSaveData.room, QuickSaveData.doorId, Vector3.zero, false, 0.1f, false, true);
            if (QuickSaveData.mapMode)
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
            PT2.camera_control.SwitchCameraConfig(QuickSaveData.camera, 0, true);
            PT2.gale_interacter.GetGaleTransform().position = QuickSaveData.position;
            WorldMapFoeLogic.X_WHERE_BATTLE_OCCURRED = QuickSaveData.encounterPosition.x;
            WorldMapFoeLogic.Y_WHERE_BATTLE_OCCURRED = QuickSaveData.encounterPosition.y;
            OpeningMenuLogic.EnableGameplayElements();

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