using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine.InputSystem;

namespace ShotgunRoulette.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch
    {

        [HarmonyPatch("Update")]
        [HarmonyPostfix]
        private static void UpdatePatch()
        {

            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                Plugin.SpawnShotgun();
            }
        }


    }
}
