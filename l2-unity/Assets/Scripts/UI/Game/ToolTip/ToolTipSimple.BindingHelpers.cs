using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;

public partial class ToolTipSimple
{
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
