
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
        if (!_empty)
        {
            if (!InventoryPacketServices.Inventory.UseItem(_objectId))
            {
                AssignEmpty();
            }
            
        }

    }
}
