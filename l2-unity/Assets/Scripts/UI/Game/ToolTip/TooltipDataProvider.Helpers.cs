using UnityEngine.UIElements;
using UnityEngine;

public partial class TooltipDataProvider : AbstractDataProvider
{
    public void AddRequiredData(VisualElement container, object item)
    {
        ItemInstance item1 = (ItemInstance)item;
        EtcItemgrp etcItem = item1.GetEtcItem();
        IDataTips text = ToolTipManager.GetInstance().GetProductText(item1);
        int countInventory = InventoryPacketServices.Inventory.GetItemByItemId(item1.ItemId) != null ? InventoryPacketServices.Inventory.GetItemByItemId(item1.ItemId).Count : 0;
        SetRequired(container, text, etcItem.Icon, 0, item1.GetTypeAccessoriesName(), etcItem.Weight.ToString() , item1.GetRequiredCount() , countInventory);
    }

    public void AddDataArmor(VisualElement container, object item , VisualTreeAsset setsElements, VisualTreeAsset setsEffect)
    {
        if (item != null)
        {
            Abstractgrp data = null;
            int price = 0;
            string armorTypeName = "";

            SetDataInfo(item, ref price, ref armorTypeName, ref data);



            if (data != null)
            {
                IDataTips text = ToolTipManager.GetInstance().GetProductText(item);

                if (data.GetType() == typeof(Armorgrp))
                {
                    Armorgrp armor = (Armorgrp)data;

                    if(armor != null)
                    {
                        AddArmor(container, price, armorTypeName, text, armor , setsElements , setsEffect,  _creator);
                    }
                }
                else if (data.GetType() == typeof(Weapongrp))
                {

                    Weapongrp weapon = (Weapongrp)data;
                    if (weapon.WeaponType == WeaponType.shield)
                    {
                        armorTypeName = GetWeaponTypeName(item);
                        AddArmorShield(container, price, armorTypeName, text, weapon);
                    }

                }
            }
        }
    }

    public void AddDataIngredient(VisualElement container , ItemInstance item)
    {
        //set icon
        IDataTips text = ToolTipManager.GetInstance().GetProductText(item);

        if (item.GetArmor() != null)
        {
            Armorgrp armor = item.GetArmor();
            SetDataIngredient(container, text, armor.Icon , item.Count , item.GetName());
        }
        else if (item.GetWeapon() != null)
        {
            Weapongrp weapon = item.GetWeapon();
            SetDataIngredient(container, text, weapon.Icon, item.Count, item.GetName());
        }
        else if (item.GetEtcItem() != null)
        {
            EtcItemgrp etcItem = item.GetEtcItem();
            SetDataIngredient(container, text, etcItem.Icon, item.Count, item.GetName());
        }


    }

    private void  SetDataInfo(object item , ref int price , ref string armorTypeName , ref Abstractgrp data)
    {
        if (item.GetType() == typeof(Product))
        {
            Product product = (Product)item;
            price = product.Price;
            armorTypeName = product.GetTypeArmorName();
            data = product.GetOAbstractItem();
        }
        else if (item.GetType() == typeof(ItemInstance))
        {
            ItemInstance itemInstance = (ItemInstance)item;
            armorTypeName = itemInstance.GetTypeArmorName();
            data = itemInstance.GetOAbstractItem();
        }
    }

    private string GetWeaponTypeName(object item)
    {
        if (item.GetType() == typeof(Product))
        {
            Product product = (Product)item;
            return product.GetTypeWeaponName();
        }
        else if (item.GetType() == typeof(ItemInstance))
        {
            ItemInstance itemInstance = (ItemInstance)item;
            return itemInstance.GetTypeWeaponName();
        }

        return "";
    }
}
