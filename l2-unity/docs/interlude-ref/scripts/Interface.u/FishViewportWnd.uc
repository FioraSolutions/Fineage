class FishViewportWnd extends UICommonAPI;

function OnLoad()
{
	RegisterEvent(2470);
	RegisterEvent(2480);
	RegisterEvent(2510);
	RegisterEvent(2520);
	return;
}

function OnEvent(int Event_ID, string param)
{
	switch(Event_ID)
	{
		// End:0x29
		case 2470:
			ShowWindowWithFocus("FishViewportWnd");
			// End:0xA8
			break;
		// End:0x4B
		case 2480:
			HideWindow("FishViewportWnd");
			// End:0xA8
			break;
		// End:0x78
		case 2510:
			ShowWindow("FishViewportWnd.btnRanking");
			// End:0xA8
			break;
		// End:0xA5
		case 2520:
			HideWindow("FishViewportWnd.btnRanking");
			// End:0xA8
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
		// End:0x1F
		case "btnRanking":
			RequestFishRanking();
			// End:0x22
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}
