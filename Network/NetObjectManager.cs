﻿using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using HarmonyLib;
using UnityEngine;

namespace ShotgunRoulette.Network
{
    [HarmonyPatch]
    internal class NetObjectManager
    {
        [HarmonyPatch(typeof(GameNetworkManager), nameof(GameNetworkManager.Start))]
        [HarmonyPostfix]
        private static void StartPatch(GameNetworkManager __instance)
        {
            if (Plugin.networkPrefab != null) return;

            Plugin.networkPrefab = Plugin.myBundle.LoadAsset<GameObject>("NetworkManager_roulette.prefab");
            Plugin.networkPrefab.AddComponent<NetHandler>();
            NetworkManager.Singleton.AddNetworkPrefab(Plugin.networkPrefab);
        }

        [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.Awake))]
        [HarmonyPostfix]
        static void SpawnNetworkHandler()
        {
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                if (Plugin.networkPrefab == null) return;

                GameObject networkHandlerHost = UnityEngine.Object.Instantiate(Plugin.networkPrefab, Vector3.zero, Quaternion.identity);
                networkHandlerHost.GetComponent<NetworkObject>().Spawn();
            }
        }


        [HarmonyPostfix, HarmonyPatch(typeof(RoundManager), nameof(RoundManager.GenerateNewFloor))]
        static void SubscribeToHandler()
        {
            NetHandler.LevelEvent += ReceivedEventFromServer;
        }

        [HarmonyPostfix, HarmonyPatch(typeof(RoundManager), nameof(RoundManager.DespawnPropsAtEndOfRound))]
        static void UnsubscribeFromHandler()
        {
            NetHandler.LevelEvent -= ReceivedEventFromServer;
        }

        static void ReceivedEventFromServer(string eventName)
        {

            Plugin.mls.LogInfo(">> Found: ReceivedEventFromServer");
            switch (eventName)
            {
                case "hello":
                    Plugin.mls.LogInfo("<> hello world! <>");
                    break;
            }
        }

        public static void RequestFromPlayer(ulong plrID, int itemPos, UnityEngine.Vector3 gunRotation)
        {
            Plugin.mls.LogInfo(">> in sendEvent 1");
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

            Plugin.mls.LogInfo(">> in sendEvent 3");
            
        }


    }
}
