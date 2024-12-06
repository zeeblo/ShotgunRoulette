using GameNetcodeStuff;
using HarmonyLib;
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
                string options = (Plugin.gunIsOnFace) ? "\n Roulette : [RMB] \n Shoot Yourself : [LMB]" : "";
                string safety = (__instance.currentlyHeldObjectServer.GetComponent<ShotgunItem>().safetyOn) ? "off" : "on";
                HUDManager.Instance.controlTipLines.Last().text = $"Turn Safety {safety} : [Q] \n Aim at you : [H] " + options;
            }
        }



    }
}
