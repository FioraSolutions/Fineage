using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;

public partial class ToolTipSimple
{
    public void EventLeftClickSlot(VisualElement ve)
    {
        if (ve == null || _windowEle == null)
            return;

        string[] ids = GetUniquePosition(ve);
        if (ids == null || ids.Length < 2)
            return;

        var template = SwitchToolTip(ids, true);

        if(template != null)
        {
            int position = Int32.Parse(ids[0]);
            int type = Int32.Parse(ids[1]);

            object data = GetDataClickLeft(type, position);

            if(data != null)
            {
                AddData(data, template);
                _selectShow = ve;
                _windowEle.style.display = DisplayStyle.Flex;
                _showToolTip.Show(ve, GetForceBelow(ids));
                return;
            }
        }

        // Nothing to describe (empty bag / gear slot): the click must not leave a stale tooltip.
        // Only the panel is hidden here — Hide() also drops the buff countdown subscriptions of
        // whatever tooltip the pointer was on, and a click on an empty cell must not do that.
        _windowEle.style.display = DisplayStyle.None;
    }


    private void AddData(object data , TemplateContainer template)
    {
        if(data.GetType() == typeof(Product))
        {
            Product product = (Product)data;
            UseProduct(product, template);
        }else if (data.GetType() == typeof(ItemInstance))
        {
            ItemInstance item = (ItemInstance)data;
            UseItem(item, template);
        }else if (data.GetType() == typeof(SkillInstance))
        {
            SkillInstance item = (SkillInstance)data;
            UseSkill( item, template);
        }
  
    }

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
