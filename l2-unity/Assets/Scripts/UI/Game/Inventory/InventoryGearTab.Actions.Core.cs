using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class InventoryGearTab : L2Tab
{
    private void EquipList(List<ItemInstance> equipItems  , int userId)
    {
        for (int i = 0; i < equipItems.Count; i++)
        {
            ItemInstance itemInstance = equipItems[i];
            EquipItem(itemInstance);
            GamePacketServices.Bus?.Equipped(itemInstance, userId);
            // GearFlowLog.Info("InventoryGearTab EquipList userId=" + userId +
                // " itemId=" + itemInstance.ItemId +
                // " body=" + itemInstance.BodyPart);
        }
    }

    private void EquipEmptyAll()
    {
        foreach (KeyValuePair<ItemSlot, GearItem> entry in _gearAnchors)
        {
            GearItem gearItem = entry.Value;
            gearItem.AssignEmpty();
        }

    }

    private void EquipItem(ItemInstance item)
    {
        switch (item.BodyPart)
        {
            case ItemSlot.lrhand:
                _equip.LRHand(item);
                break;

            case ItemSlot.fullarmor:
                _equip.FullArmor(item);
                break;
            case ItemSlot.lfinger:
                _equip.LFinger(item);
                break;

            case ItemSlot.lear:
                _equip.Lear(item);
                break;
            case ItemSlot.rhand:
                _equip.RHand(item);
                break;
            case ItemSlot.lhand:
                _equip.LHand(item);
                break;
            case ItemSlot.chest:
                _equip.Chest(item);
                break;
            case ItemSlot.legs:
                _equip.Legs(item);
                break;
            default:
                _equip.DefaultAssign(item);
                break;
        }
    }

    public void ModifiedRemove(ItemSlot bodyPart , ItemInstance item)
    {
        _equip.EquipItemEmpty(bodyPart , item);
    }
}
