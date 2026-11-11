using System.Collections.Generic;
using UnityEngine;

public partial class EquipGear
{
    public void LRHand(ItemInstance item)
    {
        if (item.IsBow())
        {
            if (!GearSlotByIsArrow(ItemSlot.lhand))
            {
                AddGearSlotAssignAlpha(ItemSlot.lhand, item, 0.8f);
            }
        }
        else
        {
            AddGearSlotAssignAlpha(ItemSlot.lhand, item, 0.8f);
        }

        AddGearSlotAssign(ItemSlot.rhand, item);
    }

    public void Legs(ItemInstance item)
    {
        if (GearSlotGetItemSlot(ItemSlot.legs) == ItemSlot.fullarmor)
        {
            AddGearEmptySlot(ItemSlot.chest);
            AddGearEmptySlot(ItemSlot.legs);
        }

        AddGearSlotAssign(ItemSlot.legs, item);
    }

    public void Chest(ItemInstance item)
    {
        if (GearSlotGetItemSlot(ItemSlot.chest) == ItemSlot.fullarmor)
        {
            AddGearEmptySlot(ItemSlot.chest);
            AddGearEmptySlot(ItemSlot.legs);
        }

        AddGearSlotAssign(ItemSlot.chest, item);
    }
}
