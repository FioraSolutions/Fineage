class OlympiadTargetWnd extends UIScript;

var int m_PlayerNum;
var int m_id;
var string m_Name;
var int m_ClassID;
var int m_MaxHP;
var int m_CurHP;
var int m_MaxCP;
var int m_CurCP;

function OnLoad()
{
	RegisterEvent(900);
	RegisterEvent(920);
	RegisterEvent(910);
	return;
}

function OnEvent(int Event_ID, string param)
{
	// End:0x55
	if(__NFUN_154__(Event_ID, 900))
	{
		Clear();
		ParseInt(param, "PlayerNum", m_PlayerNum);
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("OlympiadTargetWnd");		
	}
	else
	{
		// End:0x78
		if(__NFUN_154__(Event_ID, 920))
		{
			HandleUserInfo(param);
			UpdateStatus();			
		}
		else
		{
			// End:0x8D
			if(__NFUN_154__(Event_ID, 910))
			{
				Clear();
			}
		}
	}
	return;
}

function OnEnterState(name a_PreStateName)
{
	Clear();
	return;
}

function Clear()
{
	m_PlayerNum = 0;
	m_id = 0;
	m_Name = "";
	m_ClassID = 0;
	m_MaxHP = 0;
	m_CurHP = 0;
	m_MaxCP = 0;
	m_CurCP = 0;
	UpdateStatus();
	return;
}

function HandleUserInfo(string param)
{
	local int IsPlayer, PlayerNum;

	ParseInt(param, "IsPlayer", IsPlayer);
	// End:0x27
	if(__NFUN_155__(IsPlayer, 0))
	{
		return;
	}
	ParseInt(param, "PlayerNum", PlayerNum);
	// End:0x60
	if(__NFUN_132__(__NFUN_155__(m_PlayerNum, PlayerNum), __NFUN_150__(PlayerNum, 1)))
	{
		return;
	}
	ParseInt(param, "ID", m_id);
	ParseString(param, "Name", m_Name);
	ParseInt(param, "ClassID", m_ClassID);
	ParseInt(param, "MaxHP", m_MaxHP);
	ParseInt(param, "CurHP", m_CurHP);
	ParseInt(param, "MaxCP", m_MaxCP);
	ParseInt(param, "CurCP", m_CurCP);
	return;
}

function UpdateStatus()
{
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("OlympiadTargetWnd.txtName", m_Name);
	// End:0x77
	if(__NFUN_151__(m_MaxCP, 0))
	{
		Class'NWindow.UIAPI_WINDOW'.static.SetWindowSize("OlympiadTargetWnd.texCP", __NFUN_145__(__NFUN_144__(150, m_CurCP), m_MaxCP), 6);		
	}
	else
	{
		Class'NWindow.UIAPI_WINDOW'.static.SetWindowSize("OlympiadTargetWnd.texCP", 0, 6);
	}
	// End:0xEA
	if(__NFUN_151__(m_MaxHP, 0))
	{
		Class'NWindow.UIAPI_WINDOW'.static.SetWindowSize("OlympiadTargetWnd.texHP", __NFUN_145__(__NFUN_144__(150, m_CurHP), m_MaxHP), 6);		
	}
	else
	{
		Class'NWindow.UIAPI_WINDOW'.static.SetWindowSize("OlympiadTargetWnd.texHP", 0, 6);
	}
	return;
}
