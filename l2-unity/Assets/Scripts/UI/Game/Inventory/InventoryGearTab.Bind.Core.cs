using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class InventoryGearTab : L2Tab
{
    public InventoryGearTab()
    {
        _gearAnchors = new Dictionary<ItemSlot, GearItem> ();
        _gearCellIds = new Dictionary<int, ItemSlot>();
        _equip = new EquipGear();
    }

    public override void Initialize(VisualElement chatWindowEle, VisualElement tabContainer, VisualElement tabHeader , bool initEmpty, bool isSwitchTab)
    {
        base.Initialize(chatWindowEle, tabContainer, tabHeader , initEmpty, isSwitchTab);

        _selectedSlot = -1;

        _gearAnchors?.Clear();
        InitEmptyGear();
        _equip.Initializing(_gearAnchors);

    }

    private void InitEmptyGear()
    {
        // Interlude InventoryWnd paper doll only — no Agathion / Belt / Pendant.
        _gearAnchors = new Dictionary<ItemSlot, GearItem>();
        TryAddGearAnchor(ItemSlot.hat1, "Hat1");
        TryAddGearAnchor(ItemSlot.head, "Helmet");
        TryAddGearAnchor(ItemSlot.hat2, "Hat2");
        TryAddGearAnchor(ItemSlot.underwear, "Underwear");
        TryAddGearAnchor(ItemSlot.neck, "Neck");
        TryAddGearAnchor(ItemSlot.rhand, "Rhand");
        TryAddGearAnchor(ItemSlot.chest, "Torso");
        TryAddGearAnchor(ItemSlot.lhand, "Lhand");
        TryAddGearAnchor(ItemSlot.rear, "Rear");
        TryAddGearAnchor(ItemSlot.lear, "Lear");
        TryAddGearAnchor(ItemSlot.gloves, "Gloves");
        TryAddGearAnchor(ItemSlot.legs, "Legs");
        TryAddGearAnchor(ItemSlot.feet, "Boots");
        TryAddGearAnchor(ItemSlot.boots, "Boots");
        TryAddGearAnchor(ItemSlot.rfinger, "Rring");
        TryAddGearAnchor(ItemSlot.lfinger, "Lring");

        CreateGearSlot(_gearAnchors);
    }
}
