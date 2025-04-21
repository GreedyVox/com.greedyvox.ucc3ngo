using GreedyVox.NetCode.Game;
using GreedyVox.NetCode.Utilities;
using Unity.Netcode;
using UnityEngine;

namespace GreedyVox.NetCode.Tests
{
    public class NetworkPoolObjectTest : NetCodeObjectPool
    {
        public override void SetupSpawnManager(GameObject go, bool pool = true)
        {
            if (ComponentUtility.HasComponent<NetworkObject>(go))
                InjectSpawnManager(new NetworkSpawnTest(go), go);
        }
    }
}