using UnityEngine;
using UnityEngine.SceneManagement;

namespace GreedyVox.NetCode.Tests
{
    public class SceneManagerTest : MonoBehaviour
    {
        [SerializeField] private string m_SceneName = "Game";
        [SerializeField] private bool m_LoadSceneOnStart = false;
        [SerializeField] private LoadSceneMode m_SceneMode = LoadSceneMode.Single;
        private void Start()
        {
            if (m_LoadSceneOnStart)
                SceneLoad();
        }
        public void SceneLoad() =>
        SceneManager.LoadSceneAsync(m_SceneName, m_SceneMode);
    }
}