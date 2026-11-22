class ChatFilterWnd extends UIScript;

function OnClickButton(string strID)
{
	// End:0x3F
	if(__NFUN_122__(strID, "ChatFilterOK"))
	{
		SaveChatFilterOption();
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ChatFilterWnd");		
	}
	else
	{
		// End:0x79
		if(__NFUN_122__(strID, "ChatFilterCancel"))
		{
			Class'NWindow.UIAPI_WINDOW'.static.HideWindow("ChatFilterWnd");
		}
	}
	return;
}

function OnClickCheckBox(string strID)
{
	local ChatWnd script;
	local int chatType;

	script = ChatWnd(GetScript("ChatWnd"));
	chatType = script.m_chatType;
	// End:0x134
	if(__NFUN_122__(strID, "CheckBoxSystem"))
	{
		// End:0xD7
		if(__NFUN_129__(Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("ChatFilterWnd.CheckBoxSystem")))
		{
			Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ChatFilterWnd.CheckBoxDamage", true);
			Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ChatFilterWnd.CheckBoxItem", true);			
		}
		else
		{
			Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ChatFilterWnd.CheckBoxDamage", false);
			Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ChatFilterWnd.CheckBoxItem", false);
		}		
	}
	else
	{
		// End:0x219
		if(__NFUN_122__(strID, "SystemMsgBox"))
		{
			// End:0x1CB
			if(Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("ChatFilterWnd.SystemMsgBox"))
			{
				Class'NWindow.UIAPI_CHECKBOX'.static.EnableWindow("ChatFilterWnd.DamageBox");
				Class'NWindow.UIAPI_CHECKBOX'.static.EnableWindow("ChatFilterWnd.ItemBox");				
			}
			else
			{
				Class'NWindow.UIAPI_CHECKBOX'.static.DisableWindow("ChatFilterWnd.DamageBox");
				Class'NWindow.UIAPI_CHECKBOX'.static.DisableWindow("ChatFilterWnd.ItemBox");
			}
		}
	}
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ChatFilterWnd.CheckBoxNormal", false);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ChatFilterWnd.CheckBoxShout", false);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ChatFilterWnd.CheckBoxPledge", false);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ChatFilterWnd.CheckBoxParty", false);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ChatFilterWnd.CheckBoxTrade", false);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ChatFilterWnd.CheckBoxWhisper", false);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ChatFilterWnd.CheckBoxAlly", false);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ChatFilterWnd.CheckBoxHero", false);
	Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ChatFilterWnd.CheckBoxUnion", false);
	switch(chatType)
	{
		// End:0x3F4
		case script.1:
			Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ChatFilterWnd.CheckBoxTrade", true);
			// End:0x4B4
			break;
		// End:0x432
		case script.2:
			Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ChatFilterWnd.CheckBoxParty", true);
			// End:0x4B4
			break;
		// End:0x471
		case script.3:
			Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ChatFilterWnd.CheckBoxPledge", true);
			// End:0x4B4
			break;
		// End:0x4AE
		case script.4:
			Class'NWindow.UIAPI_CHECKBOX'.static.SetDisable("ChatFilterWnd.CheckBoxAlly", true);
			// End:0x4B4
			break;
		// End:0xFFFF
		default:
			// End:0x4B4
			break;
			break;
	}
	return;
}

