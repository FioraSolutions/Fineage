using System;

using UnityEngine;

public partial class EventBus : MonoBehaviour
{
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
