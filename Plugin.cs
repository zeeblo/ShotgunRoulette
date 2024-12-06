using HarmonyLib;
using BepInEx;
using BepInEx.Logging;

using UnityEngine;
using ShotgunRoulette.Patches;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Collections;
using GameNetcodeStuff;
using BepInEx.Configuration;
using ShotgunRoulette.Players;

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
        public static ConfigEntry<string>? gunRotationBind;


        private void Awake()
        {
            gunRotationBind = Config.Bind("Gameplay Controls", "Gun Rotation", "h", "Point the gun at yourself");
            PatchAll();
            Controls.InitControls();

        }



        private void PatchAll()
        {
            _harmony.PatchAll(typeof(PlayerControllerBPatch));
            _harmony.PatchAll(typeof(NutcrackerEnemyAIPatch));
            //_harmony.PatchAll(typeof(HUDManagerPatch));
            _harmony.PatchAll(typeof(ShotgunItemPatch));
        }


        public static bool ToggleGunRotation(PlayerControllerB __instance)
        {
            if (__instance.currentlyHeldObjectServer == null) return false;
            if (!__instance.currentlyHeldObjectServer.itemProperties.itemName.ToLower().Contains("shotgun")) return false;

            Plugin.rouletteEnabled = !Plugin.rouletteEnabled;
            if (Plugin.rouletteEnabled)
            {
                __instance.currentlyHeldObjectServer.transform.localScale = new UnityEngine.Vector3(0.28f, 0.28f, -0.28f);
            }
            else
            {
                __instance.currentlyHeldObjectServer.transform.localScale = new UnityEngine.Vector3(0.28f, 0.28f, 0.28f);
            }

            return Plugin.rouletteEnabled;
        }



        public static void SpawnShotgun()
        {
            Vector3 position = GameNetworkManager.Instance.localPlayerController.transform.position;
            SelectableLevel currentLevel = RoundManager.Instance.playersManager.levels[6];

            UnityEngine.Object.Instantiate<GameObject>(currentLevel.Enemies[9].enemyType.enemyPrefab, position, Quaternion.identity)
                .gameObject.GetComponentInChildren<NetworkObject>().Spawn(true);

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
