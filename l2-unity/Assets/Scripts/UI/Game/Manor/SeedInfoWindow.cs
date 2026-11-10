using System;

﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;


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

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/Manor/SeedInfoWindow");
        _tabTemplate = LoadAsset(L2InventoryUiAssets.Tab);
        _buttonTemplate = LoadAsset("Data/UI/_Elements/Template/Elements/Buttons/DefaultButton");
        _tabHeaderTemplate = LoadAsset(L2InventoryUiAssets.TabHeader);
    }


    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _creatorWindow =  new CreatorTabsWindows();
            _creatorTable = new CreatorTableWindows();
            WireAuxiliaryGameUIPacketServices();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireAuxiliaryGameUIPacketServices();
    }

    private void WireAuxiliaryGameUIPacketServices()
    {
        AuxiliaryGameUIPacketServices.Bind(seedInfo: this);
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);
        yield return new WaitForEndOfFrame();

        var dragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);
        var content = (VisualElement)GetElementById("content");

        _footerContainer = content.Q<VisualElement>("footerContent");
        _creatorTable.LoadAsset(LoadAsset);

        _defaultColumns0 = GetColumnWindowTab0();
        _defaultColumns1 = GetColumnWindowTab1();
        _defaultColumns2 = GetColumnWindowTab2();

        _creatorWindow.InitContentTabs(_tabsName);
        _creatorWindow.CreateTabs(content, _tabTemplate, _tabHeaderTemplate);
        _creatorWindow.InsertTablesIntoContent(_creatorTable, _defaultColumns0, true);
        _creatorWindow.EventSwitchTabByIndexOfTab += OnSwitchTab;
        AddContent0Footer(_creatorWindow , _footerContainer);



        RegisterCloseWindowEvent("btn-close-frame");
        RegisterClickWindowEvent(_windowEle, dragArea);
        OnCenterScreen(_root);

    }



















}
