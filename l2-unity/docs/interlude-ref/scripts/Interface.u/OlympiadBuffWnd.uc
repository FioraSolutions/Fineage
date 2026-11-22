class OlympiadBuffWnd extends UIScript;

const NSTATUSICON_FRAMESIZE = 12;
const NSTATUSICON_MAXCOL = 12;

var int m_PlayerNum;
var int m_PlayerID;
var string m_WindowName;

function SetPlayerNum(int PlayerNum)
{
	m_PlayerNum = PlayerNum;
	m_WindowName = __NFUN_112__(__NFUN_112__("OlympiadBuff", string(PlayerNum)), "Wnd");
	return;
}

function OnLoad()
{
	RegisterEvent(930);
	RegisterEvent(940);
	RegisterEvent(910);
	return;
}

function OnEnterState(name a_PreStateName)
{
	Clear();
	m_PlayerID = 0;
	return;
}

function OnEvent(int Event_ID, string param)
{
	// End:0x1D
	if(__NFUN_154__(Event_ID, 930))
	{
		HandleBuffShow(param);		
	}
	else
	{
		// End:0x3A
		if(__NFUN_154__(Event_ID, 940))
		{
			HandleBuffInfo(param);			
		}
		else
		{
			// End:0x56
			if(__NFUN_154__(Event_ID, 910))
			{
				Clear();
				m_PlayerID = 0;
			}
		}
	}
	return;
}

function Clear()
{
	Class'NWindow.UIAPI_STATUSICONCTRL'.static.Clear(__NFUN_112__(m_WindowName, ".StatusIcon"));
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow(m_WindowName);
	return;
}

function HandleBuffShow(string param)
{
	local int PlayerNum;

	ParseInt(param, "PlayerNum", PlayerNum);
	// End:0x39
	if(__NFUN_132__(__NFUN_155__(m_PlayerNum, PlayerNum), __NFUN_150__(PlayerNum, 1)))
	{
		return;
	}
	ParseInt(param, "PlayerID", m_PlayerID);
	return;
}

function HandleBuffInfo(string param)
{
	local int PlayerID, i, Max, CurRow;
	local StatusIconInfo Info;
	local Rect rectWnd;

	ParseInt(param, "PlayerID", PlayerID);
	// End:0x38
	if(__NFUN_132__(__NFUN_155__(m_PlayerID, PlayerID), __NFUN_150__(PlayerID, 1)))
	{
		return;
	}
	Clear();
	CurRow = -1;
	ParseInt(param, "Max", Max);
	i = 0;
	J0x65:

	// End:0x225 [Loop If]
	if(__NFUN_150__(i, Max))
	{
		// End:0xB3
		if(__NFUN_180__(__NFUN_173__(float(i), float(12)), float(0)))
		{
			Class'NWindow.UIAPI_STATUSICONCTRL'.static.AddRow(__NFUN_112__(m_WindowName, ".StatusIcon"));
			__NFUN_165__(CurRow);
		}
		ParseInt(param, __NFUN_112__("SkillID_", string(i)), Info.ClassID);
		ParseInt(param, __NFUN_112__("SkillLevel_", string(i)), Info.Level);
		ParseInt(param, __NFUN_112__("RemainTime_", string(i)), Info.RemainTime);
		ParseString(param, __NFUN_112__("Name_", string(i)), Info.Name);
		ParseString(param, __NFUN_112__("IconName_", string(i)), Info.IconName);
		ParseString(param, __NFUN_112__("Description_", string(i)), Info.Description);
		Info.Size = 24;
		Info.BackTex = "L2UI.EtcWndBack.AbnormalBack";
		Info.bShow = true;
		Class'NWindow.UIAPI_STATUSICONCTRL'.static.AddCol(__NFUN_112__(m_WindowName, ".StatusIcon"), CurRow, Info);
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x65;
	}
	// End:0x2B9
	if(__NFUN_151__(Max, 0))
	{
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow(m_WindowName);
		rectWnd = Class'NWindow.UIAPI_WINDOW'.static.GetRect(__NFUN_112__(m_WindowName, ".StatusIcon"));
		Class'NWindow.UIAPI_WINDOW'.static.SetWindowSize(m_WindowName, __NFUN_146__(rectWnd.nWidth, 12), rectWnd.nHeight);
		Class'NWindow.UIAPI_WINDOW'.static.SetFrameSize(m_WindowName, 12, rectWnd.nHeight);
	}
	return;
}
