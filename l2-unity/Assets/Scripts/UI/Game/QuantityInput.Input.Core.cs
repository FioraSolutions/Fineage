using System;
using System.Linq;
using UnityEngine.UIElements;
using static L2Slot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class QuantityInput : L2PopupWindow
{
    private void Back(ClickEvent evt)
    {
        string value1 = ToolTipsUtils.ConvertPriceToNormal(_userInput.value);
        if(value1.Length == 1) _userInput.value = "0";
        if (value1.Length > 0)
        {
            string modifiedString = value1.Substring(0, value1.Length - 1);
            _userInput.value = modifiedString;
        }

    }

    private void OnAll(ClickEvent evt)
    {
        if (_count > 0)
        {
            _userInput.value = _count.ToString();
        }
    }

    private void OnClick(ClickEvent evt ,  VisualElement inputBg)
    {
        if (!inputBg.ClassListContains("calculator_field_click_bg"))
        {
            inputBg.AddToClassList("calculator_field_click_bg");
        }
    }

    private void OnClickSuccess(ClickEvent evt)
    {
        string value = ToolTipsUtils.ConvertPriceToNormal(_userInput.value);
        OnButtonOk?.Invoke(value);
        base.HideWindow();
    }
}
