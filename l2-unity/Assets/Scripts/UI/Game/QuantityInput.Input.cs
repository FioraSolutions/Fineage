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

    private void OnClickClose(ClickEvent evt)
    {
        OnButtonClose?.Invoke();
        base.HideWindow();
    }

    private void OnValueChanged(ChangeEvent<string> evt)
    {

        _userInput.UnregisterValueChangedCallback(OnValueChanged);

        if (evt.newValue.Length == 0 & evt.previousValue.Length >= 1)
        {
            _userInput.SetValueWithoutNotify("0");
            _userInput.RegisterValueChangedCallback(OnValueChanged);
            return;
        }
        

        string digitsOnly = ToolTipsUtils.ConvertPriceToNormal(evt.newValue);
        if (digitsOnly.Length > 20) return;
        if (string.IsNullOrEmpty(digitsOnly) || !long.TryParse(digitsOnly, out long newValue))
        {
            _userInput.SetValueWithoutNotify(evt.previousValue);
            _userInput.RegisterValueChangedCallback(OnValueChanged);
            return;
        }

        digitsOnly = digitsOnly.Trim();

        string formattedValue = ToolTipsUtils.ConvertToPrice(long.Parse(digitsOnly));
        
        _userInput.SetValueWithoutNotify(formattedValue);

        if (formattedValue.Length > 1) SetCursorToEnd(formattedValue);
        _userInput.RegisterValueChangedCallback(OnValueChanged);
    }

    private void SetCursorToEnd(string formattedValue)
    {
        _userInput.cursorIndex = formattedValue.Length + 1;
    }

    public void ShowWindow(int  allCount)
    {
        _count = allCount;
        base.ShowWindow();
        OnCenterScreen(_root);
        // Prefill 1 (Interlude-like). "All" still fills max; OK with 0 used to send empty buys.
        _userInput.value = "1";
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }
}
