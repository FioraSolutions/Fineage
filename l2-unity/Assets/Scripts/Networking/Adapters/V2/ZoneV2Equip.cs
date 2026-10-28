using System;
using System.Collections.Generic;

/// <summary>
/// Pure equip/unequip helpers shared by the zone client and the wire tests.
/// Change codes match Fineage-Backend <c>InventoryChange</c> (0 add, 1 modify, 2 remove).
/// Client inventory flags match <c>PlayerInventory.InventoryChange</c> (added 1, modified 2, removed 3).
/// </summary>
public static class ZoneV2InventoryBook
{
    public const int Add = 0;
    public const int Modify = 1;
    public const int Remove = 2;

    public const int ClientAdded = 1;
    public const int ClientModified = 2;
    public const int ClientRemoved = 3;

    public static int ToClientChange(int zoneChange)
    {
        switch (zoneChange)
        {
            case Add:
                return ClientAdded;
            case Modify:
                return ClientModified;
            case Remove:
                return ClientRemoved;
            default:
                return ClientModified;
        }
    }

    public static void Apply(Dictionary<int, ZoneV2ItemEntry> items, ZoneV2InventoryChange change)
    {
        if (items == null)
            return;
        if (change.Change == Remove)
        {
            items.Remove(change.ObjectId);
            return;
        }

        items[change.ObjectId] = new ZoneV2ItemEntry
        {
            ObjectId = change.ObjectId,
            ItemId = change.ItemId,
            Count = change.Count,
            EquippedSlot = change.EquippedSlot,
            BodySlot = string.IsNullOrEmpty(change.BodySlot) ? "none" : change.BodySlot,
            Kind = string.IsNullOrEmpty(change.Kind) ? "etc" : change.Kind,
            Icon = change.Icon ?? "",
            WeaponType = change.WeaponType ?? "",
            ArmorType = change.ArmorType ?? ""
        };
    }
}

/// <summary>Short lines for zone <c>Error</c> 1099. A server message wins when the body included one.</summary>
public static class ZoneV2EquipNotices
{
    public static string Line(string code, string message)
    {
        if (!string.IsNullOrWhiteSpace(message))
            return message.Trim();

        switch (code)
        {
            case "not_owned":
                return "You do not have that item.";
            case "not_equippable":
                return "You cannot equip that.";
            case "dead":
                return "You cannot do that while dead.";
            case "blocked":
                return "That equipment slot is blocked.";
            case "bad_slot":
                return "That equipment slot is invalid.";
            case "empty":
                return "Nothing is equipped there.";
            case "forbidden":
                return "That command is not allowed.";
            case "world":
                return "You are not in the world yet.";
            case "bad_request":
                return "The server could not do that.";
            default:
                return "The server refused that.";
        }
    }
}

/// <summary>Chat <c>//give itemId [count]</c> becomes bypass <c>admin_give</c>. Admin-only on the zone.</summary>
public static class ZoneV2GiveCommand
{
    public static bool TryParse(string bypass, out int itemId, out int count)
    {
        itemId = 0;
        count = 1;
        if (string.IsNullOrWhiteSpace(bypass))
            return false;

        string text = bypass.Trim();
        if (text.StartsWith("admin_", StringComparison.Ordinal))
            text = text.Substring("admin_".Length);

        string[] parts = text.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2)
            return false;
        if (!string.Equals(parts[0], "give", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(parts[0], "giveitem", StringComparison.OrdinalIgnoreCase))
            return false;
        if (!int.TryParse(parts[1], out itemId) || itemId <= 0)
            return false;
        if (parts.Length >= 3 && (!int.TryParse(parts[2], out count) || count < 1))
            count = 1;
        return true;
    }
}
