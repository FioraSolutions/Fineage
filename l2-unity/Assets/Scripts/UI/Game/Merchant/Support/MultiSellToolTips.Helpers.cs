using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class MultiSellToolTips
{
    private VisualElement SwitchToArmor( VisualTreeAsset _windowTemplateArmor)
    {
      

        var armor = ToolTipsUtils.CloneOne(_windowTemplateArmor);
        var boxBackground = armor.Q<VisualElement>("Box");
        var topBackground = armor.Q<VisualElement>("Top");
        var centerBackground = armor.Q<VisualElement>("Center");
        var growIcon = armor.Q<VisualElement>("GrowIcon");
        ClearBorder(boxBackground, topBackground, centerBackground, growIcon);

        return armor;

    }

    public void AddIngredient(List<Ingredient> listIngredient, VisualElement contentPanel2)
    {
        foreach (Ingredient ingredient in listIngredient)
        {
            VisualElement template = GetIngredientContainer(_itemTemplateIngredient);
            if (template != null)
            {
                UseIngredient(template, ingredient.GetItemInstance());
                contentPanel2.Add(template);
            }

        }
    }

    private void ClearBorder(VisualElement boxBackground, VisualElement topBackground, VisualElement centerBackground, VisualElement growIcon)
    {
        StyleColor currentColor = boxBackground.style.backgroundColor;
        boxBackground.style.backgroundColor = new Color(currentColor.value.r, currentColor.value.g, currentColor.value.b, 0);

        ResetBorder(boxBackground);

        topBackground.style.borderBottomWidth = 0;
        centerBackground.style.borderBottomWidth = 0;

        if (growIcon != null)
        {
            ResetBorder(growIcon);
        }
    }

    private void ReduceInterval(List<VisualElement> list)
    {
        foreach (VisualElement item in list)
        {
            if (item != null)
            {
                if (item.name.Equals("phisAtkText"))
                {
                    item.style.marginTop = -2;
                    // Debug.Log("phisAtkText -2 enabled");
                }
                else
                {
                    item.style.marginTop = 2;
                }

            }

        }
    }

    private void ResetBorder(VisualElement element)
    {
        element.style.borderBottomWidth = 0;
        element.style.borderLeftWidth = 0;
        element.style.borderRightWidth = 0;
        element.style.borderTopWidth = 0;
    }

    public VisualElement GetContainer(ItemInstance item)
    {
        switch (item.Category)
        {
            case ItemCategory.Weapon:
                return SwitchToWeapon(_windowTemplateWeapon);
            case ItemCategory.Jewel:
            case ItemCategory.Item:
                return SwitchToAccessories(_windowTemplateAcccesories);
            case ItemCategory.ShieldArmor:
                return SwitchToArmor(_windowTemplateArmor);
        }
        return null;
    }

    public VisualElement GetIngredientContainer(VisualTreeAsset itemTemplateIngredient)
    {
        return ToolTipsUtils.CloneOne(itemTemplateIngredient);
    }
}
