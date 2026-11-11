using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;

public partial class ToolTipSimple
{
    private TemplateContainer GetProductContainer(Product product)
    {
        switch (product.GetTypeItem())
        {
            case EnumType2.TYPE2_WEAPON:
                return SwitchToWeapon();
            case EnumType2.TYPE2_ACCESSORY:
            case EnumType2.TYPE2_OTHER:
                return SwitchToAccessories();
            case EnumType2.TYPE2_SHIELD_ARMOR:
                return SwitchToArmor();
        }
        return null;
    }

    private TemplateContainer GetInventoryContainer(ItemInstance item)
    {
        switch (item.Category)
        {
            case ItemCategory.Weapon:
                return SwitchToWeapon();
            case ItemCategory.Jewel:
            case ItemCategory.Item:
                return SwitchToAccessories();
            case ItemCategory.RequiredItem:
                return SwitchToRequiredAccessories();
            case ItemCategory.ShieldArmor:
                return SwitchToArmor();
        }
        return null;
    }

    private TemplateContainer GetSkillContainer(SkillInstance item)
    {
        return SwitchToSkill();
    }

    public TemplateContainer GetContainer(ItemInstance item)
    {
        switch (item.Category)
        {
            case ItemCategory.Weapon:
                return SwitchToWeapon();
            case ItemCategory.Jewel:
            case ItemCategory.Item:
                return SwitchToAccessories();
            case ItemCategory.ShieldArmor:
                return SwitchToArmor();
        }
        return null;
    }
}
