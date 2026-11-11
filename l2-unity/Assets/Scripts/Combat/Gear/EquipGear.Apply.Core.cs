using System.Collections.Generic;
using UnityEngine;

public partial class EquipGear
{
    public void Initializing(Dictionary<ItemSlot, GearItem> gearAnchors)
    {
        _gearAnchors = gearAnchors;
        SetAnchor(gearAnchors);
    }

    public void LHand(ItemInstance item)
    {

        //AddGearEmptySlot(ItemSlot.rhand);
        if(GearSlotGetItemSlot(ItemSlot.rhand) == ItemSlot.lrhand || GearSlotGetItemSlot(ItemSlot.lhand) == ItemSlot.lrhand)
        {

            AddGearEmptySlot(ItemSlot.lhand);
            if (!GearSlotByIsBow(ItemSlot.rhand)) AddGearEmptySlot(ItemSlot.rhand);
        }
        AddGearSlotAssign(ItemSlot.lhand, item);
    }

    public void RHand(ItemInstance item)
    {
        if (GearSlotGetItemSlot(ItemSlot.rhand) == ItemSlot.lrhand || GearSlotGetItemSlot(ItemSlot.lhand) == ItemSlot.lrhand)
        {
            AddGearEmptySlot(ItemSlot.lhand);
            AddGearEmptySlot(ItemSlot.rhand);
        }

        AddGearSlotAssign(ItemSlot.rhand, item);
    }
}
