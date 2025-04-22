using System.Collections;
using Opsive.Shared.Game;
using Opsive.UltimateCharacterController.Traits;
using Unity.Netcode;
using UnityEngine;

namespace GreedyVox.NetCode.Game
{
    public class NetCodeSpawnerManager : NetworkBehaviour
    {
        [SerializeField] private GameObject[] m_GameObjects;
        public override void OnNetworkSpawn()
        {
            base.OnNetworkSpawn();
            if (!IsServer || m_GameObjects == null) return;
            StartCoroutine(UpdateSpawner());
        }
        private IEnumerator UpdateSpawner()
        {
            while (isActiveAndEnabled)
            {
                yield return null;
                if (Input.GetMouseButtonUp(2))
                    SpawnObject(m_GameObjects[UnityEngine.Random.Range(0, m_GameObjects.Length)]);
            }
        }
        private void SpawnObject(GameObject obj)
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(
            new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0)), out var hit, 100.0f))
            {
                var go = ObjectPoolBase.Instantiate(obj);
                NetCodeObjectPool.NetworkSpawn(obj, go, true);
                if (go.TryGetComponent(out Respawner com))
                    com.Respawn(hit.point, Quaternion.identity, true);
            }
        }
    }
}