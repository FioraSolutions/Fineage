using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class InventoryWindow
{
    private void SwitchItemList(InventoryTab switchTo)
    {
        // Interlude: Items (main / non-quest bag) + Quest.
        if (switchTo.MainTab || switchTo.TabName.Equals("Items") || switchTo.TabName.Equals("All"))
        {
            List<ItemInstance> items_collect = InventoryPacketServices.Inventory.GetPlayerInvetoryToList();
            switchTo.SetItemList(FilterOutQuestItems(items_collect));
            return;
        }

        List<ItemInstance> filterList = InventoryPacketServices.Inventory.FilterInventory(switchTo.GetFilterCategories);
        if (filterList != null && filterList.Count > 0)
            switchTo.SetItemList(filterList);
        else
            switchTo.AddAllEmptyInventory(0);
    }

    static List<ItemInstance> FilterOutQuestItems(List<ItemInstance> items)
    {
        if (items == null)
            return new List<ItemInstance>();
        var result = new List<ItemInstance>(items.Count);
        for (int i = 0; i < items.Count; i++)
        {
            ItemInstance item = items[i];
            if (item != null && item.Category != ItemCategory.Quest)
                result.Add(item);
        }
        return result;
    }

    public int GetEmptySlot()
    {
        return _tabs[0].GetEmptySlot();
    }
}
