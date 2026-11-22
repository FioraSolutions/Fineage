class PetStatusWnd extends UIScript;

const NSTATUSICON_MAXCOL = 10;

var bool m_bBuff;
var bool m_bShow;
var int m_PetID;

function OnLoad()
{
	RegisterEvent(250);
	RegisterEvent(1000);
	RegisterEvent(1040);
	RegisterEvent(1050);
	RegisterEvent(1130);
	m_bShow = false;
	m_bBuff = false;
	return;
}

function OnShow()
{
	local int PetID, IsPetOrSummoned;

	PetID = Class'NWindow.UIDATA_PET'.static.GetPetID();
	IsPetOrSummoned = Class'NWindow.UIDATA_PET'.static.GetIsPetOrSummoned();
	// End:0x63
	if(__NFUN_132__(__NFUN_150__(PetID, 0), __NFUN_155__(IsPetOrSummoned, 2)))
	{
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("PetStatusWnd");		
	}
	else
	{
		m_bShow = true;
	}
	return;
}

function OnHide()
{
	m_bShow = false;
	return;
}

function OnEnterState(name a_PreStateName)
{
	m_bBuff = false;
	return;
}

function OnEvent(int Event_ID, string param)
{
	// End:0x15
	if(__NFUN_154__(Event_ID, 250))
	{
		HandlePetInfoUpdate();		
	}
	else
	{
		// End:0x2D
		if(__NFUN_154__(Event_ID, 1130))
		{
			HandlePetStatusClose();			
		}
		else
		{
			// End:0x45
			if(__NFUN_154__(Event_ID, 1040))
			{
				HandlePetStatusShow();				
			}
			else
			{
				// End:0x62
				if(__NFUN_154__(Event_ID, 1000))
				{
					HandleShowBuffIcon(param);					
				}
				else
				{
					// End:0x7C
					if(__NFUN_154__(Event_ID, 1050))
					{
						HandlePetStatusSpelledList(param);
					}
				}
			}
		}
	}
	return;
}

function Clear()
{
	Class'NWindow.UIAPI_STATUSICONCTRL'.static.Clear("PetStatusWnd.StatusIcon");
	Class'NWindow.UIAPI_NAMECTRL'.static.SetName("PetStatusWnd.PetName", "", 0, 2);
	UpdateHPBar(0, 0);
	UpdateMPBar(0, 0);
	UpdateFatigueBar(0, 0);
	return;
}

function HandlePetStatusClose()
{
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("PetStatusWnd");
	PlayConsoleSound(6);
	return;
}

function HandlePetInfoUpdate()
{
	local string Name;
	local int HP, maxHP, MP, maxMP, Fatigue, MaxFatigue;

	local PetInfo Info;

	m_PetID = 0;
	// End:0x95
	if(GetPetInfo(Info))
	{
		m_PetID = Info.nID;
		Name = Info.Name;
		HP = Info.nCurHP;
		MP = Info.nCurMP;
		Fatigue = Info.nFatigue;
		maxHP = Info.nMaxHP;
		maxMP = Info.nMaxMP;
		MaxFatigue = Info.nMaxFatigue;
	}
	Class'NWindow.UIAPI_NAMECTRL'.static.SetName("PetStatusWnd.PetName", Name, 0, 2);
	UpdateHPBar(HP, maxHP);
	UpdateMPBar(MP, maxMP);
	UpdateFatigueBar(Fatigue, MaxFatigue);
	return;
}

function HandlePetStatusShow()
{
	Clear();
	Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("PetStatusWnd");
	Class'NWindow.UIAPI_WINDOW'.static.SetFocus("PetStatusWnd");
	return;
}

function HandlePetStatusSpelledList(string param)
{
	local int i, Max, BuffCnt, CurRow;
	local StatusIconInfo Info;

	CurRow = -1;
	Class'NWindow.UIAPI_STATUSICONCTRL'.static.Clear("PetStatusWnd.StatusIcon");
	Info.Size = 16;
	Info.bShow = true;
	ParseInt(param, "Max", Max);
	i = 0;
	J0x69:

	// End:0x15C [Loop If]
	if(__NFUN_150__(i, Max))
	{
		ParseInt(param, __NFUN_112__("SkillID_", string(i)), Info.ClassID);
		// End:0x152
		if(__NFUN_151__(Info.ClassID, 0))
		{
			Info.IconName = Class'NWindow.UIDATA_SKILL'.static.GetIconName(Info.ClassID, 1);
			// End:0x119
			if(__NFUN_180__(__NFUN_173__(float(BuffCnt), float(10)), float(0)))
			{
				__NFUN_165__(CurRow);
				Class'NWindow.UIAPI_STATUSICONCTRL'.static.AddRow("PetStatusWnd.StatusIcon");
			}
			Class'NWindow.UIAPI_STATUSICONCTRL'.static.AddCol("PetStatusWnd.StatusIcon", CurRow, Info);
			__NFUN_165__(BuffCnt);
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x69;
	}
	UpdateBuff(m_bBuff);
	return;
}

function HandleShowBuffIcon(string param)
{
	local int nShow;

	ParseInt(param, "Show", nShow);
	// End:0x2B
	if(__NFUN_154__(nShow, 1))
	{
		UpdateBuff(true);		
	}
	else
	{
		UpdateBuff(false);
	}
	return;
}

function OnLButtonDown(WindowHandle a_WindowHandle, int X, int Y)
{
	local Rect rectWnd;
	local UserInfo UserInfo;

	rectWnd = Class'NWindow.UIAPI_WINDOW'.static.GetRect("PetStatusWnd");
	// End:0x84
	if(__NFUN_130__(__NFUN_151__(X, __NFUN_146__(rectWnd.nX, 13)), __NFUN_150__(X, __NFUN_147__(__NFUN_146__(rectWnd.nX, rectWnd.nWidth), 10))))
	{
		// End:0x84
		if(GetPlayerInfo(UserInfo))
		{
			RequestAction(m_PetID, UserInfo.Loc);
		}
	}
	return;
}

function OnClickButton(string strID)
{
	switch(strID)
	{
		// End:0x1C
		case "btnBuff":
			OnBuffButton();
			// End:0x1F
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnBuffButton()
{
	UpdateBuff(__NFUN_129__(m_bBuff));
	return;
}

function UpdateBuff(bool bShow)
{
	// End:0x34
	if(bShow)
	{
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("PetStatusWnd.StatusIcon");		
	}
	else
	{
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("PetStatusWnd.StatusIcon");
	}
	m_bBuff = bShow;
	return;
}

function UpdateHPBar(int Value, int MaxValue)
{
	Class'NWindow.UIAPI_BARCTRL'.static.SetValue("PetStatusWnd.barHP", MaxValue, Value);
	return;
}

function UpdateMPBar(int Value, int MaxValue)
{
	Class'NWindow.UIAPI_BARCTRL'.static.SetValue("PetStatusWnd.barMP", MaxValue, Value);
	return;
}

function UpdateFatigueBar(int Value, int MaxValue)
{
	Class'NWindow.UIAPI_BARCTRL'.static.SetValue("PetStatusWnd.barFatigue", MaxValue, Value);
	return;
}
