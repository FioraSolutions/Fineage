using System;
using System.Collections.Generic;
using Fineage.Net.Contract;
using UnityEngine;

public static partial class NetGearSession
{
    public static void NoteItemList(NetItemEntry[] rows, int version)
    {
        _items = rows ?? System.Array.Empty<NetItemEntry>();
        _itemVersion = version;
        _hasItems = true;
        _itemsApplied = false;
        _updates.Clear();
        _updateVersions.Clear();
        _resyncArmed = false;
        FlushPending();
    }

    public static void NoteInventoryUpdate(NetInventoryChange[] rows, int version)
    {
        if (rows == null)
        {
            Debug.LogError("[Inventory] InventoryUpdate rows are null");
            RequestResync("null InventoryUpdate");
            return;
        }
        _updates.Add(rows);
        _updateVersions.Add(version);
        FlushPending();
    }

    public static void RequestResync(string reason)
    {
        if (_resyncArmed)
            return;
        _resyncArmed = true;
        Debug.LogError("[Inventory] resync requested (" + reason + ")");
        if (!ZoneCommands.Send(new RequestInventoryResyncCommand()))
            Debug.LogError("[Inventory] resync request was not sent");
    }

    public static void FlushPending()
    {
        bool hasWork = (_hasItems && !_itemsApplied) || _updates.Count > 0;
        if (!hasWork)
            return;

        if (!CanApplyInventory())
            return;

        PlayerInventory inventory = InventoryPacketServices.Inventory;
        if (inventory == null)
        {
            Debug.LogError("[Inventory] PlayerInventory is missing; ItemList was not applied");
            return;
        }

        try
        {
            if (_hasItems && !_itemsApplied)
            {
                if (_itemVersion < 0)
                {
                    Debug.LogError("[Inventory] ItemList version could not be read; snapshot was not applied");
                    _itemsApplied = true;
                    _updates.Clear();
                    _updateVersions.Clear();
                    RequestResync("unreadable ItemList");
                    return;
                }

                inventory.ReplaceFromServer(ToRows(_items), _itemVersion);
                _itemsApplied = true;
                Debug.Log("[Net] ItemList rows=" + _items.Length + " version=" + _itemVersion);
            }

            if (!_itemsApplied)
                return;

            int count = _updates.Count < _updateVersions.Count ? _updates.Count : _updateVersions.Count;
            for (int i = 0; i < count; i++)
            {
                if (_updateVersions[i] < 0)
                {
                    Debug.LogError("[Inventory] InventoryUpdate version could not be read; change was not applied");
                    RequestResync("unreadable InventoryUpdate");
                    break;
                }
                inventory.ApplyFromServer(ToChanges(_updates[i]), _updateVersions[i]);
            }

            _updates.Clear();
            _updateVersions.Clear();
        }
        catch (Exception ex)
        {
            Debug.LogError("[Inventory] apply failed: " + ex);
            _itemsApplied = true;
            _updates.Clear();
            _updateVersions.Clear();
            RequestResync("apply failed");
        }
    }

    public static void SyncWornEnchant(CharacterInventory book)
    {
        PlayerAppearance appearance = LocalAppearance();
        bool worn = book != null && book.PaperdollRows().Length > 0;
        if (appearance == null)
        {
            if (worn)
                Debug.LogError("[Inventory] equipped items have no local appearance; enchant glow was not applied");
            return;
        }
        if (appearance.SlotEnchant == null || appearance.SlotEnchant.Length < NetPaperdoll.Count)
            appearance.SlotEnchant = new int[NetPaperdoll.Count];
        for (int i = 0; i < appearance.SlotEnchant.Length; i++)
            appearance.SlotEnchant[i] = 0;
        appearance.WeaponEnchant = 0;
        if (book == null)
            return;

        CharacterInventory.Row[] rows = book.PaperdollRows();
        for (int i = 0; i < rows.Length; i++)
        {
            int slot = rows[i].EquippedSlot;
            if (slot < 0 || slot >= appearance.SlotEnchant.Length)
            {
                Debug.LogError("[Inventory] objectId " + rows[i].ObjectId + " equipped slot " + slot + " is outside the paperdoll");
                continue;
            }
            int capped = EnchantGlow.Cap(rows[i].EnchantLevel);
            appearance.SlotEnchant[slot] = capped;
            if (slot == NetPaperdoll.RightHand)
                appearance.WeaponEnchant = capped;
        }
        RefreshLocalEnchantGlows();
    }

    static CharacterInventory.Row[] ToRows(NetItemEntry[] entries)
    {
        if (entries == null)
            return System.Array.Empty<CharacterInventory.Row>();
        var rows = new CharacterInventory.Row[entries.Length];
        for (int i = 0; i < entries.Length; i++)
            rows[i] = ToRow(entries[i]);
        return rows;
    }

    static CharacterInventory.Change[] ToChanges(NetInventoryChange[] entries)
    {
        if (entries == null)
            return System.Array.Empty<CharacterInventory.Change>();
        var changes = new CharacterInventory.Change[entries.Length];
        for (int i = 0; i < entries.Length; i++)
        {
            NetInventoryChange entry = entries[i];
            changes[i] = new CharacterInventory.Change
            {
                Code = entry.Change,
                Row = ToRow(new NetItemEntry
                {
                    ObjectId = entry.ObjectId,
                    ItemId = entry.ItemId,
                    Count = entry.Count,
                    EquippedSlot = entry.EquippedSlot,
                    BodySlot = entry.BodySlot,
                    Kind = entry.Kind,
                    Icon = entry.Icon,
                    WeaponType = entry.WeaponType,
                    ArmorType = entry.ArmorType,
                    EnchantLevel = entry.EnchantLevel
                })
            };
        }
        return changes;
    }

    static PlayerAppearance LocalAppearance()
    {
        PlayerEntity player = DomainPacketServices.LocalPlayer;
        PlayerAppearance live = player != null ? player.Appearance as PlayerAppearance : null;
        if (live != null)
            return live;
        PlayerInfoInterlude info = ZoneCommands.LocalPlayerInfo;
        return info.Appearance;
    }

    static void RefreshLocalEnchantGlows()
    {
        PlayerEntity player = DomainPacketServices.LocalPlayer;
        if (player != null)
            player.RefreshVisuals();
    }
}
