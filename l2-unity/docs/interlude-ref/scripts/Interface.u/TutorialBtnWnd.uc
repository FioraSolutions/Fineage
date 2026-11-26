class TutorialBtnWnd extends UICommonAPI;

function OnLoad()
{
	RegisterEvent(1510);
	return;
}

function OnEvent(int Event_ID, string param)
{
	local int iEffectNumber;

	ParseInt(param, "QuestionID", iEffectNumber);
	switch(Event_ID)
	{
		// End:0x74
		case 1510:
			ShowWindowWithFocus("TutorialBtnWnd");
			Class'NWindow.UIAPI_EFFECTBUTTON'.static.BeginEffect("TutorialBtnWnd.btnTutorial", iEffectNumber);
			// End:0x77
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
		// End:0x30
		case "btnTutorial":
			HideWindow("TutorialBtnWnd");
			// End:0x33
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}
