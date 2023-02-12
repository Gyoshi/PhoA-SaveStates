using HarmonyLib;
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
        [SaveOnReload]
        public static string savedRoom;

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

        private static void OnUpdate(UnityModManager.ModEntry modEntry, float dt)
        {
            //GaleInteracter galeInteracter = (GaleInteracter)typeof(PT2).GetField("gale_interacter", BindingFlags.Static).GetValue(null);
            if (Input.GetKeyDown(KeyCode.Keypad0))
            {
                modEntry.Logger.Log("num0");
                savedRoom = typeof(PT2).GetField("_room_to_load", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null).ToString();
                modEntry.Logger.Log("Saved room : " + savedRoom);
                //galeInteracter.DisplayNumAboveHead(0, DamageNumberLogic.DISPLAY_STYLE.GALE_DAMAGE, false);
            }
            if (PT2.director.control.GRAB_HELD && PT2.director.control.CAM_PRESSED)
            {
                modEntry.Logger.Log(":)");
                PT2.LoadLevel(savedRoom, 0, Vector3.zero, false, 0.1f, false, true);
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