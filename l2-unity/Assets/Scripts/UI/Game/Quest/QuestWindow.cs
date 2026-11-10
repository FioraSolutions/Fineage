using System;

﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class QuestWindow : L2TwoPanels
{

    private static QuestWindow _instance;
    private BuilderTabs _builderTabs;

    private VisualTreeAsset _tabTemplate;
    private VisualTreeAsset _tabHeaderTemplate;
    private VisualTreeAsset _tabContenTemplate;
    private VisualTreeAsset _singleInsideContentTemplate;
    private VisualTreeAsset _toggleButtonTemplate;
    protected VisualTreeAsset _rowsTemplate;
    private QuestInstance _selectQuest;

    private IContent _questTabPanelSingle;
    private IContent _questTabPanelRepeat;
    private DataProviderDetailedInfo _dataProvider;
    private ICreatorVerticalScroll _creatorRewardTables;
    private List<OtherModel> _questsList = new List<OtherModel>();

    private const string _singleTabName = "Single";
    private const string _repeatTabName = "Repeat";
    private const string _epicTabName = "Epic";
    private const string _transferTabName = "Transfer";
    private const string _specialTabName = "Special";

   // private VisualElement _detailedInfoElement;

    private Label _labelAutoNotify;
    private VisualElement _imageBoxAutoNotify;
    private bool _isAutoNotifyCheckeNotify = false;


    private Label _labelTracking;
    private VisualElement _imageBoxTracking;
    private bool _isAutoNotifyCheckedTracking = true;
    [Obsolete("Use [Inject] / PacketServices instead")]
    public static QuestWindow Instance
    {
        get { return _instance; }
    }
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _builderTabs = new BuilderTabs();
            _questTabPanelSingle = new QuestTabPanel();
            _questTabPanelRepeat = new QuestTabPanel();
            _dataProvider = new DataProviderDetailedInfo();
            _creatorRewardTables = new CreatorVerticalScrollWindows();
            WireQuestPacketServices();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireQuestPacketServices();
    }

    private void WireQuestPacketServices()
    {
        QuestPacketServices.Bind(quest: this);
    }

    private void OnDestroy()
    {
        _instance = null;
        _builderTabs = null;
        _questTabPanelSingle = null;
        _questTabPanelRepeat = null;
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/Quest/QuestWindow");


        _tabTemplate = LoadAsset(L2InventoryUiAssets.Tab);
        _tabHeaderTemplate = LoadAsset("Data/UI/_Elements/Game/SkillLearn/SkillTabHeader");
        _singleInsideContentTemplate = LoadAsset("Data/UI/_Elements/Game/Quest/AllContent/SingleInsideContent");
        _tabContenTemplate = LoadAsset("Data/UI/_Elements/Game/Quest/QuestTabContent");
        _toggleButtonTemplate = LoadAsset("Data/UI/_Elements/Template/Elements/ToggleButtons/ToggleButton");
        _rowsTemplate = LoadAsset("Data/UI/_Elements/Template/Quest/TableReward/RewardItem");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);
        yield return new WaitForEndOfFrame();

        var dragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);
        var content = (VisualElement)GetElementById("contentQuest");
        var detailedButton = (Button)GetElementById("DetailedButton");
        var abortButton = (Button)GetElementById("AbortButton");

        _labelAutoNotify = (Label)GetElementById("labelCancelAutoNotify");
        _imageBoxAutoNotify = (VisualElement)GetElementById("CheckBox1");

        _labelTracking = (Label)GetElementById("labelTracking");
        _imageBoxTracking = (VisualElement)GetElementById("CheckBox2");


        detailedButton?.RegisterCallback<ClickEvent>(evt => OnClickButtonDetailedInfo(evt));
        abortButton?.RegisterCallback<ClickEvent>(evt => OnClickAbortButton(evt));

        _imageBoxAutoNotify?.RegisterCallback<ClickEvent>(evt => OnClickCheckBoxAutoNotify(evt));
        _imageBoxTracking?.RegisterCallback<ClickEvent>(evt => OnClickCheckBoxAutoTracking(evt));
        _labelAutoNotify?.RegisterCallback<ClickEvent>(evt => OnClickCheckBoxAutoNotify(evt));
        _labelTracking?.RegisterCallback<ClickEvent>(evt => OnClickCheckBoxAutoTracking(evt));

        var darkenerElements = _windowEle.Query<VisualElement>("Darkener").ToList();

        //DetailedInfo
        darkenerElements[0].style.opacity = new StyleFloat(0.3f);
        //Quest
        darkenerElements[1].style.opacity = new StyleFloat(0.9f);

        _detailedInfoElement = (VisualElement)GetElementById("detailedInfo");
        var windowTemplate = (VisualElement)GetElementById("windowTemplate");

        SetMouseOverDetectionSubElement(_detailedInfoElement);
        SetMouseOverDetectionRefreshTargetElement(windowTemplate);


        _builderTabs.InitContentTabs(new string[5] { _singleTabName, _repeatTabName, _epicTabName , _transferTabName , _specialTabName });
        _builderTabs.CreateTabs(content, _tabTemplate, _tabHeaderTemplate);

        //_creatorRewardTables.InitTradeTabs(new string[] { "ALL", "Other" });
        _creatorRewardTables.CreateTabs(_detailedInfoElement, null, _rowsTemplate);

        _questTabPanelSingle.SetTemplateContent(_tabContenTemplate , new List<VisualTreeAsset>() { _singleInsideContentTemplate , _toggleButtonTemplate });
        _questTabPanelRepeat.SetTemplateContent(_tabContenTemplate, new List<VisualTreeAsset>() { _singleInsideContentTemplate, _toggleButtonTemplate });

        _questTabPanelRepeat.OnButtonClick += OnClickButton;
        _questTabPanelSingle.OnButtonClick += OnClickButton;
        _builderTabs.EventSwitchOut += OnSwitchEventOut;

        VisualElement element = _builderTabs.GetActiveContent();
        _questTabPanelSingle.GetOrCreateTab(element);

        RegisterCloseWindowEvent("btn-close-frame");
        //RegisterCloseWindowEventByName("CloseButton");
        RegisterClickWindowEvent(_windowEle, dragArea);
        OnCenterScreen(_root);

        HideWindow();
        AddTestRewardData();
    }

    private List<QuestInstance> repeatQuests;
    private List<QuestInstance> nonRepeatQuests;













}

public class ModelQuestDemoReward
{
    private string _nameReward;
    private string _decReward;
    private string _icon;

    public ModelQuestDemoReward(string nameReward, string decReward, string icon)
    {
        _nameReward = nameReward;
        _decReward = decReward;
        _icon = icon;
    }

    public string NameReward => _nameReward;
    public string DecReward => _decReward;
    public string Icon => _icon;
}