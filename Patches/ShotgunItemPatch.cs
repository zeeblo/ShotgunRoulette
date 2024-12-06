using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using UnityEngine.InputSystem;

namespace ShotgunRoulette.Patches
{
    [HarmonyPatch(typeof(ShotgunItem))]
    internal class ShotgunItemPatch
    {


        /// <summary>
        /// Prevent user from shooting potential entity infront of them
        /// </summary>
        [HarmonyPatch(nameof(ShotgunItem.ShootGunAndSync))]
        [HarmonyPrefix]
        private static bool ShootFace(ShotgunItem __instance)
        {
            if (StartOfRound.Instance.inShipPhase && Plugin.rouletteEnabled)
            {
                return false;
            }
            if (Plugin.rouletteEnabled)
            {
                GameNetworkManager.Instance.localPlayerController.thisPlayerBody.transform.Rotate(0, 180, 0);
                GameNetworkManager.Instance.localPlayerController.gameplayCamera.transform.localEulerAngles = new UnityEngine.Vector3(0, 0, GameNetworkManager.Instance.localPlayerController.gameplayCamera.transform.localEulerAngles.z);
            }

            return true;
        }


        /// <summary>
        /// Prevent user from shooting themself in space
        /// </summary>
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


        [HarmonyPatch(nameof(ShotgunItem.Update))]
        [HarmonyPostfix]
        private static void UpdatePatch(ShotgunItem __instance)
        {
            __instance.shellsLoaded = 2;
        }

    }
}
