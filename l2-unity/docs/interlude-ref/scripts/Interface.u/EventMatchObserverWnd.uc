class EventMatchObserverWnd extends UICommonAPI;

const TIMERID_Show = 1;
const TIMERID_Msg = 2;

enum EMessageMode
{
	MESSAGEMODE_Normal,             // 0
	MESSAGEMODE_LeftRight,          // 1
	MESSAGEMODE_Off                 // 2
};

struct SkillMsgInfo
{
	var int AttackerTeamID;
	var int AttackerUserID;
	var string AttackerName;
	var int DefenderTeamID;
	var int DefenderUserID;
	var string DefenderName;
	var string SkillName;
};

var int m_Score1;
var int m_Score2;
var string m_TeamName1;
var string m_TeamName2;
var int m_SelectedUserID[2];
var bool m_ClassOrName;
var WindowHandle m_hTopWnd;
var WindowHandle m_hPlayerWnd[2];
var BarHandle m_hPlayerCPBar[2];
var BarHandle m_hPlayerHPBar[2];
var BarHandle m_hPlayerMPBar[2];
var TextBoxHandle m_hPlayerLvClassTextBox[2];
var TextBoxHandle m_hPlayerNameTextBox[2];
var WindowHandle m_hPlayerBuffCoverWnd[2];
var StatusIconHandle m_hPlayerBuffWnd[2];
var WindowHandle m_hParty1Wnd;
var WindowHandle m_hParty1MemberWnd[9];
var TextBoxHandle m_hParty1MemberNameTextBox[9];
var TextBoxHandle m_hParty1MemberClassTextBox[9];
var BarHandle m_hParty1MemberHPBar[9];
var BarHandle m_hParty1MemberCPBar[9];
var BarHandle m_hParty1MemberMPBar[9];
var WindowHandle m_hParty1MemberSelectedTex[9];
var WindowHandle m_hParty2Wnd;
var WindowHandle m_hParty2MemberWnd[9];
var TextBoxHandle m_hParty2MemberNameTextBox[9];
var TextBoxHandle m_hParty2MemberClassTextBox[9];
var BarHandle m_hParty2MemberHPBar[9];
var BarHandle m_hParty2MemberCPBar[9];
var BarHandle m_hParty2MemberMPBar[9];
var WindowHandle m_hParty2MemberSelectedTex[9];
var TextBoxHandle m_hTeamName1TextBox;
var TextBoxHandle m_hTeamName2TextBox;
var TextureHandle m_hScore1Tex;
var TextureHandle m_hScore2Tex;
var WindowHandle m_hMsgLeftWnd[6];
var TextBoxHandle m_hMsgLeftAttackerTextBox[6];
var TextBoxHandle m_hMsgLeftDefenderTextBox[6];
var TextBoxHandle m_hMsgLeftSkillTextBox[6];
var WindowHandle m_hMsgRightWnd[6];
var TextBoxHandle m_hMsgRightAttackerTextBox[6];
var TextBoxHandle m_hMsgRightDefenderTextBox[6];
var TextBoxHandle m_hMsgRightSkillTextBox[6];
var int m_Party1UserIDList[9];
var int m_Party2UserIDList[9];
var int m_MsgStartIndex;
var int m_Team1MsgStartIndex;
var int m_Team2MsgStartIndex;
var SkillMsgInfo m_MsgList[6];
var SkillMsgInfo m_Team1MsgList[6];
var SkillMsgInfo m_Team2MsgList[6];
var EventMatchObserverWnd.EMessageMode m_MsgMode;

