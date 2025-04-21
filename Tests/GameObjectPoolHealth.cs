using Opsive.Shared.Events;
using Opsive.UltimateCharacterController.Traits;
using UnityEngine;

namespace GreedyVox.NetCode.Tests
{
    [RequireComponent(typeof(AttributeManager))]
    public class GameObjectPoolHealth : Health
    {
        protected virtual void OnEnable()
        {
            EventHandler.ExecuteEvent(m_GameObject, "OnRespawn");
            base.Start();
        }
    }
}