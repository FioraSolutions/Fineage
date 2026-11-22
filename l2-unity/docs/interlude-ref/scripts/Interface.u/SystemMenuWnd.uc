class SystemMenuWnd extends UICommonAPI;

function OnLoad()
{
	RegisterEvent(1710);
	RegisterEvent(1900);
	SetMenuString();
	return;
}

function OnClickButton(string strID)
{
	switch(strID)
	{
		// End:0x1B
		case "btnBBS":
			HandleShowBoardWnd();
			// End:0xD8
			break;
		// End:0x31
		case "btnMacro":
			HandleShowMacroListWnd();
			// End:0xD8
			break;
		// End:0x4A
		case "btnHelpHtml":
			HandleShowHelpHtmlWnd();
			// End:0xD8
			break;
		// End:0x63
		case "btnPetition":
			HandleShowPetitionBegin();
			// End:0xD8
			break;
		// End:0x7A
		case "btnOption":
			HandleShowOptionWnd();
			// End:0xD8
			break;
		// End:0xA9
		case "btnRestart":
			DialogHide();
			DialogShow(4, GetSystemMessage(126));
			DialogSetID(0);
			// End:0xD8
			break;
		// End:0xD5
		case "btnQuit":
			DialogHide();
			DialogShow(4, GetSystemMessage(125));
			DialogSetID(1);
			// End:0xD8
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnEvent(int Event_ID, string param)
{
	// End:0x54
	if(__NFUN_154__(Event_ID, 1710))
	{
		// End:0x51
		if(DialogIsMine())
		{
			// End:0x4B
			if(__NFUN_154__(DialogGetID(), 0))
			{
				Class'NWindow.UIAPI_WINDOW'.static.HideWindow("SystemMenuWnd");
				ExecRestart();				
			}
			else
			{
				ExecQuit();
			}
		}		
	}
	else
	{
		// End:0x69
		if(__NFUN_154__(Event_ID, 1900))
		{
			SetMenuString();
		}
	}
	return;
}

function HandleShowBoardWnd()
{
	local string strParam;

	ParamAdd(strParam, "Init", "1");
	ExecuteEvent(1190, strParam);
	return;
}

function HandleShowHelpHtmlWnd()
{
	local string strParam;

	ParamAdd(strParam, "FilePath", "..\\L2text\\help.htm");
	ExecuteEvent(1210, strParam);
	return;
}

function HandleShowMacroListWnd()
{
	ExecuteEvent(1230);
	return;
}

function HandleShowPetitionBegin()
{
	// End:0x4E
	if(Class'NWindow.UIAPI_WINDOW'.static.IsShowWindow("UserPetitionWnd"))
	{
		PlayConsoleSound(6);
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("UserPetitionWnd");		
	}
	else
	{
		PlayConsoleSound(5);
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("UserPetitionWnd");
		Class'NWindow.UIAPI_WINDOW'.static.SetFocus("UserPetitionWnd");
	}
	return;
}

function HandleShowOptionWnd()
{
	// End:0x42
	if(Class'NWindow.UIAPI_WINDOW'.static.IsShowWindow("OptionWnd"))
	{
		PlayConsoleSound(6);
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("OptionWnd");		
	}
	else
	{
		PlayConsoleSound(5);
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("OptionWnd");
		Class'NWindow.UIAPI_WINDOW'.static.SetFocus("OptionWnd");
	}
	return;
}

function SetMenuString()
{
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SystemMenuWnd.txtBBS", __NFUN_112__(GetSystemString(387), "(Alt+B)"));
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("SystemMenuWnd.txtMacro", __NFUN_112__(GetSystemString(711), "(Alt+R)"));
	return;
}
