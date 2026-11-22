class ShortcutWnd extends UICommonAPI;

const MAX_Page = 10;
const MAX_ShortcutPerPage = 12;

enum EJoyShortcut
{
	JOYSHORTCUT_Left,               // 0
	JOYSHORTCUT_Center,             // 1
	JOYSHORTCUT_Right               // 2
};

var int CurrentShortcutPage;
var int CurrentShortcutPage2;
var int CurrentShortcutPage3;
var bool m_IsLocked;
var bool m_IsVertical;
var bool m_IsJoypad;
var bool m_IsJoypadExpand;
var bool m_IsJoypadOn;
var bool m_IsExpand1;
var bool m_IsExpand2;
var bool m_IsShortcutExpand;
var string m_ShortcutWndName;

function OnLoad()
{
	RegisterEvent(630);
	RegisterEvent(640);
	RegisterEvent(660);
	RegisterEvent(650);
	RegisterEvent(590);
	RegisterEvent(600);
	RegisterEvent(610);
	RegisterEvent(620);
	m_IsLocked = GetOptionBool("Game", "IsLockShortcutWnd");
	m_IsExpand1 = GetOptionBool("Game", "Is1ExpandShortcutWnd");
	m_IsExpand2 = GetOptionBool("Game", "Is2ExpandShortcutWnd");
	m_IsVertical = GetOptionBool("Game", "IsShortcutWndVertical");
	InitShortPageNum();
	return;
}

function OnDefaultPosition()
{
	m_IsExpand1 = false;
	m_IsExpand2 = false;
	SetVertical(true);
	InitShortPageNum();
	ArrangeWnd();
	ExpandWnd();
	return;
}

function OnEnterState(name a_PreStateName)
{
	ArrangeWnd();
	ExpandWnd();
	// End:0x21
	if(__NFUN_254__(a_PreStateName, 'LoadingState'))
	{
		InitShortPageNum();
	}
	return;
}

