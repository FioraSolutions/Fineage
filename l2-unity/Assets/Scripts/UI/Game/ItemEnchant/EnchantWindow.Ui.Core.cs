using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;
using VContainer;

public partial class EnchantWindow : L2PopupWindow
{
    private void OnDestroy()
    {
        _instance = null;
    }

    private EnchantSlot ChangeSlotBoxType(VisualElement slotBox , EnchantSlot enchantSlot)
    {
        VisualElement slotElement = L2SlotGridLayout.InstantiateSlot(_inventorySlotTemplate, slotBox);
        enchantSlot = CreateEnchantSlot(1, slotElement, SlotType.Enchant);
        enchantSlot.AssignEmpty();
        return enchantSlot;
    }

    private EnchantSlot ChangeSlotBoxTypeChoice(VisualElement slotBox, EnchantSlot enchantSlot)
    {
        VisualElement slotElement = L2SlotGridLayout.InstantiateSlot(_inventorySlotChoiceTemplate, slotBox);
        enchantSlot = CreateEnchantSlot(1, slotElement, SlotType.Enchant);
        enchantSlot.AssignEmpty();
        return enchantSlot;
    }

    private void InitSlotElement1(VisualElement slotBox)
    {
        VisualElement slotElement = L2SlotGridLayout.InstantiateSlot(_inventorySlotTemplate, slotBox);
        _enchant1 = CreateEnchantSlot(0, slotElement, SlotType.Enchant);
        _enchant1.AssignEmpty();
    }
}