function SaveChatFilterOption()
{
	local ChatWnd script;
	local int chatType;
	local bool bChecked;

	script = ChatWnd(GetScript("ChatWnd"));
	chatType = script.m_chatType;
	script.m_filterInfo[chatType].bSystem = int(Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("ChatFilterWnd.CheckBoxSystem"));
	script.m_filterInfo[chatType].bUseitem = int(Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("ChatFilterWnd.CheckBoxItem"));
	script.m_filterInfo[chatType].bDamage = int(Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("ChatFilterWnd.CheckBoxDamage"));
	script.m_filterInfo[chatType].bChat = int(Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("ChatFilterWnd.CheckBoxChat"));
	script.m_filterInfo[chatType].bNormal = int(Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("ChatFilterWnd.CheckBoxNormal"));
	script.m_filterInfo[chatType].bParty = int(Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("ChatFilterWnd.CheckBoxParty"));
	script.m_filterInfo[chatType].bShout = int(Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("ChatFilterWnd.CheckBoxShout"));
	script.m_filterInfo[chatType].bTrade = int(Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("ChatFilterWnd.CheckBoxTrade"));
	script.m_filterInfo[chatType].bClan = int(Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("ChatFilterWnd.CheckBoxPledge"));
	script.m_filterInfo[chatType].bWhisper = int(Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("ChatFilterWnd.CheckBoxWhisper"));
	script.m_filterInfo[chatType].bAlly = int(Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("ChatFilterWnd.CheckBoxAlly"));
	script.m_filterInfo[chatType].bHero = int(Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("ChatFilterWnd.CheckBoxHero"));
	script.m_filterInfo[chatType].bUnion = int(Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("ChatFilterWnd.CheckBoxUnion"));
	script.m_NoUnionCommanderMessage = int(Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("ChatFilterWnd.CheckBoxCommand"));
	bChecked = Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("ChatFilterWnd.SystemMsgBox");
	SetOptionBool("Game", "SystemMsgWnd", bChecked);
	bChecked = Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("ChatFilterWnd.DamageBox");
	SetOptionBool("Game", "SystemMsgWndDamage", bChecked);
	bChecked = Class'NWindow.UIAPI_CHECKBOX'.static.IsChecked("ChatFilterWnd.ItemBox");
	SetOptionBool("Game", "SystemMsgWndExpendableItem", bChecked);
	// End:0x556
	if(GetOptionBool("Game", "SystemMsgWnd"))
	{
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("SystemMsgWnd");		
	}
	else
	{
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("SystemMsgWnd");
	}
	SetINIBool(script.m_sectionName[chatType], "system", bool(script.m_filterInfo[chatType].bSystem), "chatfilter.ini");
	SetINIBool(script.m_sectionName[chatType], "damage", bool(script.m_filterInfo[chatType].bDamage), "chatfilter.ini");
	SetINIBool(script.m_sectionName[chatType], "useitems", bool(script.m_filterInfo[chatType].bUseitem), "chatfilter.ini");
	SetINIBool(script.m_sectionName[chatType], "chat", bool(script.m_filterInfo[chatType].bChat), "chatfilter.ini");
	SetINIBool(script.m_sectionName[chatType], "normal", bool(script.m_filterInfo[chatType].bNormal), "chatfilter.ini");
	SetINIBool(script.m_sectionName[chatType], "party", bool(script.m_filterInfo[chatType].bParty), "chatfilter.ini");
	SetINIBool(script.m_sectionName[chatType], "shout", bool(script.m_filterInfo[chatType].bShout), "chatfilter.ini");
	SetINIBool(script.m_sectionName[chatType], "market", bool(script.m_filterInfo[chatType].bTrade), "chatfilter.ini");
	SetINIBool(script.m_sectionName[chatType], "pledge", bool(script.m_filterInfo[chatType].bClan), "chatfilter.ini");
	SetINIBool(script.m_sectionName[chatType], "tell", bool(script.m_filterInfo[chatType].bWhisper), "chatfilter.ini");
	SetINIBool(script.m_sectionName[chatType], "ally", bool(script.m_filterInfo[chatType].bAlly), "chatfilter.ini");
	SetINIBool(script.m_sectionName[chatType], "hero", bool(script.m_filterInfo[chatType].bHero), "chatfilter.ini");
	SetINIBool(script.m_sectionName[chatType], "union", bool(script.m_filterInfo[chatType].bUnion), "chatfilter.ini");
	SetINIBool("global", "command", bool(script.m_NoUnionCommanderMessage), "chatfilter.ini");
	return;
}
