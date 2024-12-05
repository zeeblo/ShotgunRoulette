using HarmonyLib;
using BepInEx;
using BepInEx.Logging;

using UnityEngine;
using ShotgunRoulette.Patches;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Collections;
using GameNetcodeStuff;

namespace ShotgunRoulette
{
    [BepInPlugin(MOD_GUID, MOD_Name, MOD_Version)]
    public class Plugin : BaseUnityPlugin
    {
        private const string MOD_GUID = "ShotgunRoulette.zeeblo.dev";
        private const string MOD_Name = "zeeblo.ShotgunRoulette";
        private const string MOD_Version = "0.1.0";
        private readonly Harmony _harmony = new(MOD_GUID);
        public static Plugin? instance;
        internal static ManualLogSource mls = BepInEx.Logging.Logger.CreateLogSource(MOD_GUID);
        public static bool rouletteEnabled = false;



        private void Awake()
        {
            PatchAll();
            Plugin.mls.LogInfo("Hello World");
        }



        private void PatchAll()
        {
            _harmony.PatchAll(typeof(PlayerControllerBPatch));
            _harmony.PatchAll(typeof(NutcrackerEnemyAIPatch));
            _harmony.PatchAll(typeof(HUDManagerPatch));
        }


        public static void SpawnShotgun()
        {
            Vector3 position = GameNetworkManager.Instance.localPlayerController.transform.position;
            SelectableLevel currentLevel = RoundManager.Instance.playersManager.levels[6];

            UnityEngine.Object.Instantiate<GameObject>(currentLevel.Enemies[9].enemyType.enemyPrefab, position, Quaternion.identity)
                .gameObject.GetComponentInChildren<NetworkObject>().Spawn(true);

        }

        public static bool ToggleRoulette(PlayerControllerB __instance)
        {
            Plugin.mls.LogInfo($">>> roulette_b4: {Plugin.rouletteEnabled}");
            Plugin.mls.LogInfo($">>> roulette_tstb4: {!Plugin.rouletteEnabled}");

            Plugin.rouletteEnabled = !Plugin.rouletteEnabled;
            if (Plugin.rouletteEnabled)
            {
                if (__instance.currentlyHeldObjectServer != null && __instance.currentlyHeldObjectServer.itemProperties.itemName.ToLower().Contains("shotgun"))
                {
                    __instance.currentlyHeldObjectServer.transform.localScale = new UnityEngine.Vector3(0.28f, 0.28f, -0.28f);
                }
                Plugin.mls.LogInfo("<><>in enabled roulette");
            }
            else
            {
                if (__instance.currentlyHeldObjectServer != null && __instance.currentlyHeldObjectServer.itemProperties.itemName.ToLower().Contains("shotgun"))
                {
                    __instance.currentlyHeldObjectServer.transform.localScale = new UnityEngine.Vector3(0.28f, 0.28f, 0.28f);
                }
                Plugin.mls.LogInfo("<><>in DISABLED roulette");
            }
            Plugin.mls.LogInfo($">>> roulette_a4: {Plugin.rouletteEnabled}");
            Plugin.mls.LogInfo($">>> roulette_tsta4: {!Plugin.rouletteEnabled}");
            return Plugin.rouletteEnabled;
        }

        /// <summary>
        /// Removes extra NutCracker(s) that may have spawned
        /// </summary>
        public static IEnumerator DespawnEnemies()
        {
            yield return new WaitForSeconds(3);

            Scene SampleScene = SceneManager.GetSceneAt(0);
            foreach (GameObject obj in SampleScene.GetRootGameObjects())
            {
                if (obj.name.Contains("Nutcracker"))
                {
                    Destroy(obj.gameObject);
                }
            }
        }
    }
}
