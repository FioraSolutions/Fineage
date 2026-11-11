using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class InventoryGearTab : L2Tab
{
    private Dictionary<int, ItemSlot> _gearCellIds;
    private Dictionary<ItemSlot, GearItem> _gearAnchors;
    [SerializeField] private int _selectedSlot = -1;
    private EquipGear _equip;

}
