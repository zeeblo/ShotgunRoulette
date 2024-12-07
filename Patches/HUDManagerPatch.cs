using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;


namespace ShotgunRoulette.Patches
{
    [HarmonyPatch(typeof(HUDManager))]
    internal class HUDManagerPatch
    {

        /// <summary>
        /// When user presses [RMB] they'll toggle the roulette mode
        /// </summary>
        [HarmonyPatch(nameof(HUDManager.PingScan_performed))]
        [HarmonyPrefix]
        private static bool Roulette()
        {
            if (Plugin.gunIsOnFace)
            {
                Plugin.rouletteNumber = Plugin.random.Next(1, 5);
                Plugin.randomDamage = Plugin.random.Next(95, 145);
                Plugin.mls.LogInfo($">>> Random Number: {Plugin.rouletteNumber} | Dmg: {Plugin.randomDamage}");

                PlayerControllerBPatch.rouletteMode = !PlayerControllerBPatch.rouletteMode;

                if (PlayerControllerBPatch.rouletteMode)
                {
                    PlayerControllerB localplayer = GameNetworkManager.Instance.localPlayerController;
                    localplayer.currentlyHeldObjectServer.gameObject.GetComponent<AudioSource>().PlayOneShot(Plugin.SFX_revolverSpin, 0.6f);
                }

                return false;
            }
            return true;
        }


    }
}
