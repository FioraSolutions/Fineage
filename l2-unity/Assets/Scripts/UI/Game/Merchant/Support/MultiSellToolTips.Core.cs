using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class MultiSellToolTips
{
    public MultiSellToolTips(TooltipDataProvider dataProvider)
    {
        _dataProvider = dataProvider;
    }

    public void SetTemplate(VisualTreeAsset windowTemplateWeapon , VisualTreeAsset windowTemplateAcccesories, VisualTreeAsset windowTemplateArmor , VisualTreeAsset itemTemplateIngredient)
    {
        _windowTemplateWeapon = windowTemplateWeapon;
        _windowTemplateAcccesories = windowTemplateAcccesories;
        _windowTemplateArmor = windowTemplateArmor;
        _itemTemplateIngredient = itemTemplateIngredient;
    }

    public void UseItem(ItemInstance item, VisualElement template)
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
            case ItemCategory.ShieldArmor:
                _dataProvider.AddDataArmor(template, item, null, null);
                break;
        }
    }

    private void UseIngredient(VisualElement template, ItemInstance item)
    {
        if (item != null)
        {
            _dataProvider.AddDataIngredient(template, item);
        }
    }

    private VisualElement SwitchToWeapon( VisualTreeAsset _windowTemplateWeapon)
    {
       
        VisualElement weapon = ToolTipsUtils.CloneOne(_windowTemplateWeapon);
        var boxBackground = weapon.Q<VisualElement>("Box");
        var topBackground = weapon.Q<VisualElement>("Top");
        var centerBackground = weapon.Q<VisualElement>("Center");
        var growIcon = weapon.Q<VisualElement>("GrowIcon");

        ClearBorder(boxBackground, topBackground, centerBackground, growIcon);
        List<VisualElement> listElement = GetReduceElement(weapon);
        ReduceInterval(listElement);
        return weapon;
    }

    private List<VisualElement> GetReduceElement(VisualElement weapon)
    {
        List<VisualElement> listElement = new List<VisualElement>(7);
        var phisAtkText = weapon.Q<VisualElement>("phisAtkText");
        listElement.Add(phisAtkText);
        var magAtkText = weapon.Q<VisualElement>("magAtkText");
        listElement.Add(magAtkText);
        var speedText = weapon.Q<VisualElement>("speedText");
        listElement.Add(speedText);
        var critText = weapon.Q<VisualElement>("critText");
        listElement.Add(critText);
        var soulText = weapon.Q<VisualElement>("soulText");
        listElement.Add(soulText);
        var spiritText = weapon.Q<VisualElement>("spiritText");
        listElement.Add(spiritText);
        var weightText = weapon.Q<VisualElement>("weightText");
        listElement.Add(weightText);
        return listElement;
    }

    private VisualElement SwitchToAccessories(VisualTreeAsset _windowTemplateAcccesories)
    {
        

        var accessories = ToolTipsUtils.CloneOne(_windowTemplateAcccesories);
        var boxBackground = accessories.Q<VisualElement>("Box");
        var topBackground = accessories.Q<VisualElement>("Top");
        var centerBackground = accessories.Q<VisualElement>("Center");
        var growIcon = accessories.Q<VisualElement>("GrowIcon");
        ClearBorder(boxBackground, topBackground, centerBackground, growIcon);

        return accessories;
    }
}
