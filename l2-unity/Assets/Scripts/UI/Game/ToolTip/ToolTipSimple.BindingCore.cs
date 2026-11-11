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

    private void HandleTimeLeftUpdated(int skillId, float newTimeLeft, VisualElement elementToUpdate)
    {
        if (elementToUpdate != null)
        {
            Label label = (Label)elementToUpdate;
            label.text = TimeUtils.FormatTime(newTimeLeft);
        }
    }

    private bool GetForceBelow(string[] ids)
    {
        int position = Int32.Parse(ids[0]);
        int type = Int32.Parse(ids[1]);

        switch (type)
        {
            case (int)SlotType.BuffPanel:
                return true;

            default: return false;
        }
    }

    private void SetSimpleSingleToolTip(Product product , TemplateContainer template)
    {
        if (product != null)
        {
            IDataTips text = ToolTipManager.GetInstance().GetProductText(product);
            var _descriptedText = (Label)template.Q<VisualElement>(null, "DescriptedLabel");
            var _nameText = (Label)template.Q<VisualElement>(null, "Heading");

            _descriptedText.style.fontSize = 12;
            _nameText.style.paddingLeft = 0;
            _descriptedText.style.color = ToolTipsUtils.GetColorPrice(text.GetPrice());

            var icon = template.Q<VisualElement>(null, "Icon");
            var groubBoxIcon = template.Q<VisualElement>(null, "Grow");

            SetIcon(icon , groubBoxIcon , null);
            string decription = text.GetPrice();

            if (!string.IsNullOrEmpty(decription) && decription.Equals("0") != true)
            {
                SetDataToolTip(_nameText, _descriptedText, text.GetName(), "Price: " + ToolTipsUtils.ConvertToPrice(Int32.Parse(decription)) + " Adena");
            }
            else
            {
                SetDataToolTip(_nameText, _descriptedText, text.GetName(), "");
            }
            
        }
        else
        {
            _windowEle.style.display = DisplayStyle.None;
            _showToolTip.Hide(null);
        }
    }

    private VisualElement SetSkillToolTip(SkillInstance skillInstance, TemplateContainer template , float leftTime = 0 )
    {
        IDataTips data = ToolTipManager.GetInstance().GetProductText(skillInstance);
       return  _dataProvider.SetDataSkillInTemplate(template, skillInstance, data , leftTime);
    }
}
