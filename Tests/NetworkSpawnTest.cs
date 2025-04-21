using Opsive.Shared.Game;
using Unity.Netcode;
using UnityEngine;

namespace GreedyVox.NetCode.Tests
{
    public class NetworkSpawnTest : INetworkPrefabInstanceHandler
    {
        private GameObject m_Prefab;
        public NetworkSpawnTest(GameObject fab) { m_Prefab = fab; }
        public NetworkObject Instantiate(ulong ID, Vector3 pos, Quaternion rot)
        {
            var go = ObjectPoolBase.Instantiate(m_Prefab, pos, rot);
            return go?.GetComponent<NetworkObject>();
        }
        public void Destroy(NetworkObject net) =>
        ObjectPool.Destroy(net?.gameObject);
    }
}