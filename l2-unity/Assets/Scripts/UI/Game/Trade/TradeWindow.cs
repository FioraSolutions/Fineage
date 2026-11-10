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

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _exchangeSlots = new List<ExchangeSlot>();
            _partnerExchangeSlots = new List<ExchangeSlot>();
            _exchangeItems = new List<ItemInstance>();
            WireTradePacketServices();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireTradePacketServices();
    }

    private void WireTradePacketServices()
    {
        TradePacketServices.Bind(trade: this);
    }


    private void OnDestroy()
    {
        _instance = null;
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/TradeWindow");
        _inventorySlotTemplate = LoadAsset(L2InventoryUiAssets.Slot);
    }

    protected override void InitWindow(VisualElement root)
    {
        base.InitWindow(root);

        var dragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);

        RegisterCloseWindowEvent("btn-close-frame");
        RegisterClickWindowEvent(_windowEle, dragArea);

        _playerNameLabel = (Label)GetElementById("PlayerNameLabel");
        _okButton = (Button)GetElementById("OKButton");
        _cancelButton = (Button)GetElementById("CancelButton");

        // Инициализация слотов обмена
        InitializeExchangeSlots(GetElementById("PlayerOfferContent"), _exchangeSlots);
        InitializeExchangeSlots(GetElementById("OtherPlayerOfferContent"), _partnerExchangeSlots);

        // Подписываемся на события кнопок
        _okButton.AddManipulator(new ButtonClickSoundManipulator(_okButton));
        _cancelButton.AddManipulator(new ButtonClickSoundManipulator(_cancelButton));

        _okButton.RegisterCallback<ClickEvent>(OnOkClicked);
        _cancelButton.RegisterCallback<ClickEvent>(OnCancelClicked);
    }


    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        OnCenterScreen(root);
    }

















    // Класс для слотов обмена
}
