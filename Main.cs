﻿using HarmonyLib;
using System;
using System.Reflection;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityModManagerNet;

namespace SaveStates
{
    #if DEBUG
    [EnableReloading]
    #endif
    static class Main
    {
        [SaveOnReload] public static string room;
        [SaveOnReload] public static Vector3 position;
        [SaveOnReload] public static GALE_MODE mode;
        [SaveOnReload] public static GALE_MODE loadMode;

        public static Harmony harmony;

        // Compiled without dependencies on UnityModManagerNet
        static void Load(UnityModManager.ModEntry modEntry)
        {
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
                modEntry.Logger.Log("num0");
                PT2.gale_interacter.DisplayNumAboveHead(10, DamageNumberLogic.DISPLAY_STYLE.HOVER_AND_FLASH_RED, true);

                // Save room
                room = Traverse.Create(typeof(PT2)).Field("_room_to_load").GetValue() as string;
                modEntry.Logger.Log("Saved room : " + room);

                //object[] args = { 1, DamageNumberLogic.DISPLAY_STYLE.GALE_DAMAGE, false };
                //MethodInfo displayNumber = typeof(GaleInteracter).GetMethod("DisplayNumAboveHead");
                //modEntry.Logger.Log(displayNumber.Name);
                //object interacterInstance = typeof(PT2).GetField("gale_interacter").GetValue(null);
                //modEntry.Logger.Log("Attempting Invocation");
                //displayNumber.Invoke(interacterInstance, args);

                // Save position
                position = PT2.gale_interacter.GetGaleTransform().position;
                modEntry.Logger.Log("saved position : " + position);

                //mode = ???;
                //loadMode = Traverse.Create(typeof(GaleLogicOne)).Field("_gale_state_on_level_load").GetValue<GALE_MODE>(PT2.gale_script);
            }
            if (PT2.director.control.GRAB_HELD && PT2.director.control.CAM_PRESSED)
            {
                //PT2.gale_script.SetGaleModeOnLevelLoad(mode);
                //PT2.gale_script.SendGaleCommand(GALE_CMD.SET_GALE_MODE);
                //PT2.gale_script.SetGaleModeOnLevelLoad(loadMode);

                PT2.LoadLevel(room, 0, Vector3.zero, false, 0.1f, false, true);
                PT2.gale_interacter.GetGaleTransform().position = position;
                OpeningMenuLogic.EnableGameplayElements();

                PT2.gale_interacter.DisplayNumAboveHead(10, DamageNumberLogic.DISPLAY_STYLE.HOVER_AND_FLASH_GREEN, true);
                modEntry.Logger.Log("ロード済み");
            }
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