using System;
using System.Collections.Generic;

public static partial class NetEnchant
{
    public static void Apply(Dictionary<int, NetItemEntry> items, NetEnchantResult result)
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
        NetItemEntry item;
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

    public static bool IsScrollType(string etcItemType)
    {
        if (string.IsNullOrEmpty(etcItemType))
            return false;
        return etcItemType.EndsWith("_enchant_wp", StringComparison.Ordinal)
            || etcItemType.EndsWith("_enchant_am", StringComparison.Ordinal)
            || etcItemType.EndsWith("_enchant_ag", StringComparison.Ordinal);
    }

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
                return NetEquipNotices.Line(code, message);
        }
    }
}
