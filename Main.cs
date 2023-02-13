using HarmonyLib;
using System;
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
        [SaveOnReload] public static string room;
        [SaveOnReload] public static int doorId;
        [SaveOnReload] public static Vector3 position;
        [SaveOnReload] public static Vector3 encounterPosition;
        [SaveOnReload] public static int camera;
        [SaveOnReload] public static bool mapMode;

        public static Harmony harmony;

        // Compiled without dependencies on UnityModManagerNet
        static void Load(UnityModManager.ModEntry modEntry)
        {
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

            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                QuickSave(modEntry);

            }
            if (PT2.director.control.GRAB_HELD && PT2.director.control.CAM_PRESSED)
            {
                QuickLoad(modEntry);
            }
        }
        private static void QuickSave(UnityModManager.ModEntry modEntry)
        {
            modEntry.Logger.Log("num0");
            PT2.gale_interacter.DisplayNumAboveHead(10, DamageNumberLogic.DISPLAY_STYLE.HOVER_AND_FLASH_RED, true);

            // Save room
            room = Traverse.Create(typeof(PT2)).Field("_room_to_load").GetValue() as string;
            doorId = LevelBuildLogic.door_end_id;
            modEntry.Logger.Log("Saved room : " + room);

            // Save position
            position = PT2.gale_interacter.GetGaleTransform().position;
            encounterPosition = new Vector3(WorldMapFoeLogic.X_WHERE_BATTLE_OCCURRED, WorldMapFoeLogic.Y_WHERE_BATTLE_OCCURRED, 0f);
            //checkpoint = { PT2.gale_interacter._checkpoint_location, ...}
            modEntry.Logger.Log("Saved position : " + position);

            // Save camera
            camera = PT2.camera_control._curr_camera_config;

            // Saving Gail's exact state (rolling, dying, climbing, etc.) is probly not worth the effort
            //galeState = Traverse.Create(typeof(GaleLogicOne)).Field("StateFn").GetValue<Action>(PT2.gale_script);
            // Save more general mode
            FieldInfo field = typeof(GaleLogicOne).GetField("_gale_state_on_level_load", BindingFlags.NonPublic | BindingFlags.Instance);
            mapMode = (GALE_MODE)field.GetValue(PT2.gale_script) == GALE_MODE.MAP_MODE;
            modEntry.Logger.Log("Saved map mode : " + mapMode);
        }
        private static void QuickLoad(UnityModManager.ModEntry modEntry)
        {
            PT2.LoadLevel(room, doorId, Vector3.zero, false, 0.1f, false, true);
            if (mapMode)
            {
                PT2.gale_script.SetGaleModeOnLevelLoad(GALE_MODE.MAP_MODE);
            }
            else
            {
                PT2.gale_script.SetGaleModeOnLevelLoad(GALE_MODE.DEFAULT);
            }
            PT2.gale_script.SendGaleCommand(GALE_CMD.SET_GALE_MODE);
            PT2.gale_script.SendGaleCommand(GALE_CMD.RESET); //idk what this does but it removes at least 1 weird bug so

            PT2.gale_interacter.GetGaleTransform().position = position;
            WorldMapFoeLogic.X_WHERE_BATTLE_OCCURRED = encounterPosition.x;
            WorldMapFoeLogic.Y_WHERE_BATTLE_OCCURRED = encounterPosition.y;

            PT2.camera_control.SwitchCameraConfig(camera, 0, true);
            OpeningMenuLogic.EnableGameplayElements();

            PT2.gale_interacter.DisplayNumAboveHead(10, DamageNumberLogic.DISPLAY_STYLE.HOVER_AND_FLASH_GREEN, true);
            //TODO: add nice sound effects
            modEntry.Logger.Log("ロード済み");
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