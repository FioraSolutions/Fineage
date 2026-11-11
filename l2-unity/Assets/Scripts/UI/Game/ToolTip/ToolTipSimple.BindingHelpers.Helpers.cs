using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;

public partial class ToolTipSimple
{
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
