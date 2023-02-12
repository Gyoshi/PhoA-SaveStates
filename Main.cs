using HarmonyLib;
using System.Reflection;
using UnityEngine;
using UnityModManagerNet;
using static UnityModManagerNet.UnityModManager;

namespace SaveStates
{
    #if DEBUG
    [EnableReloading]
    #endif
    static class Main
    {
        //[SaveOnReload]
        //public static string name;

        private static Harmony harmony;

        // Compiled without dependencies on UnityModManagerNet
        static void Load(UnityModManager.ModEntry modEntry)
        {
            modEntry.OnUpdate = OnUpdate;
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

        private static void OnUpdate(UnityModManager.ModEntry modEntry, float dt)
        {
            
            if (Input.GetKeyDown(KeyCode.Keypad0))
            {

                modEntry.Logger.Log("num0");
                PT2.gale_interacter.DisplayNumAboveHead(0, DamageNumberLogic.DISPLAY_STYLE.NUM_POPUP_N_FALL_GRAY);
            }
            if (PT2.director.control.CROUCH_PRESSED)
            {
                modEntry.Logger.Log(":)");
                PT2.LoadLevel("test_fps", 0, Vector3.zero, false, 0.1f, false, true);
                PT2.gale_interacter.DisplayNumAboveHead(0, DamageNumberLogic.DISPLAY_STYLE.GALE_DAMAGE);
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