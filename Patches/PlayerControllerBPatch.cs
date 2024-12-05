using System.Numerics;
using System.Reflection;
using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ShotgunRoulette.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch
    {


        [HarmonyPatch(nameof(PlayerControllerB.Update))]
        [HarmonyPostfix]
        private static void UpdatePatch(PlayerControllerB __instance)
        {

            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                Plugin.SpawnShotgun();
            }
            
        }


        [HarmonyPatch(nameof(PlayerControllerB.ActivateItem_performed))]
        [HarmonyPostfix]
        private static void Roulette(PlayerControllerB __instance)
        {
            // Check if Item is first usable
            MethodInfo CanUseItemRaw = typeof(PlayerControllerB).GetMethod("CanUseItem", BindingFlags.NonPublic | BindingFlags.Instance);
            bool CanUseItem = (bool)CanUseItemRaw.Invoke(__instance, null);

            if (Plugin.rouletteEnabled == false) return;
            if (__instance.currentlyHeldObjectServer == null) return;
            if (CanUseItem == false) return;


            //UnityEngine.Vector3 playerPos = __instance.transform.position;

            //__instance.KillPlayer(playerPos);
        }

    }
}
