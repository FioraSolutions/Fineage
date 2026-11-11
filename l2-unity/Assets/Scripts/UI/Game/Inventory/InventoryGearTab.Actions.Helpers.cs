using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class InventoryGearTab : L2Tab
{
    public void ModifiedReplace(ItemInstance item , int userId)
    {
        EquipItem(item);
        GamePacketServices.Bus?.Equipped(item, userId);
    }

    public override void SelectSlot(int slotPosition)
    {
        GearItem gearItem = ConvertIdToGearType(slotPosition);

        if (gearItem != null)
        {
            if (_selectedSlot != -1)
            {
                // The previously selected cell can be gone after an equip swap; never deref null there.
                GearItem gearLastSelect = ConvertIdToGearType(_selectedSlot);
                gearLastSelect?.UnSelectGear();
            }

            gearItem.SetSelectGear();
            _selectedSlot = slotPosition;
        }

    }

    public ItemSlot CellSlot(int position)
    {
        if (_gearCellIds != null && _gearCellIds.ContainsKey(position))
            return _gearCellIds[position];
        return ItemSlot.none;
    }

    public GearItem ConvertIdToGearType(int slotPosition)
    {
        if (_gearCellIds.ContainsKey(slotPosition))
        {
            ItemSlot slotSelect = _gearCellIds[slotPosition];
            GearItem gearitem =  _gearAnchors[slotSelect];
            return gearitem;
        }
        return null;
    }
}
