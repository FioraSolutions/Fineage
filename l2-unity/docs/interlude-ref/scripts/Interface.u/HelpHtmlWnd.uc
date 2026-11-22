class HelpHtmlWnd extends UIScript;

var bool m_bShow;

function OnLoad()
{
	RegisterState("HelpHtmlWnd", "GamingState");
	RegisterState("HelpHtmlWnd", "LoginState");
	RegisterEvent(1210);
	RegisterEvent(1220);
	m_bShow = false;
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

function OnEvent(int Event_ID, string param)
{
	// End:0x1D
	if(__NFUN_154__(Event_ID, 1210))
	{
		HandleShowHelp(param);		
	}
	else
	{
		// End:0x37
		if(__NFUN_154__(Event_ID, 1220))
		{
			HandleLoadHelpHtml(param);
		}
	}
	return;
}

function HandleShowHelp(string param)
{
	local string strPath;

	// End:0x30
	if(m_bShow)
	{
		PlayConsoleSound(6);
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("HelpHtmlWnd");		
	}
	else
	{
		ParseString(param, "FilePath", strPath);
		// End:0xC3
		if(__NFUN_151__(__NFUN_125__(strPath), 0))
		{
			Class'NWindow.UIAPI_HTMLCTRL'.static.LoadHtml("HelpHtmlWnd.HtmlViewer", strPath);
			PlayConsoleSound(5);
			Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("HelpHtmlWnd");
			Class'NWindow.UIAPI_WINDOW'.static.SetFocus("HelpHtmlWnd");
		}
	}
	return;
}

function HandleLoadHelpHtml(string param)
{
	local string strHtml;

	ParseString(param, "HtmlString", strHtml);
	// End:0x55
	if(__NFUN_151__(__NFUN_125__(strHtml), 0))
	{
		Class'NWindow.UIAPI_HTMLCTRL'.static.LoadHtmlFromString("HelpHtmlWnd.HtmlViewer", strHtml);
	}
	return;
}
