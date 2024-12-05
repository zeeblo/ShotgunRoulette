using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;

namespace ShotgunRoulette.Patches
{
    [HarmonyPatch(typeof(ShotgunItem))]
    internal class ShotgunItemPatch
    {

        [HarmonyPatch(nameof(ShotgunItem.ShootGun))]
        [HarmonyPrefix]
        private static bool ShootGunPatch(ShotgunItem __instance)
        {
            if (StartOfRound.Instance.inShipPhase && Plugin.rouletteEnabled)
            {
                __instance.gunAudio.PlayOneShot(__instance.noAmmoSFX);
                return false;
            }
            return true;
        }

    }
}
