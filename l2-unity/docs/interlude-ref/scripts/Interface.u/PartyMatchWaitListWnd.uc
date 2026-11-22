class PartyMatchWaitListWnd extends PartyMatchWndCommon;

var int entire_page;
var int current_page;
var int RoomNumber;
var int MaxPartyMemberCount;
var int MinLevel;
var int MaxLevel;
var int LootingMethodID;
var int RoomZoneID;

function OnLoad()
{
	RegisterEvent(1550);
	RegisterEvent(1610);
	RegisterEvent(1620);
	entire_page = 1;
	current_page = 1;
	return;
}

function OnShow()
{
	current_page = 1;
	return;
}

function OnEvent(int a_EventID, string param)
{
	switch(a_EventID)
	{
		// End:0x1D
		case 1550:
			HandlePartyMatchRoomStart(param);
			// End:0x4C
			break;
		// End:0x33
		case 1610:
			HandlePartyMatchWaitListStart(param);
			// End:0x4C
			break;
		// End:0x49
		case 1620:
			HandlePartyMatchWaitList(param);
			// End:0x4C
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function HandlePartyMatchRoomStart(string param)
{
	ParseInt(param, "RoomNum", RoomNumber);
	ParseInt(param, "MaxMember", MaxPartyMemberCount);
	ParseInt(param, "MinLevel", MinLevel);
	ParseInt(param, "MaxLevel", MaxLevel);
	ParseInt(param, "LootingMethodID", LootingMethodID);
	ParseInt(param, "ZoneID", RoomZoneID);
	return;
}

function HandlePartyMatchWaitListStart(string param)
{
	local int AllCount, Count;
	local string totalPages, currentPage, page_info;

	ParseInt(param, "AllCount", AllCount);
	ParseInt(param, "Count", Count);
	totalPages = string(__NFUN_146__(__NFUN_145__(AllCount, 64), 1));
	entire_page = __NFUN_146__(__NFUN_145__(AllCount, 64), 1);
	currentPage = string(current_page);
	page_info = __NFUN_112__(__NFUN_112__(currentPage, "/"), totalPages);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PartyMatchWaitListWnd.MemberCount", page_info);
	Class'NWindow.UIAPI_LISTCTRL'.static.DeleteAllItem("PartyMatchWaitListWnd.WaitListCtrl");
	CheckButtonAlive();
	return;
}

function HandlePartyMatchWaitList(string param)
{
	local string Name;
	local int ClassID, Level;
	local LVDataRecord Record;

	ParseString(param, "Name", Name);
	ParseInt(param, "ClassID", ClassID);
	ParseInt(param, "Level", Level);
	Record.LVDataList.Length = 3;
	Record.LVDataList[0].szData = Name;
	Record.LVDataList[1].szTexture = GetClassIconName(ClassID);
	Record.LVDataList[1].nTextureWidth = 11;
	Record.LVDataList[1].nTextureHeight = 11;
	Record.LVDataList[1].szData = string(ClassID);
	Record.LVDataList[2].szData = GetAmbiguousLevelString(Level, false);
	Class'NWindow.UIAPI_LISTCTRL'.static.InsertRecord("PartyMatchWaitListWnd.WaitListCtrl", Record);
	return;
}

function OnClickButton(string a_strButtonName)
{
	switch(a_strButtonName)
	{
		// End:0x22
		case "RefreshButton":
			OnRefreshButtonClick();
			// End:0xA4
			break;
		// End:0x3D
		case "WhisperButton":
			OnWhisperButtonClick();
			// End:0xA4
			break;
		// End:0x5C
		case "PartyInviteButton":
			OnInviteButtonClick();
			// End:0xA4
			break;
		// End:0x75
		case "CloseButton":
			OnCloseButtonClick();
			// End:0xA4
			break;
		// End:0x8B
		case "prev_btn":
			OnPrevbuttonClick();
			// End:0xA4
			break;
		// End:0xA1
		case "next_btn":
			OnNextbuttonClick();
			// End:0xA4
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnRefreshButtonClick()
{
	Class'NWindow.PartyMatchAPI'.static.RequestPartyMatchWaitList(current_page, MinLevel, MaxLevel, 0);
	return;
}

function OnNextbuttonClick()
{
	current_page = __NFUN_146__(current_page, 1);
	Class'NWindow.PartyMatchAPI'.static.RequestPartyMatchWaitList(current_page, MinLevel, MaxLevel, 0);
	return;
}

function OnPrevbuttonClick()
{
	current_page = __NFUN_147__(current_page, 1);
	Class'NWindow.PartyMatchAPI'.static.RequestPartyMatchWaitList(current_page, MinLevel, MaxLevel, 0);
	return;
}

function OnWhisperButtonClick()
{
	local LVDataRecord Record;
	local string szData1;

	Record = Class'NWindow.UIAPI_LISTCTRL'.static.GetSelectedRecord("PartyMatchWaitListWnd.WaitListCtrl");
	szData1 = Record.LVDataList[0].szData;
	// End:0x71
	if(__NFUN_123__(szData1, ""))
	{
		SetChatMessage(__NFUN_112__(__NFUN_112__("\"", szData1), " "));
	}
	return;
}

function OnInviteButtonClick()
{
	local LVDataRecord Record;

	Record = Class'NWindow.UIAPI_LISTCTRL'.static.GetSelectedRecord("PartyMatchWaitListWnd.WaitListCtrl");
	Class'NWindow.PartyMatchAPI'.static.RequestAskJoinPartyRoom(Record.LVDataList[0].szData);
	return;
}

function OnCloseButtonClick()
{
	local PartyMatchWnd script;

	script = PartyMatchWnd(GetScript("PartyMatchWnd"));
	// End:0x4A
	if(__NFUN_119__(script, none))
	{
		script.SetWaitListWnd(false);
		script.ShowHideWaitListWnd();
	}
	return;
}

function OnDBClickListCtrlRecord(string a_ListCtrlName)
{
	local LVDataRecord Record;

	// End:0x1A
	if(__NFUN_123__(a_ListCtrlName, "WaitListCtrl"))
	{
		return;
	}
	Record = Class'NWindow.UIAPI_LISTCTRL'.static.GetSelectedRecord("PartyMatchWaitListWnd.WaitListCtrl");
	SetChatMessage(__NFUN_112__(__NFUN_112__("\"", Record.LVDataList[0].szData), " "));
	return;
}

function CheckButtonAlive()
{
	Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("PartyMatchWaitListWnd.prev_btn");
	Class'NWindow.UIAPI_WINDOW'.static.EnableWindow("PartyMatchWaitListWnd.next_btn");
	// End:0x98
	if(__NFUN_154__(current_page, 1))
	{
		Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("PartyMatchWaitListWnd.prev_btn");
	}
	// End:0xD6
	if(__NFUN_154__(current_page, entire_page))
	{
		Class'NWindow.UIAPI_WINDOW'.static.DisableWindow("PartyMatchWaitListWnd.next_btn");
	}
	return;
}
