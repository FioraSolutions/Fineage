class OlympiadPlayerWnd extends UIScript;

const MAX_OLYMPIAD_SKILL_MSG = 5;

var int m_PlayerNum;
var string m_WindowName;
var string m_BuffWindowName;
var bool m_Expand;
var int m_id;
var string m_Name;
var int m_ClassID;
var int m_MaxHP;
var int m_CurHP;
var int m_MaxCP;
var int m_CurCP;
var string m_Msg[5];
var int m_MsgStartLine;

function SetPlayerNum(int PlayerNum)
{
	m_PlayerNum = PlayerNum;
	m_WindowName = __NFUN_112__(__NFUN_112__("OlympiadPlayer", string(PlayerNum)), "Wnd");
	m_BuffWindowName = __NFUN_112__(__NFUN_112__("OlympiadBuff", string(PlayerNum)), "Wnd");
	return;
}

function OnLoad()
{
	RegisterEvent(920);
	RegisterEvent(910);
	RegisterEvent(290);
	RegisterEvent(280);
	SetExpandMode(false);
	return;
}

function OnEnterState(name a_PreStateName)
{
	Clear();
	SetExpandMode(m_Expand);
	return;
}

function OnEvent(int Event_ID, string param)
{
	// End:0x23
	if(__NFUN_154__(Event_ID, 920))
	{
		HandleUserInfo(param);
		UpdateStatus();		
	}
	else
	{
		// End:0x40
		if(__NFUN_154__(Event_ID, 290))
		{
			HandleMagicSkillUse(param);			
		}
		else
		{
			// End:0x5D
			if(__NFUN_154__(Event_ID, 280))
			{
				HandleAttack(param);				
			}
			else
			{
				// End:0x72
				if(__NFUN_154__(Event_ID, 910))
				{
					Clear();
				}
			}
		}
	}
	return;
}

function Clear()
{
	local int i;

	m_id = 0;
	m_Name = "";
	m_ClassID = 0;
	m_MaxHP = 0;
	m_CurHP = 0;
	m_MaxCP = 0;
	m_CurCP = 0;
	i = 0;
	J0x39:

	// End:0x5D [Loop If]
	if(__NFUN_150__(i, 5))
	{
		m_Msg[i] = "";
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x39;
	}
	UpdateStatus();
	UpdateMsg("");
	return;
}

function HandleUserInfo(string param)
{
	local int IsPlayer, PlayerNum, PlayerID;
	local string strParam;

	ParseInt(param, "IsPlayer", IsPlayer);
	// End:0x27
	if(__NFUN_155__(IsPlayer, 1))
	{
		return;
	}
	ParseInt(param, "PlayerNum", PlayerNum);
	// End:0x60
	if(__NFUN_132__(__NFUN_155__(m_PlayerNum, PlayerNum), __NFUN_150__(PlayerNum, 1)))
	{
		return;
	}
	ParseInt(param, "ID", PlayerID);
	ParseString(param, "Name", m_Name);
	ParseInt(param, "ClassID", m_ClassID);
	ParseInt(param, "MaxHP", m_MaxHP);
	ParseInt(param, "CurHP", m_CurHP);
	ParseInt(param, "MaxCP", m_MaxCP);
	ParseInt(param, "CurCP", m_CurCP);
	// End:0x162
	if(__NFUN_155__(m_id, PlayerID))
	{
		m_id = PlayerID;
		ParamAdd(strParam, "PlayerNum", string(m_PlayerNum));
		ParamAdd(strParam, "PlayerID", string(PlayerID));
		ExecuteEvent(930, strParam);
	}
	return;
}

function HandleMagicSkillUse(string param)
{
	local int Id, SkillID;
	local string paramsend, strMsg;

	ParseInt(param, "AttackerID", Id);
	// End:0x3A
	if(__NFUN_132__(__NFUN_150__(Id, 1), __NFUN_155__(Id, m_id)))
	{
		return;
	}
	ParseInt(param, "SkillID", SkillID);
	// End:0x71
	if(__NFUN_132__(__NFUN_151__(0, SkillID), __NFUN_150__(1999, SkillID)))
	{
		return;
	}
	ParamAdd(paramsend, "Type", string(int(4)));
	ParamAdd(paramsend, "param1", string(SkillID));
	ParamAdd(paramsend, "param2", "1");
	AddSystemMessageParam(paramsend);
	strMsg = EndSystemMessageParam(46, true);
	UpdateMsg(strMsg);
	return;
}

