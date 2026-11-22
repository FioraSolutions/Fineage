class SummonedStatusWnd extends UIScript;

const NSTATUSICON_MAXCOL = 10;

var bool m_bBuff;
var bool m_bShow;
var int m_PetID;
var bool m_bSummonedStarted;

function OnLoad()
{
	RegisterEvent(250);
	RegisterEvent(1000);
	RegisterEvent(1100);
	RegisterEvent(1110);
	RegisterEvent(1120);
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
	// End:0x8B
	if(__NFUN_132__(__NFUN_150__(PetID, 0), __NFUN_155__(IsPetOrSummoned, 1)))
	{
		Debug(__NFUN_112__(__NFUN_112__(__NFUN_112__("Hide ", string(IsPetOrSummoned)), " "), string(PetID)));
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("SummonedStatusWnd");		
	}
	else
	{
		m_bShow = true;
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("SummonedStatusWnd");
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
	OnShow();
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
			HandlePetSummonedStatusClose();			
		}
		else
		{
			// End:0x45
			if(__NFUN_154__(Event_ID, 1100))
			{
				HandleSummonedStatusShow();				
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
					// End:0x7F
					if(__NFUN_154__(Event_ID, 1110))
					{
						HandleSummonedStatusSpelledList(param);						
					}
					else
					{
						// End:0x99
						if(__NFUN_154__(Event_ID, 1120))
						{
							HandleSummonedStatusRemainTime(param);
						}
					}
				}
			}
		}
	}
	return;
}

function Clear()
{
	ClearBuff();
	Class'NWindow.UIAPI_NAMECTRL'.static.SetName("SummonedStatusWnd.PetName", "", 0, 2);
	UpdateHPBar(0, 0);
	UpdateMPBar(0, 0);
	return;
}

function ClearBuff()
{
	Class'NWindow.UIAPI_STATUSICONCTRL'.static.Clear("SummonedStatusWnd.StatusIcon");
	return;
}

function HandleSummonedStatusRemainTime(string param)
{
	local int RemainTime, MaxTime;

	ParseInt(param, "RemainTime", RemainTime);
	ParseInt(param, "MaxTime", MaxTime);
	// End:0x74
	if(m_bSummonedStarted)
	{
		Class'NWindow.UIAPI_PROGRESSCTRL'.static.SetPos("SummonedStatusWnd.progFATIGUE", RemainTime);		
	}
	else
	{
		ClearBuff();
		Class'NWindow.UIAPI_PROGRESSCTRL'.static.SetProgressTime("SummonedStatusWnd.progFATIGUE", MaxTime);
		Class'NWindow.UIAPI_PROGRESSCTRL'.static.Start("SummonedStatusWnd.progFATIGUE");
		m_bSummonedStarted = true;
	}
	return;
}

function HandlePetSummonedStatusClose()
{
	InitFATIGUEBar();
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("SummonedStatusWnd");
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
	Class'NWindow.UIAPI_NAMECTRL'.static.SetName("SummonedStatusWnd.PetName", Name, 0, 2);
	UpdateHPBar(HP, maxHP);
	UpdateMPBar(MP, maxMP);
	return;
}

function HandleSummonedStatusShow()
{
	Clear();
	Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("SummonedStatusWnd");
	Class'NWindow.UIAPI_WINDOW'.static.SetFocus("SummonedStatusWnd");
	Debug("HandleSummonedStatusShow");
	return;
}

function HandleSummonedStatusSpelledList(string param)
{
	local int i, Max, BuffCnt, CurRow;
	local StatusIconInfo Info;

	CurRow = -1;
	Class'NWindow.UIAPI_STATUSICONCTRL'.static.Clear("SummonedStatusWnd.StatusIcon");
	Info.Size = 16;
	Info.bShow = true;
	ParseInt(param, "Max", Max);
	i = 0;
	J0x6E:

	// End:0x16B [Loop If]
	if(__NFUN_150__(i, Max))
	{
		ParseInt(param, __NFUN_112__("SkillID_", string(i)), Info.ClassID);
		// End:0x161
		if(__NFUN_151__(Info.ClassID, 0))
		{
			Info.IconName = Class'NWindow.UIDATA_SKILL'.static.GetIconName(Info.ClassID, 1);
			// End:0x123
			if(__NFUN_180__(__NFUN_173__(float(BuffCnt), float(10)), float(0)))
			{
				__NFUN_165__(CurRow);
				Class'NWindow.UIAPI_STATUSICONCTRL'.static.AddRow("SummonedStatusWnd.StatusIcon");
			}
			Class'NWindow.UIAPI_STATUSICONCTRL'.static.AddCol("SummonedStatusWnd.StatusIcon", CurRow, Info);
			__NFUN_165__(BuffCnt);
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x6E;
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

	rectWnd = Class'NWindow.UIAPI_WINDOW'.static.GetRect("SummonedStatusWnd");
	// End:0x89
	if(__NFUN_130__(__NFUN_151__(X, __NFUN_146__(rectWnd.nX, 13)), __NFUN_150__(X, __NFUN_147__(__NFUN_146__(rectWnd.nX, rectWnd.nWidth), 10))))
	{
		// End:0x89
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
	// End:0x39
	if(bShow)
	{
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("SummonedStatusWnd.StatusIcon");		
	}
	else
	{
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("SummonedStatusWnd.StatusIcon");
	}
	m_bBuff = bShow;
	return;
}

function UpdateHPBar(int Value, int MaxValue)
{
	Class'NWindow.UIAPI_BARCTRL'.static.SetValue("SummonedStatusWnd.barHP", MaxValue, Value);
	return;
}

function UpdateMPBar(int Value, int MaxValue)
{
	Class'NWindow.UIAPI_BARCTRL'.static.SetValue("SummonedStatusWnd.barMP", MaxValue, Value);
	return;
}

function InitFATIGUEBar()
{
	Class'NWindow.UIAPI_PROGRESSCTRL'.static.Stop("SummonedStatusWnd.progFATIGUE");
	Class'NWindow.UIAPI_PROGRESSCTRL'.static.Reset("SummonedStatusWnd.progFATIGUE");
	m_bSummonedStarted = false;
	return;
}
