using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

public partial class PlayerInventory : MonoBehaviour
{
    private void OnDestroy()
    {
        _instance = null;
    }

    public void SetInventory(Dictionary<int, ItemInstance> items, Dictionary<int, ItemInstance> equipItems , bool openInventory , int adenaCount , int usedSlot)
    {
        var rows = new List<CharacterInventory.Row>();
        AppendRows(items, rows);
        AppendRows(equipItems, rows);
        var errors = new List<string>();
        int generation = Book.Generation;
        Book.Replace(rows, 0, errors);
        for (int i = 0; i < errors.Count; i++)
            Debug.LogError("[Inventory] " + errors[i]);
        if (Book.ResyncRequired)
            NetGearSession.RequestResync("ItemList");
        if (adenaCount != Book.Adena)
            Debug.LogError("[Inventory] ItemList adena " + adenaCount + " does not match the rows (" + Book.Adena + ")");
        if (usedSlot != Book.Count)
            Debug.LogError("[Inventory] ItemList slot count " + usedSlot + " does not match the rows (" + Book.Count + ")");
        if (Book.Generation != generation || openInventory)
            Publish(openInventory);
    }

    public void RefreshWindow()
    {
        InventoryWindow window = InventoryPacketServices.Window;
        if (window == null || !window.IsItemSurfaceReady)
            return;
        window.RebindFromModel();
    }

    static void AppendRows(Dictionary<int, ItemInstance> source, List<CharacterInventory.Row> rows)
    {
        if (source == null)
            return;
        foreach (ItemInstance item in source.Values)
        {
            if (item == null)
            {
                Debug.LogError("[Inventory] ItemList row is null");
                continue;
            }
            rows.Add(RowFromItem(item));
        }
    }

    public void ChangeItemOrder(int fromSlot, int toSlot)
    {
        //ItemInstance fromItem = GetItemBySlot(fromSlot);
        //ItemInstance toItem = GetItemBySlot(toSlot);

        //if (fromItem == null)
        //{
         //   Debug.LogError($"Can't change item order, item not found at slot {fromSlot}.");
        //    return;
        //}

        //List<InventoryOrder> orders = new List<InventoryOrder>();
        //orders.Add(new InventoryOrder(fromItem.ObjectId, toSlot));

        //if (toItem != null)
        //{
        //    orders.Add(new InventoryOrder(toItem.ObjectId, fromSlot));
        //}

        //InventoryPacketServices.Window.SelectSlot(toSlot);
        // Debug.Log("пїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅ");
        //GameClient.Instance.ClientPacketHandler.UpdateInventoryOrder(orders);
    }
}
