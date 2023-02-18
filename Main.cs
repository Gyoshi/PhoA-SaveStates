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
        public static Dictionary<int, QuickSaveData> slots = new Dictionary<int, QuickSaveData>();
        public static int maxSlot = 16;

        public static QuickSaveData data;
        public static string dataPath;
        public static int currentSlot = 1;
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
            readFiles(dataPath);
            data = GetSlot(ref currentSlot);

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
                    data.QuickSave();
                    slots[currentSlot] = data;

                    // Save quicksave data to disk
                    data.Write(dataPath);

                    PT2.sound_g.PlayGlobalCommonSfx(122, 1f, 0.5f, 1);
                    PT2.display_messages.DisplayMessage("Saved to Slot " + currentSlot, DisplayMessagesLogic.MSG_TYPE.GALE_MINUS_STATUS);
                }
                // Load
                bool loadRequested = PT2.director.control.GRAB_PRESSED || Input.GetKeyDown(KeyCode.End);
                if (loadRequested && data.loadAvailable)
                {
                    data.QuickLoad();

                    PT2.sound_g.PlayGlobalCommonSfx(96, 0.7f, 1.5f, 2);
                    PT2.display_messages.DisplayMessage("Loaded Slot " + currentSlot, DisplayMessagesLogic.MSG_TYPE.GALE_PLUS_STATUS);
                }
                if (loadRequested && !data.loadAvailable)
                {
                    PT2.sound_g.PlayGlobalCommonSfx(134, 1f, 1f, 2);
                    PT2.display_messages.DisplayMessage("Slot " + currentSlot + " is empty!", DisplayMessagesLogic.MSG_TYPE.INVENTORY_FULL);
                }
                // Swap slots
                if (PT2.director.control.SPRINT_PRESSED || Input.GetKeyDown(KeyCode.PageUp)) { currentSlot--; }
                else if (PT2.director.control.CROUCH_PRESSED || Input.GetKeyDown(KeyCode.PageDown)) { currentSlot++; }
                else { goto NOSWAP; }
                data = GetSlot(ref currentSlot);
                PT2.sound_g.PlayGlobalCommonSfx(124, 1f, 1f, 2);
                
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

        public static void readFiles(string dataPath)
        {
            foreach (string file in Directory.GetFiles(dataPath))
            {
                int slotNumber = int.Parse(Path.GetFileName(file).Remove(2));
                if (slotNumber < 1 || slotNumber > maxSlot)
                    continue;

                slots[slotNumber] = QuickSaveData.Read(file);
            }
        }
        private static QuickSaveData GetSlot(ref int slotNumber)
        {
            // Adding max because % does negative values wrong
            slotNumber = (slotNumber + maxSlot - 1) % maxSlot + 1;

            string message = "Slot " + slotNumber;

            if (slots.ContainsKey(slotNumber))
            {
                data = slots[slotNumber];
                message += "<sprite=30>";
            }
            else
            {
                data = new QuickSaveData();
            }
            PT2.display_messages.DisplayMessage(message, DisplayMessagesLogic.MSG_TYPE.SMALL_ITEM_GET);
            return data;
        }
    }

    [HarmonyPatch(typeof(CameraLogic), "ZoomSimple")]
    public class Zoom_Patch
    {
        public static bool Prefix()
        {
            if (Main.CAM_HELD && Main.player.GetButtonDown("Alt Tool"))
                return true;
            return false;
        }
    }
}