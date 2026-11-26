class GMDetailStatusWnd extends DetailStatusWnd;

var string temp1;
var bool bShow;
var UserInfo m_ObservingUserInfo;

function OnLoad()
{
	temp1 = "Water/Air/Ground";
	RegisterEvent(2290);
	RegisterEvent(2404);
	Class'NWindow.UIAPI_WINDOW'.static.SetAnchor(__NFUN_112__(m_WindowName, ".txtMovingSpeed"), m_WindowName, "TopLeft", "TopLeft", 139, 208);
	Class'NWindow.UIAPI_WINDOW'.static.SetAnchor(__NFUN_112__(m_WindowName, ".txtMagicCastingSpeed"), m_WindowName, "TopLeft", "TopLeft", 186, 177);
	Class'NWindow.UIAPI_WINDOW'.static.SetAnchor(__NFUN_112__(m_WindowName, ".txtHeadMovingSpeed"), m_WindowName, "TopLeft", "TopLeft", 137, 192);
	Class'NWindow.UIAPI_WINDOW'.static.SetAnchor(__NFUN_112__(m_WindowName, ".txtHeadMagicCastingSpeed"), m_WindowName, "TopLeft", "TopLeft", 137, 177);
	Class'NWindow.UIAPI_WINDOW'.static.SetAnchor(__NFUN_112__(m_WindowName, ".txtGmMoving"), m_WindowName, "TopLeft", "TopLeft", 139, 220);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText(__NFUN_112__(m_WindowName, ".txtGmMoving"), temp1);
	bShow = false;
	return;
}

function OnShow()
{
	return;
}

function OnHide()
{
	return;
}

function ShowStatus(string a_Param)
{
	// End:0x0E
	if(__NFUN_122__(a_Param, ""))
	{
		return;
	}
	// End:0x31
	if(bShow)
	{
		m_hOwnerWnd.HideWindow();
		bShow = false;		
	}
	else
	{
		Class'NWindow.GMAPI'.static.RequestGMCommand(1, a_Param);
		bShow = true;
	}
	return;
}

function OnEvent(int a_EventID, string a_Param)
{
	switch(a_EventID)
	{
		// End:0x39
		case 2290:
			// End:0x36
			if(HandleGMObservingUserInfoUpdate())
			{
				m_hOwnerWnd.ShowWindow();
				m_hOwnerWnd.SetFocus();
			}
			// End:0x52
			break;
		// End:0x4F
		case 2404:
			HandleGMUpdateHennaInfo(a_Param);
			// End:0x52
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function bool HandleGMObservingUserInfoUpdate()
{
	local UserInfo ObservingUserInfo;

	// End:0x22
	if(Class'NWindow.GMAPI'.static.GetObservingUserInfo(ObservingUserInfo))
	{
		HandleUpdateUserInfo();
		return true;		
	}
	else
	{
		return false;
	}
	return;
}

function HandleGMUpdateHennaInfo(string a_Param)
{
	HandleUpdateHennaInfo(a_Param);
	HandleGMObservingUserInfoUpdate();
	return;
}

function bool GetMyUserInfo(out UserInfo a_MyUserInfo)
{
	local bool Result;

	Result = Class'NWindow.GMAPI'.static.GetObservingUserInfo(m_ObservingUserInfo);
	// End:0x34
	if(Result)
	{
		a_MyUserInfo = m_ObservingUserInfo;
		return true;		
	}
	else
	{
		return false;
	}
	return;
}

function string GetMovingSpeed(UserInfo a_UserInfo)
{
	local int WaterMaxSpeed, WaterMinSpeed, AirMaxSpeed, AirMinSpeed, GroundMaxSpeed, GroundMinSpeed;

	local string MovingSpeed;

	WaterMaxSpeed = int(__NFUN_171__(float(a_UserInfo.nWaterMaxSpeed), a_UserInfo.fNonAttackSpeedModifier));
	WaterMinSpeed = int(__NFUN_171__(float(a_UserInfo.nWaterMinSpeed), a_UserInfo.fNonAttackSpeedModifier));
	AirMaxSpeed = int(__NFUN_171__(float(a_UserInfo.nAirMaxSpeed), a_UserInfo.fNonAttackSpeedModifier));
	AirMinSpeed = int(__NFUN_171__(float(a_UserInfo.nAirMinSpeed), a_UserInfo.fNonAttackSpeedModifier));
	GroundMaxSpeed = int(__NFUN_171__(float(a_UserInfo.nGroundMaxSpeed), a_UserInfo.fNonAttackSpeedModifier));
	GroundMinSpeed = int(__NFUN_171__(float(a_UserInfo.nGroundMinSpeed), a_UserInfo.fNonAttackSpeedModifier));
	MovingSpeed = __NFUN_112__(__NFUN_112__(string(WaterMaxSpeed), ","), string(WaterMinSpeed));
	MovingSpeed = __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(MovingSpeed, "/"), string(AirMaxSpeed)), ","), string(AirMinSpeed));
	MovingSpeed = __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(MovingSpeed, "/"), string(GroundMaxSpeed)), ","), string(GroundMinSpeed));
	return MovingSpeed;
	return;
}

function float GetMyExpRate()
{
	return __NFUN_171__(GetExpRate(m_ObservingUserInfo.nCurExp, m_ObservingUserInfo.nLevel), 100.0000000);
	return;
}

defaultproperties
{
	m_WindowName="GMDetailStatusWnd.InnerWnd"
}