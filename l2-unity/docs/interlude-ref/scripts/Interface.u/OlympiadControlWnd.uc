class OlympiadControlWnd extends UIScript;

function OnLoad()
{
	return;
}

function OnClickButton(string strID)
{
	switch(strID)
	{
		// End:0x25
		case "btnStop":
			Class'NWindow.OlympiadAPI'.static.RequestOlympiadObserverEnd();
			// End:0x4B
			break;
		// End:0x48
		case "btnOtherGame":
			Class'NWindow.OlympiadAPI'.static.RequestOlympiadMatchList();
			// End:0x4B
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}
