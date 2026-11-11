using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;

public partial class ToolTipSimple
{
    private TemplateContainer SwitchToolTip(string[] ids, bool isClickLeft)
    {
        int position = Int32.Parse(ids[0]);
        int type = Int32.Parse(ids[1]);

        if (isClickLeft)
        {
            switch (type)
            {
                case (int)SlotType.PriceSell:
                    Product productSell = ToolTipManager.GetInstance().FindProductInSellList(position);
                    if (productSell != null) return GetProductContainer(productSell);
                    break;
                case (int)SlotType.PriceBuy:
                    Product productBuy = ToolTipManager.GetInstance().FindProductInBuyList(position);
                    if (productBuy != null) return GetProductContainer(productBuy);
                    break;

                case (int)SlotType.Inventory:
                    ItemInstance item = InventoryPacketServices.Window.GetItemByPosition(position);
                    if (item != null) return GetInventoryContainer(item);
                    break;
                case (int)SlotType.Enchant:
                    ItemInstance enchantItem = ItemEnchantPacketServices.Enchant.GetItemByPosition(position);
                    if (enchantItem != null) return GetInventoryContainer(enchantItem);
                    break;
                case (int)SlotType.Multisell:
                    ItemInstance multiSelltItem = MultiSellPacketServices.MultiSell.GetItemByPosition(position);
                    if (multiSelltItem != null) return GetInventoryContainer(multiSelltItem);
                    break;
                case (int)SlotType.Recipe:
                    ItemInstance receiptInstance = CraftPacketServices.RecipeBook.GetRecipeInstanceByPosition(position);
                    if (receiptInstance != null) return GetInventoryContainer(receiptInstance);
                    break;
                case (int)SlotType.RecipeCraftItem:
                    ItemInstance requiredItem = CraftPacketServices.Crafting.GetRequiredInstanceByPosition(position);
                    if (requiredItem != null) return GetInventoryContainer(requiredItem);
                    break;
                case (int)SlotType.Gear:
                    GearItem gearItem = InventoryPacketServices.Window.GetGearPosition(position);
                    return GetGearContainer(gearItem);
                case (int)SlotType.SkillWindow:
                    //It is not the position that is used, but the skill ID. We have many panels on this form, so we pass its ID to tooltips, and there is no need for it anymore.
                    int skillId  = position;
                    SkillInstance skillItem = SkillLearnPacketServices.SkillList.GetSkillInstanceBySkillId(skillId);
                    return GetSkillContainer(skillItem);
            }
        }
        else
        {
            switch (type)
            {
                case (int)SlotType.PriceSell:
                case (int)SlotType.PriceBuy:
                    return SwitchToSimple();
                case (int)SlotType.Inventory:
                    return SwitchToString();
                case (int)SlotType.Enchant:
                    return SwitchToString();
                case (int)SlotType.Multisell:
                    return SwitchToString();
                case (int)SlotType.Recipe:
                    return SwitchToString();
                case (int)SlotType.RecipeCraftItem:
                    return SwitchToString();
                case (int)SlotType.SkillBar:
                    return SwitchToString();
                case (int)SlotType.Gear:
                    GearItem gearItem = InventoryPacketServices.Window.GetGearPosition(position);
                    return GetGearStringContainer(gearItem);
                case (int)SlotType.SkillWindow:
                                       //It is not the position that is used, but the skill ID. We have many panels on this form, so we pass its ID to tooltips, and there is no need for it anymore.
                    int skillId  = position;
                    SkillInstance skillItem = SkillLearnPacketServices.SkillList.GetSkillInstanceBySkillId(skillId);
                    return GetSkillContainer(skillItem);
                case (int)SlotType.BuffPanel:
                    SkillInstance instance = AuxiliaryGameUIPacketServices.Buffer.GetCellByPosition(position);
                    return GetSkillContainer(instance);
            }
        }

        return null;
    }

    private TemplateContainer SwitchToSimple()
    {
        _content.Clear();
        var template = _windowTemplateSimple.CloneTree();
        _content.Add(template);
        _contentInside = template.Q<VisualElement>(className: "content");
        _contentInside.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        return template;
    }

    private TemplateContainer SwitchToString()
    {
        _content.Clear();
        var template = _windowTemplateString.CloneTree();
        _content.Add(template);
        _contentInside = template.Q<VisualElement>(className: "content");
        _contentInside.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        return template;
    }

    private TemplateContainer SwitchToWeapon()
    {
        _content.Clear();
        var weapon = _windowTemplateWeapon.CloneTree();
        _content.Add(weapon);
        _contentInside = weapon.Q<VisualElement>(className: "content");
        _contentInside.RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
        return weapon;
    }
}
