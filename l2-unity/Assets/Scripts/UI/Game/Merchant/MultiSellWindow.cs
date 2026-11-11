using System;

public partial class MultiSellWindow : L2PopupWindow
{

    private static MultiSellWindow _instance;
    private ICreatorTradeTab _creatorWindow;

    private ScrollView _scrollView1;
    private ScrollView _scrollView2;
    private VisualElement _contentPanel1;
    private VisualElement _contentPanel2;

    private VisualTreeAsset _windowTemplateWeapon;
    private VisualTreeAsset _windowTemplateAcccesories;
    private VisualTreeAsset _windowTemplateArmor;
    private VisualTreeAsset _itemTemplateIngredient;
    private MultiSellListDto _listMultisell;
    private TooltipDataProvider _dataProvider;
    private MultiSellToolTips _toolTips;
    private TextField _userInput;
    private int _listId;
    private int _entryId = -1;
    // private CreateScroller _createScroller;
    private VisualElement _selectContainer;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static MultiSellWindow Instance { get { return _instance; } }















}
