using UnityEngine;

namespace GreedyVox.NetCode
{
    public abstract class AbstractSingletonBehaviour<T> : MonoBehaviour where T : AbstractSingletonBehaviour<T>, new()
    {
        #region Basic getters/setters
        // Whether or not this object should persist when loading new scenes.
        // This should be set in the child classes Init() method.
        [field: SerializeField] public bool Persist { get; protected set; } = false;
        #endregion
        protected static T _Instance;
        public static T Instance
        {
            get
            {
                // This would only EVER be null if some other MonoBehavior requests the instance in its' Awake method.
                if (_Instance == null)
                {
                    Debug.Log($"[UnitySingleton] Finding instance of '{typeof(T).ToString()}' object.");
                    _Instance = FindObjectOfType(typeof(T)) as T;
                    // This should only occur if 'T' hasn't been attached to any game objects in the scene.
                    if (!_Instance)
                        Debug.LogError($"[UnitySingleton] No instance of '{typeof(T).ToString()}' found!");
                }
                return _Instance;
            }
        }
        // Make sure no "ghost" objects are left behind when applicaton quits.
        protected virtual void OnApplicationQuit() => _Instance = null;
        // This will initialize our instance, if it hasn't already been prompted to do so by
        // another MonoBehavior's Awake() requesting it first.
        protected virtual void Awake()
        {
            Debug.Log("[UnitySingleton] Awake");
            if (_Instance == null)
            {
                Debug.Log("[UnitySingleton] Initializing Singleton in Awake");
                _Instance = (T)this;
                if (Persist)
                    DontDestroyOnLoad(gameObject);
            }
        }
    }
}