using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public class InventoryWindow : L2PopupWindow
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
    void ApplyInterludeInventoryTextures()
    {
        // Full Inventory_Back plate (tab cutouts + bag frame). Gear is hitboxes only.
        SetResourcesBackground("Container", "Data/UI/Inventory/Inventory_Interlude_Back");
        VisualElement gear = GetElementById("Gear");
        if (gear != null)
            gear.style.backgroundImage = StyleKeyword.None;

        // Header FrameBack + sys buttons — same load path (resource() is unreliable).
        PaintNamedBackground("Left-Frame-Element", "Data/UI/Frame/Interlude/FrameBackLeft");
        PaintNamedBackground("Center-Frame-Element", "Data/UI/Frame/Interlude/FrameBackMid");
        PaintNamedBackground("Right-Frame-Element", "Data/UI/Frame/Interlude/FrameBackRight");
        // System menu (⋮) — not chatting_option wrench.
        PaintNamedBackground("Options-Window-Button", "Data/UI/Frame/Interlude/FrameOptionBtn");
        PaintNamedBackground("Minimize-Window-Button", "Data/UI/Frame/Interlude/FrameMiniBtn");
        PaintNamedBackground("Close-Window-Button", "Data/UI/Frame/Interlude/FrameCloseBtn");

        VisualElement trashBg = _windowEle?.Q<VisualElement>("TrashBtn")?.Q<VisualElement>("ButtonBg");
        if (trashBg != null)
            L2SlotIconView.TrySetResourcesBackground(
                trashBg,
                "Data/UI/Inventory/Inventory_Interlude_Trash");

        // UITK resource() often fails for these strips — stamp from Resources.Load.
        PaintWeightFillParts();
    }

    void PaintWeightFillParts()
    {
        VisualElement gauge = _weiBarBg ?? GetElementById("WeightGauge");
        if (gauge == null)
            return;
        L2SlotIconView.TrySetResourcesBackground(
            gauge.Q("Left"), "Data/UI/Inventory/Inventory_Interlude_Weight1");
        L2SlotIconView.TrySetResourcesBackground(
            gauge.Q("Center"), "Data/UI/Inventory/Inventory_Interlude_Weight2");
        L2SlotIconView.TrySetResourcesBackground(
            gauge.Q("Right"), "Data/UI/Inventory/Inventory_Interlude_Weight3");
    }

    void SetResourcesBackground(string elementId, string resourcesPath)
    {
        VisualElement ele = GetElementById(elementId);
        if (ele == null)
            return;
        L2SlotIconView.TrySetResourcesBackground(ele, resourcesPath);
    }

    void PaintNamedBackground(string elementName, string resourcesPath)
    {
        VisualElement ele = _windowEle?.Q(elementName);
        if (ele == null)
            return;
        L2SlotIconView.TrySetResourcesBackground(ele, resourcesPath);
    }

    /// <summary>
    /// UITK resource() often misses Interlude tab sprites — stamp Texture2D after headers exist.
    /// </summary>
    void ApplyInterludeTabChrome()
    {
        if (_inventoryTabView == null)
            return;

        Texture2D tabActive = L2SlotIconView.LoadResourcesTexture(
            "Data/UI/Inventory/Inventory_Interlude_Tab1");
        Texture2D tabIdle = L2SlotIconView.LoadResourcesTexture(
            "Data/UI/Inventory/Inventory_Interlude_Tab2");
        Texture2D tabHover = L2SlotIconView.LoadResourcesTexture(
            "Data/UI/Inventory/Inventory_Interlude_Tab2_over");

        foreach (VisualElement header in _inventoryTabView.Query(className: "tab-header").ToList())
        {
            ApplyTabHeaderBackground(header, tabActive, tabIdle);

            header.RegisterCallback<MouseEnterEvent>(_ =>
            {
                if (IsTabHeaderActive(header) || tabHover == null)
                    return;
                header.style.backgroundImage = new StyleBackground(tabHover);
            });
            header.RegisterCallback<MouseLeaveEvent>(_ =>
                ApplyTabHeaderBackground(header, tabActive, tabIdle));
        }
    }

    static bool IsTabHeaderActive(VisualElement header) =>
        header.ClassListContains("active") || header.ClassListContains("currentlySelectedTab");

    static void ApplyTabHeaderBackground(VisualElement header, Texture2D active, Texture2D idle)
    {
        Texture2D tex = IsTabHeaderActive(header) ? active : idle;
        if (tex != null)
            header.style.backgroundImage = new StyleBackground(tex);
    }


    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);

        yield return new WaitForEndOfFrame();

        CreateTabs();

        CreateMinimizedWindow();

        yield return new WaitForEndOfFrame();

       // UpdateItemList(_playerItems);
    }

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

    private void CreateTabs()
    {
        _inventoryTabView = GetElementById("InventoryTabView");

        VisualElement tabHeaderContainer = _inventoryTabView.Q<VisualElement>("tab-header-container");
        // Interlude: tabs left, (n/max) count right. Prefer dedicated host when present.
        VisualElement tabHeaderHost = _inventoryTabView.Q<VisualElement>("tab-headers") ?? tabHeaderContainer;
        VisualElement tabContainer = _inventoryTabView.Q<VisualElement>("tab-content-container");

        for (int i = 0; i < _tabs.Count; i++)
        {
            VisualElement tabElement = _tabTemplate.CloneTree()[0];
            tabElement.name = _tabs[i].TabName;
            tabElement.AddToClassList("unselected-tab");

            VisualElement tabHeaderElement = _tabHeaderTemplate.CloneTree()[0];
            tabHeaderElement.name = _tabs[i].TabName;
            tabHeaderElement.Q<Label>().text = _tabs[i].TabName;

            tabHeaderHost.Add(tabHeaderElement);
            tabContainer.Add(tabElement);

            _tabs[i].Initialize(_windowEle, tabElement, tabHeaderElement , true , true);
        }

        if (_tabs.Count > 0)
        {
            SwitchTab(_tabs[0]);
        }

        _tabs[0].MainTab = true;
        ApplyInterludeTabChrome();

        _gearTab = new InventoryGearTab();
        _gearTab.Initialize(_windowEle, null, null , true , true);
        _itemSurfaceReady = true;

        // ItemList normally arrives during EnterWorld, before this window is built, so the
        // rows are cached in PlayerInventory. Push them now that the tabs exist.
        if (InventoryPacketServices.Inventory != null)
            InventoryPacketServices.Inventory.RefreshWindow();
        RefreshWeightFromPlayer();
    }

    public override void ShowWindow()
    {
        base.ShowWindow();
        RefreshWeightFromPlayer();
    }

    /// <summary>
    /// Status sheet already mirrors <see cref="PlayerStats"/> from UserInfo; inventory used to
    /// keep a stale "00.00%" until the next UserInfo fanout hit <see cref="UpdateStats"/>.
    /// </summary>
    void RefreshWeightFromPlayer()
    {
        PlayerEntity player = DomainPacketServices.LocalPlayer;
        if (player?.Stats is PlayerStats stats)
        {
            UpdateStats(stats);
            return;
        }

        PlayerInfoInterlude info = ZoneCommands.Transport != null
            ? ZoneCommands.Transport.PlayerInfo
            : default;
        if (info.Stats != null)
            UpdateStats(info.Stats);
    }

    public int GetEmptySlot()
    {
        return _tabs[0].GetEmptySlot();
    }
    public bool SwitchTab(InventoryTab switchTo)
    {
        
        if (_activeTab != switchTo)
        {
            if (_activeTab != null)
            {
                _activeTab.TabContainer.AddToClassList("unselected-tab");
                _activeTab.TabHeader.RemoveFromClassList("active");
            }

            switchTo.TabContainer.RemoveFromClassList("unselected-tab");
            switchTo.TabHeader.AddToClassList("active");
            //ScrollDown(switchTo.Scroller);
            SwitchItemList(switchTo);
            _activeTab = switchTo;
            RefreshInterludeTabChromeImages();
            return true;
        }
        
        return false;
    }

    void RefreshInterludeTabChromeImages()
    {
        if (_inventoryTabView == null)
            return;

        Texture2D tabActive = L2SlotIconView.LoadResourcesTexture(
            "Data/UI/Inventory/Inventory_Interlude_Tab1");
        Texture2D tabIdle = L2SlotIconView.LoadResourcesTexture(
            "Data/UI/Inventory/Inventory_Interlude_Tab2");

        foreach (VisualElement header in _inventoryTabView.Query(className: "tab-header").ToList())
            ApplyTabHeaderBackground(header, tabActive, tabIdle);
    }

    private void SwitchItemList(InventoryTab switchTo)
    {
        // Interlude: Items (main / non-quest bag) + Quest.
        if (switchTo.MainTab || switchTo.TabName.Equals("Items") || switchTo.TabName.Equals("All"))
        {
            List<ItemInstance> items_collect = InventoryPacketServices.Inventory.GetPlayerInvetoryToList();
            switchTo.SetItemList(FilterOutQuestItems(items_collect));
            return;
        }

        List<ItemInstance> filterList = InventoryPacketServices.Inventory.FilterInventory(switchTo.GetFilterCategories);
        if (filterList != null && filterList.Count > 0)
            switchTo.SetItemList(filterList);
        else
            switchTo.AddAllEmptyInventory(0);
    }

    static List<ItemInstance> FilterOutQuestItems(List<ItemInstance> items)
    {
        if (items == null)
            return new List<ItemInstance>();
        var result = new List<ItemInstance>(items.Count);
        for (int i = 0; i < items.Count; i++)
        {
            ItemInstance item = items[i];
            if (item != null && item.Category != ItemCategory.Quest)
                result.Add(item);
        }
        return result;
    }

    public void UpdateStats(PlayerStats stats)
    {
        if (stats == null)
            return;

        _currentWeight = stats.CurrWeight;
        _maximumWeight = stats.MaxWeight;
        float weightRatio = stats.MaxWeight > 0
            ? (float)stats.CurrWeight / stats.MaxWeight
            : 0f;
        if (_weightLabel != null)
            _weightLabel.text = $"{weightRatio * 100f:0.00}%";

        // WeightBg = track, WeightGauge = Left/Center/Right fill clipped by width.
        VisualElement track = _weiBar;
        VisualElement fill = _weiBarBg;
        if (track == null || fill == null)
            return;

        PaintWeightFillParts();
        ApplyInventoryWeightFill(weightRatio);

        // First open often runs before UITK has resolved the track width — retry next frame.
        if (track.resolvedStyle.width <= 1f)
            StartCoroutine(ApplyInventoryWeightFillNextFrame(weightRatio));
    }

    IEnumerator ApplyInventoryWeightFillNextFrame(float weightRatio)
    {
        yield return null;
        ApplyInventoryWeightFill(weightRatio);
    }

    void ApplyInventoryWeightFill(float weightRatio)
    {
        VisualElement track = _weiBar;
        VisualElement fill = _weiBarBg;
        if (track == null || fill == null)
            return;

        float trackWidth = track.resolvedStyle.width;
        if (float.IsNaN(trackWidth) || trackWidth <= 1f)
            trackWidth = track.layout.width;
        if (float.IsNaN(trackWidth) || trackWidth <= 1f)
            trackWidth = 160f;

        fill.style.position = Position.Absolute;
        fill.style.left = 0;
        fill.style.top = 0;
        fill.style.height = 12f;
        fill.style.overflow = Overflow.Hidden;
        fill.style.flexDirection = FlexDirection.Row;
        fill.style.unityBackgroundScaleMode = ScaleMode.StretchToFill;
        L2ResourceBarView.SetFillFixed(fill, track, weightRatio, trackWidth);
    }

    public void SetItemList(List<ItemInstance> allItems , List<ItemInstance> quipAllItems, int adenaCount, int usedSlots , int userId)
    {
        //_adenaCountLabel.text = adenaCount.ToString();
        SetAdenaCountLabel(adenaCount);
        //_tabs[0].ForEach((tab) =>
        //{
        _tabs[0].SetItemList(FilterOutQuestItems(allItems));
        _gearTab.SetEquipList(quipAllItems , userId);
        _slotCount = PLAYER_INVENTORY_SIZE;
        _inventoryCountLabel.text = $"({usedSlots}/{_slotCount})";
        // });
        SwitchTab(_tabs[0]);
        RefreshWeightFromPlayer();
}
    public void UpdateItemList(List<ItemInstance> removeAndAdd , List<ItemInstance> modified , List<ItemInstance> listEquipModified,  int adenaCount , int usedSlots , ChangeInventoryData changeInventoryData , int userId)
    {
        if (_activeTab.MainTab)
        {

            // Slot count
            _slotCount = PLAYER_INVENTORY_SIZE;
            _inventoryCountLabel.text = $"({usedSlots}/{_slotCount})";
            SetAdenaCountLabel(adenaCount);
            _tabs[0].UpdateItemList(
                FilterOutQuestItems(removeAndAdd),
                FilterOutQuestItems(modified),
                changeInventoryData);
            _gearTab.UpdateEquipList(listEquipModified , changeInventoryData , userId);
        }
        else
        {

            _slotCount = PLAYER_INVENTORY_SIZE;
            _inventoryCountLabel.text = $"({usedSlots}/{_slotCount})";
            SetAdenaCountLabel(adenaCount);
            _activeTab.SetItemList(InventoryPacketServices.Inventory.FilterInventory(_activeTab.GetFilterCategories));
            _gearTab.SetEquipList(InventoryPacketServices.Inventory.GetPlayerEquipToList(), userId);
        }

    }

    public void RemoveOldEquipItemOrNoQuipItem(ChangeInventoryData changeInventoryData , int userId)
    {
        List<ItemInstance> obsoleteItemsInventory = changeInventoryData.ItemsInventory();
        List<ItemInstance> obsoleteItemsGear = changeInventoryData.ItemsGears();
        List<ReplaceGearData> obsoleteItemsGearReplace = changeInventoryData.ItemsReplace();

        foreach (ItemInstance item in obsoleteItemsInventory)
        {
            if (!obsoleteItemsGearReplace.Any(replaceData => replaceData.GetGear().EqualsBodyPart(item.BodyPart , item.IsBow())))
            {
                _tabs[0].ModifiedRemove(item);
            }
        }

        obsoleteItemsGear.ForEach(item =>
        {
            _gearTab.ModifiedRemove(item.BodyPart, item);
            GamePacketServices.Bus?.UnEquipped(item , userId);
        });


        foreach (ReplaceGearData item in obsoleteItemsGearReplace)
        {

            ItemInstance source = item.GetSource();
            ItemInstance gearItem = item.GetGear();

            InventorySlot source_item_slot = _tabs[0].GetInventorySlot(source.ObjectId);

            if (source_item_slot != null && source_item_slot.ItemInstance != null)
            {
                _gearTab.ModifiedReplace(source_item_slot.ItemInstance , userId);
            }



            if(gearItem != null & source_item_slot != null)
            {
                GamePacketServices.Bus?.UnEquipped(gearItem , userId);
                source_item_slot.AssignItem(gearItem);
            }
            //if there are 2 items in LHand and Rhand, we move 1 to the position of the one who is trying to change, and insert the second item into the inventory
            else if (gearItem != null & source_item_slot == null)
            {
                GamePacketServices.Bus?.UnEquipped(gearItem, userId);
                _tabs[0].AddItem(gearItem);
            }

        }


    }


    private void SetAdenaCountLabel(int count )
    {
        _adenaCountLabel.text = ToolTipsUtils.ConvertToPrice(count);
    }

 
    public void ToggleHideWindowManual()
    {
        if (InventoryPacketServices.Inventory == null)
            return;

        Dictionary<int, ItemInstance> allItems = GetInventoryItems();
        Dictionary<int, ItemInstance> equipItems = GetInventoryEquipItems();
        // Empty is fine; only a null dictionary means the snapshot never arrived. Both start
        // empty in PlayerInventory.Awake now, so an empty bag simply opens empty.
        if (allItems == null)
            allItems = new Dictionary<int, ItemInstance>();
        if (equipItems == null)
            equipItems = new Dictionary<int, ItemInstance>();

        int adenaCount = GetAdenaCount(allItems.Values.ToList());

        if (_isWindowHidden)
        {
             InventoryPacketServices.Inventory.SetInventory(allItems, equipItems , true, adenaCount , allItems.Count + equipItems.Count);
        }
        else
        {
            HideWindow();
        }
    }

    private Dictionary<int, ItemInstance> GetInventoryItems()
    {
       return  InventoryPacketServices.Inventory.GetPlayerInventory();
    }

    private Dictionary<int, ItemInstance> GetInventoryEquipItems()
    {
        return InventoryPacketServices.Inventory.GetPlayerEquipInventory();
    }

    private int GetAdenaCount(List<ItemInstance> modified)
    {
        ItemInstance item = modified.FirstOrDefault(o => o.Category == ItemCategory.Adena);
        return (item != null) ? item.Count : 0;
    }


    public ItemInstance GetItemByPosition(int position)
    {
        if(_activeTab != null)
        {
            return _activeTab.GetInventorySlotByPosition(position);
        }

        return null;
    }

    public GearItem GetGearPosition(int position)
    {
       return _gearTab.ConvertIdToGearType(position);
    }


}
