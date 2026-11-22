using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class InventoryGearTab : L2Tab
{
    public void BindEquip(List<ItemInstance> equipItems)
    {
        if (_gearAnchors == null)
        {
            Debug.LogError("[Inventory] paperdoll anchors are not built");
            return;
        }
        EquipEmptyAll();
        if (equipItems == null)
            return;
        for (int i = 0; i < equipItems.Count; i++)
        {
            ItemInstance item = equipItems[i];
            if (item != null)
                EquipItem(item);
        }
        if (_selectedSlot != -1)
            SelectSlot(_selectedSlot);
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

    public void ModifiedRemove(ItemSlot bodyPart, ItemInstance item)
    {
        PlayerInventory inventory = InventoryPacketServices.Inventory;
        if (inventory == null)
        {
            Debug.LogError("[Inventory] paperdoll remove has no inventory model");
            return;
        }
        BindEquip(inventory.GetPlayerEquipToList());
    }
}
