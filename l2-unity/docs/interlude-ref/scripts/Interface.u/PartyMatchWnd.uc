class PartyMatchWnd extends UIScript;

var int m_CurrentPageNum;
var int CompletelyQuitPartyMatching;
var bool bOpenStateLobby;

function OnLoad()
{
	RegisterEvent(1540);
	RegisterEvent(1570);
	RegisterEvent(1550);
	m_CurrentPageNum = 0;
	CompletelyQuitPartyMatching = 0;
	bOpenStateLobby = false;
	Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("PartyMatchWnd.LocationFilterComboBox", 1);
	Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("PartyMatchWnd.LevelFilterComboBox", 1);
	return;
}

function OnShow()
{
	Class'NWindow.UIAPI_LISTCTRL'.static.ShowScrollBar("PartyMatchWnd.PartyMatchListCtrl", false);
	return;
}

function OnSendPacketWhenHiding()
{
	Class'NWindow.PartyMatchAPI'.static.RequestExitPartyMatchingWaitingRoom();
	return;
}

function OnHide()
{
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("PartyMatchMakeRoomWnd");
	return;
}

function OnEvent(int a_EventID, string param)
{
	local PartyMatchMakeRoomWnd script;

	script = PartyMatchMakeRoomWnd(GetScript("PartyMatchMakeRoomWnd"));
	switch(a_EventID)
	{
		// End:0x12A
		case 1540:
			// End:0x8F
			if(__NFUN_154__(CompletelyQuitPartyMatching, 1))
			{
				Class'NWindow.PartyMatchAPI'.static.RequestExitPartyMatchingWaitingRoom();
				Class'NWindow.UIAPI_WINDOW'.static.HideWindow("PartyMatchWnd");
				script.OnCancelButtonClick();
				CompletelyQuitPartyMatching = 0;
				SetWaitListWnd(false);				
			}
			else
			{
				UpdateWaitListWnd();
				// End:0x109
				if(__NFUN_242__(Class'NWindow.UIAPI_WINDOW'.static.IsShowWindow("PartyMatchWnd"), false))
				{
					Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("PartyMatchWnd");
					Class'NWindow.UIAPI_LISTCTRL'.static.ShowScrollBar("PartyMatchWnd.PartyMatchListCtrl", false);
				}
				Class'NWindow.UIAPI_WINDOW'.static.SetFocus("PartyMatchWnd");
			}
			// End:0x16C
			break;
		// End:0x140
		case 1570:
			HandlePartyMatchList(param);
			// End:0x16C
			break;
		// End:0x169
		case 1550:
			Class'NWindow.UIAPI_WINDOW'.static.HideWindow("PartyMatchWnd");
			// End:0x16C
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnButtonTimer(bool bExpired)
{
	// End:0xAB
	if(bExpired)
	{
		Class'NWindow.UIAPI_BUTTON'.static.EnableWindow("PartyMatchWnd.PrevBtn");
		Class'NWindow.UIAPI_BUTTON'.static.EnableWindow("PartyMatchWnd.NextBtn");
		Class'NWindow.UIAPI_BUTTON'.static.EnableWindow("PartyMatchWnd.AutoJoinBtn");
		Class'NWindow.UIAPI_BUTTON'.static.EnableWindow("PartyMatchWnd.RefreshBtn");		
	}
	else
	{
		Class'NWindow.UIAPI_BUTTON'.static.DisableWindow("PartyMatchWnd.PrevBtn");
		Class'NWindow.UIAPI_BUTTON'.static.DisableWindow("PartyMatchWnd.NextBtn");
		Class'NWindow.UIAPI_BUTTON'.static.DisableWindow("PartyMatchWnd.AutoJoinBtn");
		Class'NWindow.UIAPI_BUTTON'.static.DisableWindow("PartyMatchWnd.RefreshBtn");
	}
	return;
}

function HandlePartyMatchList(string param)
{
	local int Count, i;
	local LVDataRecord Record;
	local int Number;
	local string PartyRoomName, PartyLeader;
	local int ZoneID, MinLevel, MaxLevel, MinMemberCnt, MaxMemberCnt;

	Class'NWindow.UIAPI_LISTCTRL'.static.DeleteAllItem("PartyMatchWnd.PartyMatchListCtrl");
	Record.LVDataList.Length = 6;
	ParseInt(param, "PageNum", m_CurrentPageNum);
	ParseInt(param, "RoomCount", Count);
	i = 0;
	J0x7A:

	// End:0x29C [Loop If]
	if(__NFUN_150__(i, Count))
	{
		ParseInt(param, __NFUN_112__("RoomNum_", string(i)), Number);
		ParseString(param, __NFUN_112__("Leader_", string(i)), PartyLeader);
		ParseInt(param, __NFUN_112__("ZoneID_", string(i)), ZoneID);
		ParseInt(param, __NFUN_112__("MinLevel_", string(i)), MinLevel);
		ParseInt(param, __NFUN_112__("MaxLevel_", string(i)), MaxLevel);
		ParseInt(param, __NFUN_112__("CurMember_", string(i)), MinMemberCnt);
		ParseInt(param, __NFUN_112__("MaxMember_", string(i)), MaxMemberCnt);
		ParseString(param, __NFUN_112__("RoomName_", string(i)), PartyRoomName);
		Record.LVDataList[0].szData = string(Number);
		Record.LVDataList[1].szData = PartyLeader;
		Record.LVDataList[2].szData = PartyRoomName;
		Record.LVDataList[3].szData = GetZoneNameWithZoneID(ZoneID);
		Record.LVDataList[4].szData = __NFUN_112__(__NFUN_112__(string(MinLevel), "-"), string(MaxLevel));
		Record.LVDataList[5].szData = __NFUN_112__(__NFUN_112__(string(MinMemberCnt), "/"), string(MaxMemberCnt));
		Class'NWindow.UIAPI_LISTCTRL'.static.InsertRecord("PartyMatchWnd.PartyMatchListCtrl", Record);
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x7A;
	}
	return;
}

function OnClickButton(string a_strButtonName)
{
	switch(a_strButtonName)
	{
		// End:0x1F
		case "RefreshBtn":
			OnRefreshBtnClick();
			// End:0x9A
			break;
		// End:0x34
		case "PrevBtn":
			OnPrevBtnClick();
			// End:0x9A
			break;
		// End:0x49
		case "NextBtn":
			OnNextBtnClick();
			// End:0x9A
			break;
		// End:0x62
		case "MakeRoomBtn":
			OnMakeRoomBtnClick();
			// End:0x9A
			break;
		// End:0x7B
		case "AutoJoinBtn":
			OnAutoJoinBtnClick();
			// End:0x9A
			break;
		// End:0x97
		case "WaitListButton":
			OnWaitListButton();
			// End:0x9A
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnWaitListButton()
{
	ToggleWaitListWnd();
	UpdateWaitListWnd();
	return;
}

function OnRefreshBtnClick()
{
	RequestPartyRoomListLocal(1);
	return;
}

function OnPrevBtnClick()
{
	local int WantedPageNum;

	// End:0x15
	if(__NFUN_153__(1, m_CurrentPageNum))
	{
		WantedPageNum = 1;		
	}
	else
	{
		WantedPageNum = __NFUN_147__(m_CurrentPageNum, 1);
	}
	RequestPartyRoomListLocal(WantedPageNum);
	return;
}

function OnNextBtnClick()
{
	RequestPartyRoomListLocal(__NFUN_146__(m_CurrentPageNum, 1));
	return;
}

function RequestPartyRoomListLocal(int a_Page)
{
	Class'NWindow.PartyMatchAPI'.static.RequestPartyRoomList(a_Page, GetLocationFilter(), GetLevelFilter());
	return;
}

function OnMakeRoomBtnClick()
{
	local PartyMatchMakeRoomWnd script;
	local UserInfo PlayerInfo;

	script = PartyMatchMakeRoomWnd(GetScript("PartyMatchMakeRoomWnd"));
	// End:0x106
	if(__NFUN_119__(script, none))
	{
		script.SetRoomNumber(0);
		script.SetTitle(GetSystemMessage(1398));
		script.SetMaxPartyMemberCount(12);
		// End:0x106
		if(GetPlayerInfo(PlayerInfo))
		{
			// End:0xB0
			if(__NFUN_151__(__NFUN_147__(PlayerInfo.nLevel, 5), 0))
			{
				script.SetMinLevel(__NFUN_147__(PlayerInfo.nLevel, 5));				
			}
			else
			{
				script.SetMinLevel(1);
			}
			// End:0xF5
			if(__NFUN_152__(__NFUN_146__(PlayerInfo.nLevel, 5), 80))
			{
				script.SetMaxLevel(__NFUN_146__(PlayerInfo.nLevel, 5));				
			}
			else
			{
				script.SetMaxLevel(80);
			}
		}
	}
	script.InviteState = 0;
	Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("PartyMatchMakeRoomWnd");
	Class'NWindow.UIAPI_WINDOW'.static.SetFocus("PartyMatchMakeRoomWnd");
	return;
}

function OnDBClickListCtrlRecord(string a_ListCtrlName)
{
	local int SelectedRecordIndex;
	local LVDataRecord Record;

	// End:0x20
	if(__NFUN_123__(a_ListCtrlName, "PartyMatchListCtrl"))
	{
		return;
	}
	SelectedRecordIndex = Class'NWindow.UIAPI_LISTCTRL'.static.GetSelectedIndex("PartyMatchWnd.PartyMatchListCtrl");
	Record = Class'NWindow.UIAPI_LISTCTRL'.static.GetRecord("PartyMatchWnd.PartyMatchListCtrl", SelectedRecordIndex);
	Class'NWindow.PartyMatchAPI'.static.RequestJoinPartyRoom(int(Record.LVDataList[0].szData));
	return;
}

function OnAutoJoinBtnClick()
{
	Class'NWindow.PartyMatchAPI'.static.RequestJoinPartyRoomAuto(m_CurrentPageNum, GetLocationFilter(), GetLevelFilter());
	return;
}

function int GetLocationFilter()
{
	return Class'NWindow.UIAPI_COMBOBOX'.static.GetReserved("PartyMatchWnd.LocationFilterComboBox", Class'NWindow.UIAPI_COMBOBOX'.static.GetSelectedNum("PartyMatchWnd.LocationFilterComboBox"));
	return;
}

function int GetLevelFilter()
{
	return Class'NWindow.UIAPI_COMBOBOX'.static.GetSelectedNum("PartyMatchWnd.LevelFilterComboBox");
	return;
}

function SetWaitListWnd(bool bShow)
{
	bOpenStateLobby = bShow;
	return;
}

function ShowHideWaitListWnd()
{
	// End:0x77
	if(bOpenStateLobby)
	{
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("PartyMatchWnd.PartyMatchOutWaitListWnd");
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("PartyMatchWnd.PartyMatchWaitListWnd");		
	}
	else
	{
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("PartyMatchWnd.PartyMatchOutWaitListWnd");
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("PartyMatchWnd.PartyMatchWaitListWnd");
	}
	return;
}

function UpdateWaitListWnd()
{
	local int MinLevel, MaxLevel;

	// End:0x98
	if(IsShowWaitListWnd())
	{
		MinLevel = int(Class'NWindow.UIAPI_EDITBOX'.static.GetString("PartyMatchOutWaitListWnd.MinLevel"));
		MaxLevel = int(Class'NWindow.UIAPI_EDITBOX'.static.GetString("PartyMatchOutWaitListWnd.MaxLevel"));
		Class'NWindow.PartyMatchAPI'.static.RequestPartyMatchWaitList(1, MinLevel, MaxLevel, 1);
	}
	return;
}

function ToggleWaitListWnd()
{
	bOpenStateLobby = __NFUN_129__(bOpenStateLobby);
	ShowHideWaitListWnd();
	return;
}

function bool IsShowWaitListWnd()
{
	return bOpenStateLobby;
	return;
}
