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

}
