using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class ClanWindow : L2TwoPanels
{

    private static ClanWindow _instance;
    private MasterClan _masterClan;
    private ClanDetailedInfo _detailedClan;
    private ICreatorTables _creatorTableWindows;

    private PledgeClanInfoDto _pladgeClanInfo = new PledgeClanInfoDto();

    private DropdownField _dropdown;
    private string _selectDropDown = "";

    private DataProviderClanInfo _dataProviderClanInfo;

    private Label _labelEditAuth;
    private Label _labelMemberInfo;
    private Label _labelPrivileges;
    private Label _labelClanInfo;
    private Label _labelPenalty;
    private Label _labelLeave;
    private Label _labelClanEntry;
    private Label _labelInvite;

    private VisualElement _zoneActions;
    private Label _clanNameCaption;
    private TextField _clanNameField;
    private Button _createClanButton;
    private Button _kickButton;
    private Button _dissolveButton;
    private Button _cancelDissolveButton;
    private int _selectedMemberId;

    //data
    private PledgeShowMemberListAllDto _packet;

    [Obsolete("Use [Inject] / PacketServices instead")]
    public static ClanWindow Instance { get { return _instance; } }
    public bool IsReady { get { return _windowEle != null && _dropdown != null; } }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _creatorTableWindows = new CreatorTableWindows();
            _masterClan = new MasterClan();

            _dataProviderClanInfo = new DataProviderClanInfo();
            _detailedClan = new ClanDetailedInfo(_dataProviderClanInfo);
            WireClanPacketServices();
            EnsureAskJoinWindow();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireClanPacketServices();
    }

    private void WireClanPacketServices()
    {
        ClanPacketServices.Bind(clan: this);
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/Clan/ClanWindow");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {

        InitWindow(root);

        yield return new WaitForEndOfFrame();

        defaultHeight = 447;
        defaultWidth = 538;

        var dragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);


        _dropdown = _windowEle.Q<DropdownField>("clanComboBox");
        _dropdown.pickingMode = PickingMode.Position;
        DisableEventOnOver(_dropdown);
        _dropdown.RegisterCallback<PointerDownEvent>(OnDropdownPointer, TrickleDown.TrickleDown);

        var master_table_content = GetElementByClass("master-table-list");

        _creatorTableWindows.InitTable(master_table_content);
        _creatorTableWindows.LoadAsset(LoadAsset);
        _masterClan.ForEachClan(_creatorTableWindows);
        _creatorTableWindows.OnRowClicked += _masterClan.SelectMember;
        _creatorTableWindows.OnRowClicked += SelectMember;

        _detailedClan.LoadAssets(LoadAsset);

        var memberButton = (UnityEngine.UIElements.Button)GetElementById("MemberButton");
        memberButton?.RegisterCallback<ClickEvent>(evt => OnClickShowMember(evt));

        var privilegesButton = (UnityEngine.UIElements.Button)GetElementById("PrivilegesButton");
        privilegesButton?.RegisterCallback<ClickEvent>(evt => OnClickPrivileges(evt));
   
        var editAuthButton = (UnityEngine.UIElements.Button)GetElementById("EditAuthButton");
        editAuthButton?.RegisterCallback<ClickEvent>(evt => OnClickEditAuthButton(evt));

        var clanInfo = (UnityEngine.UIElements.Button)GetElementById("ClanInoButton");
        clanInfo?.RegisterCallback<ClickEvent>(evt => OnClickClanInfo(evt));

        var penaltyButton = (UnityEngine.UIElements.Button)GetElementById("PenaltyButton");
        penaltyButton?.RegisterCallback<ClickEvent>(evt => OnClickPenalty(evt));

        var leaveButton = (UnityEngine.UIElements.Button)GetElementById("LeaveButton");
        leaveButton?.RegisterCallback<ClickEvent>(evt => OnClickLeave(evt));

        var inviteButton = (UnityEngine.UIElements.Button)GetElementById("InviteButton");
        inviteButton?.RegisterCallback<ClickEvent>(evt => OnClickInvite(evt));

        BuildZoneActions();
        _creatorTableWindows.OnRowClicked += OnZoneRowClicked;

        _detailedInfoElement = (VisualElement)GetElementById("detailedInfo");
        var masterClan = (VisualElement)GetElementById("masterClan");

        _detailedClan.SetDetailedInfoElement(_detailedInfoElement);

        _labelEditAuth = (UnityEngine.UIElements.Label)GetElementById("EditAuthLabel");
        _labelMemberInfo = (UnityEngine.UIElements.Label)GetElementById("MemberInfoLabel");
        _labelPrivileges = (UnityEngine.UIElements.Label)GetElementById("PrivilegesLabel");
        _labelClanInfo = (UnityEngine.UIElements.Label)GetElementById("ClanInfoLabel");
        _labelPenalty = (UnityEngine.UIElements.Label)GetElementById("PenaltyLabel");
        _labelLeave = (UnityEngine.UIElements.Label)GetElementById("LeaveLabel");
        _labelClanEntry = (UnityEngine.UIElements.Label)GetElementById("ClanEntryLabel");
        _labelInvite = (UnityEngine.UIElements.Label)GetElementById("InviteLabel");


        SetMouseOverDetectionSubElement(_detailedInfoElement);
        SetMouseOverDetectionRefreshTargetElement(masterClan);


        RegisterCloseWindowEvent("btn-close-frame");
        RegisterClickWindowEvent(_windowEle, dragArea);
        HideDetailedInfo();

        OnCenterScreen(_root);
        EnabledUnkButtons();

        if (_packet != null)
            ApplyClanData(_packet);
    }

    public void EnsureAskJoinWindow()
    {
        if (GetComponent<ClanAskJoinWindow>() == null)
            gameObject.AddComponent<ClanAskJoinWindow>();
    }

    public void ApplyZoneBook()
    {
        if (_windowEle == null || _dropdown == null)
            return;

        NetClanBook book = NetClanBook.Shared;
        if (!book.HasClan)
        {
            _packet = null;
            _selectedMemberId = 0;
            ShowNoClan();
            TuneZoneButtons(book);
            return;
        }

        var members = new List<ClanMember>();
        NetPledgeMember[] rows = book.Members;
        for (int i = 0; i < rows.Length; i++)
        {
            NetPledgeMember row = rows[i];
            members.Add(new ClanMember(row.Name, row.Level, row.ClassId, 0, 0, row.Online ? 1 : 0, row.PledgeType));
        }

        var snapshot = new PledgeShowMemberListAllDto();
        snapshot.ApplyZoneSnapshot(book.ClanId, book.Name, book.Level, book.LeaderName(), book.LeaderObjectId, book.CrestId, members);
        AddClanData(snapshot);
        Label peace = _windowEle.Q<Label>("labelAtPeace");
        if (peace != null)
            peace.text = book.Dissolving ? "Dissolving" : "At Peace";
        TuneZoneButtons(book);
    }

    //Detailed Clan
    public void UpdateDetailedInfo(object packet)
    {
        _detailedClan.UpdateDetailedInfo(packet , _detailedInfoElement , _packet);
        ShowDetailedInfo();
    }

    public void ShowGradeInfo(object packet)
    {
        _detailedClan.UpdateDetailedInfo(packet, _detailedInfoElement, _packet);
        ShowDetailedInfo();
    }


    //Master Clan 

    private void OnDestroy()
    {
        _instance = null; 
        _creatorTableWindows = null;
        _masterClan = null;
        _detailedClan = null;
        _dataProviderClanInfo = null;
    }
}