function OnEvent(int a_EventID, string a_Param)
{
	switch(a_EventID)
	{
		// End:0x1D
		case 640:
			HandleShortcutPageUpdate(a_Param);
			// End:0xC1
			break;
		// End:0x33
		case 660:
			HandleShortcutJoypad(a_Param);
			// End:0xC1
			break;
		// End:0x49
		case 590:
			HandleJoypadLButtonDown(a_Param);
			// End:0xC1
			break;
		// End:0x5F
		case 600:
			HandleJoypadLButtonUp(a_Param);
			// End:0xC1
			break;
		// End:0x75
		case 610:
			HandleJoypadRButtonDown(a_Param);
			// End:0xC1
			break;
		// End:0x8B
		case 620:
			HandleJoypadRButtonUp(a_Param);
			// End:0xC1
			break;
		// End:0xA1
		case 630:
			HandleShortcutUpdate(a_Param);
			// End:0xC1
			break;
		// End:0xBE
		case 650:
			HandleShortcutClear();
			ArrangeWnd();
			ExpandWnd();
			// End:0xC1
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function InitShortPageNum()
{
	CurrentShortcutPage = 0;
	CurrentShortcutPage2 = 1;
	CurrentShortcutPage3 = 2;
	return;
}

function HandleShortcutPageUpdate(string param)
{
	local int i, nShortcutID, ShortcutPage;

	// End:0xFD
	if(ParseInt(param, "ShortcutPage", ShortcutPage))
	{
		// End:0x3C
		if(__NFUN_132__(__NFUN_151__(0, ShortcutPage), __NFUN_152__(10, ShortcutPage)))
		{
			return;
		}
		CurrentShortcutPage = ShortcutPage;
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText(__NFUN_112__(__NFUN_112__("ShortcutWnd.", m_ShortcutWndName), ".PageNumTextBox"), string(__NFUN_146__(CurrentShortcutPage, 1)));
		nShortcutID = __NFUN_144__(CurrentShortcutPage, 12);
		i = 0;
		J0x9E:

		// End:0xFD [Loop If]
		if(__NFUN_150__(i, 12))
		{
			Class'NWindow.UIAPI_SHORTCUTITEMWINDOW'.static.UpdateShortcut(__NFUN_112__(__NFUN_112__(__NFUN_112__("ShortcutWnd.", m_ShortcutWndName), ".Shortcut"), string(__NFUN_146__(i, 1))), nShortcutID);
			__NFUN_165__(nShortcutID);
			__NFUN_163__(i);
			// [Loop Continue]
			goto J0x9E;
		}
	}
	return;
}

function HandleShortcutUpdate(string param)
{
	local int nShortcutID, nShortcutNum;

	ParseInt(param, "ShortcutID", nShortcutID);
	nShortcutNum = __NFUN_146__(int(__NFUN_173__(float(nShortcutID), float(12))), 1);
	// End:0x86
	if(IsShortcutIDInCurPage(CurrentShortcutPage, nShortcutID))
	{
		Class'NWindow.UIAPI_SHORTCUTITEMWINDOW'.static.UpdateShortcut(__NFUN_112__(__NFUN_112__(__NFUN_112__("ShortcutWnd.", m_ShortcutWndName), ".Shortcut"), string(nShortcutNum)), nShortcutID);
	}
	// End:0xDA
	if(IsShortcutIDInCurPage(CurrentShortcutPage2, nShortcutID))
	{
		Class'NWindow.UIAPI_SHORTCUTITEMWINDOW'.static.UpdateShortcut(__NFUN_112__(__NFUN_112__(__NFUN_112__("ShortcutWnd.", m_ShortcutWndName), "_1.Shortcut"), string(nShortcutNum)), nShortcutID);
	}
	// End:0x12E
	if(IsShortcutIDInCurPage(CurrentShortcutPage3, nShortcutID))
	{
		Class'NWindow.UIAPI_SHORTCUTITEMWINDOW'.static.UpdateShortcut(__NFUN_112__(__NFUN_112__(__NFUN_112__("ShortcutWnd.", m_ShortcutWndName), "_2.Shortcut"), string(nShortcutNum)), nShortcutID);
	}
	return;
}

function HandleShortcutClear()
{
	local int i;

	i = 0;
	J0x07:

	// End:0x212 [Loop If]
	if(__NFUN_150__(i, 12))
	{
		Class'NWindow.UIAPI_SHORTCUTITEMWINDOW'.static.Clear(__NFUN_112__("ShortcutWnd.ShortcutWndVertical.Shortcut", string(__NFUN_146__(i, 1))));
		Class'NWindow.UIAPI_SHORTCUTITEMWINDOW'.static.Clear(__NFUN_112__("ShortcutWnd.ShortcutWndVertical_1.Shortcut", string(__NFUN_146__(i, 1))));
		Class'NWindow.UIAPI_SHORTCUTITEMWINDOW'.static.Clear(__NFUN_112__("ShortcutWnd.ShortcutWndVertical_2.Shortcut", string(__NFUN_146__(i, 1))));
		Class'NWindow.UIAPI_SHORTCUTITEMWINDOW'.static.Clear(__NFUN_112__("ShortcutWnd.ShortcutWndHorizontal.Shortcut", string(__NFUN_146__(i, 1))));
		Class'NWindow.UIAPI_SHORTCUTITEMWINDOW'.static.Clear(__NFUN_112__("ShortcutWnd.ShortcutWndHorizontal_1.Shortcut", string(__NFUN_146__(i, 1))));
		Class'NWindow.UIAPI_SHORTCUTITEMWINDOW'.static.Clear(__NFUN_112__("ShortcutWnd.ShortcutWndHorizontal_2.Shortcut", string(__NFUN_146__(i, 1))));
		Class'NWindow.UIAPI_SHORTCUTITEMWINDOW'.static.Clear(__NFUN_112__("ShortcutWnd.ShortcutWndJoypadExpand.Shortcut", string(__NFUN_146__(i, 1))));
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x07;
	}
	i = 0;
	J0x219:

	// End:0x272 [Loop If]
	if(__NFUN_150__(i, 4))
	{
		Class'NWindow.UIAPI_SHORTCUTITEMWINDOW'.static.Clear(__NFUN_112__("ShortcutWnd.ShortcutWndJoypad.Shortcut", string(__NFUN_146__(i, 1))));
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x219;
	}
	return;
}

function HandleShortcutJoypad(string a_Param)
{
	local int OnOff;

	// End:0x95
	if(ParseInt(a_Param, "OnOff", OnOff))
	{
		// End:0x59
		if(__NFUN_154__(1, OnOff))
		{
			m_IsJoypadOn = true;
			ShowWindow(__NFUN_112__(__NFUN_112__("ShortcutWnd.", m_ShortcutWndName), ".JoypadBtn"));			
		}
		else
		{
			// End:0x95
			if(__NFUN_154__(0, OnOff))
			{
				m_IsJoypadOn = false;
				HideWindow(__NFUN_112__(__NFUN_112__("ShortcutWnd.", m_ShortcutWndName), ".JoypadBtn"));
			}
		}
	}
	return;
}

function HandleJoypadLButtonUp(string a_Param)
{
	SetJoypadShortcut(1);
	return;
}

function HandleJoypadLButtonDown(string a_Param)
{
	SetJoypadShortcut(0);
	return;
}

function HandleJoypadRButtonUp(string a_Param)
{
	SetJoypadShortcut(1);
	return;
}

function HandleJoypadRButtonDown(string a_Param)
{
	SetJoypadShortcut(2);
	return;
}

function SetJoypadShortcut(ShortcutWnd.EJoyShortcut a_JoyShortcut)
{
	local int i, nShortcutID;

	switch(a_JoyShortcut)
	{
		// End:0x238
		case 0:
			Class'NWindow.UIAPI_TEXTURECTRL'.static.SetTexture("ShortcutWnd.ShortcutWndJoypadExpand.JoypadButtonBackTex", "L2UI_CH3.ShortcutWnd.joypad2_back_over1");
			Class'NWindow.UIAPI_TEXTURECTRL'.static.SetAnchor("ShortcutWnd.ShortcutWndJoypadExpand.JoypadButtonBackTex", "ShortcutWnd.ShortcutWndJoypadExpand", "TopLeft", "TopLeft", 28, 0);
			Class'NWindow.UIAPI_TEXTURECTRL'.static.SetTexture("ShortcutWnd.ShortcutWndJoypad.JoypadLButtonTex", "L2UI_ch3.Joypad.joypad_L_HOLD");
			Class'NWindow.UIAPI_TEXTURECTRL'.static.SetTexture("ShortcutWnd.ShortcutWndJoypad.JoypadRButtonTex", "L2UI_ch3.Joypad.joypad_R");
			nShortcutID = __NFUN_146__(__NFUN_144__(CurrentShortcutPage, 12), 4);
			i = 0;
			J0x1D0:

			// End:0x235 [Loop If]
			if(__NFUN_150__(i, 4))
			{
				Class'NWindow.UIAPI_SHORTCUTITEMWINDOW'.static.UpdateShortcut(__NFUN_112__("ShortcutWnd.ShortcutWndJoypad.Shortcut", string(__NFUN_146__(i, 1))), nShortcutID);
				__NFUN_165__(nShortcutID);
				__NFUN_163__(i);
				// [Loop Continue]
				goto J0x1D0;
			}
			// End:0x697
			break;
		// End:0x460
		case 1:
			Class'NWindow.UIAPI_TEXTURECTRL'.static.SetTexture("ShortcutWnd.ShortcutWndJoypadExpand.JoypadButtonBackTex", "L2UI_CH3.ShortcutWnd.joypad2_back_over2");
			Class'NWindow.UIAPI_TEXTURECTRL'.static.SetAnchor("ShortcutWnd.ShortcutWndJoypadExpand.JoypadButtonBackTex", "ShortcutWnd.ShortcutWndJoypadExpand", "TopLeft", "TopLeft", 158, 0);
			Class'NWindow.UIAPI_TEXTURECTRL'.static.SetTexture("ShortcutWnd.ShortcutWndJoypad.JoypadLButtonTex", "L2UI_ch3.Joypad.joypad_L");
			Class'NWindow.UIAPI_TEXTURECTRL'.static.SetTexture("ShortcutWnd.ShortcutWndJoypad.JoypadRButtonTex", "L2UI_ch3.Joypad.joypad_R");
			nShortcutID = __NFUN_144__(CurrentShortcutPage, 12);
			i = 0;
			J0x3F8:

			// End:0x45D [Loop If]
			if(__NFUN_150__(i, 4))
			{
				Class'NWindow.UIAPI_SHORTCUTITEMWINDOW'.static.UpdateShortcut(__NFUN_112__("ShortcutWnd.ShortcutWndJoypad.Shortcut", string(__NFUN_146__(i, 1))), nShortcutID);
				__NFUN_165__(nShortcutID);
				__NFUN_163__(i);
				// [Loop Continue]
				goto J0x3F8;
			}
			// End:0x697
			break;
		// End:0x694
		case 2:
			Class'NWindow.UIAPI_TEXTURECTRL'.static.SetTexture("ShortcutWnd.ShortcutWndJoypadExpand.JoypadButtonBackTex", "L2UI_CH3.ShortcutWnd.joypad2_back_over3");
			Class'NWindow.UIAPI_TEXTURECTRL'.static.SetAnchor("ShortcutWnd.ShortcutWndJoypadExpand.JoypadButtonBackTex", "ShortcutWnd.ShortcutWndJoypadExpand", "TopLeft", "TopLeft", 288, 0);
			Class'NWindow.UIAPI_TEXTURECTRL'.static.SetTexture("ShortcutWnd.ShortcutWndJoypad.JoypadLButtonTex", "L2UI_ch3.Joypad.joypad_L");
			Class'NWindow.UIAPI_TEXTURECTRL'.static.SetTexture("ShortcutWnd.ShortcutWndJoypad.JoypadRButtonTex", "L2UI_ch3.Joypad.joypad_R_HOLD");
			nShortcutID = __NFUN_146__(__NFUN_144__(CurrentShortcutPage, 12), 8);
			i = 0;
			J0x62C:

			// End:0x691 [Loop If]
			if(__NFUN_150__(i, 4))
			{
				Class'NWindow.UIAPI_SHORTCUTITEMWINDOW'.static.UpdateShortcut(__NFUN_112__("ShortcutWnd.ShortcutWndJoypad.Shortcut", string(__NFUN_146__(i, 1))), nShortcutID);
				__NFUN_165__(nShortcutID);
				__NFUN_163__(i);
				// [Loop Continue]
				goto J0x62C;
			}
			// End:0x697
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnClickButton(string a_strID)
{
	switch(a_strID)
	{
		// End:0x1C
		case "PrevBtn":
			OnPrevBtn();
			// End:0x131
			break;
		// End:0x31
		case "NextBtn":
			OnNextBtn();
			// End:0x131
			break;
		// End:0x47
		case "PrevBtn2":
			OnPrevBtn2();
			// End:0x131
			break;
		// End:0x5D
		case "NextBtn2":
			OnNextBtn2();
			// End:0x131
			break;
		// End:0x73
		case "PrevBtn3":
			OnPrevBtn3();
			// End:0x131
			break;
		// End:0x89
		case "NextBtn3":
			OnNextBtn3();
			// End:0x131
			break;
		// End:0x9E
		case "LockBtn":
			OnClickLockBtn();
			// End:0x131
			break;
		// End:0xB5
		case "UnlockBtn":
			OnClickUnlockBtn();
			// End:0x131
			break;
		// End:0xCC
		case "RotateBtn":
			OnRotateBtn();
			// End:0x131
			break;
		// End:0xE3
		case "JoypadBtn":
			OnJoypadBtn();
			// End:0x131
			break;
		// End:0xFA
		case "ExpandBtn":
			OnExpandBtn();
			// End:0x131
			break;
		// End:0x114
		case "ExpandButton":
			OnClickExpandShortcutButton();
			// End:0x131
			break;
		// End:0x12E
		case "ReduceButton":
			OnClickExpandShortcutButton();
			// End:0x131
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnPrevBtn()
{
	local int nNewPage;

	nNewPage = __NFUN_147__(CurrentShortcutPage, 1);
	// End:0x24
	if(__NFUN_151__(0, nNewPage))
	{
		nNewPage = __NFUN_147__(10, 1);
	}
	SetCurPage(nNewPage);
	return;
}

function OnPrevBtn2()
{
	local int nNewPage;

	nNewPage = __NFUN_147__(CurrentShortcutPage2, 1);
	// End:0x24
	if(__NFUN_151__(0, nNewPage))
	{
		nNewPage = __NFUN_147__(10, 1);
	}
	SetCurPage2(nNewPage);
	return;
}

function OnPrevBtn3()
{
	local int nNewPage;

	nNewPage = __NFUN_147__(CurrentShortcutPage3, 1);
	// End:0x24
	if(__NFUN_151__(0, nNewPage))
	{
		nNewPage = __NFUN_147__(10, 1);
	}
	SetCurPage3(nNewPage);
	return;
}

function OnNextBtn()
{
	local int nNewPage;

	nNewPage = __NFUN_146__(CurrentShortcutPage, 1);
	// End:0x21
	if(__NFUN_152__(10, nNewPage))
	{
		nNewPage = 0;
	}
	SetCurPage(nNewPage);
	return;
}

function OnNextBtn2()
{
	local int nNewPage;

	nNewPage = __NFUN_146__(CurrentShortcutPage2, 1);
	// End:0x21
	if(__NFUN_152__(10, nNewPage))
	{
		nNewPage = 0;
	}
	SetCurPage2(nNewPage);
	return;
}

function OnNextBtn3()
{
	local int nNewPage;

	nNewPage = __NFUN_146__(CurrentShortcutPage3, 1);
	// End:0x21
	if(__NFUN_152__(10, nNewPage))
	{
		nNewPage = 0;
	}
	SetCurPage3(nNewPage);
	return;
}

function OnClickLockBtn()
{
	UnLock();
	return;
}

function OnClickUnlockBtn()
{
	Lock();
	return;
}

function OnRotateBtn()
{
	SetVertical(__NFUN_129__(m_IsVertical));
	// End:0x128
	if(m_IsVertical)
	{
		Class'NWindow.UIAPI_WINDOW'.static.SetAnchor("ShortcutWnd.ShortcutWndVertical", "ShortcutWnd.ShortcutWndHorizontal", "BottomRight", "BottomRight", 0, 0);
		Class'NWindow.UIAPI_WINDOW'.static.ClearAnchor("ShortcutWnd.ShortcutWndVertical");
		Class'NWindow.UIAPI_WINDOW'.static.SetAnchor("ShortcutWnd.ShortcutWndHorizontal", "ShortcutWnd.ShortcutWndVertical", "BottomRight", "BottomRight", 0, 0);		
	}
	else
	{
		Class'NWindow.UIAPI_WINDOW'.static.SetAnchor("ShortcutWnd.ShortcutWndHorizontal", "ShortcutWnd.ShortcutWndVertical", "BottomRight", "BottomRight", 0, 0);
		Class'NWindow.UIAPI_WINDOW'.static.ClearAnchor("ShortcutWnd.ShortcutWndHorizontal");
		Class'NWindow.UIAPI_WINDOW'.static.SetAnchor("ShortcutWnd.ShortcutWndVertical", "ShortcutWnd.ShortcutWndHorizontal", "BottomRight", "BottomRight", 0, 0);
	}
	// End:0x253
	if(__NFUN_242__(m_IsExpand2, true))
	{
		Expand1();
		Expand2();		
	}
	else
	{
		// End:0x265
		if(__NFUN_242__(m_IsExpand1, true))
		{
			Expand1();
		}
	}
	Class'NWindow.UIAPI_WINDOW'.static.SetFocus(__NFUN_112__("ShortcutWnd.", m_ShortcutWndName));
	return;
}

function OnJoypadBtn()
{
	SetJoypad(__NFUN_129__(m_IsJoypad));
	Class'NWindow.UIAPI_WINDOW'.static.SetFocus(__NFUN_112__("ShortcutWnd.", m_ShortcutWndName));
	return;
}

function OnExpandBtn()
{
	SetJoypadExpand(__NFUN_129__(m_IsJoypadExpand));
	Class'NWindow.UIAPI_WINDOW'.static.SetFocus(__NFUN_112__("ShortcutWnd.", m_ShortcutWndName));
	return;
}

function SetCurPage(int a_nCurPage)
{
	// End:0x1B
	if(__NFUN_132__(__NFUN_151__(0, a_nCurPage), __NFUN_152__(10, a_nCurPage)))
	{
		return;
	}
	Class'NWindow.ShortcutAPI'.static.SetShortcutPage(a_nCurPage);
	return;
}

function SetCurPage2(int a_nCurPage)
{
	local int i, nShortcutID;

	// End:0x1B
	if(__NFUN_132__(__NFUN_151__(0, a_nCurPage), __NFUN_152__(10, a_nCurPage)))
	{
		return;
	}
	CurrentShortcutPage2 = a_nCurPage;
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("ShortcutWnd.", m_ShortcutWndName), "."), m_ShortcutWndName), "_1"), ".PageNumTextBox"), string(__NFUN_146__(CurrentShortcutPage2, 1)));
	nShortcutID = __NFUN_144__(CurrentShortcutPage2, 12);
	i = 0;
	J0x8F:

	// End:0x100 [Loop If]
	if(__NFUN_150__(i, 12))
	{
		Class'NWindow.UIAPI_SHORTCUTITEMWINDOW'.static.UpdateShortcut(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("ShortcutWnd.", m_ShortcutWndName), "."), m_ShortcutWndName), "_1"), ".Shortcut"), string(__NFUN_146__(i, 1))), nShortcutID);
		__NFUN_165__(nShortcutID);
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x8F;
	}
	return;
}

