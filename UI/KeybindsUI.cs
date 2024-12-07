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

        private static void AddCustomBinds()
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

            AddCustomBinds();
            int myMaxVertical = 8;
            __instance.currentVertical = 0;
            __instance.currentHorizontal = 0;
            Vector2 anchoredPosition = new Vector2(__instance.horizontalOffset * (float)__instance.currentHorizontal, __instance.verticalOffset * (float)__instance.currentVertical);
            bool flag = false;
            int num = 0;


            List<RemappableKey> mergedBinds = allCustomBinds.Concat(__instance.remappableKeys).ToList();
            for (int i = 0; i < mergedBinds.Count; i++)
            {
                
                if (mergedBinds[i].currentInput == null)
                {
                    continue;
                }

                Plugin.mls.LogInfo($"anchorPos: {anchoredPosition}");
                Plugin.mls.LogInfo($"currentVert: {__instance.currentVertical} | {__instance.currentHorizontal}");

                GameObject gameObject = UnityEngine.Object.Instantiate(__instance.keyRemapSlotPrefab, __instance.keyRemapContainer);
                __instance.keySlots.Add(gameObject);
                gameObject.GetComponentInChildren<TextMeshProUGUI>().text = mergedBinds[i].ControlName;
                gameObject.GetComponent<RectTransform>().anchoredPosition = anchoredPosition;
                SettingsOption componentInChildren = gameObject.GetComponentInChildren<SettingsOption>();
                componentInChildren.rebindableAction = mergedBinds[i].currentInput;
                componentInChildren.rebindableActionBindingIndex = mergedBinds[i].rebindingIndex;
                componentInChildren.gamepadOnlyRebinding = mergedBinds[i].gamepadOnly;


                int rebindingIndex = mergedBinds[i].rebindingIndex;
                int num2 = Mathf.Max(rebindingIndex, 0);
                componentInChildren.currentlyUsedKeyText.text = InputControlPath.ToHumanReadableString(componentInChildren.rebindableAction.action.bindings[num2].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);


                if (!flag && i + 1 < mergedBinds.Count && mergedBinds[i + 1].gamepadOnly)
                {
                    num = (int)(myMaxVertical + 2f);
                    __instance.currentVertical = 0;
                    __instance.currentHorizontal = 0;
                    GameObject gameObject2 = UnityEngine.Object.Instantiate(__instance.sectionTextPrefab, __instance.keyRemapContainer);
                    gameObject2.GetComponent<RectTransform>().anchoredPosition = new Vector2(-40f, (0f - __instance.verticalOffset) * (float)num);
                    gameObject2.GetComponentInChildren<TextMeshProUGUI>().text = "REBIND CONTROLLERS";
                    __instance.keySlots.Add(gameObject2);
                    flag = true;
                }
                else
                {
                    __instance.currentVertical++;
                    if ((float)__instance.currentVertical > myMaxVertical)
                    {
                        __instance.currentVertical = 0;
                        __instance.currentHorizontal++;
                    }
                    /*
                    if ((float)__instance.currentHorizontal > 2)
                    {
                        __instance.currentVertical++;
                        __instance.currentHorizontal = 0;
                    }
                    */
                }
                anchoredPosition = new Vector2(__instance.horizontalOffset * (float)__instance.currentHorizontal, (0f - __instance.verticalOffset) * (float)(__instance.currentVertical + num));
            }
            return false;
        }

    }
}
