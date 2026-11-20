using System;
using System.Collections.Generic;
using Fineage.Net.Contract;
using UnityEngine;

public static partial class NetGearSession
{
    public static void NoteItemList(NetItemEntry[] rows)
    {
        _items = rows ?? System.Array.Empty<NetItemEntry>();
        _hasItems = true;
        _itemsApplied = false;
        _updates.Clear();
        FlushPending();
    }

    public static void NoteInventoryUpdate(NetInventoryChange[] rows)
    {
        if (rows == null || rows.Length == 0)
            return;
        _updates.Add(rows);
        FlushPending();
    }

    public static void FlushPending()
    {
        bool hasWork = (_hasItems && !_itemsApplied) || _updates.Count > 0;
        if (!hasWork)
            return;

        if (!CanApplyInventory())
            return;

        try
        {
            if (_hasItems && !_itemsApplied)
            {
                ItemListDto list = ToItemListDto(_items);
                InventoryApplyService inventory = ApplyServices.Inventory;
                if (inventory != null) inventory.ApplyItemList(list);
                else ItemListApplyIncomingPacketServices.ApplyItemList(list);
                _itemsApplied = true;
                Debug.Log("[Net] ItemList rows=" + _items.Length);
            }

            if (!_itemsApplied)
                return;

            bool wornEnchant = false;
            if (_hasItems)
                wornEnchant |= ApplyWornEnchantFromItems(_items);

            for (int i = 0; i < _updates.Count; i++)
            {
                InventoryUpdateDto update = ToInventoryUpdateDto(_updates[i]);
                InventoryApplyService inventory = ApplyServices.Inventory;
                if (inventory != null) inventory.ApplyInventoryUpdate(update);
                else InventoryUpdateApplyIncomingPacketServices.ApplyInventoryUpdate(update);
                wornEnchant |= ApplyWornEnchantFromChanges(_updates[i]);
            }

            _updates.Clear();
            if (wornEnchant)
                RefreshLocalEnchantGlows();
        }
        catch (System.Exception ex)
        {
            if (!_warned)
            {
                Debug.LogWarning("[Net] inventory apply deferred: " + ex.Message);
                _warned = true;
            }
        }
    }

    static bool ApplyWornEnchantFromItems(NetItemEntry[] rows)
    {
        if (rows == null || rows.Length == 0)
            return false;
        bool touched = false;
        for (int i = 0; i < rows.Length; i++)
        {
            NetItemEntry row = rows[i];
            if (row.EquippedSlot < 0)
                continue;
            touched |= WriteWornEnchant(row.EquippedSlot, row.EnchantLevel);
        }
        return touched;
    }

    static bool ApplyWornEnchantFromChanges(NetInventoryChange[] rows)
    {
        if (rows == null || rows.Length == 0)
            return false;
        bool touched = false;
        for (int i = 0; i < rows.Length; i++)
        {
            NetInventoryChange row = rows[i];
            if (row.EquippedSlot < 0)
                continue;
            touched |= WriteWornEnchant(row.EquippedSlot, row.EnchantLevel);
        }
        return touched;
    }

    static bool WriteWornEnchant(int slot, int level)
    {
        PlayerAppearance appearance = LocalAppearance();
        if (appearance == null)
            return false;
        if (appearance.SlotEnchant == null || appearance.SlotEnchant.Length < NetPaperdoll.Count)
            appearance.SlotEnchant = new int[NetPaperdoll.Count];
        if (slot < 0 || slot >= appearance.SlotEnchant.Length)
            return false;
        int capped = EnchantGlow.Cap(level);
        appearance.SlotEnchant[slot] = capped;
        if (slot == NetPaperdoll.RightHand)
            appearance.WeaponEnchant = capped;
        return true;
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
