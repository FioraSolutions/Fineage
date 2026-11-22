class MacroInfoWnd extends UICommonAPI;

var bool m_bShow;
var string m_strInfo;

function OnLoad()
{
	m_bShow = false;
	m_strInfo = "";
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
		// End:0x1A
		case "btnOk":
			OnClickOk();
			// End:0x34
			break;
		// End:0x31
		case "btnCancel":
			OnClickCancel();
			// End:0x34
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function SetInfoText(string strInfo)
{
	m_strInfo = strInfo;
	Class'NWindow.UIAPI_MULTIEDITBOX'.static.SetString("MacroInfoWnd.txtInfo", strInfo);
	return;
}

function string GetInfoText()
{
	m_strInfo = Class'NWindow.UIAPI_MULTIEDITBOX'.static.GetString("MacroInfoWnd.txtInfo");
	return m_strInfo;
	return;
}

function OnClickOk()
{
	local string tempStr;

	tempStr = Class'NWindow.UIAPI_MULTIEDITBOX'.static.GetString("MacroInfoWnd.txtInfo");
	// End:0x87
	if(__NFUN_151__(__NFUN_125__(tempStr), 32))
	{
		Class'NWindow.UIAPI_MULTIEDITBOX'.static.SetString("MacroInfoWnd.txtInfo", __NFUN_128__(tempStr, 32));
		tempStr = GetSystemMessage(837);
		DialogShow(5, tempStr);
		return;
	}
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("MacroInfoWnd");
	return;
}

function OnClickCancel()
{
	Class'NWindow.UIAPI_MULTIEDITBOX'.static.SetString("MacroInfoWnd.txtInfo", m_strInfo);
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("MacroInfoWnd");
	return;
}

function Clear()
{
	m_strInfo = "";
	Class'NWindow.UIAPI_MULTIEDITBOX'.static.SetString("MacroInfoWnd.txtInfo", "");
	return;
}
