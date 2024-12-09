using Unity.Netcode;

namespace ShotgunRoulette.Network
{
    public class NetHandler : NetworkBehaviour
    {
        
        public override void OnNetworkSpawn()
        {
            LevelEvent = null;

            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                instance?.gameObject.GetComponent<NetworkObject>().Despawn();
            }
                
            instance = this;

            base.OnNetworkSpawn();
        }

        [ClientRpc]
        public void RotateGunClientRpc(ulong plrID, int itemPos, UnityEngine.Vector3 gunRotation)
        {
            // Gun is already rotated on the client, no need to do it again.
            if (GameNetworkManager.Instance.localPlayerController.playerClientId == plrID) return;
            // Rotate gun for everyone else (altho it's actually just being scaled)
            StartOfRound.Instance.allPlayerScripts[plrID].ItemSlots[itemPos].transform.localScale = gunRotation;
        }

        [ServerRpc(RequireOwnership = false)]
        public void RotateGunServerRpc(ulong plrID, int itemPos, UnityEngine.Vector3 gunRotation)
        {
            RotateGunClientRpc(plrID, itemPos, gunRotation);
            //LevelEvent?.Invoke(eventName);
        }

        public static event Action<String>? LevelEvent;
        public static NetHandler instance { get; private set; }


    }
}
