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
using ShotgunRoulette.UI;
using System.Reflection;
using ShotgunRoulette.Network;

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
        public static List<ConfigEntry<string>> AllHotkeys = new List<ConfigEntry<string>>();
        public static ConfigEntry<string>? gunRotationBind;
        public static System.Random random = new System.Random();
        public static int rouletteNumber = random.Next(1, 5);
        public static int randomDamage = random.Next(95, 145);
        public static string MainDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Replace("file:\\", "");
        public static string BundleDir = MainDir + "\\Assets\\Assetbundles\\roulette";
        public static AssetBundle myBundle = AssetBundle.LoadFromFile(BundleDir);
        public static AudioClip? SFX_revolverSpin;
        public static GameObject? networkPrefab;



        private void Awake()
        {
            instance = this;
            gunRotationBind = Config.Bind("Gameplay Controls", "Aim at you", "h", "Point the gun at yourself");
            AllHotkeys.Add(gunRotationBind);
            NetcodePatcher();
            PatchAll();
            AssetLoader();
            Controls.InitControls();

        }



        private void PatchAll()
        {
            _harmony.PatchAll(typeof(PlayerControllerBPatch));
            _harmony.PatchAll(typeof(NutcrackerEnemyAIPatch));
            _harmony.PatchAll(typeof(HUDManagerPatch));
            _harmony.PatchAll(typeof(ShotgunItemPatch));
            _harmony.PatchAll(typeof(KeybindsUI));
            _harmony.PatchAll(typeof(NetObjectManager));
        }


        private void NetcodePatcher()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }
        }

        public static bool ToggleGunRotation(PlayerControllerB __instance)
        {
            PlayerControllerB localplayer = GameNetworkManager.Instance.localPlayerController;
            if (__instance.ItemSlots[__instance.currentItemSlot] == null) return false;
            if (__instance.ItemSlots[__instance.currentItemSlot].GetComponent<ShotgunItem>() != null) return false;

            Plugin.gunIsOnFace = !Plugin.gunIsOnFace;
            
            if (Plugin.gunIsOnFace)
            {
                UnityEngine.Vector3 gunRotation = new UnityEngine.Vector3(0.28f, 0.28f, -0.28f);
                __instance.ItemSlots[__instance.currentItemSlot].transform.localScale = gunRotation;

                NetObjectManager.RequestFromPlayer(localplayer.playerClientId, __instance.currentItemSlot, gunRotation);
            }
            else
            {
                UnityEngine.Vector3 gunRotation = new UnityEngine.Vector3(0.28f, 0.28f, 0.28f);
                __instance.ItemSlots[__instance.currentItemSlot].transform.localScale = gunRotation;

                NetObjectManager.RequestFromPlayer(localplayer.playerClientId, __instance.currentItemSlot, gunRotation);
            }

            return Plugin.gunIsOnFace;
        }


        private static void AssetLoader()
        {
            AudioClip revolverSpin = myBundle.LoadAsset<AudioClip>("revolver-spin.mp3");
            SFX_revolverSpin = revolverSpin;
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
