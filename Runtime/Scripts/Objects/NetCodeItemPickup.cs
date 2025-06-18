using System;
using GreedyVox.NetCode.Data;
using GreedyVox.NetCode.Interfaces;
using Opsive.Shared.Game;
using Opsive.Shared.Utility;
using Opsive.UltimateCharacterController.Inventory;
using Opsive.UltimateCharacterController.Objects;
using Opsive.UltimateCharacterController.Objects.CharacterAssist;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

/// <summary>
/// Initializes the item pickup over the network.
/// </summary>
namespace GreedyVox.NetCode.Objects
{
    public class NetCodeItemPickup : ItemPickup, IPayload
    {
        private TrajectoryObject m_TrajectoryObject;
        private PayloadItemPickup m_Data;
        /// <summary>
        /// Initialize the default values.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            m_TrajectoryObject = gameObject.GetCachedComponent<TrajectoryObject>();
        }
        /// <summary>
        /// Initialize the default data values.
        /// </summary>
        private void Start()
        {
            var net = m_TrajectoryObject?.Owner.GetCachedComponent<NetworkObject>();
            m_Data = new PayloadItemPickup()
            {
                OwnerID = net == null ? -1L : (long)net.OwnerClientId,
                ItemCount = m_ItemDefinitionAmounts.Length,
                ItemID = GetArrayDataIDs(m_ItemDefinitionAmounts),
                ItemAmounts = GetArrayDataAmounts(m_ItemDefinitionAmounts),
                Velocity = m_TrajectoryObject == null ? Vector3.zero : m_TrajectoryObject.Velocity,
                Torque = m_TrajectoryObject == null ? Vector3.zero : m_TrajectoryObject.Torque,
            };
        }
        /// <summary>
        /// Initializes the object. This will be called from an object creating the projectile (such as a weapon).
        /// </summary>
        /// <param name="id">The id used to differentiate this projectile from others.</param>
        /// <param name="owner">The object that instantiated the trajectory object.</param>
        public void Initialize(uint id, GameObject own) { }
        private int[] GetArrayDataAmounts(ItemIdentifierAmount[] items)
        {
            var dat = new int[items.Length];
            for (var i = 0; i < dat.Length; i++)
                dat[i] = items[i].Amount;
            return dat;
        }
        private uint[] GetArrayDataIDs(ItemIdentifierAmount[] items)
        {
            var dat = new uint[items.Length];
            for (var i = 0; i < dat.Length; i++)
                dat[i] = (items[i].ItemIdentifier as ItemType).ID;
            return dat;
        }
        /// <summary>
        /// Returns the maximus size for the fast buffer writer
        /// </summary>               
        public int MaxBufferSize()
        {
            return
            FastBufferWriter.GetWriteSize<int>() +
            FastBufferWriter.GetWriteSize(m_Data.OwnerID) +
            FastBufferWriter.GetWriteSize(m_Data.ItemCount) +
            FastBufferWriter.GetWriteSize(m_Data.Torque) +
            FastBufferWriter.GetWriteSize(m_Data.Velocity) +
            FastBufferWriter.GetWriteSize(m_Data.ItemID ?? Array.Empty<uint>()) +
            FastBufferWriter.GetWriteSize(m_Data.ItemAmounts ?? Array.Empty<int>());
        }
        /// <summary>
        /// The object has been spawned, write the payload data.
        /// </summary>
        public bool PayLoad(ref int idx, out FastBufferWriter writer)
        {
            try
            {
                using (writer = new FastBufferWriter(MaxBufferSize(), Allocator.Temp))
                {
                    writer.WriteValueSafe(idx);
                    writer.WriteValueSafe(m_Data);
                }
                return true;
            }
            catch (Exception e)
            {
                NetworkLog.LogErrorServer(e.Message);
                return false;
            }
        }
        /// <summary>
        /// The object has been spawned, read the payload data.
        /// </summary>
        public void PayLoad(in FastBufferReader reader, GameObject go = default)
        {
            reader.ReadValueSafe(out m_Data);
            // Return the old.
            for (int i = 0; i < m_ItemDefinitionAmounts.Length; i++)
                GenericObjectPool.Return(m_ItemDefinitionAmounts[i]);
            // Setup the item counts.
            var length = (m_Data.ItemCount - (m_TrajectoryObject != null ? 2 : 0)) / 2;
            if (m_ItemDefinitionAmounts.Length != length)
                m_ItemDefinitionAmounts = new ItemIdentifierAmount[length];
            for (int n = 0; n < length; n++)
                m_ItemDefinitionAmounts[n] = new ItemIdentifierAmount(ItemIdentifierTracker.GetItemIdentifier(
                    m_Data.ItemID[n]).GetItemDefinition(), m_Data.ItemAmounts[n]);
            Initialize(true);
            // Setup the trajectory object.
            if (m_TrajectoryObject != null)
            {
                var velocity = m_Data.Velocity;
                var torque = m_Data.Torque;
                GameObject originator = null;
                if (NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue((ulong)m_Data.OwnerID, out var obj))
                    originator = obj.gameObject;
                m_TrajectoryObject.Initialize(velocity, torque, originator);
            }
        }
    }
}