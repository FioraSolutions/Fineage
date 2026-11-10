

public partial class TradeWindow
{
    public void AddData(TradeStartDto data)
    {
        _playerId = data.PlayerId;
    }

    private void InitializeExchangeSlots(VisualElement content, List<ExchangeSlot> slots)
    {
        slots.Clear();
        if (content == null)
            return;

        content.Clear();
        for (int i = 0; i < 8; i++)
        {
            var slotElement = new VisualElement { name = "exchangeSlot" + (i + 1) };
            slotElement.style.width = 36;
            slotElement.style.height = 36;
            slotElement.style.marginRight = 4;
            content.Add(slotElement);
            slots.Add(new ExchangeSlot(i, slotElement, SlotType.Trade));
        }
    }

    public void ApplyOwnOfferItem(ItemInstance item)
    {
        if (item == null)
            return;
        ExchangeSlot empty = _exchangeSlots.Find(slot => slot.IsEmpty);
        if (empty == null)
            return;
        empty.AssignItem(item);
        if (!_exchangeItems.Exists(existing => existing.ObjectId == item.ObjectId))
            _exchangeItems.Add(item);
    }

    public void ApplyPartnerOfferItem(ItemInstance item)
    {
        if (item == null)
            return;
        ExchangeSlot empty = _partnerExchangeSlots.Find(slot => slot.IsEmpty);
        empty?.AssignItem(item);
    }

    public void SetOwnConfirmState(bool confirmed)
    {
        if (_okButton != null)
            _okButton.SetEnabled(!confirmed);
    }

    public void SetPartnerConfirmState(bool confirmed)
    {
        // Partner confirm is shown on their client; no separate control in this window.
    }

    public void ShowWindow(string playerName)
    {
        _playerName = playerName;

        base.ShowWindow();

        UpdatePlayerName();
        ClearAllExchangeSlots();
        if (_okButton != null)
            _okButton.SetEnabled(true);
    }

    private void UpdatePlayerName()
    {
        if (_playerNameLabel != null)
            _playerNameLabel.text = _playerName;
        Label otherLabel = (Label)GetElementById("OtherPlayerOfferLabel");
        if (otherLabel != null && !string.IsNullOrEmpty(_playerName))
            otherLabel.text = _playerName + "'s Offer";
    }

    private void ClearAllExchangeSlots()
    {
        ClearSlotList(_exchangeSlots);
        ClearSlotList(_partnerExchangeSlots);
        _exchangeItems.Clear();
    }

    static void ClearSlotList(List<ExchangeSlot> slots)
    {
        if (slots == null)
            return;
        foreach (var slot in slots)
            slot.AssignEmpty();
    }

    public void AddExchangeItem(ItemInstance item)
    {
        // Находим первый пустой слот
        var emptySlot = _exchangeSlots.Find(slot => slot.IsEmpty);
        if (emptySlot != null)
        {
            emptySlot.AssignItem(item);
            _exchangeItems.Add(item);

            // Отправляем пакет добавления предмета в обмен
            ZoneCommands.Send(new AddTradeItemCommand(0, item.ObjectId, item.Count));
        }
        else
        {
            // Debug.LogWarning("No empty slots available in exchange window");
        }
    }

    public void RemoveExchangeItem(int objectId)
    {
        var slot = _exchangeSlots.Find(s => !s.IsEmpty && s.ObjectId == objectId);
        if (slot != null)
        {
            slot.AssignEmpty();
            _exchangeItems.RemoveAll(item => item.ObjectId == objectId);
        }
    }

    public List<ItemInstance> GetExchangeItems()
    {
        return _exchangeItems;
    }

}
