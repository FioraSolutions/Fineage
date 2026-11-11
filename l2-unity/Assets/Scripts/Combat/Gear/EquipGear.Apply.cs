using System.Collections.Generic;
using UnityEngine;

public partial class EquipGear
{

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






}
