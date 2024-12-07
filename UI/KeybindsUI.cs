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



        [HarmonyPatch(typeof(KepRemapPanel), nameof(KepRemapPanel.LoadKeybindsUI))]
        [HarmonyPrefix]
        private static bool LoadBindsPatch(KepRemapPanel __instance)
        {
            __instance.maxVertical += 1f;

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

    }
}