function SetCurPage3(int a_nCurPage)
{
	local int i, nShortcutID;

	// End:0x1B
	if(__NFUN_132__(__NFUN_151__(0, a_nCurPage), __NFUN_152__(10, a_nCurPage)))
	{
		return;
	}
	CurrentShortcutPage3 = a_nCurPage;
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("ShortcutWnd.", m_ShortcutWndName), "."), m_ShortcutWndName), "_1."), m_ShortcutWndName), "_2"), ".PageNumTextBox"), string(__NFUN_146__(CurrentShortcutPage3, 1)));
	nShortcutID = __NFUN_144__(CurrentShortcutPage3, 12);
	i = 0;
	J0x9D:

	// End:0x11C [Loop If]
	if(__NFUN_150__(i, 12))
	{
		Class'NWindow.UIAPI_SHORTCUTITEMWINDOW'.static.UpdateShortcut(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("ShortcutWnd.", m_ShortcutWndName), "."), m_ShortcutWndName), "_1."), m_ShortcutWndName), "_2"), ".Shortcut"), string(__NFUN_146__(i, 1))), nShortcutID);
		__NFUN_165__(nShortcutID);
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x9D;
	}
	return;
}

function bool IsShortcutIDInCurPage(int PageNum, int a_nShortcutID)
{
	// End:0x15
	if(__NFUN_151__(__NFUN_144__(PageNum, 12), a_nShortcutID))
	{
		return false;
	}
	// End:0x2D
	if(__NFUN_152__(__NFUN_144__(__NFUN_146__(PageNum, 1), 12), a_nShortcutID))
	{
		return false;
	}
	return true;
	return;
}

