using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;

public partial class ToolTipSimple
{
    private void AddData(string[] ids, TemplateContainer template)
    {
        int position = Int32.Parse(ids[0]);
        int type = Int32.Parse(ids[1]);

        switch (type)
        {
            case (int)SlotType.PriceSell:
                Product productSell = ToolTipManager.GetInstance().FindProductInSellList(position);
                SetSimpleSingleToolTip(productSell, template);
                break;

            case (int)SlotType.PriceBuy:
                Product productBuy = ToolTipManager.GetInstance().FindProductInBuyList(position);
                SetSimpleSingleToolTip(productBuy, template);
                break;

            case (int)SlotType.Inventory:
                ItemInstance itemInstance = InventoryPacketServices.Window.GetItemByPosition(position);
                SetSimpleItemSingleToolTip(itemInstance, template);
                break;
            case (int)SlotType.Enchant:
                ItemInstance itemEnchant = ItemEnchantPacketServices.Enchant.GetItemByPosition(position);
                SetSimpleItemSingleToolTip(itemEnchant, template);
                break;
            case (int)SlotType.Multisell:
                ItemInstance itemMultisell = MultiSellPacketServices.MultiSell.GetItemByPosition(position);
                SetSimpleItemSingleToolTip(itemMultisell, template);
                break;
            case (int)SlotType.Recipe:
                ItemInstance recipeInstance = CraftPacketServices.RecipeBook.GetRecipeInstanceByPosition(position);
                SetSimpleItemSingleToolTip(recipeInstance, template);
                break;
            case (int)SlotType.RecipeCraftItem:
                ItemInstance requiredItem = CraftPacketServices.Crafting.GetRequiredInstanceByPosition(position);
                SetSimpleRequireditemToolTip(requiredItem, template);
                break;
            case (int)SlotType.SkillWindow:
                int skillId = position;
                SkillInstance skillInstance = SkillLearnPacketServices.SkillList.GetSkillInstanceBySkillId(skillId);
                SetSkillToolTip(skillInstance, template);
                break;
            case (int)SlotType.SkillBar:
                Shortcut skillbarInstance = SkillbarPacketServices.Skillbar.GetShortcutByPosition(position);
                SetSimpleItemSingleToolTip(skillbarInstance, template);
                break;
            case (int)SlotType.BuffPanel:
                SkillInstance instance = AuxiliaryGameUIPacketServices.Buffer.GetCellByPosition(position);
                SetBuffPanelToolTip(instance, template);
                break;
            case (int)SlotType.Gear:
                SetSimpleItemSingleToolTip(GetGearTooltipItem(
                    InventoryPacketServices.Window != null ? InventoryPacketServices.Window.GetGearPosition(position) : null), template);
                break;
        }
    }

    private void SetBuffPanelToolTip(SkillInstance instance , TemplateContainer template)
    {
        if (instance != null)
        {
            int skillId = instance.SkillID;
            float buffTimeLeft = AuxiliaryGameUIPacketServices.Buffer.GetLeftTimeByPosition(skillId);

            VisualElement updateElement = SetSkillToolTip(instance, template, buffTimeLeft);
            BindingLeftTime(updateElement, buffTimeLeft, skillId);
        }
    }

    private void BindingLeftTime(VisualElement updateElement , float buffTimeLeft , int skillId)
    {
        if (updateElement != null && buffTimeLeft > 0)
        {
            Action<int, float> timeUpdateHandler = (skillId, timeLeft) => HandleTimeLeftUpdated(skillId, timeLeft, updateElement);
            AuxiliaryGameUIPacketServices.Buffer.TrackSkillId(skillId);
            AuxiliaryGameUIPacketServices.Buffer.SetSubscrible(timeUpdateHandler);
        }
    }
}
