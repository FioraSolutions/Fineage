class QuestBtnWnd extends UICommonAPI;

function OnLoad()
{
	RegisterEvent(1520);
	return;
}

function OnEvent(int Event_ID, string param)
{
	local int iEffectNumber;

	ParseInt(param, "QuestID", iEffectNumber);
	switch(Event_ID)
	{
		// End:0x8D
		case 1520:
			ShowWindowWithFocus("QuestBtnWnd");
			Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("QuestBtnWnd.btnQuest");
			Class'NWindow.UIAPI_EFFECTBUTTON'.static.BeginEffect("QuestBtnWnd.btnQuest", iEffectNumber);
			// End:0x90
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
		// End:0x2A
		case "btnQuest":
			HideWindow("QuestBtnWnd");
			// End:0x2D
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}
