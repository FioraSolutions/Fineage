using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;
using VContainer;

public class TradeWindow : L2PopupWindow
{

    [Inject] GameClient _gameClient;

    private GameClient Game => TradePacketServices.Game;
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
        TradePacketServices.Bind(game: _gameClient, trade: this);
    }

    public void AddData(TradeStartDto data)
    {
        _playerId = data.PlayerId;
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/TradeWindow");
        _inventorySlotTemplate = LoadAsset("Data/UI/_Elements/Template/Slot");
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
            Game.Send(new AddTradeItemCommand(0, item.ObjectId, item.Count));
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

    private void OnOkClicked(ClickEvent evt)
    {
        SendOkResponse();
    }

    private void OnCancelClicked(ClickEvent evt)
    {
        SendCancelResponse();
        HideWindow();
    }

    private void SendOkResponse()
    {
        // Отправляем пакет подтверждения обмена
        Game.Send(new TradeDoneCommand(1));

        OnExchangeResponse?.Invoke(true);
    }

    private void SendCancelResponse()
    {
        // Отправляем пакет отмены обмена
        Game.Send(new TradeDoneCommand(0));

        OnExchangeResponse?.Invoke(false);
    }

    public override void HideWindow()
    {
        base.HideWindow();

        // Очищаем слоты при закрытии
        ClearAllExchangeSlots();
    }
    // Класс для слотов обмена
    public class ExchangeSlot : L2DraggableSlot
    {
        private int _count;
        private long _remainingTime;
        private SlotClickSoundManipulator _slotClickSoundManipulator;
        private int _objectId;
        private ItemCategory _itemCategory;
        protected bool _empty = true;

        private ItemInstance _itemInstance;
        public ItemInstance ItemInstance { get { return _itemInstance; } }
        public int Count { get { return _count; } }
        public long RemainingTime { get { return _remainingTime; } }
        public ItemCategory ItemCategory { get { return _itemCategory; } }
        public int ObjectId { get { return _objectId; } }
        public int ItemId { get { return _id; } }
        public bool IsEmpty { get { return _empty; } }

        public ExchangeSlot(int position, VisualElement slotElement, SlotType slotType)
            : base(position, slotElement, slotType, true, false)
        {
            _empty = true;

            if (_slotClickSoundManipulator == null)
            {
                _slotClickSoundManipulator = new SlotClickSoundManipulator(_slotElement);
                _slotElement.AddManipulator(_slotClickSoundManipulator);
            }
        }

        public void AssignEmpty()
        {
            _empty = true;
            _id = 0;
            _name = "Unknown";
            _description = "Unknown item.";
            _itemInstance = null;
            _objectId = 0;
            _enchantLevel = 0;

            if (_slotElement != null && SlotBg != null)
            {
                StyleBackground background = new StyleBackground(IconManager.Instance.GetInvetoryDefaultBackground());
                SlotBg.style.backgroundImage = background;
                _slotDragManipulator.enabled = false;
            }
        }

        public void AssignItem(ItemInstance item)
        {
            _slotElement.RemoveFromClassList("empty");

            AddDataItem(item);
            _itemInstance = item;
            _count = item.Count;
            _objectId = item.ObjectId;
            _enchantLevel = item.EnchantLevel;
            _id = item.ItemId;
            _remainingTime = item.RemainingTime;
            _empty = false;

            if (_slotElement != null)
            {
                AddImage(this);
                AddTooltip(item);
                _slotDragManipulator.enabled = true;
            }
        }

        private void AddDataItem(ItemInstance item)
        {
            if (item.ItemData != null)
            {
                _id = item.ItemData.Id;
                _name = item.ItemData.ItemName.Name;
                _description = item.ItemData.ItemName.Description;
                _icon = item.ItemData.Icon;
                _objectId = item.ObjectId;
                _itemCategory = item.Category;
            }
            else
            {
                // Debug.LogWarning($"Item data is null for item {item.ItemId}.");
                _id = 0;
                _name = "Unknown";
                _description = "Unknown item.";
                _icon = "";
                _objectId = -1;
                _itemCategory = ItemCategory.Item;
            }
        }

        private void AddImage(ExchangeSlot slot)
        {
            if (SlotBg == null) return;

            StyleBackground background = new StyleBackground(IconManager.Instance.GetIcon(_id));
            SlotBg.style.backgroundImage = background;
        }

        private void AddTooltip(ItemInstance item)
        {
            string tooltipText = $"{_name} ({_count})";
            if (item.Category == ItemCategory.Weapon ||
                item.Category == ItemCategory.Jewel ||
                item.Category == ItemCategory.ShieldArmor)
            {
                tooltipText = _name;
            }

            if (_tooltipManipulator != null)
            {
                _tooltipManipulator.SetText(tooltipText);
            }
        }

        public override void ClearManipulators()
        {
            base.ClearManipulators();

            if (_slotClickSoundManipulator != null)
            {
                _slotElement.RemoveManipulator(_slotClickSoundManipulator);
                _slotClickSoundManipulator = null;
            }
        }

        protected override void HandleLeftClick()
        {
            // При левом клике - удаляем предмет из обмена
            if (!_empty)
            {
                TradeWindow.Instance.RemoveExchangeItem(_objectId);
            }
        }

        protected override void HandleRightClick()
        {
            // То же самое для правого клика
            if (!_empty)
            {
                TradeWindow.Instance.RemoveExchangeItem(_objectId);
            }
        }

        protected override void HandleMiddleClick()
        {
            // Не используется в обмене
        }
    }
}