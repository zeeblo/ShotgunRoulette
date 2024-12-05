using GameNetcodeStuff;
using HarmonyLib;
using UnityEngine.InputSystem;

namespace ShotgunRoulette.Patches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    internal class PlayerControllerBPatch
    {


        private static void CheckForGun(PlayerControllerB __instance)
        {
            if (__instance.currentlyHeldObjectServer != null && __instance.currentlyHeldObjectServer.itemProperties.itemName.ToLower().Contains("shotgun"))
            {
                //HUDManager.Instance.ClearControlTips();
                HUDManager.Instance.controlTipLines.Last().text = $"{HUDManager.Instance.controlTipLines.Last().text} \n Roulette : [H]";
            }

        }

        [HarmonyPatch(nameof(PlayerControllerB.Update))]
        [HarmonyPostfix]
        private static void UpdatePatch(PlayerControllerB __instance)
        {

            if (Keyboard.current.digit1Key.wasPressedThisFrame)
            {
                Plugin.SpawnShotgun();
            }

            if (Keyboard.current.hKey.wasPressedThisFrame)
            {
                if (__instance.currentlyHeldObjectServer != null && __instance.currentlyHeldObjectServer.itemProperties.itemName.ToLower().Contains("shotgun"))
                {
                    __instance.currentlyHeldObjectServer.transform.localScale = new UnityEngine.Vector3(0.2f, 0.2f, -0.2f);
                }
            }
        }

        [HarmonyPatch(nameof(PlayerControllerB.GrabObject))]
        [HarmonyPostfix]
        private static void BeginGrabObjectPatch(PlayerControllerB __instance)
        {
            CheckForGun(__instance);
        }

        [HarmonyPatch(nameof(PlayerControllerB.SwitchToItemSlot))]
        [HarmonyPostfix]
        private static void SwitchToItemSlotPatch(PlayerControllerB __instance)
        {
            CheckForGun(__instance);
        }

    }
}
