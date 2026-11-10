using System;

public partial class MultiSellWindow
{
    private List<Ingredient> GetIngredient(List<MultiSellData> data , int itemId)
    {
        int index = 1;
        foreach (MultiSellData item in data)
        {
            List<ItemInstance> listItem = item.ProductList;
            bool result =  listItem.Any(x => x.ItemId == itemId);
            if (result)
            {
                _entryId = item.ZoneEntryId != 0 ? item.ZoneEntryId : index;
                return item.IngredientList;
            }
            index++;
        }

        return null;
    }

    public bool IfMore1Item(List<MultiSellData> data, int itemId)
    {
        List<MultiSellData>  list = data.Where(x => x.ProductList[0].ItemId == itemId).ToList();

        if(list.Count > 1)
        {
            return true;
        }
        return false;
    }

    private List<Ingredient> GetIngredientByIndex(List<MultiSellData> data, int index)
    {
        if(ArrayUtils.IsValidIndexList(data, index))
        {
            _entryId = data[index].ZoneEntryId != 0 ? data[index].ZoneEntryId : index;
            return data[index].IngredientList;
        }
        return null;
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

    private void OnClick(ClickEvent evt)
    {
        _userInput.value = "1";
    }

    private void OnSendServer(ClickEvent evt)
    {

        SystemMessagePacketServices.Window.OnButtonOk += OkExchange;
        SystemMessagePacketServices.Window.OnButtonClosed += OnCancel;
        SystemMessagePacketServices.Window.ShowWindowDialogYesOrNot("Are you sure you want to exchange?");
    }

    private void OkExchange()
    {
        string value = ToolTipsUtils.ConvertPriceToNormal(_userInput.value);
        ZoneCommands.Send(new MultiSellChooseCommand(_listId, _entryId, int.Parse(value)));
        CancelEvent();
    }

    public void OnCancel()
    {
        CancelEvent();
    }
    private void CancelEvent()
    {
        SystemMessagePacketServices.Window.OnButtonOk -= OkExchange;
        SystemMessagePacketServices.Window.OnButtonClosed -= OnCancel;
    }

    private void OnDestroy()
    {
        _instance = null;
    }


}
