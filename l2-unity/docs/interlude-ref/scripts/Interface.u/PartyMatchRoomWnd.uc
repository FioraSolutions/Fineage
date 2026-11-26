class PartyMatchRoomWnd extends PartyMatchWndCommon;

var int RoomNumber;
var int CurPartyMemberCount;
var int MaxPartyMemberCount;
var int MinLevel;
var int MaxLevel;
var int LootingMethodID;
var int RoomZoneID;
var int MyMembershipType;
var string RoomTitle;
var bool m_bPartyMatchRoomStart;
var bool m_bRequestExitPartyRoom;

function OnLoad()
{
	RegisterEvent(40);
	RegisterEvent(1550);
	RegisterEvent(1560);
	RegisterEvent(1580);
	RegisterEvent(1590);
	RegisterEvent(1600);
	RegisterEvent(1630);
	m_bPartyMatchRoomStart = false;
	m_bRequestExitPartyRoom = false;
	return;
}

function OnSendPacketWhenHiding()
{
	local PartyMatchWnd script;

	script = PartyMatchWnd(GetScript("PartyMatchWnd"));
	// End:0x5A
	if(__NFUN_119__(script, none))
	{
		script.CompletelyQuitPartyMatching = 1;
		script.SetWaitListWnd(false);
		script.ShowHideWaitListWnd();
	}
	ExitPartyRoom();
	return;
}

function OnEnterState(name a_PreStateName)
{
	// End:0x4D
	if(m_bPartyMatchRoomStart)
	{
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("PartyMatchRoomWnd");
		Class'NWindow.UIAPI_WINDOW'.static.SetFocus("PartyMatchRoomWnd");
	}
	return;
}

