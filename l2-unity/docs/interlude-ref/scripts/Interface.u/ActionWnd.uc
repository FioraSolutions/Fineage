class ActionWnd extends UICommonAPI;

var bool m_bShow;

function OnLoad()
{
	RegisterEvent(1300);
	RegisterEvent(1310);
	RegisterEvent(1900);
	m_bShow = false;
	HideScrollBar();
	return;
}

function OnShow()
{
	Class'NWindow.ActionAPI'.static.RequestActionList();
	m_bShow = true;
	return;
}

function OnHide()
{
	m_bShow = false;
	return;
}

function OnEvent(int Event_ID, string param)
{
	// End:0x18
	if(__NFUN_154__(Event_ID, 1300))
	{
		HandleActionListStart();		
	}
	else
	{
		// End:0x35
		if(__NFUN_154__(Event_ID, 1310))
		{
			HandleActionList(param);			
		}
		else
		{
			// End:0x4A
			if(__NFUN_154__(Event_ID, 1900))
			{
				HandleLanguageChanged();
			}
		}
	}
	return;
}

function OnClickItem(string strID, int Index)
{
	local ItemInfo infItem;

	// End:0x6A
	if(__NFUN_130__(__NFUN_122__(strID, "ActionBasicItem"), __NFUN_151__(Index, -1)))
	{
		// End:0x67
		if(__NFUN_129__(Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("ActionWnd.ActionBasicItem", Index, infItem)))
		{
			return;
		}		
	}
	else
	{
		// End:0xD4
		if(__NFUN_130__(__NFUN_122__(strID, "ActionPartyItem"), __NFUN_151__(Index, -1)))
		{
			// End:0xD1
			if(__NFUN_129__(Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("ActionWnd.ActionPartyItem", Index, infItem)))
			{
				return;
			}			
		}
		else
		{
			// End:0x13D
			if(__NFUN_130__(__NFUN_122__(strID, "ActionSocialItem"), __NFUN_151__(Index, -1)))
			{
				// End:0x13D
				if(__NFUN_129__(Class'NWindow.UIAPI_ITEMWINDOW'.static.GetItem("ActionWnd.ActionSocialItem", Index, infItem)))
				{
					return;
				}
			}
		}
	}
	DoAction(infItem.ClassID);
	return;
}

function HideScrollBar()
{
	Class'NWindow.UIAPI_ITEMWINDOW'.static.ShowScrollBar("ActionWnd.ActionBasicItem", false);
	Class'NWindow.UIAPI_ITEMWINDOW'.static.ShowScrollBar("ActionWnd.ActionPartyItem", false);
	Class'NWindow.UIAPI_ITEMWINDOW'.static.ShowScrollBar("ActionWnd.ActionSocialItem", false);
	return;
}

function HandleLanguageChanged()
{
	Class'NWindow.ActionAPI'.static.RequestActionList();
	return;
}

function HandleActionListStart()
{
	Clear();
	return;
}

function Clear()
{
	Class'NWindow.UIAPI_ITEMWINDOW'.static.Clear("ActionWnd.ActionBasicItem");
	Class'NWindow.UIAPI_ITEMWINDOW'.static.Clear("ActionWnd.ActionPartyItem");
	Class'NWindow.UIAPI_ITEMWINDOW'.static.Clear("ActionWnd.ActionSocialItem");
	return;
}

function HandleActionList(string param)
{
	local string WndName;
	local int tmp;
	local UIEventManager.EActionCategory Type;
	local int ActionID;
	local string strActionName, strIconName, strDescription, strCommand;
	local ItemInfo infItem;

	ParseInt(param, "Type", tmp);
	ParseInt(param, "ActionID", ActionID);
	ParseString(param, "Name", strActionName);
	ParseString(param, "IconName", strIconName);
	ParseString(param, "Description", strDescription);
	ParseString(param, "Command", strCommand);
	infItem.ClassID = ActionID;
	infItem.Name = strActionName;
	infItem.IconName = strIconName;
	infItem.Description = strDescription;
	infItem.ItemSubType = int(3);
	infItem.MacroCommand = strCommand;
	Type = byte(tmp);
	// End:0x12C
	if(__NFUN_154__(int(Type), int(1)))
	{
		WndName = "ActionBasicItem";		
	}
	else
	{
		// End:0x156
		if(__NFUN_154__(int(Type), int(2)))
		{
			WndName = "ActionPartyItem";			
		}
		else
		{
			// End:0x181
			if(__NFUN_154__(int(Type), int(3)))
			{
				WndName = "ActionSocialItem";				
			}
			else
			{
				return;
			}
		}
	}
	Class'NWindow.UIAPI_ITEMWINDOW'.static.AddItem(__NFUN_112__("ActionWnd.", WndName), infItem);
	return;
}
