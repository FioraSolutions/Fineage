using System;

public sealed partial class ItemDropGrpCatalog
{
    public bool IsStickWeapon(int itemId)
    {
        ItemTable items = _items != null ? _items : TablePacketServices.Items;
        if (items == null)
            return false;

        Weapon weapon = items.GetWeapon(itemId);
        if (weapon == null || weapon.Weapongrp == null)
            return false;

        switch (weapon.Weapongrp.WeaponType)
        {
            case WeaponType.sword:
            case WeaponType.bigword:
            case WeaponType.dagger:
            case WeaponType.blunt:
            case WeaponType.bigblunt:
            case WeaponType.dual:
            case WeaponType.pole:
            case WeaponType.staff:
                return true;
            default:
                return false;
        }
    }

    public bool IsWeapon(int itemId)
    {
        ItemTable items = _items != null ? _items : TablePacketServices.Items;
        return items != null && items.GetWeapon(itemId) != null;
    }

    public bool IsHerb(int itemId)
    {
        ItemName name = TablePacketServices.ItemNames.GetItemName(itemId);
        if (name != null && !string.IsNullOrEmpty(name.Name) &&
            name.Name.IndexOf("Herb", System.StringComparison.OrdinalIgnoreCase) >= 0)
            return true;

        Abstractgrp grp = ResolveGrp(itemId);
        string drop = grp != null ? grp.DropModel : null;
        if (string.IsNullOrEmpty(drop))
            return itemId >= 8600 && itemId <= 8614 ||
                   itemId >= 8154 && itemId <= 8157 ||
                   itemId == 8952 || itemId == 8953;

        string lower = drop.ToLowerInvariant();
        return lower.IndexOf("drop_meat", System.StringComparison.Ordinal) >= 0 ||
               lower.IndexOf("drop_magic_flower", System.StringComparison.Ordinal) >= 0 ||
               lower.IndexOf("herb", System.StringComparison.Ordinal) >= 0 ||
               lower.IndexOf("drop_attack_speed_up", System.StringComparison.Ordinal) >= 0 ||
               lower.IndexOf("drop_magic_speed_up", System.StringComparison.Ordinal) >= 0 ||
               lower.IndexOf("drop_critical_up", System.StringComparison.Ordinal) >= 0 ||
               lower.IndexOf("drop_move_up", System.StringComparison.Ordinal) >= 0 ||
               lower.IndexOf("drop_warrior_set", System.StringComparison.Ordinal) >= 0 ||
               lower.IndexOf("drop_priest_set", System.StringComparison.Ordinal) >= 0 ||
               lower.IndexOf("drop_recovery_set", System.StringComparison.Ordinal) >= 0;
    }

    public static bool IsCoinDropModel(string dropModel)
    {
        if (string.IsNullOrEmpty(dropModel))
            return false;
        return dropModel.IndexOf("coin_m00", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
               dropModel.IndexOf("coin00", System.StringComparison.OrdinalIgnoreCase) >= 0 ||
               dropModel.IndexOf("coin01", System.StringComparison.OrdinalIgnoreCase) >= 0;
    }
}
