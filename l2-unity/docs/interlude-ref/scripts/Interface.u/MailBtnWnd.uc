class MailBtnWnd extends UICommonAPI;

function OnLoad()
{
	RegisterEvent(1530);
	return;
}

function OnEvent(int Event_ID, string param)
{
	local int iEffectNumber;

	ParseInt(param, "IdxMail", iEffectNumber);
	switch(Event_ID)
	{
		// End:0x65
		case 1530:
			ShowWindowWithFocus("MailBtnWnd");
			Class'NWindow.UIAPI_EFFECTBUTTON'.static.BeginEffect("MailBtnWnd.btnMail", iEffectNumber);
			// End:0x68
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function OnClickButton(string strID)
{
	switch(strID)
	{
		// End:0x28
		case "btnMail":
			HideWindow("MailBtnWnd");
			// End:0x2B
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}