function HandleAttack(string param)
{
	local int AttackerID;
	local string AttackerName;
	local int DefenderID, Critical, Miss, ShieldDefense;
	local string paramsend, strMsg;

	ParseInt(param, "AttackerID", AttackerID);
	ParseString(param, "AttackerName", AttackerName);
	ParseInt(param, "DefenderID", DefenderID);
	ParseInt(param, "Critical", Critical);
	ParseInt(param, "Miss", Miss);
	ParseInt(param, "ShieldDefense", ShieldDefense);
	// End:0xDD
	if(__NFUN_130__(__NFUN_151__(AttackerID, 0), __NFUN_154__(AttackerID, m_id)))
	{
		// End:0xDA
		if(__NFUN_151__(Critical, 0))
		{
			UpdateMsg(GetSystemMessage(44));
		}		
	}
	else
	{
		// End:0x174
		if(__NFUN_130__(__NFUN_151__(DefenderID, 0), __NFUN_154__(DefenderID, m_id)))
		{
			// End:0x15B
			if(__NFUN_151__(Miss, 0))
			{
				ParamAdd(paramsend, "Type", string(int(0)));
				ParamAdd(paramsend, "param1", AttackerName);
				AddSystemMessageParam(paramsend);
				strMsg = EndSystemMessageParam(42, true);
				UpdateMsg(strMsg);				
			}
			else
			{
				// End:0x174
				if(__NFUN_151__(ShieldDefense, 0))
				{
					UpdateMsg(GetSystemMessage(111));
				}
			}
		}
	}
	return;
}

function UpdateMsg(string strMsg)
{
	local int i, CurPos;

	m_Msg[m_MsgStartLine] = strMsg;
	m_MsgStartLine = int(__NFUN_173__(float(__NFUN_146__(m_MsgStartLine, 1)), float(5)));
	i = 0;
	J0x30:

	// End:0x9C [Loop If]
	if(__NFUN_150__(i, 5))
	{
		CurPos = int(__NFUN_173__(float(__NFUN_146__(m_MsgStartLine, i)), float(5)));
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText(__NFUN_112__(__NFUN_112__(m_WindowName, ".txtMsg"), string(__NFUN_147__(__NFUN_147__(5, 1), i))), m_Msg[CurPos]);
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x30;
	}
	return;
}

function UpdateStatus()
{
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText(__NFUN_112__(m_WindowName, ".txtName"), m_Name);
	// End:0x66
	if(__NFUN_151__(m_MaxCP, 0))
	{
		Class'NWindow.UIAPI_WINDOW'.static.SetWindowSize(__NFUN_112__(m_WindowName, ".texCP"), __NFUN_145__(__NFUN_144__(326, m_CurCP), m_MaxCP), 6);		
	}
	else
	{
		Class'NWindow.UIAPI_WINDOW'.static.SetWindowSize(__NFUN_112__(m_WindowName, ".texCP"), 0, 6);
	}
	// End:0xC8
	if(__NFUN_151__(m_MaxHP, 0))
	{
		Class'NWindow.UIAPI_WINDOW'.static.SetWindowSize(__NFUN_112__(m_WindowName, ".texHP"), __NFUN_145__(__NFUN_144__(326, m_CurHP), m_MaxHP), 6);		
	}
	else
	{
		Class'NWindow.UIAPI_WINDOW'.static.SetWindowSize(__NFUN_112__(m_WindowName, ".texHP"), 0, 6);
	}
	return;
}

function OnFrameExpandClick(bool bIsExpand)
{
	SetExpandMode(bIsExpand);
	m_Expand = bIsExpand;
	return;
}

function SetExpandMode(bool bExpand)
{
	local Rect rectWnd, rectBuffWnd;

	// End:0x4F
	if(bExpand)
	{
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow(__NFUN_112__(m_WindowName, ".BackTex"));
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow(__NFUN_112__(m_WindowName, ".BackExpTex"));		
	}
	else
	{
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow(__NFUN_112__(m_WindowName, ".BackTex"));
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow(__NFUN_112__(m_WindowName, ".BackExpTex"));
	}
	rectWnd = Class'NWindow.UIAPI_WINDOW'.static.GetRect(m_WindowName);
	rectBuffWnd = Class'NWindow.UIAPI_WINDOW'.static.GetRect(m_BuffWindowName);
	// End:0x125
	if(bExpand)
	{
		// End:0x122
		if(__NFUN_132__(__NFUN_154__(__NFUN_146__(rectWnd.nY, 46), rectBuffWnd.nY), __NFUN_154__(__NFUN_146__(rectWnd.nY, 47), rectBuffWnd.nY)))
		{
			Class'NWindow.UIAPI_WINDOW'.static.MoveEx(m_BuffWindowName, 0, 80);
		}		
	}
	else
	{
		// End:0x17B
		if(__NFUN_132__(__NFUN_154__(__NFUN_146__(rectWnd.nY, 126), rectBuffWnd.nY), __NFUN_154__(__NFUN_146__(rectWnd.nY, 127), rectBuffWnd.nY)))
		{
			Class'NWindow.UIAPI_WINDOW'.static.MoveEx(m_BuffWindowName, 0, -80);
		}
	}
	return;
}