function Lock()
{
	m_IsLocked = true;
	SetOptionBool("Game", "IsLockShortcutWnd", true);
	// End:0x8E
	if(IsShowWindow("ShortcutWnd"))
	{
		ShowWindow(__NFUN_112__(__NFUN_112__("ShortcutWnd.", m_ShortcutWndName), ".LockBtn"));
		HideWindow(__NFUN_112__(__NFUN_112__("ShortcutWnd.", m_ShortcutWndName), ".UnlockBtn"));
	}
	return;
}

function UnLock()
{
	m_IsLocked = false;
	SetOptionBool("Game", "IsLockShortcutWnd", false);
	// End:0x8E
	if(IsShowWindow("ShortcutWnd"))
	{
		ShowWindow(__NFUN_112__(__NFUN_112__("ShortcutWnd.", m_ShortcutWndName), ".UnlockBtn"));
		HideWindow(__NFUN_112__(__NFUN_112__("ShortcutWnd.", m_ShortcutWndName), ".LockBtn"));
	}
	return;
}

function SetVertical(bool a_IsVertical)
{
	m_IsVertical = a_IsVertical;
	SetOptionBool("Game", "IsShortcutWndVertical", m_IsVertical);
	ArrangeWnd();
	ExpandWnd();
	return;
}

