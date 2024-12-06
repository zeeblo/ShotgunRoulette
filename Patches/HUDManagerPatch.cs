using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using TMPro;

namespace ShotgunRoulette.Patches
{
    [HarmonyPatch(typeof(HUDManager))]
    internal class HUDManagerPatch
    {

        [HarmonyPatch(nameof(HUDManager.ChangeControlTipMultiple))]
        [HarmonyPrefix]
        private static bool UITips(HUDManager __instance, string[] allLines, bool holdingItem = false, Item? itemProperties = null)
        {

            if (holdingItem && itemProperties != null)
            {
                __instance.controlTipLines[0].text = "Drop " + itemProperties.itemName + " : [G]";
                Array.Resize(ref allLines, allLines.Length + 1);
                allLines[allLines.Length - 1] = "Roulette : [H]";

            }
            if (allLines == null)
            {
                return false;
            }
            int num = 0;
            if (holdingItem)
            {
                num = 1;
            }
            for (int i = 0; i < allLines.Length && i + num < __instance.controlTipLines.Length; i++)
            {
                string text = allLines[i];
                if (StartOfRound.Instance.localPlayerUsingController)
                {
                    StringBuilder stringBuilder = new StringBuilder(text);
                    stringBuilder.Replace("[E]", "[D-pad up]");
                    stringBuilder.Replace("[Q]", "[D-pad down]");
                    stringBuilder.Replace("[LMB]", "[Y]");
                    stringBuilder.Replace("[RMB]", "[R-Trigger]");
                    stringBuilder.Replace("[G]", "[B]");
                    text = stringBuilder.ToString();
                }
                else
                {
                    text = text.Replace("[RMB]", "[LMB]");
                }
                __instance.controlTipLines[i + num].text = text;
            }

            /*
            if (itemProperties != null && itemProperties.itemName.ToLower().Contains("shotgun"))
            {
                __instance.controlTipLines.Last().text = $"{HUDManager.Instance.controlTipLines.Last().text} \n Roulette : [H]";
            }
            */
            return false;
        }


    }
}