function OnLoad()
{
	local int i;

	m_hTopWnd = GetHandle("TopWnd");
	m_hTeamName1TextBox = TextBoxHandle(GetHandle("TopWnd.TeamName1"));
	m_hTeamName2TextBox = TextBoxHandle(GetHandle("TopWnd.TeamName2"));
	m_hScore1Tex = TextureHandle(GetHandle("TopWnd.Score1Tex"));
	m_hScore2Tex = TextureHandle(GetHandle("TopWnd.Score2Tex"));
	i = 0;
	J0xA7:

	// End:0x287 [Loop If]
	if(__NFUN_150__(i, 2))
	{
		m_hPlayerWnd[i] = GetHandle(__NFUN_112__(__NFUN_112__("Player", string(__NFUN_146__(i, 1))), "Wnd"));
		m_hPlayerCPBar[i] = BarHandle(GetHandle(__NFUN_112__(__NFUN_112__("Player", string(__NFUN_146__(i, 1))), "Wnd.CPBar")));
		m_hPlayerHPBar[i] = BarHandle(GetHandle(__NFUN_112__(__NFUN_112__("Player", string(__NFUN_146__(i, 1))), "Wnd.HPBar")));
		m_hPlayerMPBar[i] = BarHandle(GetHandle(__NFUN_112__(__NFUN_112__("Player", string(__NFUN_146__(i, 1))), "Wnd.MPBar")));
		m_hPlayerLvClassTextBox[i] = TextBoxHandle(GetHandle(__NFUN_112__(__NFUN_112__("Player", string(__NFUN_146__(i, 1))), "Wnd.LvClassTextBox")));
		m_hPlayerNameTextBox[i] = TextBoxHandle(GetHandle(__NFUN_112__(__NFUN_112__("Player", string(__NFUN_146__(i, 1))), "Wnd.NameTextBox")));
		m_hPlayerBuffCoverWnd[i] = GetHandle(__NFUN_112__(__NFUN_112__("Player", string(__NFUN_146__(i, 1))), "BuffWnd"));
		m_hPlayerBuffWnd[i] = StatusIconHandle(GetHandle(__NFUN_112__(__NFUN_112__("Player", string(__NFUN_146__(i, 1))), "BuffWnd.StatusIconCtrl")));
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0xA7;
	}
	m_hParty1Wnd = GetHandle("Party1Wnd");
	i = 0;
	J0x2A5:

	// End:0x4A1 [Loop If]
	if(__NFUN_150__(i, 9))
	{
		m_hParty1MemberWnd[i] = GetHandle(__NFUN_112__(__NFUN_112__("Party1Wnd.PartyMember", string(__NFUN_146__(i, 1))), "Wnd"));
		m_hParty1MemberNameTextBox[i] = TextBoxHandle(GetHandle(__NFUN_112__(__NFUN_112__("Party1Wnd.PartyMember", string(__NFUN_146__(i, 1))), "Wnd.Name")));
		m_hParty1MemberClassTextBox[i] = TextBoxHandle(GetHandle(__NFUN_112__(__NFUN_112__("Party1Wnd.PartyMember", string(__NFUN_146__(i, 1))), "Wnd.Class")));
		m_hParty1MemberHPBar[i] = BarHandle(GetHandle(__NFUN_112__(__NFUN_112__("Party1Wnd.PartyMember", string(__NFUN_146__(i, 1))), "Wnd.HPBar")));
		m_hParty1MemberCPBar[i] = BarHandle(GetHandle(__NFUN_112__(__NFUN_112__("Party1Wnd.PartyMember", string(__NFUN_146__(i, 1))), "Wnd.CPBar")));
		m_hParty1MemberMPBar[i] = BarHandle(GetHandle(__NFUN_112__(__NFUN_112__("Party1Wnd.PartyMember", string(__NFUN_146__(i, 1))), "Wnd.MPBar")));
		m_hParty1MemberSelectedTex[i] = GetHandle(__NFUN_112__(__NFUN_112__("Party1Wnd.PartyMember", string(__NFUN_146__(i, 1))), "Wnd.SelectedTex"));
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x2A5;
	}
	m_hParty2Wnd = GetHandle("Party2Wnd");
	i = 0;
	J0x4BF:

	// End:0x6BB [Loop If]
	if(__NFUN_150__(i, 9))
	{
		m_hParty2MemberWnd[i] = GetHandle(__NFUN_112__(__NFUN_112__("Party2Wnd.PartyMember", string(__NFUN_146__(i, 1))), "Wnd"));
		m_hParty2MemberNameTextBox[i] = TextBoxHandle(GetHandle(__NFUN_112__(__NFUN_112__("Party2Wnd.PartyMember", string(__NFUN_146__(i, 1))), "Wnd.Name")));
		m_hParty2MemberClassTextBox[i] = TextBoxHandle(GetHandle(__NFUN_112__(__NFUN_112__("Party2Wnd.PartyMember", string(__NFUN_146__(i, 1))), "Wnd.Class")));
		m_hParty2MemberHPBar[i] = BarHandle(GetHandle(__NFUN_112__(__NFUN_112__("Party2Wnd.PartyMember", string(__NFUN_146__(i, 1))), "Wnd.HPBar")));
		m_hParty2MemberCPBar[i] = BarHandle(GetHandle(__NFUN_112__(__NFUN_112__("Party2Wnd.PartyMember", string(__NFUN_146__(i, 1))), "Wnd.CPBar")));
		m_hParty2MemberMPBar[i] = BarHandle(GetHandle(__NFUN_112__(__NFUN_112__("Party2Wnd.PartyMember", string(__NFUN_146__(i, 1))), "Wnd.MPBar")));
		m_hParty2MemberSelectedTex[i] = GetHandle(__NFUN_112__(__NFUN_112__("Party2Wnd.PartyMember", string(__NFUN_146__(i, 1))), "Wnd.SelectedTex"));
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x4BF;
	}
	i = 0;
	J0x6C2:

	// End:0x8D2 [Loop If]
	if(__NFUN_150__(i, 6))
	{
		m_hMsgLeftWnd[i] = GetHandle(__NFUN_112__("MsgWnd.MsgLeft.Msg", string(__NFUN_146__(i, 1))));
		m_hMsgRightWnd[i] = GetHandle(__NFUN_112__("MsgWnd.MsgRight.Msg", string(__NFUN_146__(i, 1))));
		m_hMsgLeftAttackerTextBox[i] = TextBoxHandle(GetHandle(__NFUN_112__(__NFUN_112__("MsgWnd.MsgLeft.Msg", string(__NFUN_146__(i, 1))), ".Attacker")));
		m_hMsgLeftDefenderTextBox[i] = TextBoxHandle(GetHandle(__NFUN_112__(__NFUN_112__("MsgWnd.MsgLeft.Msg", string(__NFUN_146__(i, 1))), ".Defender")));
		m_hMsgLeftSkillTextBox[i] = TextBoxHandle(GetHandle(__NFUN_112__(__NFUN_112__("MsgWnd.MsgLeft.Msg", string(__NFUN_146__(i, 1))), ".Skill")));
		m_hMsgRightAttackerTextBox[i] = TextBoxHandle(GetHandle(__NFUN_112__(__NFUN_112__("MsgWnd.MsgRight.Msg", string(__NFUN_146__(i, 1))), ".Attacker")));
		m_hMsgRightDefenderTextBox[i] = TextBoxHandle(GetHandle(__NFUN_112__(__NFUN_112__("MsgWnd.MsgRight.Msg", string(__NFUN_146__(i, 1))), ".Defender")));
		m_hMsgRightSkillTextBox[i] = TextBoxHandle(GetHandle(__NFUN_112__(__NFUN_112__("MsgWnd.MsgRight.Msg", string(__NFUN_146__(i, 1))), ".Skill")));
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x6C2;
	}
	RegisterEvent(2220);
	RegisterEvent(2240);
	RegisterEvent(2230);
	RegisterEvent(2250);
	RegisterEvent(2260);
	RegisterEvent(290);
	RegisterEvent(90);
	return;
}