function SetJoypad(bool a_IsJoypad)
{
	m_IsJoypad = a_IsJoypad;
	ArrangeWnd();
	return;
}

function SetJoypadExpand(bool a_IsJoypadExpand)
{
	m_IsJoypadExpand = a_IsJoypadExpand;
	// End:0xB4
	if(m_IsJoypadExpand)
	{
		Class'NWindow.UIAPI_WINDOW'.static.SetAnchor("ShortcutWnd.ShortcutWndJoypadExpand", "ShortcutWnd.ShortcutWndJoypad", "TopLeft", "TopLeft", 0, 0);
		Class'NWindow.UIAPI_WINDOW'.static.ClearAnchor("ShortcutWnd.ShortcutWndJoypadExpand");		
	}
	else
	{
		Class'NWindow.UIAPI_WINDOW'.static.SetAnchor("ShortcutWnd.ShortcutWndJoypad", "ShortcutWnd.ShortcutWndJoypadExpand", "TopLeft", "TopLeft", 0, 0);
		Class'NWindow.UIAPI_WINDOW'.static.ClearAnchor("ShortcutWnd.ShortcutWndJoypad");
	}
	ArrangeWnd();
	return;
}

function ArrangeWnd()
{
	local Rect WindowRect;

	// End:0x140
	if(m_IsJoypad)
	{
		HideWindow("ShortcutWnd.ShortcutWndVertical");
		HideWindow("ShortcutWnd.ShortcutWndHorizontal");
		// End:0xD4
		if(m_IsJoypadExpand)
		{
			HideWindow("ShortcutWnd.ShortcutWndJoypad");
			ShowWindow("ShortcutWnd.ShortcutWndJoypadExpand");
			m_ShortcutWndName = "ShortcutWndJoypadExpand";			
		}
		else
		{
			HideWindow("ShortcutWnd.ShortcutWndJoypadExpand");
			ShowWindow("ShortcutWnd.ShortcutWndJoypad");
			m_ShortcutWndName = "ShortcutWndJoypad";
		}		
	}
	else
	{
		HideWindow("ShortcutWnd.ShortcutWndJoypadExpand");
		HideWindow("ShortcutWnd.ShortcutWndJoypad");
		// End:0x288
		if(m_IsVertical)
		{
			m_ShortcutWndName = "ShortcutWndVertical";
			WindowRect = Class'NWindow.UIAPI_WINDOW'.static.GetRect("ShortcutWnd.ShortcutWndVertical");
			// End:0x235
			if(__NFUN_150__(WindowRect.nY, 0))
			{
				Class'NWindow.UIAPI_WINDOW'.static.MoveTo("ShortcutWnd.ShortcutWndVertical", WindowRect.nX, 0);
			}
			HideWindow("ShortcutWnd.ShortcutWndHorizontal");
			ShowWindow("ShortcutWnd.ShortcutWndVertical");			
		}
		else
		{
			m_ShortcutWndName = "ShortcutWndHorizontal";
			WindowRect = Class'NWindow.UIAPI_WINDOW'.static.GetRect("ShortcutWnd.ShortcutWndHorizontal");
			// End:0x32A
			if(__NFUN_150__(WindowRect.nX, 0))
			{
				Class'NWindow.UIAPI_WINDOW'.static.MoveTo("ShortcutWnd.ShortcutWndHorizontal", 0, WindowRect.nY);
			}
			HideWindow("ShortcutWnd.ShortcutWndVertical");
			ShowWindow("ShortcutWnd.ShortcutWndHorizontal");
		}
		// End:0x3AF
		if(m_IsJoypadOn)
		{
			ShowWindow(__NFUN_112__(__NFUN_112__("ShortcutWnd.", m_ShortcutWndName), ".JoypadBtn"));			
		}
		else
		{
			HideWindow(__NFUN_112__(__NFUN_112__("ShortcutWnd.", m_ShortcutWndName), ".JoypadBtn"));
		}
	}
	// End:0x3EA
	if(m_IsLocked)
	{
		Lock();		
	}
	else
	{
		UnLock();
	}
	SetCurPage(CurrentShortcutPage);
	SetCurPage2(CurrentShortcutPage2);
	SetCurPage3(CurrentShortcutPage3);
	// End:0x42E
	if(__NFUN_242__(m_IsExpand1, true))
	{
		m_IsShortcutExpand = true;
		HandleExpandButton();		
	}
	else
	{
		// End:0x44B
		if(__NFUN_242__(m_IsExpand2, true))
		{
			m_IsShortcutExpand = false;
			HandleExpandButton();			
		}
		else
		{
			m_IsShortcutExpand = true;
			HandleExpandButton();
		}
	}
	return;
}

