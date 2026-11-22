class SystemMsgWnd extends UIScript;

function OnLoad()
{
	RegisterEvent(150);
	return;
}

function OnEvent(int a_EventID, string a_Param)
{
	switch(a_EventID)
	{
		// End:0x69
		case 150:
			// End:0x49
			if(GetOptionBool("Game", "SystemMsgWnd"))
			{
				Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("SystemMsgWnd");				
			}
			else
			{
				Class'NWindow.UIAPI_WINDOW'.static.HideWindow("SystemMsgWnd");
			}
			// End:0x6F
			break;
		// End:0xFFFF
		default:
			// End:0x6F
			break;
			break;
	}
	return;
}

function OnShow()
{
	Class'NWindow.UIAPI_WINDOW'.static.SetAnchor("ChatFilterWnd", "SystemMsgWnd", "TopLeft", "BottomLeft", 0, -5);
	ChangeAnchorEffectButton("SystemMsgWnd");
	return;
}

function OnHide()
{
	Class'NWindow.UIAPI_WINDOW'.static.SetAnchor("ChatFilterWnd", "ChatWnd", "TopLeft", "BottomLeft", 0, -5);
	ChangeAnchorEffectButton("ChatWnd");
	return;
}

function ChangeAnchorEffectButton(string strID)
{
	Class'NWindow.UIAPI_WINDOW'.static.SetAnchor("TutorialBtnWnd", strID, "TopLeft", "BottomLeft", 5, -5);
	Class'NWindow.UIAPI_WINDOW'.static.SetAnchor("QuestBtnWnd", strID, "TopLeft", "BottomLeft", 42, -5);
	Class'NWindow.UIAPI_WINDOW'.static.SetAnchor("MailBtnWnd", strID, "TopLeft", "BottomLeft", 79, -5);
	return;
}
