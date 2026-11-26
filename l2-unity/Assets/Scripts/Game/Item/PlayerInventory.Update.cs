using System;
using System.Collections.Generic;
using UnityEngine;

public partial class PlayerInventory
{
    public CharacterInventory Book
    {
        get
        {
            if (_book == null)
                _book = new CharacterInventory();
            return _book;
        }
    }

    public int AdenaCount { get { return Book.Adena; } }

    public int UsedSlots { get { return Book.Count; } }

    public int CountOfItem(int itemId)
    {
        return Book.SumItem(itemId);
    }

    public void ReplaceFromServer(IReadOnlyList<CharacterInventory.Row> rows, int version)
    {
        var errors = new List<string>();
        int generation = Book.Generation;
        CharacterInventory.ApplyStatus status = Book.Replace(rows, version, errors);
        LogBook(errors);
        if (Book.ResyncRequired)
            NetGearSession.RequestResync("ItemList");
        if (status == CharacterInventory.ApplyStatus.Applied && Book.Generation != generation)
            Publish(false);
    }

    public void ApplyFromServer(IReadOnlyList<CharacterInventory.Change> changes, int version)
    {
        var errors = new List<string>();
        int generation = Book.Generation;
        CharacterInventory.ApplyStatus status = Book.Apply(changes, version, errors);
        LogBook(errors);
        if (status == CharacterInventory.ApplyStatus.Gap || status == CharacterInventory.ApplyStatus.Rejected)
            NetGearSession.RequestResync("InventoryUpdate");
        else if (Book.ResyncRequired)
            NetGearSession.RequestResync("InventoryUpdate");
        if (status == CharacterInventory.ApplyStatus.Applied && Book.Generation != generation)
            Publish(false);
    }

    public void UpdateInventory(Dictionary<int, ItemInstance> items, Dictionary<int, ItemInstance> equipItems)
    {
        var changes = new List<CharacterInventory.Change>();
        if (!TryAppendChanges(items, changes) || !TryAppendChanges(equipItems, changes))
        {
            NetGearSession.RequestResync("legacy InventoryUpdate");
            return;
        }
        ApplyFromServer(changes, 0);
    }

    static void LogBook(List<string> errors)
    {
        if (errors == null)
            return;
        for (int i = 0; i < errors.Count; i++)
            Debug.LogError("[Inventory] " + errors[i]);
    }

    static bool TryAppendChanges(Dictionary<int, ItemInstance> source, List<CharacterInventory.Change> changes)
    {
        if (source == null)
            return true;
        foreach (ItemInstance item in source.Values)
        {
            if (item == null)
            {
                Debug.LogError("[Inventory] legacy update row is null");
                return false;
            }
            int code;
            if (!TryWireChange(item.LastChange, out code))
            {
                Debug.LogError("[Inventory] legacy update objectId " + item.ObjectId + " has change " + item.LastChange);
                return false;
            }
            changes.Add(new CharacterInventory.Change { Code = code, Row = RowFromItem(item) });
        }
        return true;
    }

    static bool TryWireChange(int lastChange, out int code)
    {
        if (lastChange == (int)InventoryChange.ADDED)
        {
            code = CharacterInventory.Add;
            return true;
        }
        if (lastChange == (int)InventoryChange.MODIFIED)
        {
            code = CharacterInventory.Modify;
            return true;
        }
        if (lastChange == (int)InventoryChange.REMOVED)
        {
            code = CharacterInventory.Remove;
            return true;
        }
        code = -1;
        return false;
    }
}