function ExpandWnd()
{
	// End:0x51
	if(__NFUN_132__(__NFUN_242__(m_IsExpand1, true), __NFUN_242__(m_IsExpand2, true)))
	{
		// End:0x34
		if(__NFUN_242__(m_IsExpand2, true))
		{
			m_IsShortcutExpand = false;
			Expand2();
		}
		// End:0x4E
		if(__NFUN_242__(m_IsExpand1, true))
		{
			m_IsShortcutExpand = false;
			Expand1();
		}		
	}
	else
	{
		m_IsShortcutExpand = true;
		Reduce();
	}
	return;
}

function Expand1()
{
	m_IsShortcutExpand = true;
	m_IsExpand1 = true;
	SetOptionBool("Game", "Is1ExpandShortcutWnd", m_IsExpand1);
	Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("ShortcutWnd.ShortcutWndVertical_1");
	Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("ShortcutWnd.ShortcutWndHorizontal_1");
	HandleExpandButton();
	return;
}

function Expand2()
{
	m_IsShortcutExpand = false;
	m_IsExpand2 = true;
	SetOptionBool("Game", "Is2ExpandShortcutWnd", m_IsExpand2);
	Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("ShortcutWnd.ShortcutWndVertical_2");
	Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("ShortcutWnd.ShortcutWndHorizontal_2");
	HandleExpandButton();
	return;
}

