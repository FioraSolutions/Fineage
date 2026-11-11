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

}
