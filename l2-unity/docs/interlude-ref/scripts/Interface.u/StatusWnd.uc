class StatusWnd extends UIScript;

var int m_UserID;
var bool m_bReceivedUserInfo;

function OnLoad()
{
	RegisterEvent(70);
	RegisterEvent(180);
	RegisterEvent(190);
	RegisterEvent(200);
	RegisterEvent(210);
	RegisterEvent(220);
	RegisterEvent(230);
	RegisterEvent(240);
	return;
}

function OnEnterState(name a_PreStateName)
{
	m_bReceivedUserInfo = false;
	UpdateUserInfo();
	return;
}

function UpdateUserInfo()
{
	local UserInfo UserInfo;

	// End:0x163
	if(GetPlayerInfo(UserInfo))
	{
		m_bReceivedUserInfo = true;
		m_UserID = UserInfo.nID;
		Class'NWindow.UIAPI_STATUSBARCTRL'.static.SetPoint("StatusWnd.CPBar", UserInfo.nCurCP, UserInfo.nMaxCP);
		Class'NWindow.UIAPI_STATUSBARCTRL'.static.SetPoint("StatusWnd.HPBar", UserInfo.nCurHP, UserInfo.nMaxHP);
		Class'NWindow.UIAPI_STATUSBARCTRL'.static.SetPoint("StatusWnd.MPBar", UserInfo.nCurMP, UserInfo.nMaxMP);
		Class'NWindow.UIAPI_STATUSBARCTRL'.static.SetPointExp("StatusWnd.EXPBar", UserInfo.nCurExp, UserInfo.nLevel);
		Class'NWindow.UIAPI_NAMECTRL'.static.SetName("StatusWnd.UserName", UserInfo.Name, 0, 1);
		Class'NWindow.UIAPI_TEXTBOX'.static.SetInt("StatusWnd.StatusWnd_LevelTextBox", UserInfo.nLevel);
	}
	return;
}

function OnLButtonDown(WindowHandle a_WindowHandle, int X, int Y)
{
	local Rect rectWnd;

	rectWnd = Class'NWindow.UIAPI_WINDOW'.static.GetRect("StatusWnd");
	// End:0x64
	if(__NFUN_130__(__NFUN_151__(X, __NFUN_146__(rectWnd.nX, 13)), __NFUN_150__(X, __NFUN_147__(__NFUN_146__(rectWnd.nX, rectWnd.nWidth), 10))))
	{
		RequestSelfTarget();
	}
	return;
}

function OnEvent(int a_EventID, string a_Param)
{
	switch(a_EventID)
	{
		// End:0x15
		case 180:
			UpdateUserInfo();
			// End:0x8D
			break;
		// End:0x28
		case 200:
			HandleUpdateInfo(a_Param);
			// End:0x8D
			break;
		// End:0x3B
		case 210:
			HandleUpdateInfo(a_Param);
			// End:0x8D
			break;
		// End:0x4E
		case 220:
			HandleUpdateInfo(a_Param);
			// End:0x8D
			break;
		// End:0x61
		case 230:
			HandleUpdateInfo(a_Param);
			// End:0x8D
			break;
		// End:0x74
		case 240:
			HandleUpdateInfo(a_Param);
			// End:0x8D
			break;
		// End:0x87
		case 70:
			HandleRegenStatus(a_Param);
			// End:0x8D
			break;
		// End:0xFFFF
		default:
			// End:0x8D
			break;
			break;
	}
	return;
}

function HandleUpdateInfo(string param)
{
	local int ServerID;

	ParseInt(param, "ServerID", ServerID);
	// End:0x3C
	if(__NFUN_132__(__NFUN_154__(m_UserID, ServerID), __NFUN_129__(m_bReceivedUserInfo)))
	{
		UpdateUserInfo();
	}
	return;
}

function HandleRegenStatus(string a_Param)
{
	local int Type, Duration, ticks;
	local float Amount;

	ParseInt(a_Param, "Type", Type);
	// End:0x99
	if(__NFUN_154__(Type, 1))
	{
		ParseInt(a_Param, "Duration", Duration);
		ParseInt(a_Param, "Ticks", ticks);
		ParseFloat(a_Param, "Amount", Amount);
		Class'NWindow.UIAPI_STATUSBARCTRL'.static.SetRegenInfo("StatusWnd.HPBar", Duration, ticks, Amount);
	}
	return;
}
