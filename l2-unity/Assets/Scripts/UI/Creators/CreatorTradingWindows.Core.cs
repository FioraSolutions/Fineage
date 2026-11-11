using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;

public partial class CreatorTradingWindows : AbstractCreator , ICreatorTradeTab
{
    public void LoadAsset(Func<string, VisualTreeAsset> loaderFunc)
    {
        _templates = new VisualTreeAsset[loadTemplate.Length];

        for (int i = 0; i < loadTemplate.Length; i++)
        {
            _templates[i] = loaderFunc(loadTemplate[i]);
        }
    }

    public void CreateTradeTabs(VisualElement inventoryTabView , SlotType slotType = SlotType.Multisell , bool isDragged = false)
    {
        if(inventoryTabView == null)
        {
            // Debug.LogWarning("CreatorTradingWindows > CreateTradeTabs: Not Found inventoryTabView ");
            return;
        }

        var tabTemplate = _templates[0];
        var tabHeaderTemplate = _templates[1];
        base.CreateTradeTabs(inventoryTabView, tabTemplate, tabHeaderTemplate , slotType, isDragged);
    }

    public void ClearSlots(List<ItemInstance> oldListItems)
    {
        base.ClearSlotsActiveTab(oldListItems);
    }
}
