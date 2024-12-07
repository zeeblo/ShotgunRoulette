﻿using HarmonyLib;
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
using ShotgunRoulette.UI;

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
        //public static bool rouletteEnabled = false;
        public static bool gunIsOnFace = false;
        public static ConfigEntry<string>? gunRotationBind;
        public static System.Random random = new System.Random();
        public static int rouletteNumber = random.Next(1, 5);
        public static int randomDamage = random.Next(95, 145);
        public static List<ConfigEntry<string>> AllHotkeys = new List<ConfigEntry<string>>();


        private void Awake()
        {
            gunRotationBind = Config.Bind("Gameplay Controls", "Aim at you", "h", "Point the gun at yourself");
            AllHotkeys.Add(gunRotationBind);

            PatchAll();
            Controls.InitControls();

        }



        private void PatchAll()
        {
            _harmony.PatchAll(typeof(PlayerControllerBPatch));
            _harmony.PatchAll(typeof(NutcrackerEnemyAIPatch));
            _harmony.PatchAll(typeof(HUDManagerPatch));
            _harmony.PatchAll(typeof(ShotgunItemPatch));
            _harmony.PatchAll(typeof(KeybindsUI));
        }


        public static bool ToggleGunRotation(PlayerControllerB __instance)
        {
            if (__instance.currentlyHeldObjectServer == null) return false;
            if (!__instance.currentlyHeldObjectServer.itemProperties.itemName.ToLower().Contains("shotgun")) return false;

            Plugin.gunIsOnFace = !Plugin.gunIsOnFace;
            if (Plugin.gunIsOnFace)
            {
                __instance.currentlyHeldObjectServer.transform.localScale = new UnityEngine.Vector3(0.28f, 0.28f, -0.28f);
            }
            else
            {
                __instance.currentlyHeldObjectServer.transform.localScale = new UnityEngine.Vector3(0.28f, 0.28f, 0.28f);
            }

            return Plugin.gunIsOnFace;
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
