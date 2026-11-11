using System.Collections.Generic;
using UnityEngine;

public partial class EquipGear
{
    public void FullArmor(ItemInstance item)
    {
        //Texture2D textures = IconPacketServices.Icons.GetOtherIcon(item.ItemId, 2);
        AddGearSlotAssignByIndexIcon(ItemSlot.chest, item , 0);
        AddGearSlotAssignByIndexIconAlpha(ItemSlot.legs, item , 2);
    }

    public void LFinger(ItemInstance item)
    {
        if (!GearSlotIsEmpty(ItemSlot.lfinger) && !GearSlotIsEmpty(ItemSlot.rfinger))
        {
            int itemId = GetItemIdBySlotType(ItemSlot.lfinger);

            if (item.ItemId != itemId)
            {
                AddIfNotExists(item, ItemSlot.lfinger);
            }
            else
            {
                AddIfNotExists(item, ItemSlot.rfinger);
            }
        }
        else
        {
            //checks that this is not a duplicate item.objectId since UpdateItems is often called when the part is already equipped
            if (!ContainsItemInTypes(item, new[] { ItemSlot.lfinger, ItemSlot.rfinger }))
            {
                if (GearSlotIsEmpty(item.BodyPart))
                {
                    AddIfNotExists(item, ItemSlot.lfinger);
                }
                else
                {

                    AddIfNotExists(item, ItemSlot.rfinger);
                }
            }
        }
    }

    public void DefaultAssign(ItemInstance item)
    {
        //Debug.Log("Item id >>>>> " + item.ItemId);
        //Debug.Log("Slot ID  >>>>> " + item.Slot);
        //Debug.Log("BodyPart ID  >>>> " + item.BodyPart);
        ItemSlot slot = item.BodyPart;
        if (slot != ItemSlot.none)
        {
            if (_gearAnchors.ContainsKey(slot))
            {
                AddGearSlotAssign(slot, item);
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
    }

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
