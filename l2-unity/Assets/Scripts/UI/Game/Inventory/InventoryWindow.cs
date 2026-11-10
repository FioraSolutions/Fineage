using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class InventoryWindow : L2PopupWindow
{
    public static int PLAYER_INVENTORY_SIZE = 81;

    /// <summary>Interlude InventoryWnd bag columns (ItemWindow). Classic expand used 9/12.</summary>
    public const int BagColumns = 6;

    private VisualTreeAsset _tabTemplate;
    private VisualTreeAsset _tabHeaderTemplate;
    private VisualTreeAsset _inventorySlotTemplate;
    private VisualTreeAsset _minimizedTemplate;
    private VisualElement _inventoryTabView;
    private VisualElement _minimizedInventoryBtn;
    private InventoryTab _activeTab;


    private MouseOverDetectionManipulator _minimizedInventoryMouseOverManipulator;
    private DragManipulator _minimizedInventoryDragManipulator;
    [SerializeField] private bool _expanded = false;
    private VisualElement _expandButton;

    [SerializeField] private InventoryGearTab _gearTab;
    [SerializeField] private List<InventoryTab> _tabs;

    public List<ItemInstance> _playerItems;

    [SerializeField] private int _usedSlots;
    [SerializeField] private int _slotCount;
    [SerializeField] private int _currentWeight;
    [SerializeField] private int _maximumWeight;
    [SerializeField] private int _adenaCount;

    private Label _inventoryCountLabel;
    private Label _weightLabel;
    private Label _adenaCountLabel;

    private VisualElement _weiBar;
    private VisualElement _weiBarBg;
    protected bool _isWindowHiddenMain = true;
    public VisualTreeAsset InventorySlotTemplate { get { return _inventorySlotTemplate; } }
    public bool Expanded { get { return _expanded; } }
    public int UsedSlots { get { return _usedSlots; } }
    public int SlotCount { get { return _slotCount; } }

    private static InventoryWindow _instance;
    bool _itemSurfaceReady;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static InventoryWindow Instance
    {
        get { return _instance; }
    }

    public bool IsItemSurfaceReady { get { return _itemSurfaceReady; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(this);
        }

        WireInventoryPacketServices();
    }

    private void Start()
    {
        WireInventoryPacketServices();
    }

    private void WireInventoryPacketServices()
    {
        InventoryPacketServices.Bind(window: this);
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/Inventory/InventoryWindow");
        _tabTemplate = LoadAsset(L2InventoryUiAssets.Tab);
        _tabHeaderTemplate = LoadAsset(L2InventoryUiAssets.TabHeader);
        _inventorySlotTemplate = LoadAsset(L2InventoryUiAssets.Slot);
        _minimizedTemplate = LoadAsset("Data/UI/_Elements/Game/Inventory/InventoryMin");
    }

    protected override void InitWindow(VisualElement root)
    {
        base.InitWindow(root);

        _expanded = false;

        ApplyInterludeInventoryTextures();

        var dragArea = GetElementByClass("drag-area");
        if (dragArea != null)
        {
            DragManipulator drag = new DragManipulator(dragArea, _windowEle);
            dragArea.AddManipulator(drag);
        }

        RegisterCloseWindowEvent("btn-close-frame");
        RegisterClickWindowEvent(_windowEle, dragArea);

        // Interlude InventoryWnd has no expand control; Classic DF chrome still may.
        _expandButton = (Button)GetElementByClass("expand-btn");
        if (_expandButton != null)
        {
            _expandButton.AddManipulator(new ButtonClickSoundManipulator(_expandButton));
            _expandButton.RegisterCallback<MouseDownEvent>(OnExpandButtonPressed, TrickleDown.TrickleDown);
        }

        Button adenaDistribution = (Button)GetElementById("AdenaDistribBtn");
        if (adenaDistribution != null)
        {
            adenaDistribution.AddManipulator(new ButtonClickSoundManipulator(adenaDistribution));
            adenaDistribution.AddManipulator(new TooltipManipulator(adenaDistribution, "Adena distribution"));
        }

        Button compoundBtn = (Button)GetElementById("CompoundBtn");
        if (compoundBtn != null)
        {
            compoundBtn.AddManipulator(new ButtonClickSoundManipulator(compoundBtn));
            compoundBtn.AddManipulator(new TooltipManipulator(compoundBtn, "Compound"));
        }

        Button trashBtn = (Button)GetElementById("TrashBtn");
        if (trashBtn != null)
        {
            trashBtn.AddManipulator(new ButtonClickSoundManipulator(trashBtn));
            trashBtn.AddManipulator(new TooltipManipulator(trashBtn, "Trash"));
            _ = new L2Slot(trashBtn, 100, SlotType.Trash);
        }

        _inventoryCountLabel = GetLabelById("InventoryCount");

        _weiBar= GetElementById("WeightBg");
        _weiBarBg = GetElementById("WeightGauge");
        PaintWeightFillParts();

        _adenaCountLabel = GetLabelById("AdenaCount");
        _weightLabel = GetLabelById("CurrentWeight");
    }

    /// <summary>
    /// UITK <c>resource()</c> often fails for Interlude sprites; paint from Resources.Load.
    /// Slot coords are texture-relative (interface.xdat window Y minus BackTexture Y=20).
    /// </summary>

    private void OnExpandButtonPressed(MouseDownEvent evt)
    {
        if (!_expanded)
        {
            if (!_windowEle.ClassListContains("expanded"))
            {
                _windowEle.AddToClassList("expanded");
            }
            if (!_expandButton.ClassListContains("expanded"))
            {
                _expandButton.AddToClassList("expanded");
            }
            _expanded = true;

            //UpdateItemList(_playerItems);
        }
        else
        {
            if (_windowEle.ClassListContains("expanded"))
            {
                _windowEle.RemoveFromClassList("expanded");
            }
            if (_expandButton.ClassListContains("expanded"))
            {
                _expandButton.RemoveFromClassList("expanded");
            }
            _expanded = false;

           //UpdateItemList(_playerItems);
        }

        evt.PreventDefault();
    }

    private void CreateMinimizedWindow()
    {

        Button minimizeWindowButton = (Button)GetElementByClass("minimize-btn");
        if (minimizeWindowButton != null)
        {
            minimizeWindowButton.AddManipulator(new ButtonClickSoundManipulator(minimizeWindowButton));
            minimizeWindowButton.RegisterCallback<MouseUpEvent>(OnMinimizeInventoryClick, TrickleDown.TrickleDown);
        }

        // Minized inventory button
        _minimizedInventoryBtn = _minimizedTemplate.Instantiate()[0];
        _minimizedInventoryMouseOverManipulator = new MouseOverDetectionManipulator(_minimizedInventoryBtn);
        _minimizedInventoryBtn.AddManipulator(_minimizedInventoryMouseOverManipulator);
        _minimizedInventoryMouseOverManipulator.Disable();

        _minimizedInventoryBtn.style.left = new StyleLength(Screen.width / 2);
        _minimizedInventoryBtn.style.top = new StyleLength(Screen.height / 2);

        _minimizedInventoryBtn.RegisterCallback<ClickEvent>(OnMinimizedInventoryClick, TrickleDown.TrickleDown);
        _minimizedInventoryDragManipulator = new DragManipulator(_minimizedInventoryBtn, _minimizedInventoryBtn);
        _minimizedInventoryBtn.AddManipulator(_minimizedInventoryDragManipulator);

        _root.Add(_minimizedInventoryBtn);
        _minimizedInventoryBtn.style.display = DisplayStyle.None;
    }

    private void OnMinimizeInventoryClick(MouseUpEvent evt)
    {
        _minimizedInventoryBtn.style.display = DisplayStyle.Flex;
        _minimizedInventoryMouseOverManipulator.Enable();
        HideWindow();
    }

    private void OnMinimizedInventoryClick(ClickEvent evt)
    {
        if (!_minimizedInventoryDragManipulator.dragged)
        {
            AudioPacketServices.Audio.PlayUISound("click_01");
            _minimizedInventoryBtn.style.display = DisplayStyle.None;
            _minimizedInventoryMouseOverManipulator.Disable();
            ShowWindow();
        }
    }
}
