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
}
