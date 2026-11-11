using System;

public partial class MultiSellWindow : L2PopupWindow
{
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _creatorWindow = new CreatorTradingWindows();
            _dataProvider = new TooltipDataProvider();
            _toolTips = new MultiSellToolTips(_dataProvider);
            WireMultiSellPacketServices();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireMultiSellPacketServices();
    }

    private void WireMultiSellPacketServices()
    {
        MultiSellPacketServices.Bind(multiSell: this);
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/Merchant/MultiSellWindow");
        //_tabTemplate = LoadAsset("Data/UI/_Elements/Game/Inventory/InventoryTab");
        //_tabHeaderTemplate = LoadAsset("Data/UI/_Elements/Game/Inventory/InventoryTabHeader");

        _windowTemplateWeapon = LoadAsset("Data/UI/_Elements/Game/ToolTips/ToolTipWeapon");
        _windowTemplateAcccesories = LoadAsset("Data/UI/_Elements/Game/ToolTips/ToolTipAccessories");
        _windowTemplateArmor = LoadAsset("Data/UI/_Elements/Game/ToolTips/ToolTipArmor");
        _itemTemplateIngredient = LoadAsset("Data/UI/_Elements/Template/Merchant/ItemsIngredient");

        _creatorWindow.LoadAsset(LoadAsset);
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);
        yield return new WaitForEndOfFrame();

        _userInput = (TextField)GetElementById("UserInputField");
         var deleteButton = (Button)GetElementById("DeleteButton");
         var exchangeButton = (Button)GetElementById("ExchangeButton");
        //_inventoryTabView = GetElementById("InventoryTabView");

        _creatorWindow.InitTradeTabs(new string[] { "ALL" , "Other" });
        _creatorWindow.CreateTradeTabs(GetElementById("InventoryTabView"));

        _scrollView1 = (ScrollView)GetElementById("1Panel-ScrollView");
        _scrollView2 = (ScrollView)GetElementById("2Panel-ScrollView");

             //_createScroller.RegisterPlayerScrollEvent(_scrollView1, _scrollView1.verticalScroller);
        _contentPanel1 = _scrollView1.contentContainer;
        _contentPanel2 = _scrollView2.contentContainer;

        var dragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);
        RegisterCloseWindowEvent("btn-close-frame");
        RegisterClickWindowEvent(_windowEle, dragArea);
        RegisterCloseWindowEventByName("CloseButton");

        _userInput.value = "1";
        _userInput.RegisterValueChangedCallback(evt => OnValueChanged(evt));
        deleteButton.RegisterCallback<ClickEvent>((evt) => OnClick(evt));
        exchangeButton.RegisterCallback<ClickEvent>((evt) => OnSendServer(evt));

        OnCenterScreen(_root);

        _creatorWindow.EventLeftClick += OnEventLeftClick;
        _toolTips.SetTemplate(_windowTemplateWeapon, _windowTemplateAcccesories, _windowTemplateArmor, _itemTemplateIngredient);

        yield return new WaitForEndOfFrame();
    }
}
