using System.Collections.Generic;
using UnityEngine;

public partial class EquipGear
{
    public void EquipItemEmpty(ItemSlot bodyPart , ItemInstance item)
    {
        switch (bodyPart)
        {
            case ItemSlot.lrhand:
                AddGearEmptySlotIfItemIdEquals(ItemSlot.lhand , item.ItemId);
                AddGearEmptySlotIfItemIdEquals(ItemSlot.rhand, item.ItemId);
                break;

            case ItemSlot.fullarmor:
                AddGearEmptySlot(ItemSlot.chest);
                AddGearEmptySlot(ItemSlot.legs);
                break;

            case ItemSlot.lfinger:
                GearItem gearSlotFinger = GetSlotByObjectId(item.ObjectId);
                if (gearSlotFinger != null)
                {
                    AddGearEmptySlot(gearSlotFinger.GetSlotType());
                }

                break;

            case ItemSlot.lear:

                // if (!GearSlotIsEmpty(bodyPart))
                // {
                //     AddGearEmptySlot(ItemSlot.lear);
                // }
                // else
                // {
                //    AddGearEmptySlot(ItemSlot.rear);
                // }
                GearItem gearSlotLear = GetSlotByObjectId(item.ObjectId);
                if (gearSlotLear != null)
                {
                    AddGearEmptySlot(gearSlotLear.GetSlotType());
                }
                break;

            default:
                ItemSlot slot = bodyPart;
                if (slot != ItemSlot.none)
                {
                    if (_gearAnchors.ContainsKey(slot))
                    {
                        AddGearEmptySlot(slot);
                    }
                    else
                    {
                        // Debug.LogError("GearSlots reInitialize not found assigned slots " + slot);
                    }
                }
                else
                {
                    // Debug.LogError("Can't equip item, assigned slot is " + slot);
                }
                break;
        }
    }

    public void AddGearEmptySlot(ItemSlot slotType)
    {
        if (_gearAnchors.ContainsKey(slotType))
        {
            _gearAnchors[slotType].AssignEmpty();
        }
    }

    public void AddGearEmptySlotIfItemIdEquals(ItemSlot slotType , int itemId)
    {
        if (_gearAnchors.ContainsKey(slotType))
        {
            GearItem gearItem = _gearAnchors[slotType];
            if(gearItem.GetItemId() == itemId)
            {
                _gearAnchors[slotType].AssignEmpty();
            }
        }
    }
}
