using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using static L2Slot;
using VContainer;

public partial class EnchantWindow : L2PopupWindow
{

    private static EnchantWindow _instance;
    private VisualTreeAsset _inventorySlotTemplate;
    private VisualTreeAsset _inventorySlotChoiceTemplate;
    private VisualElement _slot1;
    private VisualElement _slot2;
    private VisualElement _progressBar;
    private Label _textLabel;
    private const string _defaultTextLabel = "Please place the item to be enchanted in the empty slot below.";
    private VisualElement _progressBarBg;
    private VisualElement _animElement;
    private List<Texture2D> _loadingAnimWaitPng;
    private List<Texture2D> _loadingAnimSuccessPng;
    private List<Texture2D> _loadingAnimFailPng;
    private bool _isStartAnim = false;
    private bool _isRun = false;
    private float _moveDuration = 1.5f;
    private float offsetLeft2 = 76;//default offset element2
    private float offsetLeft1 = 0;//default offset element2
   // private EtcItemgrp _useScroll;
    private EnchantSlot _enchant1;
    private EnchantSlot _enchant2;
    private ItemInstance _selectItem;
    private bool _zoneWire;
    private bool _closingFromServer;
    private bool _awaitingRoll;
    private int _scrollObjectId;
    private bool _frameCloseWired;

    public bool AcceptsTarget
    {
        get { return _zoneWire && !_isWindowHidden; }
    }


    [Obsolete("Use [Inject] / PacketServices instead")]
    public static EnchantWindow Instance
    {
        get { return _instance; }
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _loadingAnimWaitPng = new List<Texture2D>();
            _loadingAnimSuccessPng = new List<Texture2D>();
            _loadingAnimFailPng = new List<Texture2D>();
            CacheTexture();
            WireItemEnchantPacketServices();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireItemEnchantPacketServices();
    }

    private void WireItemEnchantPacketServices()
    {
        ItemEnchantPacketServices.Bind(enchant: this);
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/ItemEnchant/EnchantWindow");
        _inventorySlotTemplate = LoadAsset("Data/UI/_Elements/Template/SlotEnchant");
        _inventorySlotChoiceTemplate = LoadAsset("Data/UI/_Elements/Template/SlotEnchantChoice");
    }

    private void CacheTexture()
    {
        List<string> listWait = EnchantListTextures.GetNameTexturesLoading();
        List<string> listSuccess = EnchantListTextures.GetNameTexturesSuccess();
        List<string> listFail = EnchantListTextures.GetNameTexturesFail();

        AddCache(listWait, ref _loadingAnimWaitPng);
        AddCache(listSuccess, ref _loadingAnimSuccessPng);
        AddCache(listFail, ref _loadingAnimFailPng);

    }

    private void AddCache(List<string> list , ref List<Texture2D> animList)
    {
        foreach (string href in list)
        {
            animList.Add(IconPacketServices.Icons.LoadTextureOtherSources(href));
        }
    }
    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        yield return new WaitForEndOfFrame();
    }

    protected override void InitWindow(VisualElement root)
    {
        base.InitWindow(root);

        var dragArea = GetElementByClass("drag-area");

        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);
        var buttonStart = GetElementById("StartButton");
        Button closeButton = (Button)GetElementById("CancelButton");
        _progressBar = GetElementById("ProgressGauge");
        _textLabel = (Label)GetElementById("textLabel");
        _progressBarBg = GetElementById("ProgressBg");
        _animElement = GetElementById("animelement");
        var footer = GetElementById("footer");

        RegisterClickWindowEvent(_windowEle, dragArea);
        buttonStart.RegisterCallback<ClickEvent>((evt) => OnClick(evt));
        closeButton.RegisterCallback<ClickEvent>((evt) => OnCancel(evt));
        WireFrameClose();

        _slot1 = GetElementById("slot1");
        _slot2 = GetElementById("slot2");
        //_itemBox = GetElementById("itemBox");
        InitSlotElement1(_slot1);
        InitSlotElement2(_slot2);
        OnCenterScreen(root);

    }

    private void OnDestroy()
    {
        _instance = null;
    }

    private EnchantSlot ChangeSlotBoxType(VisualElement slotBox , EnchantSlot enchantSlot)
    {
        VisualElement slotElement = L2SlotGridLayout.InstantiateSlot(_inventorySlotTemplate, slotBox);
        enchantSlot = CreateEnchantSlot(1, slotElement, SlotType.Enchant);
        enchantSlot.AssignEmpty();
        return enchantSlot;
    }

    private EnchantSlot ChangeSlotBoxTypeChoice(VisualElement slotBox, EnchantSlot enchantSlot)
    {
        VisualElement slotElement = L2SlotGridLayout.InstantiateSlot(_inventorySlotChoiceTemplate, slotBox);
        enchantSlot = CreateEnchantSlot(1, slotElement, SlotType.Enchant);
        enchantSlot.AssignEmpty();
        return enchantSlot;
    }

    private void InitSlotElement1(VisualElement slotBox)
    {
        VisualElement slotElement = L2SlotGridLayout.InstantiateSlot(_inventorySlotTemplate, slotBox);
        _enchant1 = CreateEnchantSlot(0, slotElement, SlotType.Enchant);
        _enchant1.AssignEmpty();
    }

    private void InitSlotElement2(VisualElement slotBox)
    {
        VisualElement slotElement = L2SlotGridLayout.InstantiateSlot(_inventorySlotChoiceTemplate, slotBox);
        _enchant2 = CreateEnchantSlot(1, slotElement, SlotType.Enchant);
        _enchant2.AssignEmpty();
    }


    public void AddEnchantItem(int objectId)
    {
        if (_zoneWire && (objectId <= 0 || objectId == _scrollObjectId))
            return;

        PlayerInventory inventory = InventoryPacketServices.Inventory;
        if (inventory == null)
            return;
        ItemInstance itemInstance = inventory.GetItem(objectId);
        if (itemInstance == null)
        {
            Dictionary<int, ItemInstance> worn = inventory.GetPlayerEquipInventory();
            if (worn != null)
                worn.TryGetValue(objectId, out itemInstance);
        }

        if (itemInstance != null)
        {
            _selectItem = itemInstance;
            _slot2.Clear();
            _enchant2 = ChangeSlotBoxType(_slot2, _enchant2);
            _enchant2.AssignItem(itemInstance);
            if (_textLabel != null)
                _textLabel.text = NetEnchant.WithLevel(itemInstance.GetName(), itemInstance.EnchantLevel);
        }
        else
        {
            // Debug.LogWarning("EnchantWindows error not add dropItems");
        }
    }

    private EnchantSlot CreateEnchantSlot(int i, VisualElement slotElement, SlotType slotType)
    {
        return new EnchantSlot(i, slotElement, null, slotType, false);
    }
   
    private void OnClick(ClickEvent evt)
    {
        if (_awaitingRoll)
            return;
        if (_selectItem == null || (_zoneWire && _selectItem.ObjectId == _scrollObjectId))
        {
            if (_textLabel != null)
                _textLabel.text = "Select an item to enchant.";
            return;
        }

        if (_zoneWire)
            _awaitingRoll = true;
        RequestEnchantItem(_selectItem.ObjectId);
    }

    private void OnCancel(ClickEvent evt)
    {
        if (_zoneWire)
        {
            HideWindow();
            return;
        }

        RequestEnchantItem(-1);
    }

}
