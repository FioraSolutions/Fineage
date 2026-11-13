using UnityEngine;

public abstract partial class AbstractServerItem
{
    public bool IsArrow() => WeaponTypeParser.Parse(TablePacketServices.EtcItemGrps.GetEtcItem(itemId)?.EtcItemType) == WeaponType.arrow;

    public bool IsDual()
    {
        WeaponType t = TablePacketServices.WeaponGrps.GetWeapon(itemId)?.WeaponType ?? WeaponType.none;
        return t == WeaponType.dual || t == WeaponType.dualfist || t == WeaponType.fist;
    }

    public string GetDescription()
    {
        return "";
    }

    public Abstractgrp GetOAbstractItem()
    {
        var armor = GetArmor();
        var weapon = GetWeapon();
        if (armor != null) return armor;
        if (weapon != null) return weapon;

        return null;
    }

    public Texture2D GradeIcon()
    {
        Weapongrp weapon = GetWeapon();
        if (weapon != null)
            return GetGradeImage(weapon.Grade);
        Armorgrp armor = GetArmor();
        if (armor != null)
            return GetGradeImage(armor.Grade);
        EtcItemgrp etc = GetEtcItem();
        if (etc != null)
            return GetGradeImage(etc.Grade);
        return null;
    }

    protected Texture2D GetGradeImage(ItemGrade grade)
    {
        switch (grade)
        {
            case ItemGrade.none:
                return null;

            case ItemGrade.d:
                return IconPacketServices.Icons.GetInterfaceIcon("grade_d");

            case ItemGrade.c:
                return IconPacketServices.Icons.GetInterfaceIcon("grade_c");

            case ItemGrade.b:
                return IconPacketServices.Icons.GetInterfaceIcon("grade_b");

            case ItemGrade.a:
                return IconPacketServices.Icons.GetInterfaceIcon("grade_a");

            case ItemGrade.s:
                return IconPacketServices.Icons.GetInterfaceIcon("grade_s");

            default:
                return null;
        }
    }
}
