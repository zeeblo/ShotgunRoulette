using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.InputSystem;
using DunGen;

namespace ShotgunRoulette.Utils
{

    [HarmonyPatch]
    internal class GunTests
    {

        #region NutcrackerPatch

        public static bool spawnedNut = false;

        [HarmonyPatch(typeof(NutcrackerEnemyAI), nameof(NutcrackerEnemyAI.Update))]
        [HarmonyPrefix]
        private static bool UpdatePatch(NutcrackerEnemyAI __instance)
        {
            if (spawnedNut == false)
            {
                __instance.KillEnemy();
                spawnedNut = true;
                StartOfRound.Instance.StartCoroutine(DespawnEnemies());
            }

            return true;
        }

        [HarmonyPatch(nameof(ShotgunItem.Update))]
        [HarmonyPostfix]
        private static void UpdatePatch(ShotgunItem __instance)
        {
            __instance.shellsLoaded = 2;
        }

        #endregion NutcrackerPatch


        public static void InitTestControls()
        {
            Plugin.mls.LogInfo(">> added keybind");
            InputAction key = new InputAction("spawn", binding:"<Keyboard>/J", type: InputActionType.Button);
            key.performed += KeyPressed_performed;
            key.Enable();
        }

        private static void KeyPressed_performed(InputAction.CallbackContext context)
        {
            Plugin.mls.LogInfo("pressed");
            SpawnShotgun();
            spawnedNut = false;
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
                    Plugin.Destroy(obj.gameObject);
                }
            }
        }



    }
}
