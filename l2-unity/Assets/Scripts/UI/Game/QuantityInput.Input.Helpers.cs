public partial class QuantityInput : L2PopupWindow
{
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
