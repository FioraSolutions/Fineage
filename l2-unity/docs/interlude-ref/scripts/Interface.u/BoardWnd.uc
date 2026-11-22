class BoardWnd extends UIScript;

var bool m_bShow;
var bool m_bBtnLock;
var string m_Command[8];

function OnLoad()
{
	RegisterEvent(1190);
	RegisterEvent(1200);
	m_bShow = false;
	m_bBtnLock = false;
	return;
}

function OnShow()
{
	m_bShow = true;
	return;
}

function OnHide()
{
	m_bShow = false;
	return;
}

function OnEvent(int Event_ID, string param)
{
	// End:0x1D
	if(__NFUN_154__(Event_ID, 1190))
	{
		HandleShowBBS(param);		
	}
	else
	{
		// End:0x37
		if(__NFUN_154__(Event_ID, 1200))
		{
			HandleShowBoardPacket(param);
		}
	}
	return;
}

function OnClickButton(string strID)
{
	switch(strID)
	{
		// End:0x20
		case "btnBookmark":
			OnClickBookmark();
			// End:0x23
			break;
		// End:0xFFFF
		default:
			break;
	}
	// End:0x74
	if(__NFUN_122__(__NFUN_128__(strID, 7), "TabCtrl"))
	{
		strID = __NFUN_127__(strID, 7);
		// End:0x74
		if(__NFUN_129__(Class'NWindow.UIAPI_WINDOW'.static.IsMinimizedWindow("BoardWnd")))
		{
			ShowBBSTab(int(strID));
		}
	}
	return;
}

function Clear()
{
	return;
}

function HandleShowBBS(string param)
{
	local int Index, Init;

	ParseInt(param, "Index", Index);
	ParseInt(param, "Init", Init);
	// End:0x105
	if(__NFUN_151__(Init, 0))
	{
		// End:0x67
		if(m_bShow)
		{
			PlayConsoleSound(6);
			Class'NWindow.UIAPI_WINDOW'.static.HideWindow("BoardWnd");
			return;			
		}
		else
		{
			// End:0x102
			if(__NFUN_129__(Class'NWindow.UIAPI_HTMLCTRL'.static.IsPageLock("BoardWnd.HtmlViewer")))
			{
				Class'NWindow.UIAPI_HTMLCTRL'.static.SetPageLock("BoardWnd.HtmlViewer", true);
				Class'NWindow.UIAPI_TABCTRL'.static.SetTopOrder("BoardWnd.TabCtrl", 0, false);
				Class'NWindow.UIAPI_HTMLCTRL'.static.Clear("BoardWnd.HtmlViewer");
				RequestBBSBoard();
			}
		}		
	}
	else
	{
		Class'NWindow.UIAPI_TABCTRL'.static.SetTopOrder("BoardWnd.TabCtrl", Index, false);
		Class'NWindow.UIAPI_HTMLCTRL'.static.Clear("BoardWnd.HtmlViewer");
		ShowBBSTab(Index);
	}
	return;
}

function HandleShowBoardPacket(string param)
{
	local int idx, OK;
	local string Address;

	ParseInt(param, "OK", OK);
	// End:0x3A
	if(__NFUN_150__(OK, 1))
	{
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("BoardWnd");
		return;
	}
	idx = 0;
	J0x41:

	// End:0x65 [Loop If]
	if(__NFUN_150__(idx, 8))
	{
		m_Command[idx] = "";
		__NFUN_165__(idx);
		// [Loop Continue]
		goto J0x41;
	}
	ParseString(param, "Command1", m_Command[0]);
	ParseString(param, "Command2", m_Command[1]);
	ParseString(param, "Command3", m_Command[2]);
	ParseString(param, "Command4", m_Command[3]);
	ParseString(param, "Command5", m_Command[4]);
	ParseString(param, "Command6", m_Command[5]);
	ParseString(param, "Command7", m_Command[6]);
	ParseString(param, "Command8", m_Command[7]);
	m_bBtnLock = false;
	ParseString(param, "Address", Address);
	Class'NWindow.UIAPI_HTMLCTRL'.static.SetHtmlBuffData("BoardWnd.HtmlViewer", Address);
	// End:0x1DA
	if(__NFUN_129__(m_bShow))
	{
		PlayConsoleSound(5);
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("BoardWnd");
		Class'NWindow.UIAPI_WINDOW'.static.SetFocus("BoardWnd");
	}
	return;
}

function ShowBBSTab(int Index)
{
	local string strBypass;
	local UIEventManager.EControlReturnType Ret;

	switch(Index)
	{
		// End:0x25
		case 0:
			strBypass = "bypass _bbshome";
			// End:0xF0
			break;
		// End:0x45
		case 1:
			strBypass = "bypass _bbsgetfav";
			// End:0xF0
			break;
		// End:0x63
		case 2:
			strBypass = "bypass _bbsloc";
			// End:0xF0
			break;
		// End:0x82
		case 3:
			strBypass = "bypass _bbsclan";
			// End:0xF0
			break;
		// End:0xA1
		case 4:
			strBypass = "bypass _bbsmemo";
			// End:0xF0
			break;
		// End:0xC8
		case 5:
			strBypass = "bypass _maillist_0_1_0_";
			// End:0xF0
			break;
		// End:0xED
		case 6:
			strBypass = "bypass _friendlist_0_";
			// End:0xF0
			break;
		// End:0xFFFF
		default:
			break;
	}
	// End:0x144
	if(__NFUN_151__(__NFUN_125__(strBypass), 0))
	{
		Ret = Class'NWindow.UIAPI_HTMLCTRL'.static.ControllerExecution("BoardWnd.HtmlViewer", strBypass);
		// End:0x144
		if(__NFUN_154__(int(Ret), int(1)))
		{
			m_bBtnLock = true;
		}
	}
	return;
}

function OnClickBookmark()
{
	local UIEventManager.EControlReturnType Ret;

	// End:0x67
	if(__NFUN_130__(__NFUN_151__(__NFUN_125__(m_Command[7]), 0), __NFUN_129__(m_bBtnLock)))
	{
		Ret = Class'NWindow.UIAPI_HTMLCTRL'.static.ControllerExecution("BoardWnd.HtmlViewer", m_Command[7]);
		// End:0x67
		if(__NFUN_154__(int(Ret), int(1)))
		{
			m_bBtnLock = true;
		}
	}
	return;
}
