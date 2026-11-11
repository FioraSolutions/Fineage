using System.Collections.Generic;
using UnityEngine;

public partial class EquipGear
{
    public void Initializing(Dictionary<ItemSlot, GearItem> gearAnchors)
    {
        _gearAnchors = gearAnchors;
        SetAnchor(gearAnchors);
    }

   public void Lear(ItemInstance item)
    {

        if (!GearSlotIsEmpty(ItemSlot.lear) && !GearSlotIsEmpty(ItemSlot.rear))
        {
            int itemId = GetItemIdBySlotType(ItemSlot.lear);

            if (item.ItemId != itemId)
            {
                AddIfNotExists(item, ItemSlot.lear);
            }
            else
            {
                AddIfNotExists(item, ItemSlot.rear);
            }
        }
        else
        {
            //checks that this is not a duplicate item.objectId since UpdateItems is often called when the part is already equipped
            if (!ContainsItemInTypes(item, new[] { ItemSlot.lear, ItemSlot.rear }))
             {
                if (GearSlotIsEmpty(item.BodyPart))
                {
                    AddIfNotExists(item, ItemSlot.lear);
                }
                else
                {

                    AddIfNotExists(item, ItemSlot.rear);
                }
             }
        }
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
