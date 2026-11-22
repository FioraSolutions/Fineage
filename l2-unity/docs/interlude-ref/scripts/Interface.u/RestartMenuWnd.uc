class RestartMenuWnd extends UICommonAPI;

var bool m_bShow;
var bool m_bRestartON;
var bool m_bVillage;
var bool m_bAgit;
var bool m_bCastle;
var bool m_bBattleCamp;
var bool m_bOriginal;
var WindowHandle m_wndTop;
var ButtonHandle m_btnVillage;
var ButtonHandle m_btnAgit;
var ButtonHandle m_btnCastle;
var ButtonHandle m_btnBattleCamp;
var ButtonHandle m_btnOriginal;

function OnLoad()
{
	RegisterEvent(50);
	RegisterEvent(40);
	RegisterEvent(1430);
	RegisterEvent(1440);
	m_bShow = false;
	m_bRestartON = false;
	m_wndTop = GetHandle("RestartMenuWnd");
	m_btnVillage = ButtonHandle(GetHandle("RestartMenuWnd.btnVillage"));
	m_btnAgit = ButtonHandle(GetHandle("RestartMenuWnd.btnAgit"));
	m_btnCastle = ButtonHandle(GetHandle("RestartMenuWnd.btnCastle"));
	m_btnBattleCamp = ButtonHandle(GetHandle("RestartMenuWnd.btnBattleCamp"));
	m_btnOriginal = ButtonHandle(GetHandle("RestartMenuWnd.btnOriginal"));
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

function OnEnterState(name a_PreStateName)
{
	// End:0x12
	if(m_bRestartON)
	{
		ShowMe();		
	}
	else
	{
		HideMe();
	}
	return;
}

function OnEvent(int Event_ID, string param)
{
	// End:0x1A
	if(__NFUN_154__(Event_ID, 50))
	{
		HandleDie(param);		
	}
	else
	{
		// End:0x2F
		if(__NFUN_154__(Event_ID, 40))
		{
			HandleRestart();			
		}
		else
		{
			// End:0x47
			if(__NFUN_154__(Event_ID, 1430))
			{
				HandleRestartMenuShow();				
			}
			else
			{
				// End:0x5C
				if(__NFUN_154__(Event_ID, 1440))
				{
					HandleRestartMenuHide();
				}
			}
		}
	}
	return;
}

function OnClickButton(string strID)
{
	switch(strID)
	{
		// End:0x1F
		case "btnVillage":
			OnVillageClick();
			// End:0x82
			break;
		// End:0x34
		case "btnAgit":
			OnAgitClick();
			// End:0x82
			break;
		// End:0x4B
		case "btnCastle":
			OnCastleClick();
			// End:0x82
			break;
		// End:0x66
		case "btnBattleCamp":
			OnBattleCampClick();
			// End:0x82
			break;
		// End:0x7F
		case "btnOriginal":
			OnOriginalClick();
			// End:0x82
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnVillageClick()
{
	RequestRestartPoint(0);
	HideMe();
	return;
}

function OnAgitClick()
{
	RequestRestartPoint(1);
	HideMe();
	return;
}

function OnCastleClick()
{
	RequestRestartPoint(2);
	HideMe();
	return;
}

function OnBattleCampClick()
{
	RequestRestartPoint(3);
	HideMe();
	return;
}

function OnOriginalClick()
{
	RequestRestartPoint(4);
	HideMe();
	return;
}

function HandleDie(string param)
{
	local int Village, Agit, Castle, BattleCamp, Original;

	ParseInt(param, "Village", Village);
	ParseInt(param, "Agit", Agit);
	ParseInt(param, "Castle", Castle);
	ParseInt(param, "BattleCamp", BattleCamp);
	ParseInt(param, "Original", Original);
	m_bVillage = false;
	m_bAgit = false;
	m_bCastle = false;
	m_bBattleCamp = false;
	m_bOriginal = false;
	// End:0xB8
	if(__NFUN_151__(Village, 0))
	{
		m_bVillage = true;
	}
	// End:0xCB
	if(__NFUN_151__(Agit, 0))
	{
		m_bAgit = true;
	}
	// End:0xDE
	if(__NFUN_151__(Castle, 0))
	{
		m_bCastle = true;
	}
	// End:0xF1
	if(__NFUN_151__(BattleCamp, 0))
	{
		m_bBattleCamp = true;
	}
	// End:0x104
	if(__NFUN_151__(Original, 0))
	{
		m_bOriginal = true;
	}
	return;
}

function HandleRestartMenuShow()
{
	ShowMe();
	return;
}

function HandleRestartMenuHide()
{
	HideMe();
	return;
}

function HandleRestart()
{
	HideMe();
	return;
}

function ShowMe()
{
	m_bRestartON = true;
	m_wndTop.ShowWindow();
	// End:0x32
	if(m_bVillage)
	{
		m_btnVillage.ShowWindow();		
	}
	else
	{
		m_btnVillage.HideWindow();
	}
	// End:0x5C
	if(m_bAgit)
	{
		m_btnAgit.ShowWindow();		
	}
	else
	{
		m_btnAgit.HideWindow();
	}
	// End:0x86
	if(m_bCastle)
	{
		m_btnCastle.ShowWindow();		
	}
	else
	{
		m_btnCastle.HideWindow();
	}
	// End:0xB0
	if(m_bBattleCamp)
	{
		m_btnBattleCamp.ShowWindow();		
	}
	else
	{
		m_btnBattleCamp.HideWindow();
	}
	// End:0xDA
	if(m_bOriginal)
	{
		m_btnOriginal.ShowWindow();		
	}
	else
	{
		m_btnOriginal.HideWindow();
	}
	return;
}

function HideMe()
{
	m_bRestartON = false;
	m_wndTop.HideWindow();
	return;
}
