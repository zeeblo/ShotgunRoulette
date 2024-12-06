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

                // Check if Item is usable (eg. not on ladder)
                MethodInfo CanUseItemRaw = typeof(PlayerControllerB).GetMethod("CanUseItem", BindingFlags.NonPublic | BindingFlags.Instance);
                bool CanUseItem = (bool)CanUseItemRaw.Invoke(localplayer, null);

                if (StartOfRound.Instance.inShipPhase) return true;
                if (Plugin.gunIsOnFace == false) return true;
                if (localplayer.currentlyHeldObjectServer == null) return true;
                if (CanUseItem == false) return true;


                if (PlayerControllerBPatch.rouletteMode)
                {
                    Plugin.mls.LogInfo(">>> Entering Roulette");
                    if (Plugin.rouletteNumber == 1)
                    {
                        RotatePlayer();
                        localplayer.DamagePlayer(Plugin.randomDamage, causeOfDeath: CauseOfDeath.Gunshots);
                        return true;
                    }

                    __instance.gunAudio.PlayOneShot(__instance.noAmmoSFX);
                    return false;
                }

                RotatePlayer();
                localplayer.DamagePlayer(Plugin.randomDamage, causeOfDeath: CauseOfDeath.Gunshots);
                
            }

            return true;
        }

        /// <summary>
        /// Forces player to look behind them so that when they shoot
        /// they dont kill the person infront of them
        /// </summary>
        private static void RotatePlayer()
        {
            PlayerControllerB localplayer = GameNetworkManager.Instance.localPlayerController;
            localplayer.thisPlayerBody.transform.Rotate(0, 180, 0);
            localplayer.gameplayCamera.transform.localEulerAngles = new UnityEngine.Vector3(0, 0, localplayer.gameplayCamera.transform.localEulerAngles.z);

        }


        [HarmonyPatch(nameof(ShotgunItem.Update))]
        [HarmonyPostfix]
        private static void UpdatePatch(ShotgunItem __instance)
        {
            __instance.shellsLoaded = 2;
        }

    }
}
