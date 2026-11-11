using System;

public partial class SeedInfoWindow : L2PopupWindow
{
    private ICreator _creatorWindow;
    private ICreatorTables _creatorTable;
    private static SeedInfoWindow _instance;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static SeedInfoWindow Instance { get { return _instance; } }
    private VisualTreeAsset _tabTemplate;
    private VisualTreeAsset _tabHeaderTemplate;
    private VisualTreeAsset _buttonTemplate;


    private List<TableColumn> _defaultColumns0;
    private List<TableColumn> _defaultColumns1;
    private List<TableColumn> _defaultColumns2;
    private readonly string[] _tabsName = new string[] { "Seeds", "Fruit", "Bas.Info" };
    private VisualElement _footerContainer;


    private const string commandButton0 = "manor_menu_select?ask=2&state=-1&time=0";
    private const string commandButton1 = "manor_menu_select?ask=1&state=-1&time=0";

    private const string commandTab0 = "manor_menu_select?ask=3&state=1&time=0";
    private const string commandTab1 = "manor_menu_select?ask=4&state=1&time=0";
    private const string commandTab2 = "manor_menu_select?ask=5&state=1&time=0";
    private Button _buttonSeeds;
    private Button _buttonHarvst;
    private Button _buttonAll1;
    private Button _buttonAll2;

    [Inject] ItemNameTable _itemNames;

    private ItemNameTable ItemNames
    {
        get { return _itemNames != null ? _itemNames : TablePacketServices.ItemNames; }
    }

























}
