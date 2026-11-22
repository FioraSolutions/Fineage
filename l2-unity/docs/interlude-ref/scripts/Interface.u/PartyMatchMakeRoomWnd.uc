class PartyMatchMakeRoomWnd extends UIScript;

var int InviteState;
var int RoomNumber;
var string InvitedName;

function OnLoad()
{
	return;
}

function OnShow()
{
	// End:0x49
	if(__NFUN_154__(InviteState, 1))
	{
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PartyMatchMakeRoomWnd.TitletoDo", GetSystemString(1458));		
	}
	else
	{
		// End:0x93
		if(__NFUN_154__(InviteState, 2))
		{
			Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PartyMatchMakeRoomWnd.TitletoDo", GetSystemString(1460));			
		}
		else
		{
			Class'NWindow.UIAPI_TEXTBOX'.static.SetText("PartyMatchMakeRoomWnd.TitletoDo", GetSystemString(1457));
		}
	}
	return;
}

function OnClickButton(string a_strButtonName)
{
	switch(a_strButtonName)
	{
		// End:0x1D
		case "OKButton":
			OnOKButtonClick();
			// End:0x3A
			break;
		// End:0x37
		case "CancelButton":
			OnCancelButtonClick();
			// End:0x3A
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnOKButtonClick()
{
	local int MaxPartyMemberCount, MinLevel, MaxLevel;
	local string RoomTitle;

	MaxPartyMemberCount = __NFUN_146__(Class'NWindow.UIAPI_COMBOBOX'.static.GetSelectedNum("PartyMatchMakeRoomWnd.MaxPartyMemberCountComboBox"), 2);
	MinLevel = __NFUN_251__(int(Class'NWindow.UIAPI_EDITBOX'.static.GetString("PartyMatchMakeRoomWnd.MinLevelEditBox")), 1, 80);
	MaxLevel = __NFUN_251__(int(Class'NWindow.UIAPI_EDITBOX'.static.GetString("PartyMatchMakeRoomWnd.MaxLevelEditBox")), 1, 80);
	RoomTitle = Class'NWindow.UIAPI_EDITBOX'.static.GetString("PartyMatchMakeRoomWnd.TitleEditBox");
	Class'NWindow.PartyMatchAPI'.static.RequestManagePartyRoom(RoomNumber, MaxPartyMemberCount, MinLevel, MaxLevel, RoomTitle);
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("PartyMatchMakeRoomWnd");
	// End:0x17F
	if(__NFUN_154__(InviteState, 1))
	{
		Class'NWindow.PartyMatchAPI'.static.RequestAskJoinPartyRoom(InvitedName);
		InviteState = 0;
	}
	return;
}

function OnCancelButtonClick()
{
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("PartyMatchMakeRoomWnd");
	// End:0x38
	if(__NFUN_154__(InviteState, 1))
	{
		InviteState = 0;
	}
	return;
}

function SetRoomNumber(int a_RoomNumber)
{
	Debug(__NFUN_112__("PartyMatchMakeRoomWnd.SetRoomNumber ", string(a_RoomNumber)));
	RoomNumber = a_RoomNumber;
	return;
}

function SetTitle(string a_Title)
{
	Debug(__NFUN_112__("PartyMatchMakeRoomWnd.SetTitle ", a_Title));
	Class'NWindow.UIAPI_EDITBOX'.static.SetString("PartyMatchMakeRoomWnd.TitleEditBox", a_Title);
	return;
}

function SetMinLevel(int a_MinLevel)
{
	Debug(__NFUN_112__("PartyMatchMakeRoomWnd.SetMinLevel ", string(a_MinLevel)));
	Class'NWindow.UIAPI_EDITBOX'.static.SetString("PartyMatchMakeRoomWnd.MinLevelEditBox", string(a_MinLevel));
	return;
}

function SetMaxLevel(int a_MaxLevel)
{
	Debug(__NFUN_112__("PartyMatchMakeRoomWnd.SetMaxLevel ", string(a_MaxLevel)));
	Class'NWindow.UIAPI_EDITBOX'.static.SetString("PartyMatchMakeRoomWnd.MaxLevelEditBox", string(a_MaxLevel));
	return;
}

function SetMaxPartyMemberCount(int a_MaxPartyMemberCount)
{
	Debug(__NFUN_112__("PartyMatchMakeRoomWnd.SetMaxPartyMemberCount ", string(a_MaxPartyMemberCount)));
	Class'NWindow.UIAPI_COMBOBOX'.static.SetSelectedNum("PartyMatchMakeRoomWnd.MaxPartyMemberCountComboBox", __NFUN_147__(a_MaxPartyMemberCount, 2));
	return;
}
