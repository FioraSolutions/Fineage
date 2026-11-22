class PetitionWnd extends UICommonAPI;

function OnLoad()
{
	RegisterEvent(1920);
	RegisterEvent(1930);
	RegisterEvent(1940);
	SetFeedBackEnable(false);
	return;
}

function OnHide()
{
	SetFeedBackEnable(false);
	return;
}

function SetFeedBackEnable(bool a_IsEnabled)
{
	// End:0x37
	if(a_IsEnabled)
	{
		Class'NWindow.UIAPI_BUTTON'.static.EnableWindow("PetitionWnd.FeedBackButton");		
	}
	else
	{
		Class'NWindow.UIAPI_BUTTON'.static.DisableWindow("PetitionWnd.FeedBackButton");
	}
	return;
}

function OnEvent(int a_EventID, string a_Param)
{
	switch(a_EventID)
	{
		// End:0x18
		case 1920:
			HandleShowPetitionWnd();
			// End:0x47
			break;
		// End:0x2E
		case 1930:
			HandlePetitionChatMessage(a_Param);
			// End:0x47
			break;
		// End:0x44
		case 1940:
			HandleEnablePetitionFeedback(a_Param);
			// End:0x47
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnCompleteEditBox(string strID)
{
	local string Message;

	switch(strID)
	{
		// End:0x63
		case "PetitionChatEditBox":
			Message = Class'NWindow.UIAPI_EDITBOX'.static.GetString("PetitionWnd.PetitionChatEditBox");
			ProcessPetitionChatMessage(Message);
			// End:0x66
			break;
		// End:0xFFFF
		default:
			break;
	}
	Class'NWindow.UIAPI_EDITBOX'.static.Clear("PetitionWnd.PetitionChatEditBox");
	return;
}

function HandleShowPetitionWnd()
{
	// End:0x24
	if(m_hOwnerWnd.IsMinimizedWindow())
	{
		m_hOwnerWnd.NotifyAlarm();		
	}
	else
	{
		ShowWindow("PetitionWnd");
		m_hOwnerWnd.SetFocus();
	}
	return;
}

function HandlePetitionChatMessage(string a_Param)
{
	local string chatMessage;
	local Color ChatColor;
	local int ColorR, ColorG, ColorB, ColorA;

	// End:0x111
	if(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_130__(ParseString(a_Param, "Message", chatMessage), ParseInt(a_Param, "ColorR", ColorR)), ParseInt(a_Param, "ColorG", ColorG)), ParseInt(a_Param, "ColorB", ColorB)), ParseInt(a_Param, "ColorA", ColorA)))
	{
		ChatColor.R = byte(ColorR);
		ChatColor.G = byte(ColorG);
		ChatColor.B = byte(ColorB);
		ChatColor.A = byte(ColorA);
		Class'NWindow.UIAPI_TEXTLISTBOX'.static.AddString("PetitionWnd.PetitionChatWindow", chatMessage, ChatColor);
	}
	return;
}

function HandleEnablePetitionFeedback(string a_Param)
{
	local int Enable;

	// End:0x37
	if(ParseInt(a_Param, "Enable", Enable))
	{
		// End:0x30
		if(__NFUN_154__(1, Enable))
		{
			SetFeedBackEnable(true);			
		}
		else
		{
			SetFeedBackEnable(false);
		}
	}
	return;
}

function OnClickButton(string a_ControlID)
{
	switch(a_ControlID)
	{
		// End:0x23
		case "FeedBackButton":
			OnClickFeedBackButton();
			// End:0x40
			break;
		// End:0x3D
		case "CancelButton":
			OnClickCancelButton();
			// End:0x40
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnClickFeedBackButton()
{
	Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("PetitionFeedBackWnd");
	return;
}

function OnClickCancelButton()
{
	SetFeedBackEnable(false);
	Class'NWindow.PetitionAPI'.static.RequestPetitionCancel();
	return;
}

function Clear()
{
	Class'NWindow.UIAPI_TEXTLISTBOX'.static.Clear("PetitionWnd.PetitionChatWindow");
	return;
}