function OnEnterState(name a_PreStateName)
{
	UpdateScore();
	UpdateTeamName();
	UpdateTeamInfo(0);
	UpdateTeamInfo(1);
	ClearMsg();
	return;
}

function OnEvent(int a_EventID, string a_Param)
{
	switch(a_EventID)
	{
		// End:0x1D
		case 2220:
			HandleStartEventMatchObserver(a_Param);
			// End:0x9A
			break;
		// End:0x2E
		case 2240:
			UpdateScore();
			// End:0x9A
			break;
		// End:0x3F
		case 2230:
			UpdateTeamName();
			// End:0x9A
			break;
		// End:0x55
		case 2250:
			HandleEventMatchUpdateTeamInfo(a_Param);
			// End:0x9A
			break;
		// End:0x6B
		case 2260:
			HandleEventMatchUpdateUserInfo(a_Param);
			// End:0x9A
			break;
		// End:0x81
		case 290:
			HandleReceiveMagicSkillUse(a_Param);
			// End:0x9A
			break;
		// End:0x94
		case 90:
			HandleShortcutCommand(a_Param);
			// End:0x9A
			break;
		// End:0xFFFF
		default:
			// End:0x9A
			break;
			break;
	}
	return;
}

function OnTimer(int a_TimerID)
{
	local int i;

	switch(a_TimerID)
	{
		// End:0x2D
		case 1:
			m_hOwnerWnd.KillTimer(1);
			m_hTopWnd.HideWindow();
			// End:0x129
			break;
		// End:0x126
		case 2:
			i = 0;
			J0x39:

			// End:0x123 [Loop If]
			if(__NFUN_150__(i, 6))
			{
				// End:0xAF
				if(__NFUN_130__(m_hMsgLeftWnd[i].IsShowWindow(), __NFUN_155__(0, m_hMsgLeftWnd[i].GetAlpha())))
				{
					m_hMsgLeftWnd[i].SetAlpha(255);
					m_hMsgLeftWnd[i].SetAlpha(0, 2.0000000);
					// [Explicit Break]
					goto J0x123;
				}
				// End:0x119
				if(__NFUN_130__(m_hMsgRightWnd[i].IsShowWindow(), __NFUN_155__(0, m_hMsgRightWnd[i].GetAlpha())))
				{
					m_hMsgRightWnd[i].SetAlpha(255);
					m_hMsgRightWnd[i].SetAlpha(0, 2.0000000);
					// [Explicit Break]
					goto J0x123;
				}
				__NFUN_163__(i);
				// [Loop Continue]
				goto J0x39;
			}
			J0x123:

			// End:0x129
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnLButtonDown(WindowHandle a_WindowHandle, int X, int Y)
{
	local int i;

	i = 0;
	J0x07:

	// End:0x48 [Loop If]
	if(__NFUN_150__(i, 9))
	{
		// End:0x3E
		if(a_WindowHandle.IsChildOf(m_hParty1MemberWnd[i]))
		{
			SetSelectedUser(0, i);
			return;
		}
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x07;
	}
	i = 0;
	J0x4F:

	// End:0x90 [Loop If]
	if(__NFUN_150__(i, 9))
	{
		// End:0x86
		if(a_WindowHandle.IsChildOf(m_hParty2MemberWnd[i]))
		{
			SetSelectedUser(1, i);
			return;
		}
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x4F;
	}
	return;
}

function HandleStartEventMatchObserver(string a_Param)
{
	return;
}

function HandleEventMatchUpdateTeamInfo(string a_Param)
{
	local int TeamID;

	// End:0x26
	if(ParseInt(a_Param, "TeamID", TeamID))
	{
		UpdateTeamInfo(TeamID);
	}
	return;
}

function HandleEventMatchUpdateUserInfo(string a_Param)
{
	local int UserID, TeamID;

	ParseInt(a_Param, "UserID", UserID);
	ParseInt(a_Param, "TeamID", TeamID);
	UpdateUserInfo(TeamID, UserID);
	return;
}

function HandleReceiveMagicSkillUse(string a_Param)
{
	local int AttackerID, DefenderID, SkillID;
	local UserInfo AttackerInfo, DefenderInfo;
	local SkillInfo UsedSkillInfo;
	local int AttackerTeamID, AttackerUserID, DefenderTeamID, DefenderUserID;

	// End:0x23
	if(__NFUN_129__(ParseInt(a_Param, "AttackerID", AttackerID)))
	{
		return;
	}
	// End:0x46
	if(__NFUN_129__(ParseInt(a_Param, "DefenderID", DefenderID)))
	{
		return;
	}
	// End:0x66
	if(__NFUN_129__(ParseInt(a_Param, "SkillID", SkillID)))
	{
		return;
	}
	// End:0x82
	if(__NFUN_129__(GetTeamUserID(AttackerID, AttackerTeamID, AttackerUserID)))
	{
		return;
	}
	// End:0x9E
	if(__NFUN_129__(GetTeamUserID(DefenderID, DefenderTeamID, DefenderUserID)))
	{
		return;
	}
	// End:0xB5
	if(__NFUN_129__(GetUserInfo(AttackerID, AttackerInfo)))
	{
		return;
	}
	// End:0xCC
	if(__NFUN_129__(GetUserInfo(DefenderID, DefenderInfo)))
	{
		return;
	}
	// End:0xE4
	if(__NFUN_129__(GetSkillInfo(SkillID, 1, UsedSkillInfo)))
	{
		return;
	}
	AddSkillMsg(AttackerTeamID, AttackerUserID, AttackerInfo.Name, DefenderTeamID, DefenderUserID, DefenderInfo.Name, UsedSkillInfo.SkillName);
	return;
}

function HandleShortcutCommand(string a_Param)
{
	local string Command;
	local bool Draggable;

	// End:0x3A6
	if(ParseString(a_Param, "Command", Command))
	{
		switch(Command)
		{
			// End:0xA3
			case "EventMatchShowPartyWindow":
				// End:0x74
				if(m_hParty1Wnd.IsShowWindow())
				{
					m_hParty1Wnd.HideWindow();
					m_hParty2Wnd.HideWindow();					
				}
				else
				{
					m_hParty1Wnd.ShowWindow();
					m_hParty2Wnd.ShowWindow();
					UpdateTeamInfo(0);
					UpdateTeamInfo(1);
				}
				// End:0x3A6
				break;
			// End:0x16B
			case "EventMatchLockPosition":
				Draggable = m_hPlayerWnd[0].IsDraggable();
				m_hPlayerWnd[0].SetDraggable(__NFUN_129__(Draggable));
				m_hPlayerWnd[1].SetDraggable(__NFUN_129__(Draggable));
				m_hPlayerBuffCoverWnd[0].SetDraggable(__NFUN_129__(Draggable));
				m_hPlayerBuffCoverWnd[1].SetDraggable(__NFUN_129__(Draggable));
				m_hParty1Wnd.SetDraggable(__NFUN_129__(Draggable));
				m_hParty2Wnd.SetDraggable(__NFUN_129__(Draggable));
				// End:0x3A6
				break;
			// End:0x2F9
			case "EventMatchInitPosition":
				m_hPlayerWnd[0].SetAnchor("", "TopLeft", "TopLeft", 0, 98);
				m_hPlayerWnd[0].ClearAnchor();
				m_hPlayerWnd[1].SetAnchor("", "TopRight", "TopRight", 0, 98);
				m_hPlayerWnd[1].ClearAnchor();
				m_hPlayerBuffCoverWnd[0].SetAnchor("Player1Wnd", "BottomLeft", "TopLeft", 0, 0);
				m_hPlayerBuffCoverWnd[0].ClearAnchor();
				m_hPlayerBuffCoverWnd[1].SetAnchor("Player2Wnd", "BottomLeft", "TopLeft", 0, 0);
				m_hPlayerBuffCoverWnd[1].ClearAnchor();
				m_hParty1Wnd.SetAnchor("", "TopLeft", "TopLeft", 0, 340);
				m_hParty1Wnd.ClearAnchor();
				m_hParty2Wnd.SetAnchor("", "TopRight", "TopRight", 0, 340);
				m_hParty2Wnd.ClearAnchor();
				// End:0x3A6
				break;
			// End:0x335
			case "EventMatchToggleShowClassOrName":
				m_ClassOrName = __NFUN_129__(m_ClassOrName);
				RefreshClassOrName();
				// End:0x3A6
				break;
			// End:0x3A3
			case "EventMatchSwitchMessageMode":
				switch(m_MsgMode)
				{
					// End:0x36C
					case 0:
						m_MsgMode = 1;
						// End:0x39A
						break;
					// End:0x37C
					case 1:
						m_MsgMode = 2;
						// End:0x39A
						break;
					// End:0x38C
					case 2:
						m_MsgMode = 0;
						// End:0x39A
						break;
					// End:0xFFFF
					default:
						m_MsgMode = 0;
						// End:0x39A
						break;
						break;
				}
				UpdateSkillMsg();
				// End:0x3A6
				break;
			// End:0xFFFF
			default:
				break;
		}
	}
	return;
}

function RefreshClassOrName()
{
	local int i;

	// End:0x7D
	if(m_ClassOrName)
	{
		i = 0;
		J0x10:

		// End:0x7A [Loop If]
		if(__NFUN_150__(i, 9))
		{
			m_hParty1MemberNameTextBox[i].HideWindow();
			m_hParty2MemberNameTextBox[i].HideWindow();
			m_hParty1MemberClassTextBox[i].ShowWindow();
			m_hParty2MemberClassTextBox[i].ShowWindow();
			__NFUN_163__(i);
			// [Loop Continue]
			goto J0x10;
		}		
	}
	else
	{
		i = 0;
		J0x84:

		// End:0xEE [Loop If]
		if(__NFUN_150__(i, 9))
		{
			m_hParty1MemberNameTextBox[i].ShowWindow();
			m_hParty2MemberNameTextBox[i].ShowWindow();
			m_hParty1MemberClassTextBox[i].HideWindow();
			m_hParty2MemberClassTextBox[i].HideWindow();
			__NFUN_163__(i);
			// [Loop Continue]
			goto J0x84;
		}
	}
	return;
}

function UpdateTeamName()
{
	m_TeamName1 = Class'NWindow.EventMatchAPI'.static.GetTeamName(0);
	m_TeamName2 = Class'NWindow.EventMatchAPI'.static.GetTeamName(1);
	m_hTeamName1TextBox.SetText(m_TeamName1);
	m_hTeamName2TextBox.SetText(m_TeamName2);
	return;
}

function UpdateTeamInfo(int a_TeamID)
{
	local int i, PartyMemberCount;

	// End:0x1A
	if(__NFUN_130__(__NFUN_155__(0, a_TeamID), __NFUN_155__(1, a_TeamID)))
	{
		return;
	}
	PartyMemberCount = Class'NWindow.EventMatchAPI'.static.GetPartyMemberCount(a_TeamID);
	switch(a_TeamID)
	{
		// End:0xD1
		case 0:
			m_hParty1Wnd.SetWindowSize(280, __NFUN_144__(70, PartyMemberCount));
			i = 0;
			J0x63:

			// End:0xCE [Loop If]
			if(__NFUN_150__(i, 9))
			{
				// End:0xA2
				if(__NFUN_150__(i, PartyMemberCount))
				{
					m_hParty1MemberWnd[i].ShowWindow();
					UpdateUserInfo(0, i);
					// [Explicit Continue]
					goto J0xC4;
				}
				m_hParty1MemberWnd[i].HideWindow();
				m_Party1UserIDList[i] = 0;
				J0xC4:

				__NFUN_163__(i);
				// [Loop Continue]
				goto J0x63;
			}
			// End:0x16A
			break;
		// End:0x167
		case 1:
			m_hParty2Wnd.SetWindowSize(280, __NFUN_144__(70, PartyMemberCount));
			i = 0;
			J0xF9:

			// End:0x164 [Loop If]
			if(__NFUN_150__(i, 9))
			{
				// End:0x138
				if(__NFUN_150__(i, PartyMemberCount))
				{
					m_hParty2MemberWnd[i].ShowWindow();
					UpdateUserInfo(1, i);
					// [Explicit Continue]
					goto J0x15A;
				}
				m_hParty2MemberWnd[i].HideWindow();
				m_Party2UserIDList[i] = 0;
				J0x15A:

				__NFUN_163__(i);
				// [Loop Continue]
				goto J0xF9;
			}
			// End:0x16A
			break;
		// End:0xFFFF
		default:
			break;
	}
	SetSelectedUser(a_TeamID, -1);
	RefreshClassOrName();
	return;
}

function UpdateScore()
{
	m_Score1 = Class'NWindow.EventMatchAPI'.static.GetScore(0);
	m_Score2 = Class'NWindow.EventMatchAPI'.static.GetScore(1);
	m_hScore1Tex.SetTexture(__NFUN_112__("L2UI_CH3.BroadcastObs.br_score", string(m_Score1)));
	m_hScore2Tex.SetTexture(__NFUN_112__("L2UI_CH3.BroadcastObs.br_score", string(m_Score2)));
	m_hTopWnd.ShowWindow();
	m_hOwnerWnd.SetTimer(1, 7000);
	return;
}

function UpdateUserInfo(int a_TeamID, int a_UserID)
{
	local int i, CurRow;
	local EventMatchUserData UserData;
	local StatusIconInfo Info;
	local SkillInfo TheSkillInfo;
	local int Width, Height;

	// End:0x48C
	if(Class'NWindow.EventMatchAPI'.static.GetUserData(a_TeamID, a_UserID, UserData))
	{
		switch(a_TeamID)
		{
			// End:0x104
			case 0:
				m_Party1UserIDList[a_UserID] = UserData.UserID;
				m_hParty1MemberNameTextBox[a_UserID].SetText(UserData.UserName);
				m_hParty1MemberClassTextBox[a_UserID].SetText(GetClassType(UserData.UserClass));
				m_hParty1MemberHPBar[a_UserID].SetValue(UserData.HPMax, UserData.HPNow);
				m_hParty1MemberCPBar[a_UserID].SetValue(UserData.CPMax, UserData.CPNow);
				m_hParty1MemberMPBar[a_UserID].SetValue(UserData.MPMax, UserData.MPNow);
				// End:0x1E3
				break;
			// End:0x1E0
			case 1:
				m_Party2UserIDList[a_UserID] = UserData.UserID;
				m_hParty2MemberNameTextBox[a_UserID].SetText(UserData.UserName);
				m_hParty2MemberClassTextBox[a_UserID].SetText(GetClassType(UserData.UserClass));
				m_hParty2MemberHPBar[a_UserID].SetValue(UserData.HPMax, UserData.HPNow);
				m_hParty2MemberCPBar[a_UserID].SetValue(UserData.CPMax, UserData.CPNow);
				m_hParty2MemberMPBar[a_UserID].SetValue(UserData.MPMax, UserData.MPNow);
				// End:0x1E3
				break;
			// End:0xFFFF
			default:
				break;
		}
		// End:0x48C
		if(IsSelectedUser(a_TeamID, a_UserID))
		{
			m_hPlayerNameTextBox[a_TeamID].SetText(UserData.UserName);
			m_hPlayerLvClassTextBox[a_TeamID].SetText(__NFUN_112__(__NFUN_112__(__NFUN_112__("Lv", string(UserData.UserLv)), " "), GetClassType(UserData.UserClass)));
			m_hPlayerHPBar[a_TeamID].SetValue(UserData.HPMax, UserData.HPNow);
			m_hPlayerCPBar[a_TeamID].SetValue(UserData.CPMax, UserData.CPNow);
			m_hPlayerMPBar[a_TeamID].SetValue(UserData.MPMax, UserData.MPNow);
			m_hPlayerBuffWnd[a_TeamID].Clear();
			CurRow = -1;
			Debug(__NFUN_112__("Length=", string(UserData.BuffIDList.Length)));
			i = 0;
			J0x313:

			// End:0x44E [Loop If]
			if(__NFUN_150__(i, UserData.BuffIDList.Length))
			{
				// End:0x35E
				if(__NFUN_180__(float(0), __NFUN_173__(float(i), float(12))))
				{
					m_hPlayerBuffWnd[a_TeamID].InsertRow(CurRow);
					__NFUN_165__(CurRow);
				}
				// End:0x444
				if(GetSkillInfo(UserData.BuffIDList[i], 1, TheSkillInfo))
				{
					Info.Size = 10;
					Info.ClassID = UserData.BuffIDList[i];
					Info.Level = 1;
					Info.RemainTime = UserData.BuffRemainList[i];
					Info.IconName = TheSkillInfo.TexName;
					Info.Name = TheSkillInfo.SkillName;
					Info.Description = TheSkillInfo.SkillDesc;
					Info.bShow = true;
					Info.Size = 40;
					m_hPlayerBuffWnd[a_TeamID].AddCol(CurRow, Info);
				}
				__NFUN_163__(i);
				// [Loop Continue]
				goto J0x313;
			}
			m_hPlayerBuffWnd[a_TeamID].GetWindowSize(Width, Height);
			m_hPlayerBuffCoverWnd[a_TeamID].SetWindowSize(Width, Height);
		}
	}
	return;
}

function SetSelectedUser(int a_TeamID, int a_UserID)
{
	local int i;

	switch(a_TeamID)
	{
		// End:0x67
		case 0:
			i = 0;
			J0x12:

			// End:0x64 [Loop If]
			if(__NFUN_150__(i, 9))
			{
				// End:0x45
				if(__NFUN_154__(i, a_UserID))
				{
					m_hParty1MemberSelectedTex[i].ShowWindow();
					// [Explicit Continue]
					goto J0x5A;
				}
				m_hParty1MemberSelectedTex[i].HideWindow();
				J0x5A:

				__NFUN_163__(i);
				// [Loop Continue]
				goto J0x12;
			}
			// End:0xCA
			break;
		// End:0xC7
		case 1:
			i = 0;
			J0x72:

			// End:0xC4 [Loop If]
			if(__NFUN_150__(i, 9))
			{
				// End:0xA5
				if(__NFUN_154__(i, a_UserID))
				{
					m_hParty2MemberSelectedTex[i].ShowWindow();
					// [Explicit Continue]
					goto J0xBA;
				}
				m_hParty2MemberSelectedTex[i].HideWindow();
				J0xBA:

				__NFUN_163__(i);
				// [Loop Continue]
				goto J0x72;
			}
			// End:0xCA
			break;
		// End:0xFFFF
		default:
			break;
	}
	m_SelectedUserID[a_TeamID] = a_UserID;
	// End:0x175
	if(__NFUN_154__(-1, a_UserID))
	{
		m_hPlayerNameTextBox[a_TeamID].SetText("");
		m_hPlayerLvClassTextBox[a_TeamID].SetText("");
		m_hPlayerHPBar[a_TeamID].SetValue(0, 0);
		m_hPlayerCPBar[a_TeamID].SetValue(0, 0);
		m_hPlayerMPBar[a_TeamID].SetValue(0, 0);
		m_hPlayerBuffCoverWnd[a_TeamID].HideWindow();		
	}
	else
	{
		Class'NWindow.EventMatchAPI'.static.SetSelectedUser(a_TeamID, a_UserID);
		m_hPlayerBuffCoverWnd[a_TeamID].ShowWindow();
		UpdateUserInfo(a_TeamID, a_UserID);
	}
	return;
}

function bool IsSelectedUser(int a_TeamID, int a_UserID)
{
	// End:0x1A
	if(__NFUN_130__(__NFUN_155__(0, a_TeamID), __NFUN_155__(1, a_TeamID)))
	{
		return false;
	}
	// End:0x31
	if(__NFUN_155__(m_SelectedUserID[a_TeamID], a_UserID))
	{
		return false;
	}
	return true;
	return;
}

function ClearMsg()
{
	local int i;

	i = 0;
	J0x07:

	// End:0x47 [Loop If]
	if(__NFUN_150__(i, 6))
	{
		m_hMsgLeftWnd[i].HideWindow();
		m_hMsgRightWnd[i].HideWindow();
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x07;
	}
	return;
}

function bool GetTeamUserID(int a_UserClassID, out int a_TeamID, out int a_UserID)
{
	local int i;

	// End:0x0D
	if(__NFUN_154__(0, a_UserClassID))
	{
		return false;
	}
	i = 0;
	J0x14:

	// End:0x53 [Loop If]
	if(__NFUN_150__(i, 9))
	{
		// End:0x49
		if(__NFUN_154__(m_Party1UserIDList[i], a_UserClassID))
		{
			a_TeamID = 0;
			a_UserID = i;
			return true;
		}
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x14;
	}
	i = 0;
	J0x5A:

	// End:0x99 [Loop If]
	if(__NFUN_150__(i, 9))
	{
		// End:0x8F
		if(__NFUN_154__(m_Party2UserIDList[i], a_UserClassID))
		{
			a_TeamID = 1;
			a_UserID = i;
			return true;
		}
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x5A;
	}
	return;
}

function AddSkillMsg(int a_AttackerTeamID, int a_AttackerUserID, string a_AttackerName, int a_DefenderTeamID, int a_DefenderUserID, string a_DefenderName, string a_SkillName)
{
	m_MsgList[m_MsgStartIndex].AttackerTeamID = a_AttackerTeamID;
	m_MsgList[m_MsgStartIndex].AttackerUserID = a_AttackerUserID;
	m_MsgList[m_MsgStartIndex].AttackerName = a_AttackerName;
	m_MsgList[m_MsgStartIndex].DefenderTeamID = a_DefenderTeamID;
	m_MsgList[m_MsgStartIndex].DefenderUserID = a_DefenderUserID;
	m_MsgList[m_MsgStartIndex].DefenderName = a_DefenderName;
	m_MsgList[m_MsgStartIndex].SkillName = a_SkillName;
	m_MsgStartIndex = int(__NFUN_173__(float(__NFUN_146__(m_MsgStartIndex, 1)), float(6)));
	// End:0x172
	if(__NFUN_154__(0, a_AttackerTeamID))
	{
		m_Team1MsgList[m_Team1MsgStartIndex].AttackerTeamID = a_AttackerTeamID;
		m_Team1MsgList[m_Team1MsgStartIndex].AttackerUserID = a_AttackerUserID;
		m_Team1MsgList[m_Team1MsgStartIndex].AttackerName = a_AttackerName;
		m_Team1MsgList[m_Team1MsgStartIndex].DefenderTeamID = a_DefenderTeamID;
		m_Team1MsgList[m_Team1MsgStartIndex].DefenderUserID = a_DefenderUserID;
		m_Team1MsgList[m_Team1MsgStartIndex].DefenderName = a_DefenderName;
		m_Team1MsgList[m_Team1MsgStartIndex].SkillName = a_SkillName;
		m_Team1MsgStartIndex = int(__NFUN_173__(float(__NFUN_146__(m_Team1MsgStartIndex, 1)), float(6)));		
	}
	else
	{
		m_Team2MsgList[m_Team2MsgStartIndex].AttackerTeamID = a_AttackerTeamID;
		m_Team2MsgList[m_Team2MsgStartIndex].AttackerUserID = a_AttackerUserID;
		m_Team2MsgList[m_Team2MsgStartIndex].AttackerName = a_AttackerName;
		m_Team2MsgList[m_Team2MsgStartIndex].DefenderTeamID = a_DefenderTeamID;
		m_Team2MsgList[m_Team2MsgStartIndex].DefenderUserID = a_DefenderUserID;
		m_Team2MsgList[m_Team2MsgStartIndex].DefenderName = a_DefenderName;
		m_Team2MsgList[m_Team2MsgStartIndex].SkillName = a_SkillName;
		m_Team2MsgStartIndex = int(__NFUN_173__(float(__NFUN_146__(m_Team2MsgStartIndex, 1)), float(6)));
	}
	UpdateSkillMsg();
	return;
}

function UpdateSkillMsg()
{
	local int i, SkillMsgIndex;
	local Color Team1Color, Team2Color;

	Team1Color.R = 220;
	Team1Color.G = 220;
	Team1Color.B = 220;
	Team2Color.R = byte(255);
	Team2Color.G = 55;
	Team2Color.B = 55;
	switch(m_MsgMode)
	{
		// End:0x25C
		case 0:
			i = 0;
			J0x63:

			// End:0x8E [Loop If]
			if(__NFUN_150__(i, 6))
			{
				m_hMsgRightWnd[i].HideWindow();
				__NFUN_163__(i);
				// [Loop Continue]
				goto J0x63;
			}
			i = 0;
			J0x95:

			// End:0x259 [Loop If]
			if(__NFUN_150__(i, 6))
			{
				SkillMsgIndex = int(__NFUN_173__(float(__NFUN_146__(m_MsgStartIndex, i)), float(6)));
				// End:0xEC
				if(__NFUN_122__(m_MsgList[SkillMsgIndex].AttackerName, ""))
				{
					m_hMsgLeftWnd[i].HideWindow();
					// [Explicit Continue]
					goto J0x24F;
				}
				m_hMsgLeftAttackerTextBox[i].SetText(__NFUN_112__(string(__NFUN_146__(m_MsgList[SkillMsgIndex].AttackerUserID, 1)), m_MsgList[SkillMsgIndex].AttackerName));
				m_hMsgLeftDefenderTextBox[i].SetText(__NFUN_112__(string(__NFUN_146__(m_MsgList[SkillMsgIndex].DefenderUserID, 1)), m_MsgList[SkillMsgIndex].DefenderName));
				m_hMsgLeftSkillTextBox[i].SetText(m_MsgList[SkillMsgIndex].SkillName);
				// End:0x1BC
				if(__NFUN_154__(0, m_MsgList[SkillMsgIndex].AttackerTeamID))
				{
					m_hMsgLeftAttackerTextBox[i].SetTextColor(Team1Color);					
				}
				else
				{
					m_hMsgLeftAttackerTextBox[i].SetTextColor(Team2Color);
				}
				// End:0x209
				if(__NFUN_154__(0, m_MsgList[SkillMsgIndex].DefenderTeamID))
				{
					m_hMsgLeftDefenderTextBox[i].SetTextColor(Team1Color);					
				}
				else
				{
					m_hMsgLeftDefenderTextBox[i].SetTextColor(Team2Color);
				}
				m_hMsgLeftWnd[i].ShowWindow();
				m_hMsgLeftWnd[i].SetAlpha(255);
				J0x24F:

				__NFUN_163__(i);
				// [Loop Continue]
				goto J0x95;
			}
			// End:0x59E
			break;
		// End:0x54C
		case 1:
			i = 0;
			J0x268:

			// End:0x3D5 [Loop If]
			if(__NFUN_150__(i, 6))
			{
				SkillMsgIndex = int(__NFUN_173__(float(__NFUN_146__(m_Team1MsgStartIndex, i)), float(6)));
				// End:0x2BF
				if(__NFUN_122__(m_Team1MsgList[SkillMsgIndex].AttackerName, ""))
				{
					m_hMsgLeftWnd[i].HideWindow();
					// [Explicit Continue]
					goto J0x3CB;
				}
				m_hMsgLeftAttackerTextBox[i].SetText(string(__NFUN_146__(m_Team1MsgList[SkillMsgIndex].AttackerUserID, 1)));
				m_hMsgLeftDefenderTextBox[i].SetText(string(__NFUN_146__(m_Team1MsgList[SkillMsgIndex].DefenderUserID, 1)));
				m_hMsgLeftSkillTextBox[i].SetText(m_Team1MsgList[SkillMsgIndex].SkillName);
				m_hMsgLeftAttackerTextBox[i].SetTextColor(Team1Color);
				// End:0x385
				if(__NFUN_154__(0, m_Team1MsgList[SkillMsgIndex].DefenderTeamID))
				{
					m_hMsgLeftDefenderTextBox[i].SetTextColor(Team1Color);					
				}
				else
				{
					m_hMsgLeftDefenderTextBox[i].SetTextColor(Team2Color);
				}
				m_hMsgLeftWnd[i].ShowWindow();
				m_hMsgLeftWnd[i].SetAlpha(255);
				J0x3CB:

				__NFUN_163__(i);
				// [Loop Continue]
				goto J0x268;
			}
			i = 0;
			J0x3DC:

			// End:0x549 [Loop If]
			if(__NFUN_150__(i, 6))
			{
				SkillMsgIndex = int(__NFUN_173__(float(__NFUN_146__(m_MsgStartIndex, i)), float(6)));
				// End:0x433
				if(__NFUN_122__(m_Team2MsgList[SkillMsgIndex].AttackerName, ""))
				{
					m_hMsgRightWnd[i].HideWindow();
					// [Explicit Continue]
					goto J0x53F;
				}
				m_hMsgRightAttackerTextBox[i].SetText(string(__NFUN_146__(m_Team2MsgList[SkillMsgIndex].AttackerUserID, 1)));
				m_hMsgRightDefenderTextBox[i].SetText(string(__NFUN_146__(m_Team2MsgList[SkillMsgIndex].DefenderUserID, 1)));
				m_hMsgRightSkillTextBox[i].SetText(m_Team2MsgList[SkillMsgIndex].SkillName);
				m_hMsgRightAttackerTextBox[i].SetTextColor(Team2Color);
				// End:0x4F9
				if(__NFUN_154__(0, m_Team2MsgList[SkillMsgIndex].DefenderTeamID))
				{
					m_hMsgRightDefenderTextBox[i].SetTextColor(Team1Color);					
				}
				else
				{
					m_hMsgRightDefenderTextBox[i].SetTextColor(Team2Color);
				}
				m_hMsgRightWnd[i].ShowWindow();
				m_hMsgRightWnd[i].SetAlpha(255);
				J0x53F:

				__NFUN_163__(i);
				// [Loop Continue]
				goto J0x3DC;
			}
			// End:0x59E
			break;
		// End:0x59B
		case 2:
			i = 0;
			J0x558:

			// End:0x598 [Loop If]
			if(__NFUN_150__(i, 6))
			{
				m_hMsgLeftWnd[i].HideWindow();
				m_hMsgRightWnd[i].HideWindow();
				__NFUN_163__(i);
				// [Loop Continue]
				goto J0x558;
			}
			// End:0x59E
			break;
		// End:0xFFFF
		default:
			break;
	}
	m_hOwnerWnd.KillTimer(2);
	m_hOwnerWnd.SetTimer(2, 14000);
	return;
}