function OnEvent(int a_EventID, string param)
{
	switch(a_EventID)
	{
		// End:0x7B
		case 1630:
			// End:0x56
			if(Class'NWindow.UIAPI_WINDOW'.static.IsMinimizedWindow("PartyMatchRoomWnd"))
			{
				Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("PartyMatchRoomWnd");
			}
			Class'NWindow.UIAPI_WINDOW'.static.SetFocus("PartyMatchRoomWnd");
			// End:0xF5
			break;
		// End:0x91
		case 1550:
			HandlePartyMatchRoomStart(param);
			// End:0xF5
			break;
		// End:0xA2
		case 1560:
			HandlePartyMatchRoomClose();
			// End:0xF5
			break;
		// End:0xB8
		case 1580:
			HandlePartyMatchRoomMember(param);
			// End:0xF5
			break;
		// End:0xCE
		case 1590:
			HandlePartyMatchRoomMemberUpdate(param);
			// End:0xF5
			break;
		// End:0xE4
		case 1600:
			HandlePartyMatchChatMessage(param);
			// End:0xF5
			break;
		// End:0xF2
		case 40:
			HandleRestart();
			// End:0xF5
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function HandleRestart()
{
	m_bPartyMatchRoomStart = false;
	return;
}

function ExitPartyRoom()
{
	m_bRequestExitPartyRoom = true;
	switch(MyMembershipType)
	{
		// End:0x13
		case 0:
		// End:0x2F
		case 2:
			Class'NWindow.PartyMatchAPI'.static.RequestWithdrawPartyRoom(RoomNumber);
			// End:0x4D
			break;
		// End:0x4A
		case 1:
			Class'NWindow.PartyMatchAPI'.static.RequestDismissPartyRoom(RoomNumber);
			// End:0x4D
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function HandlePartyMatchRoomStart(string param)
{
	local Rect rectWnd;

	ParseInt(param, "RoomNum", RoomNumber);
	ParseInt(param, "MaxMember", MaxPartyMemberCount);
	ParseInt(param, "MinLevel", MinLevel);
	ParseInt(param, "MaxLevel", MaxLevel);
	ParseInt(param, "LootingMethodID", LootingMethodID);
	ParseInt(param, "ZoneID", RoomZoneID);
	ParseString(param, "RoomName", RoomTitle);
	UpdateData(true);
	m_bPartyMatchRoomStart = true;
	Class'NWindow.UIAPI_TEXTLISTBOX'.static.Clear("PartyMatchRoomWnd.PartyRoomChatWindow");
	// End:0x14A
	if(Class'NWindow.UIAPI_WINDOW'.static.IsMinimizedWindow("PartyMatchRoomWnd"))
	{
		Class'NWindow.UIAPI_WINDOW'.static.NotifyAlarm("PartyMatchRoomWnd");		
	}
	else
	{
		rectWnd = Class'NWindow.UIAPI_WINDOW'.static.GetRect("PartyMatchWnd");
		Class'NWindow.UIAPI_WINDOW'.static.MoveTo("PartyMatchRoomWnd", rectWnd.nX, rectWnd.nY);
		UpdateWaitListWnd();
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("PartyMatchRoomWnd");
		Class'NWindow.UIAPI_WINDOW'.static.SetFocus("PartyMatchRoomWnd");
	}
	return;
}

function UpdateWaitListWnd()
{
	local PartyMatchWnd script;

	script = PartyMatchWnd(GetScript("PartyMatchWnd"));
	// End:0x58
	if(__NFUN_119__(script, none))
	{
		// End:0x58
		if(script.IsShowWaitListWnd())
		{
			Class'NWindow.PartyMatchAPI'.static.RequestPartyMatchWaitList(1, MinLevel, MaxLevel, 0);
		}
	}
	return;
}

function OnHide()
{
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("PartyMatchMakeRoomWnd");
	return;
}

function HandlePartyMatchRoomClose()
{
	local PartyMatchWnd script;
	local PartyMatchMakeRoomWnd Script2;

	m_bPartyMatchRoomStart = false;
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("PartyMatchRoomWnd");
	// End:0xAF
	if(m_bRequestExitPartyRoom)
	{
		script = PartyMatchWnd(GetScript("PartyMatchWnd"));
		// End:0x6D
		if(__NFUN_119__(script, none))
		{
			script.OnRefreshBtnClick();
		}
		Script2 = PartyMatchMakeRoomWnd(GetScript("PartyMatchMakeRoomWnd"));
		// End:0xAF
		if(__NFUN_119__(Script2, none))
		{
			Script2.OnCancelButtonClick();
		}
	}
	m_bRequestExitPartyRoom = false;
	return;
}

function UpdateMyMembershipType()
{
	switch(MyMembershipType)
	{
		// End:0x0B
		case 0:
		// End:0xCF
		case 2:
			Class'NWindow.UIAPI_BUTTON'.static.DisableWindow("PartyMatchRoomWnd.RoomSettingButton");
			Class'NWindow.UIAPI_BUTTON'.static.DisableWindow("PartyMatchRoomWnd.BanButton");
			Class'NWindow.UIAPI_BUTTON'.static.DisableWindow("PartyMatchRoomWnd.InviteButton");
			Class'NWindow.UIAPI_BUTTON'.static.EnableWindow("PartyMatchRoomWnd.ExitButton");
			// End:0x195
			break;
		// End:0x192
		case 1:
			Class'NWindow.UIAPI_BUTTON'.static.EnableWindow("PartyMatchRoomWnd.RoomSettingButton");
			Class'NWindow.UIAPI_BUTTON'.static.EnableWindow("PartyMatchRoomWnd.BanButton");
			Class'NWindow.UIAPI_BUTTON'.static.EnableWindow("PartyMatchRoomWnd.InviteButton");
			Class'NWindow.UIAPI_BUTTON'.static.EnableWindow("PartyMatchRoomWnd.ExitButton");
			// End:0x195
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function HandlePartyMatchRoomMember(string param)
{
	local int i, ClassID, Level, MemberID;
	local string memberName;
	local int ZoneID, MembershipType;
	local PartyMatchWaitListWnd script;

	script = PartyMatchWaitListWnd(GetScript("PartyMatchWaitListWnd"));
	ParseInt(param, "MyMembershipType", MyMembershipType);
	UpdateMyMembershipType();
	Class'NWindow.UIAPI_LISTCTRL'.static.DeleteAllItem("PartyMatchRoomWnd.PartyMemberListCtrl");
	ParseInt(param, "MemberCount", CurPartyMemberCount);
	i = 0;
	J0xAA:

	// End:0x1C1 [Loop If]
	if(__NFUN_150__(i, CurPartyMemberCount))
	{
		ParseInt(param, __NFUN_112__("MemberID_", string(i)), MemberID);
		ParseString(param, __NFUN_112__("MemberName_", string(i)), memberName);
		ParseInt(param, __NFUN_112__("ClassID_", string(i)), ClassID);
		ParseInt(param, __NFUN_112__("Level_", string(i)), Level);
		ParseInt(param, __NFUN_112__("ZoneID_", string(i)), ZoneID);
		ParseInt(param, __NFUN_112__("MembershipType_", string(i)), MembershipType);
		AddMember(MemberID, memberName, ClassID, Level, ZoneID, MembershipType);
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0xAA;
	}
	UpdateData(true);
	// End:0x20F
	if(Class'NWindow.UIAPI_WINDOW'.static.IsMinimizedWindow("PartyMatchRoomWnd"))
	{
		Class'NWindow.UIAPI_WINDOW'.static.NotifyAlarm("PartyMatchRoomWnd");
	}
	// End:0x25C
	if(__NFUN_242__(Class'NWindow.UIAPI_WINDOW'.static.IsShowWindow("PartyMatchRoomWnd.PartyMatchWaitListWnd"), true))
	{
		script.OnRefreshButtonClick();
	}
	return;
}

function AddMember(int a_MemberID, string a_MemberName, int a_ClassID, int a_Level, int a_ZoneID, int a_MembershipType)
{
	local LVDataRecord Record;

	Record.LVDataList.Length = 5;
	Record.LVDataList[0].nReserved1 = a_MemberID;
	Record.LVDataList[0].szData = a_MemberName;
	Record.LVDataList[1].szData = string(a_ClassID);
	Record.LVDataList[1].szTexture = GetClassIconName(a_ClassID);
	Record.LVDataList[1].nTextureWidth = 11;
	Record.LVDataList[1].nTextureHeight = 11;
	Record.LVDataList[2].szData = GetAmbiguousLevelString(a_Level, true);
	Record.LVDataList[3].szData = GetZoneNameWithZoneID(a_ZoneID);
	switch(a_MembershipType)
	{
		// End:0x103
		case 0:
			Record.LVDataList[4].szData = GetSystemString(1061);
			// End:0x151
			break;
		// End:0x128
		case 1:
			Record.LVDataList[4].szData = GetSystemString(1062);
			// End:0x151
			break;
		// End:0x14E
		case 2:
			Record.LVDataList[4].szData = GetSystemString(1063);
			// End:0x151
			break;
		// End:0xFFFF
		default:
			break;
	}
	Class'NWindow.UIAPI_LISTCTRL'.static.InsertRecord("PartyMatchRoomWnd.PartyMemberListCtrl", Record);
	return;
}

function RemoveMember(int a_MemberID)
{
	local int RecordCount, i;
	local LVDataRecord Record;

	RecordCount = Class'NWindow.UIAPI_LISTCTRL'.static.GetRecordCount("PartyMatchRoomWnd.PartyMemberListCtrl");
	i = 0;
	J0x43:

	// End:0xF6 [Loop If]
	if(__NFUN_150__(i, RecordCount))
	{
		Record = Class'NWindow.UIAPI_LISTCTRL'.static.GetRecord("PartyMatchRoomWnd.PartyMemberListCtrl", i);
		// End:0xEC
		if(__NFUN_154__(Record.LVDataList[0].nReserved1, a_MemberID))
		{
			Class'NWindow.UIAPI_LISTCTRL'.static.DeleteRecord("PartyMatchRoomWnd.PartyMemberListCtrl", i);
			// [Explicit Break]
			goto J0xF6;
		}
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x43;
	}
	J0xF6:

	return;
}

function HandlePartyMatchRoomMemberUpdate(string param)
{
	local int UpdateType, MemberID;
	local string memberName;
	local int ClassID, Level, ZoneID, MembershipType;
	local UserInfo PlayerInfo;
	local PartyMatchWaitListWnd script;

	script = PartyMatchWaitListWnd(GetScript("PartyMatchWaitListWnd"));
	ParseInt(param, "UpdateType", UpdateType);
	ParseInt(param, "MemberID", MemberID);
	switch(UpdateType)
	{
		// End:0x122
		case 0:
			ParseString(param, "MemberName", memberName);
			ParseInt(param, "ClassID", ClassID);
			ParseInt(param, "Level", Level);
			ParseInt(param, "ZoneID", ZoneID);
			ParseInt(param, "MembershipType", MembershipType);
			AddMember(MemberID, memberName, ClassID, Level, ZoneID, MembershipType);
			CurPartyMemberCount = __NFUN_146__(CurPartyMemberCount, 1);
			// End:0x21C
			break;
		// End:0x1F8
		case 1:
			ParseString(param, "MemberName", memberName);
			ParseInt(param, "ClassID", ClassID);
			ParseInt(param, "Level", Level);
			ParseInt(param, "ZoneID", ZoneID);
			ParseInt(param, "MembershipType", MembershipType);
			RemoveMember(MemberID);
			CurPartyMemberCount = __NFUN_147__(CurPartyMemberCount, 1);
			AddMember(MemberID, memberName, ClassID, Level, ZoneID, MembershipType);
			CurPartyMemberCount = __NFUN_146__(CurPartyMemberCount, 1);
			// End:0x21C
			break;
		// End:0x219
		case 2:
			RemoveMember(MemberID);
			CurPartyMemberCount = __NFUN_147__(CurPartyMemberCount, 1);
			// End:0x21C
			break;
		// End:0xFFFF
		default:
			break;
	}
	// End:0x24F
	if(GetPlayerInfo(PlayerInfo))
	{
		// End:0x24F
		if(__NFUN_154__(PlayerInfo.nID, MemberID))
		{
			MyMembershipType = MembershipType;
			UpdateMyMembershipType();
		}
	}
	// End:0x296
	if(Class'NWindow.UIAPI_WINDOW'.static.IsMinimizedWindow("PartyMatchRoomWnd"))
	{
		Class'NWindow.UIAPI_WINDOW'.static.NotifyAlarm("PartyMatchRoomWnd");
	}
	UpdateData(true);
	// End:0x2EA
	if(__NFUN_242__(Class'NWindow.UIAPI_WINDOW'.static.IsShowWindow("PartyMatchRoomWnd.PartyMatchWaitListWnd"), true))
	{
		script.OnRefreshButtonClick();
	}
	return;
}

function HandlePartyMatchChatMessage(string param)
{
	local int tmp;
	local Color ChatColor;
	local string chatMessage;

	ParseString(param, "Msg", chatMessage);
	ParseInt(param, "ColorR", tmp);
	ChatColor.R = byte(tmp);
	ParseInt(param, "ColorG", tmp);
	ChatColor.G = byte(tmp);
	ParseInt(param, "ColorB", tmp);
	ChatColor.B = byte(tmp);
	ParseInt(param, "ColorA", tmp);
	ChatColor.A = byte(tmp);
	Class'NWindow.UIAPI_TEXTLISTBOX'.static.AddString("PartyMatchRoomWnd.PartyRoomChatWindow", chatMessage, ChatColor);
	// End:0x144
	if(Class'NWindow.UIAPI_WINDOW'.static.IsMinimizedWindow("PartyMatchRoomWnd"))
	{
		Class'NWindow.UIAPI_WINDOW'.static.NotifyAlarm("PartyMatchRoomWnd");
	}
	return;
}

function UpdateData(bool a_ToControl)
{
	// End:0x169
	if(a_ToControl)
	{
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PartyMatchRoomWnd.RoomNumber", string(RoomNumber));
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PartyMatchRoomWnd.RoomTitle", RoomTitle);
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PartyMatchRoomWnd.Location", GetZoneNameWithZoneID(RoomZoneID));
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PartyMatchRoomWnd.PartyMemberCount", __NFUN_112__(__NFUN_112__(string(CurPartyMemberCount), "/"), string(MaxPartyMemberCount)));
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PartyMatchRoomWnd.LootingMethod", GetLootingMethodName(LootingMethodID));
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PartyMatchRoomWnd.LevelLimit", __NFUN_112__(__NFUN_112__(string(MinLevel), "-"), string(MaxLevel)));
	}
	return;
}

function OnClickButton(string a_strButtonName)
{
	switch(a_strButtonName)
	{
		// End:0x23
		case "WaitListButton":
			OnWaitListButtonClick();
			// End:0x8E
			break;
		// End:0x42
		case "RoomSettingButton":
			OnRoomSettingButtonClick();
			// End:0x8E
			break;
		// End:0x59
		case "BanButton":
			OnBanButtonClick();
			// End:0x8E
			break;
		// End:0x73
		case "InviteButton":
			OnInviteButtonClick();
			// End:0x8E
			break;
		// End:0x8B
		case "ExitButton":
			OnExitButtonClick();
			// End:0x8E
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnWaitListButtonClick()
{
	local PartyMatchWnd script;

	script = PartyMatchWnd(GetScript("PartyMatchWnd"));
	// End:0x40
	if(__NFUN_119__(script, none))
	{
		script.ToggleWaitListWnd();
		UpdateWaitListWnd();
	}
	return;
}

function OnRoomSettingButtonClick()
{
	local PartyMatchMakeRoomWnd script;

	script = PartyMatchMakeRoomWnd(GetScript("PartyMatchMakeRoomWnd"));
	// End:0xA8
	if(__NFUN_119__(script, none))
	{
		script.InviteState = 2;
		script.SetRoomNumber(RoomNumber);
		script.SetTitle(RoomTitle);
		script.SetMaxPartyMemberCount(MaxPartyMemberCount);
		script.SetMinLevel(MinLevel);
		script.SetMaxLevel(MaxLevel);
	}
	Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("PartyMatchMakeRoomWnd");
	Class'NWindow.UIAPI_WINDOW'.static.SetFocus("PartyMatchMakeRoomWnd");
	return;
}

function OnBanButtonClick()
{
	local LVDataRecord Record;

	Record = Class'NWindow.UIAPI_LISTCTRL'.static.GetSelectedRecord("PartyMatchRoomWnd.PartyMemberListCtrl");
	Class'NWindow.PartyMatchAPI'.static.RequestBanFromPartyRoom(Record.LVDataList[0].nReserved1);
	return;
}

function OnInviteButtonClick()
{
	local LVDataRecord Record;

	Record = Class'NWindow.UIAPI_LISTCTRL'.static.GetSelectedRecord("PartyMatchRoomWnd.PartyMemberListCtrl");
	RequestInviteParty(Record.LVDataList[0].szData);
	return;
}

function OnExitButtonClick()
{
	ExitPartyRoom();
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("PartyMatchRoomWnd");
	return;
}

function OnCompleteEditBox(string strID)
{
	local string ChatMsg;

	// End:0xA1
	if(__NFUN_122__(strID, "PartyRoomChatEditBox"))
	{
		ChatMsg = Class'NWindow.UIAPI_EDITBOX'.static.GetString("PartyMatchRoomWnd.PartyRoomChatEditBox");
		ProcessPartyMatchChatMessage(ChatMsg);
		Class'NWindow.UIAPI_EDITBOX'.static.SetString("PartyMatchRoomWnd.PartyRoomChatEditBox", "");
	}
	return;
}

function OnChatMarkedEditBox(string strID)
{
	local Color ChatColor;

	// End:0x9C
	if(__NFUN_122__(strID, "PartyRoomChatEditBox"))
	{
		ChatColor.R = 176;
		ChatColor.G = 155;
		ChatColor.B = 121;
		ChatColor.A = byte(255);
		Class'NWindow.UIAPI_TEXTLISTBOX'.static.AddString("PartyMatchRoomWnd.PartyRoomChatWindow", GetSystemMessage(966), ChatColor);
	}
	return;
}
