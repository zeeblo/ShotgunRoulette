using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using HarmonyLib;
using UnityEngine;
using ShotgunRoulette.Network;

namespace ShotgunRoulette.Patches
{
    [HarmonyPatch(typeof(GameNetworkManager))]
    internal class GameNetworkManagerPatch
    {

    }
}
