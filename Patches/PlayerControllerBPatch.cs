using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine.InputSystem;

namespace ShotgunRoulette.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch
    {
        public static bool rouletteMode = false;

        [HarmonyPatch(nameof(PlayerControllerB.Update))]
        [HarmonyPostfix]
        private static void UpdatePatch(PlayerControllerB __instance)
        {

            if (Keyboard.current.jKey.wasPressedThisFrame)
            {
                Plugin.SpawnShotgun();
            }

            if (Plugin.gunIsOnFace)
            {
                HUDManager.Instance.ShakeCamera(ScreenShakeType.Small);
            }
            
            if (__instance.currentlyHeldObjectServer != null && __instance.currentlyHeldObjectServer.itemProperties.itemName.ToLower().Contains("shotgun"))
            {
                string options = (Plugin.gunIsOnFace) ? $"\n Roulette Mode ({rouletteMode}) : [RMB] \n Shoot Yourself : [LMB]" : "";
                string safety = (__instance.currentlyHeldObjectServer.GetComponent<ShotgunItem>().safetyOn) ? "off" : "on";
                HUDManager.Instance.controlTipLines.Last().text = $"Turn Safety {safety} : [Q] \n Aim at you : [H] " + options;
            }
        }


        /// <summary>
        /// Reset gun rotation when player dies
        /// </summary>
        [HarmonyPatch(nameof(PlayerControllerB.KillPlayer))]
        [HarmonyPrefix]
        private static void KillPlayerPatch(PlayerControllerB __instance)
        {
            Plugin.gunIsOnFace = false;
        }


        [HarmonyPatch(nameof(PlayerControllerB.ScrollMouse_performed))]
        [HarmonyPrefix]
        private static void ScrollPatch(PlayerControllerB __instance)
        {
            if (Plugin.gunIsOnFace)
            {
                Plugin.ToggleGunRotation(__instance);
            }
        }


        [HarmonyPatch(nameof(PlayerControllerB.SwitchToItemSlot))]
        [HarmonyPrefix]
        private static void SwitchToItemSlotPatch(PlayerControllerB __instance)
        {
            if (Plugin.gunIsOnFace)
            {
                Plugin.ToggleGunRotation(__instance);
            }
        }


        [HarmonyPatch(nameof(PlayerControllerB.DiscardHeldObject))]
        [HarmonyPrefix]
        private static void DiscardHeldObjectPatch(PlayerControllerB __instance)
        {
            if (Plugin.gunIsOnFace)
            {
                Plugin.ToggleGunRotation(__instance);
            }
        }

        [HarmonyPatch(nameof(PlayerControllerB.OnDisable))]
        [HarmonyPrefix]
        private static void OnDisablePatch(PlayerControllerB __instance)
        {
            if (Plugin.gunIsOnFace)
            {
                Plugin.ToggleGunRotation(__instance);
            }
        }


    }
}
