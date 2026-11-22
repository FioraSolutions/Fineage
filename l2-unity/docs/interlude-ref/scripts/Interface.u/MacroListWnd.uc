class MacroListWnd extends UICommonAPI;

const MACRO_MAX_COUNT = 24;

var bool m_bShow;
var int m_DeleteID;
var int m_Max;

function OnLoad()
{
	RegisterEvent(1710);
	RegisterEvent(1230);
	RegisterEvent(1240);
	RegisterEvent(1250);
	m_bShow = false;
	m_DeleteID = 0;
	return;
}

function OnEnterState(name a_PreStateName)
{
	Class'NWindow.MacroAPI'.static.RequestMacroList();
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
		case "btnHelp":
			OnClickHelp();
			// End:0x33
			break;
		// End:0x30
		case "btnAdd":
			OnClickAdd();
			// End:0x33
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnEvent(int Event_ID, string param)
{
	// End:0x18
	if(__NFUN_154__(Event_ID, 1230))
	{
		HandleMacroShowListWnd();		
	}
	else
	{
		// End:0x30
		if(__NFUN_154__(Event_ID, 1240))
		{
			HandleMacroUpdate();			
		}
		else
		{
			// End:0x4D
			if(__NFUN_154__(Event_ID, 1250))
			{
				HandleMacroList(param);				
			}
			else
			{
				// End:0x9E
				if(__NFUN_154__(Event_ID, 1710))
				{
					// End:0x9E
					if(DialogIsMine())
					{
						// End:0x9E
						if(__NFUN_151__(m_DeleteID, 0))
						{
							Class'NWindow.MacroAPI'.static.RequestDeleteMacro(m_DeleteID);
							m_DeleteID = 0;
							// End:0x9E
							if(__NFUN_154__(m_Max, 1))
							{
								HandleMacroList("");
							}
						}
					}
				}
			}
		}
	}
	return;
}

function OnClickItem(string strID, int Index)
{
	local ItemInfo infItem;

	// End:0x73
	if(__NFUN_130__(__NFUN_122__(strID, "MacroItem"), __NFUN_151__(Index, -1)))
	{
		// End:0x73
		if(Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("MacroListWnd.MacroItem", Index, infItem))
		{
			Class'NWindow.MacroAPI'.static.RequestUseMacro(infItem.ClassID);
		}
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

function OnClickAdd()
{
	Class'NWindow.UIAPI_MULTIEDITBOX'.static.SetString("MacroInfoWnd.txtInfo", "");
	ExecuteEvent(1260, "");
	return;
}

function HandleMacroUpdate()
{
	Class'NWindow.MacroAPI'.static.RequestMacroList();
	return;
}

function HandleMacroShowListWnd()
{
	// End:0x31
	if(m_bShow)
	{
		PlayConsoleSound(6);
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("MacroListWnd");		
	}
	else
	{
		PlayConsoleSound(5);
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("MacroListWnd");
		Class'NWindow.UIAPI_WINDOW'.static.SetFocus("MacroListWnd");
	}
	return;
}

function Clear()
{
	Class'NWindow.UIAPI_ITEMWINDOW'.static.Clear("MacroListWnd.MacroItem");
	return;
}

function HandleMacroList(string param)
{
	local int idx, Max, MacroID;
	local string strIconName, strMacroName, strDescription, strTexture, strTmp;

	local ItemInfo infItem;

	Clear();
	ParseInt(param, "Max", Max);
	m_Max = Max;
	idx = 0;
	J0x2D:

	// End:0x1AD [Loop If]
	if(__NFUN_150__(idx, Max))
	{
		MacroID = 0;
		strIconName = "";
		strMacroName = "";
		strDescription = "";
		strTexture = "";
		ParseInt(param, __NFUN_112__("ID_", string(idx)), MacroID);
		ParseString(param, __NFUN_112__("IconName_", string(idx)), strIconName);
		ParseString(param, __NFUN_112__("MacroName_", string(idx)), strMacroName);
		ParseString(param, __NFUN_112__("Description_", string(idx)), strDescription);
		ParseString(param, __NFUN_112__("TextureName_", string(idx)), strTexture);
		infItem.ClassID = MacroID;
		infItem.Name = strMacroName;
		infItem.AdditionalName = strIconName;
		infItem.IconName = strTexture;
		infItem.Description = strDescription;
		infItem.ItemSubType = int(4);
		Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem("MacroListWnd.MacroItem", infItem);
		__NFUN_165__(idx);
		// [Loop Continue]
		goto J0x2D;
	}
	// End:0x1C9
	if(__NFUN_150__(Max, 10))
	{
		strTmp = __NFUN_112__(strTmp, "0");
	}
	strTmp = __NFUN_112__(strTmp, string(Max));
	strTmp = __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("(", strTmp), "/"), string(24)), ")");
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("MacroListWnd.txtCount", strTmp);
	return;
}

function OnDropItem(string strID, ItemInfo infItem, int X, int Y)
{
	switch(strID)
	{
		// End:0x22
		case "btnTrash":
			DeleteMacro(infItem);
			// End:0x3F
			break;
		// End:0x3C
		case "btnEdit":
			EditMacro(infItem);
			// End:0x3F
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function DeleteMacro(ItemInfo infItem)
{
	local string strMsg;

	// End:0x15
	if(__NFUN_155__(infItem.ItemSubType, int(4)))
	{
		return;
	}
	strMsg = MakeFullSystemMsg(GetSystemMessage(828), infItem.Name, "");
	m_DeleteID = infItem.ClassID;
	DialogShow(4, strMsg);
	return;
}

function EditMacro(ItemInfo infItem)
{
	local int MacroID;
	local string param;

	// End:0x15
	if(__NFUN_155__(infItem.ItemSubType, int(4)))
	{
		return;
	}
	MacroID = infItem.ClassID;
	ParamAdd(param, "MacroID", string(MacroID));
	ExecuteEvent(1260, param);
	return;
}
