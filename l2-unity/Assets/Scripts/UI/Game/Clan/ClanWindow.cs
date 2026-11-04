using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;

using UnityEngine.UIElements;

public class ClanWindow : L2TwoPanels
{

    [Inject] GameClient _gameClient;

    private GameClient Game => ClanPacketServices.Game;
   
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
        ClanPacketServices.Bind(game: _gameClient, clan: this);
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
    public void AddClanData(PledgeShowMemberListAllDto packet )
    {
        _packet = packet;
        if (_windowEle == null || _dropdown == null)
        {
            return;
        }

        ApplyClanData(packet);
    }

    private void ApplyClanData(PledgeShowMemberListAllDto packet)
    {
        _dataProviderClanInfo.SetMasterClanInfo(_windowEle , packet);
        _listDropDown = _masterClan.SetDropdownList(_dropdown , packet.PledgeName);
        _masterClan.CreateMembersTable(packet.Members, _creatorTableWindows);
        SetDisabledButton(packet.SubPledgeLeaderName , packet.Members);
    }

    //not wornig 
    public void DeleteMemberData()
    {
        //_packet.Members.Clear();
        //_creatorTableWindows.UpdateTableData(_masterClan.GetEmptyData());
        //_masterClan.Up(_creatorTableWindows);
    }

    public void UpdateMemberData(PledgeShowMemberListUpdateDto packetUpdate)
    {
        ClanMember clanMember = new ClanMember(packetUpdate.MemberName, packetUpdate.Level, packetUpdate.ClassId, packetUpdate.Sex, packetUpdate.Race, packetUpdate.IsOnline, packetUpdate.PledgeType);
        _masterClan.UpdateMemberData(clanMember, _packet, _creatorTableWindows);
    }

    public void DeleteMemberData(PledgeShowMemberListDeleteDto packetDelete)
    {
        _masterClan.DeleteMemeberTable(packetDelete.MemberName, _packet, _creatorTableWindows);
    }

    public void AddMemberData(PledgeShowMemberListAddDto packetAdd)
    {
        _masterClan.AddMemberData(packetAdd.ClanMember, _packet, _creatorTableWindows);
    }

    public void UpdatePledge(PledgeInfoDto pledge)
    {
        _dataProviderClanInfo.UpdateClanInfo(_windowEle, pledge);
    }

    public void UpdateClanIdInfo(PledgeStatusChangedDto changed)
    {
        if(_packet != null & changed != null)
        {
            _packet.ClanId = changed.ClanId;
            _packet.CrestId = changed.CrestId;
            _packet.LeaderId = changed.LeaderId;
            _packet.AllyId = changed.AllyId;
            _packet.AllyCrestId = changed.AllyCrestId;

        }
    }


    private void OnClickShowMember(ClickEvent evt)
    {
    }

    private void OnClickPrivileges(ClickEvent evt)
    {
    }




    private void OnClickEditAuthButton(ClickEvent evt)
    {
        //if (!string.IsNullOrEmpty(_masterClan.GetSelectMemberName()))
        //{
        //    ZoneCommands.Send(//        new RequestPledgePowerGradeList());
        //}

    }


    private void OnClickLeave(ClickEvent evt)
    {
        if(_packet != null && !IsLeader(_packet.SubPledgeLeaderName))
        {
            SystemMessageWindow.Instance.OnButtonOk += OkLeave;
            SystemMessageWindow.Instance.OnButtonClosed += OnРЎancel;
            SystemMessageWindow.Instance.ShowWindowDialogYesOrNot("Are you sure you want to leave the clan?");
        }

    }

    private void OnClickInvite(ClickEvent evt)
    {
        if (_packet != null && IsLeader(_packet.SubPledgeLeaderName))
        {
            SystemMessageWindow.Instance.ShowWindowDialogDropdownYesOrNot(" Select a Unit " , new List<string> { _packet.PledgeName});
            SystemMessageWindow.Instance.OnButtonOk += OkInvite;
            SystemMessageWindow.Instance.OnButtonClosed += OnРЎancel;
        }

    }

