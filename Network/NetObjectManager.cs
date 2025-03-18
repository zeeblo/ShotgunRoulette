using Unity.Netcode;
using HarmonyLib;
using UnityEngine;

namespace ShotgunRoulette.Network
{
    [HarmonyPatch]
    internal class NetObjectManager
    {
        static GameObject networkPrefab;

        [HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.Start))]
        [HarmonyPostfix]
        private static void StartPatch(GameNetworkManager __instance)
        {
            if (networkPrefab != null) return;

            networkPrefab = Plugin.myBundle.LoadAsset<GameObject>("NetworkManager_roulette.prefab");
            networkPrefab.AddComponent<NetHandler>();
            NetworkManager.Singleton.AddNetworkPrefab(networkPrefab);
        }



        [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.Awake))]
        [HarmonyPostfix]
        static void SpawnNetworkHandler()
        {
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                if (networkPrefab == null) return;

                GameObject networkHandlerHost = UnityEngine.Object.Instantiate(networkPrefab, Vector3.zero, Quaternion.identity);
                networkHandlerHost.GetComponent<NetworkObject>().Spawn();
            }
        }


        public static void RequestFromPlayer(ulong plrID, int itemPos, UnityEngine.Vector3 gunRotation)
        {
            if (NetHandler.instance == null) return;

            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                // host sending info
                NetHandler.instance.RotateGunClientRpc(plrID, itemPos, gunRotation);
            }
            else
            {
                // client requesting info
                NetHandler.instance.RotateGunServerRpc(plrID, itemPos, gunRotation);
            }

        }


    }
}