using System.Reflection;
using GameNetcodeStuff;
using HarmonyLib;


namespace ShotgunRoulette.Patches
{
    [HarmonyPatch(typeof(ShotgunItem))]
    internal class ShotgunItemPatch
    {


        /// <summary>
        /// Kill the user if the gun is facing them
        /// </summary>
        [HarmonyPatch(nameof(ShotgunItem.ShootGunAndSync))]
        [HarmonyPrefix]
        private static bool ShootFace(ShotgunItem __instance)
        {
            if (StartOfRound.Instance.inShipPhase && Plugin.gunIsOnFace)
            {
                __instance.gunAudio.PlayOneShot(__instance.noAmmoSFX);
                return false;
            }
            if (Plugin.gunIsOnFace && __instance.shellsLoaded >= 1 && __instance.safetyOn == false)
            {
                PlayerControllerB localplayer = GameNetworkManager.Instance.localPlayerController;

                // Forces player to look behind them so that when they shoot
                // they dont kill the person infront of them
                localplayer.thisPlayerBody.transform.Rotate(0, 180, 0);
                localplayer.gameplayCamera.transform.localEulerAngles = new UnityEngine.Vector3(0, 0, localplayer.gameplayCamera.transform.localEulerAngles.z);

                // Check if Item is first usable
                MethodInfo CanUseItemRaw = typeof(PlayerControllerB).GetMethod("CanUseItem", BindingFlags.NonPublic | BindingFlags.Instance);
                bool CanUseItem = (bool)CanUseItemRaw.Invoke(localplayer, null);

                if (StartOfRound.Instance.inShipPhase) return true;
                if (Plugin.gunIsOnFace == false) return true;
                if (localplayer.currentlyHeldObjectServer == null) return true;
                if (CanUseItem == false) return true;

                localplayer.DamagePlayer(100, causeOfDeath: CauseOfDeath.Gunshots);
                Plugin.gunIsOnFace = false;

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
