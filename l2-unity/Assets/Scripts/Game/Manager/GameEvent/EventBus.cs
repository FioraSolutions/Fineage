using System;

﻿using System;
using UnityEngine;

/// <summary>
/// Legacy gear/level-up events on a scene <see cref="MonoBehaviour"/>.
/// New domain events (spawn, death, movement) use <see cref="IGameEventBus"/> (#143).
/// </summary>
public class EventBus : MonoBehaviour
{
    private static EventBus _instance;
    [System.Obsolete("Use [Inject] or GamePacketServices.Bus instead")]
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static EventBus Instance { get { return _instance; } }

    public event Action<Entity, int> OnLevelUp;
    public event Action<ItemInstance , int> OnEquipped;
    public event Action<ItemInstance, int> OnWeaponChanged;
    public event Action<ItemInstance , int> OnUnEquipped;


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            GamePacketServices.Bind(bus: this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }


    public void Equipped(ItemInstance item , int objectId)
    {
        int n = OnEquipped != null ? OnEquipped.GetInvocationList().Length : 0;
        // GearFlowLog.Info("EventBus.Equipped listeners=" + n +
            // " objectId=" + objectId +
            // " itemId=" + (item != null ? item.ItemId : 0) +
            // " body=" + (item != null ? item.BodyPart.ToString() : "null") +
            // " sameAsInstance=" + (this == Instance));
        OnEquipped?.Invoke(item , objectId);
    }


    public void UnEquipped(ItemInstance item, int objectId)
    {
        int n = OnUnEquipped != null ? OnUnEquipped.GetInvocationList().Length : 0;
        // GearFlowLog.Info("EventBus.UnEquipped listeners=" + n +
            // " objectId=" + objectId +
            // " itemId=" + (item != null ? item.ItemId : 0));
       OnUnEquipped?.Invoke(item , objectId);
    }

    public void LevelUp(Entity entity , int objectID)
    {
        if(entity != null)
        {
            OnLevelUp?.Invoke(entity, objectID);
            // Debug.Log("EventBus>event LevelUp " + entity.name);
        }
    }
}
