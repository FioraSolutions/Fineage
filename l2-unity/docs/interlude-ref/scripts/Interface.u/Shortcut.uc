class Shortcut extends UIScript;

const CHAT_WINDOW_NORMAL = 0;
const CHAT_WINDOW_TRADE = 1;
const CHAT_WINDOW_PARTY = 2;
const CHAT_WINDOW_CLAN = 3;
const CHAT_WINDOW_ALLY = 4;
const CHAT_WINDOW_COUNT = 5;
const CHAT_WINDOW_SYSTEM = 5;

var bool m_chatstateok;

function OnLoad()
{
	RegisterEvent(90);
	return;
}

function OnEvent(int a_EventID, string a_Param)
{
	switch(a_EventID)
	{
		// End:0x1A
		case 90:
			HandleShortcutCommand(a_Param);
			// End:0x1D
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnExitState(name a_NextStateName)
{
	// End:0x1A
	if(__NFUN_254__(a_NextStateName, 'GamingState'))
	{
		m_chatstateok = true;		
	}
	else
	{
		m_chatstateok = false;
	}
	return;
}

function HandleShortcutCommand(string a_Param)
{
	local string Command;

	// End:0x98
	if(ParseString(a_Param, "Command", Command))
	{
		switch(Command)
		{
			// End:0x3F
			case "CloseAllWindow":
				HandleCloseAllWindow();
				// End:0x98
				break;
			// End:0x5B
			case "ShowChatWindow":
				HandleShowChatWindow();
				// End:0x98
				break;
			// End:0x78
			case "SetPrevChatType":
				HandleSetPrevChatType();
				// End:0x98
				break;
			// End:0x95
			case "SetNextChatType":
				HandleSetNextChatType();
				// End:0x98
				break;
			// End:0xFFFF
			default:
				break;
		}
	}
	return;
}

function HandleShowChatWindow()
{
	local WindowHandle Handle;

	Handle = GetHandle("ChatWnd");
	// End:0x73
	if(Handle.IsShowWindow())
	{
		Handle.HideWindow();
		// End:0x70
		if(GetOptionBool("Game", "SystemMsgWnd"))
		{
			Class'NWindow.UIAPI_WINDOW'.static.HideWindow("SystemMsgWnd");
		}		
	}
	else
	{
		Handle.ShowWindow();
		// End:0xBC
		if(GetOptionBool("Game", "SystemMsgWnd"))
		{
			Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("SystemMsgWnd");
		}
	}
	return;
}

function HandleSetPrevChatType()
{
	local ChatWnd chatWndScript;

	chatWndScript = ChatWnd(GetScript("ChatWnd"));
	switch(chatWndScript.m_chatType)
	{
		// End:0xD0
		case 0:
			chatWndScript.ChatTabCtrl.MergeTab(1);
			chatWndScript.ChatTabCtrl.MergeTab(2);
			chatWndScript.ChatTabCtrl.MergeTab(3);
			chatWndScript.ChatTabCtrl.MergeTab(4);
			chatWndScript.ChatTabCtrl.SetTopOrder(4, true);
			chatWndScript.HandleTabClick("ChatTabCtrl4");
			// End:0x369
			break;
		// End:0x175
		case 1:
			chatWndScript.ChatTabCtrl.MergeTab(0);
			chatWndScript.ChatTabCtrl.MergeTab(2);
			chatWndScript.ChatTabCtrl.MergeTab(3);
			chatWndScript.ChatTabCtrl.MergeTab(4);
			chatWndScript.ChatTabCtrl.SetTopOrder(0, true);
			chatWndScript.HandleTabClick("ChatTabCtrl0");
			// End:0x369
			break;
		// End:0x21A
		case 2:
			chatWndScript.ChatTabCtrl.MergeTab(0);
			chatWndScript.ChatTabCtrl.MergeTab(1);
			chatWndScript.ChatTabCtrl.MergeTab(3);
			chatWndScript.ChatTabCtrl.MergeTab(4);
			chatWndScript.ChatTabCtrl.SetTopOrder(1, true);
			chatWndScript.HandleTabClick("ChatTabCtrl1");
			// End:0x369
			break;
		// End:0x2C0
		case 3:
			chatWndScript.ChatTabCtrl.MergeTab(0);
			chatWndScript.ChatTabCtrl.MergeTab(1);
			chatWndScript.ChatTabCtrl.MergeTab(2);
			chatWndScript.ChatTabCtrl.MergeTab(3);
			chatWndScript.ChatTabCtrl.SetTopOrder(2, true);
			chatWndScript.HandleTabClick("ChatTabCtrl2");
			// End:0x369
			break;
		// End:0x366
		case 4:
			chatWndScript.ChatTabCtrl.MergeTab(0);
			chatWndScript.ChatTabCtrl.MergeTab(1);
			chatWndScript.ChatTabCtrl.MergeTab(2);
			chatWndScript.ChatTabCtrl.MergeTab(3);
			chatWndScript.ChatTabCtrl.SetTopOrder(3, true);
			chatWndScript.HandleTabClick("ChatTabCtrl3");
			// End:0x369
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function HandleSetNextChatType()
{
	local ChatWnd chatWndScript;

	chatWndScript = ChatWnd(GetScript("ChatWnd"));
	switch(chatWndScript.m_chatType)
	{
		// End:0xCF
		case 0:
			chatWndScript.ChatTabCtrl.MergeTab(0);
			chatWndScript.ChatTabCtrl.MergeTab(2);
			chatWndScript.ChatTabCtrl.MergeTab(3);
			chatWndScript.ChatTabCtrl.MergeTab(4);
			chatWndScript.ChatTabCtrl.SetTopOrder(1, true);
			chatWndScript.HandleTabClick("ChatTabCtrl1");
			// End:0x369
			break;
		// End:0x174
		case 1:
			chatWndScript.ChatTabCtrl.MergeTab(0);
			chatWndScript.ChatTabCtrl.MergeTab(1);
			chatWndScript.ChatTabCtrl.MergeTab(3);
			chatWndScript.ChatTabCtrl.MergeTab(4);
			chatWndScript.ChatTabCtrl.SetTopOrder(2, true);
			chatWndScript.HandleTabClick("ChatTabCtrl2");
			// End:0x369
			break;
		// End:0x21A
		case 2:
			chatWndScript.ChatTabCtrl.MergeTab(0);
			chatWndScript.ChatTabCtrl.MergeTab(1);
			chatWndScript.ChatTabCtrl.MergeTab(2);
			chatWndScript.ChatTabCtrl.MergeTab(4);
			chatWndScript.ChatTabCtrl.SetTopOrder(3, true);
			chatWndScript.HandleTabClick("ChatTabCtrl3");
			// End:0x369
			break;
		// End:0x2C0
		case 3:
			chatWndScript.ChatTabCtrl.MergeTab(0);
			chatWndScript.ChatTabCtrl.MergeTab(1);
			chatWndScript.ChatTabCtrl.MergeTab(2);
			chatWndScript.ChatTabCtrl.MergeTab(3);
			chatWndScript.ChatTabCtrl.SetTopOrder(4, true);
			chatWndScript.HandleTabClick("ChatTabCtrl4");
			// End:0x369
			break;
		// End:0x366
		case 4:
			chatWndScript.ChatTabCtrl.MergeTab(1);
			chatWndScript.ChatTabCtrl.MergeTab(2);
			chatWndScript.ChatTabCtrl.MergeTab(3);
			chatWndScript.ChatTabCtrl.MergeTab(4);
			chatWndScript.ChatTabCtrl.SetTopOrder(0, true);
			chatWndScript.HandleTabClick("ChatTabCtrl0");
			// End:0x369
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function HandleCloseAllWindow()
{
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("SystemMenuWnd");
	return;
}
