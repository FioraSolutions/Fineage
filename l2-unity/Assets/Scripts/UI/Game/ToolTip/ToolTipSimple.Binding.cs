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

    private void SetSimpleItemSingleToolTip(object item, TemplateContainer template)
    {
        if (item != null)
        {
            IDataTips data = ToolTipManager.GetInstance().GetProductText(item);


            Label nameText =(Label) template.Q<VisualElement>("name");
            VisualElement gradeImg = template.Q<VisualElement>("grade");
            Label encahntText = template.Q<Label>("enchant");

            Texture2D grade = data.GetGradeTexture();

            if (nameText != null)
            {
                nameText.text = data.GetName();
            }

            SetImageElement(gradeImg, grade);

            int ehchantLevel = 0;
            if (item.GetType() == typeof(ItemInstance))
            {
                ItemInstance itemInstance = (ItemInstance)item;
                ehchantLevel = itemInstance.EnchantLevel;
            }

            SetTextEnchantIfNot0(ehchantLevel, encahntText);

        }
        else
        {
            _windowEle.style.display = DisplayStyle.None;
            _showToolTip.Hide(null);
        }
    }

    public void SetSimpleRequireditemToolTip(ItemInstance item, TemplateContainer template)
    {;
        SetSimpleItemSingleToolTip(item, template);
        SetNameRequiredItem(item, template);

    }
    public void SetNameRequiredItem(ItemInstance item, TemplateContainer template)
    {
        if(item != null)
        {
            IDataTips data = ToolTipManager.GetInstance().GetProductText(item);
            ItemInstance inventoryItem = InventoryPacketServices.Inventory.GetItemByItemId(item.ItemId);
            Label nameText = (Label)template.Q<VisualElement>("name");
            if(nameText != null) nameText.text = data.GetName(true);
        }

    }

    private void SetImageElement(VisualElement element , Texture2D texture)
    {
        if(element != null)
        {
            if (texture == null)
            {
                element.style.display = DisplayStyle.None;
            }
            else
            {
                element.style.display = DisplayStyle.Flex;
                element.style.backgroundImage = texture;
            }
        }

    }


    private void SetIcon(VisualElement icon, VisualElement groubBoxIcon, string nameIcon)
    {
        if (L2SlotIconView.TrySetNamedIcon(icon, nameIcon))
            return;

        if (groubBoxIcon != null)
            groubBoxIcon.style.display = DisplayStyle.None;
        L2SlotIconView.Clear(icon);
    }

    public void SetTextEnchantIfNot0(int data , Label element)
    {
        if (data != 0 & data != -1)
        {
            element.style.display = DisplayStyle.Flex;
            element.text = "+"+ data;
        }
        else
        {
            element.style.display = DisplayStyle.None;
        }
    }
    private void SetDataToolTip(Label labelName , Label descriptedText,  string name, string descripted)
    {
        if (!string.IsNullOrEmpty(name))
        {
            labelName.style.display = DisplayStyle.Flex;
            labelName.text = name;
        }
        else
        {
            descriptedText.style.display = DisplayStyle.None;
        }

        if (!string.IsNullOrEmpty(descripted))
        {
            descriptedText.style.display = DisplayStyle.Flex;
            descriptedText.text = descripted;
        }
        else
        {
            descriptedText.style.display = DisplayStyle.None;

        }
    }

    public void ResetPosition(Vector2 vector2)
    {
        _content.transform.position = vector2;
        SendToBack();
    }
}
