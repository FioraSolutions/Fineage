using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;

public partial class ToolTipSimple
{
    private void UseProduct(Product product, TemplateContainer template)
    {
        switch (product.GetTypeItem())
        {
            case EnumType2.TYPE2_WEAPON:
                _dataProvider.AddDataWeapon(template, product);
                break;
            case EnumType2.TYPE2_ACCESSORY:
                _dataProvider.AddDataAccessories(template, product);
                break;
            case EnumType2.TYPE2_OTHER:
                _dataProvider.AddDataOther(template, product);
                break;
            case EnumType2.TYPE2_SHIELD_ARMOR:
                _dataProvider.AddDataArmor(template, product, _setsElements , _setsEffects);
                break;
        }
    }

    private void UseItem(ItemInstance item, TemplateContainer template)
    {
        switch (item.Category)
        {
            case ItemCategory.Weapon:
                _dataProvider.AddDataWeapon(template, item);
                break;
            case ItemCategory.Jewel:
                _dataProvider.AddDataAccessories(template, item);
                break;
            case ItemCategory.Item:
                _dataProvider.AddDataOther(template, item);
                break;
            case ItemCategory.RequiredItem:
                _dataProvider.AddRequiredData(template, item);
                break;
            case ItemCategory.ShieldArmor:
                _dataProvider.AddDataArmor(template, item, _setsElements, _setsEffects);
                break;
        }
    }

    private void UseSkill(SkillInstance item, TemplateContainer template)
    {
        _dataProvider.AddDataSkill(template, item);
    }
}