    private string GetSelfName()
    {
        UserInfoDto user = StorageNpc.getInstance().GetFirstUser();
        if (user == null || user.PlayerInfoInterlude.Identity == null)
        {
            return null;
        }

        return user.PlayerInfoInterlude.Identity.Name;
    }

    private bool IsLeader(string leaderName)
    {
        string userName = GetSelfName();
        return userName != null && leaderName == userName;
    }

    private void OnClickPenalty(ClickEvent evt)
    {

            ZoneCommands.Send(new RequestUserCommandCommand(100));
        
    }

    private void OnClickClanInfo(ClickEvent evt)
    {
        _detailedClan.UpdateDetailedInfo(_pladgeClanInfo, _detailedInfoElement, _packet);
        ShowDetailedInfo();
    }


    public void SelectMember(int selectIndex, string select_text)
    {
        if (_detailedInfoElement.style.display == DisplayStyle.Flex)
        {


            switch (_detailedClan.GetShowPanel())
            {
                case 0:
                    OnClickShowMember(null);
                    break;

                case 1:
                    OnClickPrivileges(null);
                    break;
                default:
                    break;
            }
        }
    }




    private const string USS_STYLE_YELLOW = "button-label-yellow";
    private const string USS_STYLE_DISABLED = "button-label-disabled";
    private void SetDisabledButton(string leaderName, List<ClanMember> Members)
    {
        string userName = GetSelfName();
        if (userName == null || _labelEditAuth == null)
        {
            return;
        }

        ResetAllButtons();

        if (IsLeader(leaderName))
            EnabledLeaderButtons();
        else
        {
            if (Members.FirstOrDefault(m => m.MemberName == userName) == null)
                EnabledUnkButtons();
            else
                EnabledUserButtons();
        }
    }


    private void ResetAllButtons()
    {
        _labelEditAuth.AddToClassList(USS_STYLE_DISABLED);
        _labelMemberInfo.AddToClassList(USS_STYLE_DISABLED);
        _labelPrivileges.AddToClassList(USS_STYLE_DISABLED);
        _labelClanInfo.AddToClassList(USS_STYLE_DISABLED);
        _labelPenalty.AddToClassList(USS_STYLE_DISABLED);
        _labelLeave.AddToClassList(USS_STYLE_DISABLED);
        _labelClanEntry.AddToClassList(USS_STYLE_DISABLED);
        _labelInvite.AddToClassList(USS_STYLE_DISABLED);
    }


    private void EnabledLeaderButtons()
    {
        _labelEditAuth.RemoveFromClassList(USS_STYLE_DISABLED);
        _labelMemberInfo.RemoveFromClassList(USS_STYLE_DISABLED);
        _labelPrivileges.RemoveFromClassList(USS_STYLE_DISABLED);
        _labelClanInfo.RemoveFromClassList(USS_STYLE_DISABLED);
        _labelPenalty.RemoveFromClassList(USS_STYLE_DISABLED);
        _labelLeave.RemoveFromClassList(USS_STYLE_DISABLED);
        _labelInvite.RemoveFromClassList(USS_STYLE_DISABLED);


        _labelEditAuth.AddToClassList(USS_STYLE_YELLOW);
        _labelMemberInfo.AddToClassList(USS_STYLE_YELLOW);
        _labelPrivileges.AddToClassList(USS_STYLE_YELLOW);
        _labelClanInfo.AddToClassList(USS_STYLE_YELLOW);
        _labelPenalty.AddToClassList(USS_STYLE_YELLOW);
        _labelLeave.AddToClassList(USS_STYLE_YELLOW);
        _labelInvite.AddToClassList(USS_STYLE_YELLOW);

    }

