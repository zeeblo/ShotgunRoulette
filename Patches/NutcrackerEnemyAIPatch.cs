using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;

namespace ShotgunRoulette.Patches
{
    [HarmonyPatch(typeof(NutcrackerEnemyAI))]
    internal class NutcrackerEnemyAIPatch
    {
        public static bool spawnedNut = false;

        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        private static bool UpdatePatch(NutcrackerEnemyAI __instance)
        {
            if (spawnedNut == false)
            {
                __instance.KillEnemy();
                spawnedNut = true;
                StartOfRound.Instance.StartCoroutine(Plugin.DespawnEnemies());
            }

            return true;
        }
    }
}
