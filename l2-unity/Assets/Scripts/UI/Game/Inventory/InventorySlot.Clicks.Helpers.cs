
using UnityEngine;
using UnityEngine.UIElements;

public partial class InventorySlot
{
    protected override void HandleRightClick()
    {
        if (TryGiveToEnchant())
            return;
        UseItem();
    }

    protected override void HandleMiddleClick()
    {
        if (!_empty)
        {
            InventoryPacketServices.Inventory.DestroyItem(_objectId, 1);
        }
    }

    public virtual void UseItem()
    {
        if (_empty || _objectId <= 0)
            return;
        PlayerInventory inventory = InventoryPacketServices.Inventory;
        if (inventory == null)
        {
            Debug.LogError("[Inventory] use objectId " + _objectId + " has no inventory");
            return;
        }
        inventory.UseItem(_objectId);
    }
}