    private void EnabledUserButtons()
    {
        _labelMemberInfo.RemoveFromClassList(USS_STYLE_DISABLED);
        _labelPrivileges.RemoveFromClassList(USS_STYLE_DISABLED);
        _labelPenalty.RemoveFromClassList(USS_STYLE_DISABLED);
        _labelClanInfo.RemoveFromClassList(USS_STYLE_DISABLED);
        _labelLeave.RemoveFromClassList(USS_STYLE_DISABLED);

        _labelMemberInfo.AddToClassList(USS_STYLE_YELLOW);
        _labelPrivileges.AddToClassList(USS_STYLE_YELLOW);
        _labelPenalty.AddToClassList(USS_STYLE_YELLOW);
        _labelClanInfo.AddToClassList(USS_STYLE_YELLOW);
        _labelLeave.AddToClassList(USS_STYLE_YELLOW);
    }

    private void EnabledUnkButtons()
    {

        _labelPenalty.RemoveFromClassList(USS_STYLE_DISABLED);
        _labelLeave.RemoveFromClassList(USS_STYLE_DISABLED);

        _labelPenalty.AddToClassList(USS_STYLE_YELLOW);
        _labelLeave.AddToClassList(USS_STYLE_YELLOW);

    }

    private void OkLeave()
    {
        ZoneCommands.Send(new RequestWithdrawPledgeCommand());
        CancelEvent();
    }

    private void OkInvite()
    {
        if (TargetManager.Instance.HasTarget() && TargetManager.Instance.Target.GetEntity() != null)
        {
            int id = TargetManager.Instance.Target != null ? TargetManager.Instance.Target.GetEntity().Identity.Id : 0;

            ZoneCommands.Send(new RequestJoinPledgeCommand(id));
        }

        CancelEvent();
    }

    private void OnРЎancel()
    {
        CancelEvent();
    }
    private void CancelEvent()
    {
        SystemMessageWindow.Instance.OnButtonOk -= OkLeave;
        SystemMessageWindow.Instance.OnButtonClosed -= OnРЎancel;
    }


    void BuildZoneActions()
    {
        VisualElement footer = _windowEle.Q<VisualElement>("FooterButtons");
        VisualElement box = _windowEle.Q<VisualElement>("BoxContent");
        if (footer == null)
            footer = _windowEle;

        _zoneActions = new VisualElement();
        _zoneActions.style.flexDirection = FlexDirection.Row;
        _zoneActions.style.flexWrap = Wrap.Wrap;
        _zoneActions.style.marginTop = 4;
        _zoneActions.style.marginBottom = 2;

        _clanNameCaption = new Label("Clan");
        _clanNameCaption.style.color = new Color(0.7f, 0.7f, 0.7f);
        _clanNameCaption.style.unityFontStyleAndWeight = FontStyle.Normal;
        _clanNameCaption.style.fontSize = 11;
        _clanNameCaption.style.marginRight = 4;
        _clanNameCaption.style.alignSelf = Align.Center;

        _clanNameField = new TextField();
        _clanNameField.maxLength = 16;
        _clanNameField.style.width = 110;
        _clanNameField.style.height = 18;
        _clanNameField.style.marginRight = 4;

        _createClanButton = MakeZoneButton("Create", OnCreateClan);
        _kickButton = MakeZoneButton("Kick", OnKick);
        _dissolveButton = MakeZoneButton("Dissolve", OnDissolve);
        _cancelDissolveButton = MakeZoneButton("Cancel", OnCancelDissolve);

        _zoneActions.Add(_clanNameCaption);
        _zoneActions.Add(_clanNameField);
        _zoneActions.Add(_createClanButton);
        _zoneActions.Add(_kickButton);
        _zoneActions.Add(_dissolveButton);
        _zoneActions.Add(_cancelDissolveButton);
        if (box != null && footer != null && footer.parent == box)
            box.Insert(box.IndexOf(footer), _zoneActions);
        else
            footer.Insert(0, _zoneActions);
        TuneZoneButtons(NetClanBook.Shared);
    }

    static Button MakeZoneButton(string text, System.Action click)
    {
        var button = new Button(click) { text = text };
        button.style.height = 18;
        button.style.marginRight = 3;
        button.style.fontSize = 11;
        return button;
    }

