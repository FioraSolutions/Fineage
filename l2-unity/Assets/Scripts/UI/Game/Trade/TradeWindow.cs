using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;
using VContainer;

public partial class TradeWindow : L2PopupWindow
{

    private static TradeWindow _instance;
    private VisualTreeAsset _inventorySlotTemplate;

    private Label _playerNameLabel;
    private Button _okButton;
    private Button _cancelButton;

    private List<ExchangeSlot> _exchangeSlots;
    private List<ExchangeSlot> _partnerExchangeSlots;
    private string _playerName;
    private List<ItemInstance> _exchangeItems;

    private long _playerId;

    [Obsolete("Use [Inject] / PacketServices instead")]
    public static TradeWindow Instance
    {
        get { return _instance; }
    }

    public event Action<bool> OnExchangeResponse; // true = OK, false = Cancel


























    // Класс для слотов обмена
}