function Reduce()
{
	m_IsShortcutExpand = true;
	m_IsExpand1 = false;
	m_IsExpand2 = false;
	SetOptionBool("Game", "Is1ExpandShortcutWnd", m_IsExpand1);
	SetOptionBool("Game", "Is2ExpandShortcutWnd", m_IsExpand2);
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ShortcutWnd.ShortcutWndVertical_1");
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ShortcutWnd.ShortcutWndVertical_2");
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ShortcutWnd.ShortcutWndHorizontal_1");
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ShortcutWnd.ShortcutWndHorizontal_2");
	HandleExpandButton();
	return;
}

function OnClickExpandShortcutButton()
{
	// End:0x12
	if(m_IsExpand2)
	{
		Reduce();		
	}
	else
	{
		// End:0x24
		if(m_IsExpand1)
		{
			Expand2();			
		}
		else
		{
			Expand1();
		}
	}
	return;
}

function HandleExpandButton()
{
	// End:0x64
	if(m_IsShortcutExpand)
	{
		ShowWindow(__NFUN_112__(__NFUN_112__("ShortcutWnd.", m_ShortcutWndName), ".ExpandButton"));
		HideWindow(__NFUN_112__(__NFUN_112__("ShortcutWnd.", m_ShortcutWndName), ".ReduceButton"));		
	}
	else
	{
		HideWindow(__NFUN_112__(__NFUN_112__("ShortcutWnd.", m_ShortcutWndName), ".ExpandButton"));
		ShowWindow(__NFUN_112__(__NFUN_112__("ShortcutWnd.", m_ShortcutWndName), ".ReduceButton"));
	}
	return;
}

defaultproperties
{
	m_IsVertical=true
}