    void ShowNoClan()
    {
        SetLabel("labelClanName", "");
        SetLabel("labelLeaderName", "");
        SetLabel("lvlName", "0");
        SetLabel("labelCountOnline", "(0/0)");
        Label peace = _windowEle.Q<Label>("labelAtPeace");
        if (peace != null)
            peace.text = "";
        if (_dropdown != null)
            _masterClan.SetDropdownList(_dropdown, "");
        if (_creatorTableWindows != null)
            _masterClan.CreateMembersTable(new List<ClanMember>(), _creatorTableWindows);
    }

    void SetLabel(string name, string text)
    {
        Label label = _windowEle.Q<Label>(name);
        if (label != null)
            label.text = text;
    }

    void TuneZoneButtons(NetClanBook book)
    {
        if (_zoneActions == null)
            return;
        _zoneActions.style.display = DisplayStyle.None;
    }

    void OnZoneRowClicked(int index, string text)
    {
        _selectedMemberId = 0;
        if (_masterClan == null)
            return;
        string name = _masterClan.GetSelectMemberName();
        NetPledgeMember member;
        if (NetClanBook.Shared.TryMemberByName(name, out member))
            _selectedMemberId = member.ObjectId;
    }

    void OnCreateClan()
    {
        string name = _clanNameField != null ? _clanNameField.value : "";
        name = name != null ? name.Trim() : "";
        if (name.Length == 0)
        {
            NetChatFeed.NoteSystem("Enter a clan name.");
            return;
        }

        ZoneCommands.Send(new ClanCreateRequestCommand(name));
    }

    void OnKick()
    {
        if (!NetClanBook.Shared.IsLeader(SelfObjectId()))
            return;
        if (_selectedMemberId <= 0)
        {
            NetChatFeed.NoteSystem("Select a clan member to dismiss.");
            return;
        }

        if (_selectedMemberId == SelfObjectId())
        {
            NetChatFeed.NoteSystem(NetClanNotices.Map("clan_dismiss_self"));
            return;
        }

        int target = _selectedMemberId;
        Confirm("Dismiss this member from the clan?", () => ZoneCommands.Send(new ClanKickRequestCommand(target)));
    }

    void OnDissolve()
    {
        if (!NetClanBook.Shared.IsLeader(SelfObjectId()))
            return;
        Confirm("Register clan dissolution? The clan remains for 7 days.", () => ZoneCommands.Send(new ClanDissolveRequestCommand()));
    }

    void OnCancelDissolve()
    {
        if (!NetClanBook.Shared.IsLeader(SelfObjectId()))
            return;
        Confirm("Cancel the clan dissolution?", () => ZoneCommands.Send(new ClanCancelDissolveRequestCommand()));
    }

    void Confirm(string text, System.Action ok)
    {
        if (SystemMessageWindow.Instance == null || ok == null)
            return;
        System.Action closed = null;
        System.Action accept = null;
        accept = () =>
        {
            SystemMessageWindow.Instance.OnButtonOk -= accept;
            SystemMessageWindow.Instance.OnButtonClosed -= closed;
            ok();
        };
        closed = () =>
        {
            SystemMessageWindow.Instance.OnButtonOk -= accept;
            SystemMessageWindow.Instance.OnButtonClosed -= closed;
        };
        SystemMessageWindow.Instance.OnButtonOk += accept;
        SystemMessageWindow.Instance.OnButtonClosed += closed;
        SystemMessageWindow.Instance.ShowWindowDialogYesOrNot(text);
    }

    int SelfObjectId()
    {
        IZoneTransport transport = ZoneCommands.Transport;
        if (transport != null && transport.PlayerInfo.Identity != null && transport.PlayerInfo.Identity.Id > 0)
            return transport.PlayerInfo.Identity.Id;
        NetSession net = NetSession.Instance;
        return net != null ? net.SessionObjectId : 0;
    }

    private void OnDestroy()
    {
        _instance = null; 
        _creatorTableWindows = null;
        _masterClan = null;
        _detailedClan = null;
        _dataProviderClanInfo = null;
    }
}
