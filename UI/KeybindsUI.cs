using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using ShotgunRoulette.Players;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine;
using System.Data;
using System.Collections;
using Dissonance;

namespace ShotgunRoulette.UI
{

    [HarmonyPatch]
    internal class KeybindsUI
    {
        private static List<RemappableKey> allCustomBinds = new List<RemappableKey>();
        private static bool addedCustomBinds = false;

        public static void AddCustomBinds()
        {
            if (Controls.gunRotationRef == null) return;

            RemappableKey aim_at_you = new RemappableKey
            {
                ControlName = "Aim at you",
                currentInput = Controls.gunRotationRef,
                rebindingIndex = 0,
                gamepadOnly = false
            };

            allCustomBinds.Add(aim_at_you);
        }


        /// <summary>
        /// Visually display all the custom keybinds
        /// </summary>
        [HarmonyPatch(typeof(KepRemapPanel), nameof(KepRemapPanel.LoadKeybindsUI))]
        [HarmonyPrefix]
        private static bool LoadBindsPatch(KepRemapPanel __instance)
        {
            __instance.maxVertical += 1f;
            Controls.userControls.Disable();

            if (!addedCustomBinds)
            {
                for (int i = 0; i < allCustomBinds.Count; i++)
                {
                    __instance.remappableKeys.Insert(0, allCustomBinds[i]);
                }
                addedCustomBinds = true;
            }

            return true;
        }




        /// <summary>
        /// Set the new keybind and update it in the .cfg
        /// </summary>
        [HarmonyPatch(typeof(IngamePlayerSettings), nameof(IngamePlayerSettings.CompleteRebind))]
        [HarmonyPostfix]
        private static void CompleteRebindPatch(SettingsOption optionUI)
        {
            for (int i = 0; i < Plugin.AllHotkeys.Count; i++)
            {
                if (Plugin.AllHotkeys[i].Definition.Key.ToLower() == optionUI.textElement.text.ToLower())
                {

                    Plugin.AllHotkeys[i].Value = optionUI.currentlyUsedKeyText.text; // Set new keybind button
                    Plugin.AllHotkeys[i].ConfigFile.Save();
                    Controls.userControls.FindAction(Plugin.AllHotkeys[i].Definition.Key.ToLower()).ApplyBindingOverride($"<Keyboard>/{optionUI.currentlyUsedKeyText.text.ToLower()}");
                    break;


                }
            }

        }


        /// <summary>
        /// If the user closes the Keybind Menu, re-enable the custom keybinds.
        /// </summary>
        [HarmonyPatch(typeof(KepRemapPanel), nameof(KepRemapPanel.OnDisable))]
        [HarmonyPostfix]
        private static void KepOnDisablePatch()
        {
            if (Controls.userControls.enabled == false)
            {
                Controls.userControls.Enable();
            }

        }



    }
}
