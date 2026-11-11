using System;
using System.Collections;
using System.Linq;
using UnityEngine.UIElements;
using static L2Slot;
using System.Collections.Generic;
using UnityEngine;

public partial class QuantityInput : L2PopupWindow
{
    protected override IEnumerator BuildWindow(VisualElement root)
    {


        InitWindow(root);

        _userInput = (TextField)GetElementById("UserInputField");
        VisualElement inputBg = (VisualElement)GetElementById("UserInputBg");
        Button close = (Button)GetElementById("CloseButton");
        Button success = (Button)GetElementById("SuccessButton");


        close.RegisterCallback<ClickEvent>(OnClickClose);
        success.RegisterCallback<ClickEvent>(OnClickSuccess);
        RegisterClickWindowEvent(_windowEle, null);


        RegisterAllNumbers();

        _userInput.RegisterValueChangedCallback(evt => OnValueChanged(evt));
        _userInput.RegisterCallback<ClickEvent>((evt) => OnClick(evt, inputBg));

        OnCenterScreen(root);
        yield return new WaitForEndOfFrame();

        
    }

    private void RegisterAllNumbers()
    {
        for(int i = 0; i < 10; i++)
        {
            var btn = (Button)GetElementById("Button"+i);
            btn.RegisterCallback<ClickEvent>(OnPlus);
        }


        Button buttonBs = (Button)GetElementById("ButtonBs");
        buttonBs.RegisterCallback<ClickEvent>(Back);
        Button buttonC = (Button)GetElementById("ButtonC");
        buttonC.RegisterCallback<ClickEvent>(OnClear);
        Button buttonALL = (Button)GetElementById("ButtonALL");
        buttonALL.RegisterCallback<ClickEvent>(OnAll);

    }

    private void OnPlus(ClickEvent evt)
    {
        Button clickedElement = evt.currentTarget as Button;
        string number = clickedElement.name.Replace("Button", "");
        if (_userInput.value.Length > 20) return;
        if (long.TryParse(number, out _))
        {
            string value1 = ToolTipsUtils.ConvertPriceToNormal(_userInput.value + number);
            long numberInt = long.Parse(value1);
            _userInput.value = ToolTipsUtils.ConvertToPrice(numberInt);
        }
    }

    private void OnClear(ClickEvent evt)
    {
        _userInput.value = 0.ToString();
    }
}
