using System;
using System.Collections.Generic;

/// <summary>
/// Zone enchant window state. Opcodes match Fineage-Backend <c>EnchantProtocol</c> (39, 40, 41, 1060).
/// The scroll is chosen with <c>EnchantRequest</c>. Confirm and cancel follow. A rejection is <c>Error</c> 1099
/// and does not consume the scroll.
/// </summary>
public static class ZoneV2Enchant
{
    public const int ResultCancel = 0;
    public const int ResultSuccess = 1;
    public const int ResultKeep = 2;
    public const int ResultReset = 3;
    public const int ResultCrystallize = 4;

    public struct Session
    {
        public bool Open;
        public int ScrollObjectId;
        public int TargetObjectId;
    }

    public struct View
    {
        public bool CloseWindow;
        public bool RemoveItem;
        public bool ShowEnchant;
        public int ObjectId;
        public int EnchantLevel;
        public int CrystalItemId;
        public int CrystalCount;
    }

    public static Session OpenFor(int scrollObjectId)
    {
        if (scrollObjectId <= 0)
            return default;
        return new Session
        {
            Open = true,
            ScrollObjectId = scrollObjectId,
            TargetObjectId = 0
        };
    }

    /// <summary>The scroll itself is not a target. A closed window ignores the click.</summary>
    public static Session Select(Session session, int targetObjectId)
    {
        if (!session.Open || targetObjectId <= 0 || targetObjectId == session.ScrollObjectId)
            return session;
        session.TargetObjectId = targetObjectId;
        return session;
    }

    public static bool CanConfirm(Session session)
    {
        return session.Open
            && session.ScrollObjectId > 0
            && session.TargetObjectId > 0
            && session.TargetObjectId != session.ScrollObjectId;
    }

    public static View ViewOf(ZoneV2EnchantResult result)
    {
        var view = new View
        {
            CloseWindow = true,
            ObjectId = result.ObjectId,
            EnchantLevel = result.EnchantLevel,
            CrystalItemId = result.CrystalItemId,
            CrystalCount = result.CrystalCount
        };
        switch (result.Result)
        {
            case ResultSuccess:
            case ResultKeep:
            case ResultReset:
                view.ShowEnchant = result.ObjectId > 0;
                break;
            case ResultCrystallize:
                view.RemoveItem = result.ObjectId > 0;
                break;
        }

        return view;
    }

    /// <summary>
    /// Success, keep, and reset write the level from the result. Crystallize removes the target.
    /// Crystals arrive on the following <c>InventoryUpdate</c>.
    /// </summary>
    public static void Apply(Dictionary<int, ZoneV2ItemEntry> items, ZoneV2EnchantResult result)
    {
        if (items == null)
            return;
        View view = ViewOf(result);
        if (view.RemoveItem)
        {
            items.Remove(view.ObjectId);
            return;
        }

        if (!view.ShowEnchant)
            return;
        ZoneV2ItemEntry item;
        if (!items.TryGetValue(view.ObjectId, out item))
            return;
        item.EnchantLevel = view.EnchantLevel;
        items[view.ObjectId] = item;
    }

    public static string WithLevel(string name, int enchantLevel)
    {
        if (enchantLevel <= 0)
            return name ?? "";
        if (string.IsNullOrEmpty(name))
            return "+" + enchantLevel;
        return "+" + enchantLevel + " " + name;
    }

    /// <summary>
    /// EtcItemgrp types that open the enchant window. Property-increase scrolls
    /// (<c>scrl_inc_enchant_prop_*</c>) are not included.
    /// </summary>
    public static bool IsScrollType(string etcItemType)
    {
        if (string.IsNullOrEmpty(etcItemType))
            return false;
        return etcItemType.EndsWith("_enchant_wp", StringComparison.Ordinal)
            || etcItemType.EndsWith("_enchant_am", StringComparison.Ordinal)
            || etcItemType.EndsWith("_enchant_ag", StringComparison.Ordinal);
    }

    /// <summary>These <c>Error</c> 1099 codes clear the selected scroll. The window should close.</summary>
    public static bool ClearsWindow(string code)
    {
        switch (code)
        {
            case "dead":
            case "not_owned":
            case "not_scroll":
            case "no_request":
            case "not_enchantable":
            case "slot_mismatch":
            case "grade_mismatch":
            case "max_enchant":
                return true;
            default:
                return false;
        }
    }

    /// <summary>Readable line for an enchant rejection. A server sentence wins when the body included one.</summary>
    public static string Notice(string code, string message)
    {
        if (!string.IsNullOrWhiteSpace(message))
            return message.Trim();

        switch (code)
        {
            case "not_scroll":
                return "That item is not an enchant scroll.";
            case "no_request":
                return "No enchant scroll is selected.";
            case "not_enchantable":
                return "That item cannot be enchanted.";
            case "grade_mismatch":
                return "The scroll grade does not match the item.";
            case "slot_mismatch":
                return "That scroll cannot enchant this item.";
            case "max_enchant":
                return "That item is already at the maximum enchant.";
            case "dead":
                return "You cannot enchant while dead.";
            case "not_owned":
                return "That item is not in your inventory.";
            default:
                return ZoneV2EquipNotices.Line(code, message);
        }
    }
}
