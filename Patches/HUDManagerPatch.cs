using HarmonyLib;


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
                return false;
            }
            return true;
        }


    }
}
