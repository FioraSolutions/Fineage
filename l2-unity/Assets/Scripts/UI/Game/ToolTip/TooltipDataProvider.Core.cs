using UnityEngine.UIElements;
using UnityEngine;

public partial class TooltipDataProvider : AbstractDataProvider
{
    public TooltipDataProvider()
    {
        _creator = new CreatorSets();
    }

    public void AddDataSkill(VisualElement container, object item)
    {
        if (item.GetType() == typeof(SkillInstance))
        {
            SkillInstance skill = (SkillInstance)item;
            IDataTips text = ToolTipManager.GetInstance().GetProductText(skill);
            SetDataSkillInTemplate(container, skill, text , 0);
        }
    }

    public  void AddDataWeapon(VisualElement container, object item)
    {

        if (item != null)
        {
            if(item.GetType() == typeof(Product))
            {
                Product product = (Product)item;
                Weapongrp weapon = product.GetWeapon();
                if (weapon == null)
                    return;
                IDataTips text = ToolTipManager.GetInstance().GetProductText(product);
                int price = int.Parse(text.GetPrice());
                SetDataWeaponInTemplate(container, weapon, price, text);
            }else if (item.GetType() == typeof(ItemInstance))
            {
                ItemInstance item_inventory = (ItemInstance)item;
                Weapongrp weapon = item_inventory.GetWeapon();
                if (weapon == null)
                    return;
                IDataTips text = ToolTipManager.GetInstance().GetProductText(item_inventory);
                SetDataWeaponInTemplate(container, weapon, 0 , text);
            }
        }

    }

    public void AddDataAccessories(VisualElement container, object item)
    {
        if (item != null)
        {
            if (item.GetType() == typeof(Product))
            {
                Product product = (Product)item;
                Armorgrp weapon = product.GetArmor();
                if (weapon == null)
                    return;
                IDataTips text = ToolTipManager.GetInstance().GetProductText(product);
                SetDataAccessoriesInTemplate(container, weapon, text , product.Price , product.GetTypeAccessoriesName());
            }
            else if (item.GetType() == typeof(ItemInstance))
            {
                ItemInstance item_inventory = (ItemInstance)item;
                Armorgrp weapon = item_inventory.GetArmor();
                if (weapon == null)
                    return;
                IDataTips text = ToolTipManager.GetInstance().GetProductText(item_inventory);
                SetDataAccessoriesInTemplate(container, weapon, text , 0 , item_inventory.GetTypeAccessoriesName());
            }
        }
    }

    public void AddDataOther(VisualElement container, object item)
    {
        if (item != null)
        {
            if (item.GetType() == typeof(Product))
            {
                Product product = (Product)item;
                EtcItemgrp etcItem = product.GetEtcItem();
                if (etcItem == null)
                    return;
                IDataTips text = ToolTipManager.GetInstance().GetProductText(product);
                int price = int.Parse(text.GetPrice());
                SetOther(container, text, etcItem.Icon, price, product.GetTypeAccessoriesName(), etcItem.Weight.ToString());
            }
            else if (item.GetType() == typeof(ItemInstance))
            {
                ItemInstance item_inventory = (ItemInstance)item;
                EtcItemgrp etcItem = item_inventory.GetEtcItem();
                if (etcItem == null)
                    return;
                IDataTips text = ToolTipManager.GetInstance().GetProductText(item_inventory);
                SetOther(container, text, etcItem.Icon, 0, item_inventory.GetTypeAccessoriesName(), etcItem.Weight.ToString());
            }
        }
        
    }
}
