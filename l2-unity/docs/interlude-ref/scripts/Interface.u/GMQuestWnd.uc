class GMQuestWnd extends QuestTreeWnd;

var bool bShow;

function OnLoad()
{
	RegisterEvent(2320);
	RegisterEvent(2330);
	RegisterEvent(2340);
	RegisterEvent(2350);
	bShow = false;
	return;
}

function OnShow()
{
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("GMQuestWnd.InnerWnd.btnClose");
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("GMQuestWnd.InnerWnd.chkNpcPosBox");
	return;
}

function ShowQuest(string a_Param)
{
	// End:0x0E
	if(__NFUN_122__(a_Param, ""))
	{
		return;
	}
	// End:0x31
	if(bShow)
	{
		m_hOwnerWnd.HideWindow();
		bShow = false;		
	}
	else
	{
		Class'NWindow.GMAPI'.static.RequestGMCommand(4, a_Param);
		bShow = true;
	}
	return;
}

function OnClickButton(string strID)
{
	return;
}

function OnEvent(int a_EventID, string a_Param)
{
	switch(a_EventID)
	{
		// End:0x18
		case 2320:
			HandleGMObservingQuestListStart();
			// End:0x58
			break;
		// End:0x2E
		case 2330:
			HandleGMObservingQuestList(a_Param);
			// End:0x58
			break;
		// End:0x3F
		case 2340:
			HandleGMObservingQuestListEnd();
			// End:0x58
			break;
		// End:0x55
		case 2350:
			HandleGMObservingQuestItem(a_Param);
			// End:0x58
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function HandleGMObservingQuestListStart()
{
	m_hOwnerWnd.ShowWindow();
	m_hOwnerWnd.SetFocus();
	HandleQuestListStart();
	return;
}

function HandleGMObservingQuestList(string a_Param)
{
	HandleQuestList(a_Param);
	return;
}

function HandleGMObservingQuestListEnd()
{
	UpdateQuestCount();
	UpdateItemCount(0, -1);
	return;
}

function HandleGMObservingQuestItem(string a_Param)
{
	local int ClassID, ItemCount;

	ParseInt(a_Param, "ClassID", ClassID);
	ParseInt(a_Param, "ItemCount", ItemCount);
	UpdateItemCount(ClassID, ItemCount);
	return;
}

defaultproperties
{
	m_WindowName="GMQuestWnd.InnerWnd"
}