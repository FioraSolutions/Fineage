class MacroEditWnd extends UICommonAPI;

const MACRO_MAX_COUNT = 24;
const MACROCOMMAND_MAX_COUNT = 12;
const MACROICON_MAX_COUNT = 7;
const MACRO_ICONANME = "L2UI.MacroWnd.Macro_Icon";

var bool m_bShow;
var int m_CurIconNum;
var int m_CurMacroID;

function OnLoad()
{
	RegisterEvent(1710);
	RegisterEvent(1260);
	RegisterEvent(1270);
	m_bShow = false;
	m_CurIconNum = 1;
	m_CurMacroID = 0;
	InitTabOrder();
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

function OnClickButton(string strID)
{
	switch(strID)
	{
		// End:0x1C
		case "btnInfo":
			OnClickInfo();
			// End:0x8B
			break;
		// End:0x31
		case "btnHelp":
			OnClickHelp();
			// End:0x8B
			break;
		// End:0x48
		case "btnCancel":
			OnClickCancel();
			// End:0x8B
			break;
		// End:0x5D
		case "btnLeft":
			OnClickLeft();
			// End:0x8B
			break;
		// End:0x73
		case "btnRight":
			OnClickRight();
			// End:0x8B
			break;
		// End:0x88
		case "btnSave":
			OnClickSave();
			// End:0x8B
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnEvent(int Event_ID, string param)
{
	// End:0x1D
	if(__NFUN_154__(Event_ID, 1260))
	{
		HandleMacroShowEditWnd(param);		
	}
	else
	{
		// End:0x3A
		if(__NFUN_154__(Event_ID, 1270))
		{
			HandleMacroDeleted(param);			
		}
		else
		{
			// End:0x52
			if(__NFUN_154__(Event_ID, 1710))
			{
				// End:0x52
				if(DialogIsMine())
				{
				}
			}
		}
	}
	return;
}

function InitTabOrder()
{
	local int idx;

	Class'NWindow.UIAPI_WINDOW'.static.SetTabOrder("MacroEditWnd", "MacroEditWnd.txtName", "MacroEditWnd.txtShortName");
	Class'NWindow.UIAPI_WINDOW'.static.SetTabOrder("MacroEditWnd.txtName", "MacroEditWnd.txtShortName", "MacroEditWnd");
	Class'NWindow.UIAPI_WINDOW'.static.SetTabOrder("MacroEditWnd.txtShortName", "MacroEditWnd.txtEdit0", "MacroEditWnd.txtName");
	idx = 0;
	J0xFA:

	// End:0x24F [Loop If]
	if(__NFUN_150__(idx, 12))
	{
		// End:0x16C
		if(__NFUN_154__(idx, 0))
		{
			Class'NWindow.UIAPI_WINDOW'.static.SetTabOrder("MacroEditWnd.txtEdit0", "MacroEditWnd.txtEdit1", "MacroEditWnd.txtShortName");
			// [Explicit Continue]
			goto J0x245;
		}
		// End:0x1D3
		if(__NFUN_154__(idx, __NFUN_147__(12, 1)))
		{
			Class'NWindow.UIAPI_WINDOW'.static.SetTabOrder("MacroEditWnd.txtEdit11", "MacroEditWnd.txtName", "MacroEditWnd.txtEdit10");
			// [Explicit Continue]
			goto J0x245;
		}
		Class'NWindow.UIAPI_WINDOW'.static.SetTabOrder(__NFUN_112__("MacroEditWnd.txtEdit", string(idx)), __NFUN_112__("MacroEditWnd.txtEdit", string(__NFUN_146__(idx, 1))), __NFUN_112__("MacroEditWnd.txtEdit", string(__NFUN_147__(idx, 1))));
		J0x245:

		__NFUN_165__(idx);
		// [Loop Continue]
		goto J0xFA;
	}
	return;
}

function OnClickInfo()
{
	// End:0x48
	if(Class'NWindow.UIAPI_WINDOW'.static.IsShowWindow("MacroInfoWnd"))
	{
		PlayConsoleSound(6);
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("MacroInfoWnd");		
	}
	else
	{
		PlayConsoleSound(5);
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("MacroInfoWnd");
		Class'NWindow.UIAPI_WINDOW'.static.SetFocus("MacroInfoWnd");
	}
	return;
}

function OnClickHelp()
{
	local string strParam;

	ParamAdd(strParam, "FilePath", "..\\L2text\\help_macro.htm");
	ExecuteEvent(1210, strParam);
	return;
}

function OnClickCancel()
{
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("MacroEditWnd");
	return;
}

function OnClickSave()
{
	SaveMacro();
	return;
}

function OnDropItem(string strID, ItemInfo infItem, int X, int Y)
{
	// End:0x0F
	if(__NFUN_150__(__NFUN_125__(strID), 1))
	{
		return;
	}
	// End:0x28
	if(__NFUN_123__(__NFUN_128__(strID, 7), "txtEdit"))
	{
		return;
	}
	Class'NWindow.UIAPI_EDITBOX'.static.SetString(__NFUN_112__("MacroEditWnd.", strID), infItem.MacroCommand);
	Class'NWindow.UIAPI_EDITBOX'.static.SetHighLight(__NFUN_112__("MacroEditWnd.", strID), false);
	return;
}

function OnDragItemStart(string strID, ItemInfo infItem)
{
	// End:0x0F
	if(__NFUN_150__(__NFUN_125__(strID), 1))
	{
		return;
	}
	// End:0x28
	if(__NFUN_123__(__NFUN_128__(strID, 7), "txtEdit"))
	{
		return;
	}
	Class'NWindow.UIAPI_EDITBOX'.static.SetHighLight(__NFUN_112__("MacroEditWnd.", strID), true);
	return;
}

function OnDragItemEnd(string strID)
{
	// End:0x0F
	if(__NFUN_150__(__NFUN_125__(strID), 1))
	{
		return;
	}
	// End:0x28
	if(__NFUN_123__(__NFUN_128__(strID, 7), "txtEdit"))
	{
		return;
	}
	Class'NWindow.UIAPI_EDITBOX'.static.SetHighLight(__NFUN_112__("MacroEditWnd.", strID), false);
	return;
}

function OnChangeEditBox(string strID)
{
	switch(strID)
	{
		// End:0x21
		case "txtShortName":
			UpdateIconName();
			// End:0x24
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnClickLeft()
{
	__NFUN_166__(m_CurIconNum);
	// End:0x1A
	if(__NFUN_150__(m_CurIconNum, 1))
	{
		m_CurIconNum = 7;
	}
	UpdateIcon();
	return;
}

function OnClickRight()
{
	__NFUN_165__(m_CurIconNum);
	// End:0x1A
	if(__NFUN_151__(m_CurIconNum, 7))
	{
		m_CurIconNum = 1;
	}
	UpdateIcon();
	return;
}

function UpdateIcon()
{
	Class'NWindow.UIAPI_TEXTURECTRL'.static.SetTexture("MacroEditWnd.texMacro", __NFUN_112__("L2UI.MacroWnd.Macro_Icon", string(m_CurIconNum)));
	return;
}

function UpdateIconName()
{
	local string strShortName;

	strShortName = Class'NWindow.UIAPI_EDITBOX'.static.GetString("MacroEditWnd.txtShortName");
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("MacroEditWnd.txtMacroName", strShortName);
	return;
}

function Clear()
{
	local int idx;
	local WindowHandle m_MacroEditWnd;

	m_CurIconNum = 1;
	m_MacroEditWnd = GetHandle("MacroEditWnd.areaEdit");
	Class'NWindow.UIAPI_EDITBOX'.static.SetString("MacroEditWnd.txtName", "");
	Class'NWindow.UIAPI_EDITBOX'.static.SetString("MacroEditWnd.txtShortName", "");
	idx = 0;
	J0x84:

	// End:0xCA [Loop If]
	if(__NFUN_150__(idx, 12))
	{
		Class'NWindow.UIAPI_EDITBOX'.static.SetString(__NFUN_112__("MacroEditWnd.txtEdit", string(idx)), "");
		__NFUN_165__(idx);
		// [Loop Continue]
		goto J0x84;
	}
	UpdateIcon();
	UpdateIconName();
	m_MacroEditWnd.SetScrollPosition(0);
	return;
}

function HandleMacroDeleted(string param)
{
	local int MacroID;

	ParseInt(param, "MacroID", MacroID);
	// End:0x58
	if(__NFUN_130__(m_bShow, __NFUN_154__(m_CurMacroID, MacroID)))
	{
		PlayConsoleSound(6);
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("MacroEditWnd");
	}
	return;
}

function HandleMacroShowEditWnd(string param)
{
	local int MacroCount;
	local Color TextColor;

	Clear();
	m_CurMacroID = 0;
	// End:0x8C
	if(ParseInt(param, "MacroID", m_CurMacroID))
	{
		SetMacroID(m_CurMacroID);
		// End:0x64
		if(__NFUN_129__(m_bShow))
		{
			PlayConsoleSound(5);
			Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("MacroEditWnd");
		}
		Class'NWindow.UIAPI_WINDOW'.static.SetFocus("MacroEditWnd.txtName");		
	}
	else
	{
		// End:0xBD
		if(m_bShow)
		{
			PlayConsoleSound(6);
			Class'NWindow.UIAPI_WINDOW'.static.HideWindow("MacroEditWnd");			
		}
		else
		{
			MacroCount = Class'NWindow.UIDATA_MACRO'.static.GetMacroCount();
			// End:0x130
			if(__NFUN_153__(MacroCount, 24))
			{
				TextColor.R = 176;
				TextColor.G = 155;
				TextColor.B = 121;
				TextColor.A = byte(255);
				DialogShow(5, GetSystemMessage(797));
				DialogSetID(0);
				return;
			}
			PlayConsoleSound(5);
			Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("MacroEditWnd");
			Class'NWindow.UIAPI_WINDOW'.static.SetFocus("MacroEditWnd.txtName");
		}
	}
	return;
}

function SetMacroID(int MacroID)
{
	local int idx;
	local MacroInfo Info;
	local MacroInfoWnd script;

	// End:0x0D
	if(__NFUN_150__(MacroID, 1))
	{
		return;
	}
	// End:0x17E
	if(Class'NWindow.UIDATA_MACRO'.static.GetMacroInfo(MacroID, Info))
	{
		Class'NWindow.UIAPI_EDITBOX'.static.SetString("MacroEditWnd.txtName", Info.Name);
		Class'NWindow.UIAPI_EDITBOX'.static.SetString("MacroEditWnd.txtShortName", Info.IconName);
		m_CurIconNum = int(__NFUN_234__(Info.IconTextureName, 1));
		// End:0xB3
		if(__NFUN_150__(m_CurIconNum, 1))
		{
			m_CurIconNum = 1;
		}
		UpdateIcon();
		script = MacroInfoWnd(GetScript("MacroInfoWnd"));
		script.SetInfoText(Info.Description);
		idx = 0;
		J0xF8:

		// End:0x17E [Loop If]
		if(__NFUN_150__(idx, 12))
		{
			Debug(__NFUN_112__("info.CommandList[idx] : ", Info.CommandList[idx]));
			Class'NWindow.UIAPI_EDITBOX'.static.SetString(__NFUN_112__("MacroEditWnd.txtEdit", string(idx)), Info.CommandList[idx]);
			__NFUN_165__(idx);
			// [Loop Continue]
			goto J0xF8;
		}
	}
	return;
}

function SaveMacro()
{
	local int idx;
	local MacroInfoWnd script;
	local string Name, IconName, Description, Command;
	local array<string> CommandList;

	script = MacroInfoWnd(GetScript("MacroInfoWnd"));
	Name = Class'NWindow.UIAPI_EDITBOX'.static.GetString("MacroEditWnd.txtName");
	IconName = Class'NWindow.UIAPI_EDITBOX'.static.GetString("MacroEditWnd.txtShortName");
	Description = script.GetInfoText();
	idx = 0;
	J0x96:

	// End:0x102 [Loop If]
	if(__NFUN_150__(idx, 12))
	{
		Command = Class'NWindow.UIAPI_EDITBOX'.static.GetString(__NFUN_112__("MacroEditWnd.txtEdit", string(idx)));
		CommandList.Insert(CommandList.Length, 1);
		CommandList[__NFUN_147__(CommandList.Length, 1)] = Command;
		__NFUN_165__(idx);
		// [Loop Continue]
		goto J0x96;
	}
	// End:0x152
	if(Class'NWindow.MacroAPI'.static.RequestMakeMacro(m_CurMacroID, Name, IconName, __NFUN_147__(m_CurIconNum, 1), Description, CommandList))
	{
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("MacroEditWnd");
	}
	return;
}
