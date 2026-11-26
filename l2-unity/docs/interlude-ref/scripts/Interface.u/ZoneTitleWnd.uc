class ZoneTitleWnd extends UICommonAPI;

const StartZoneNameX = 100;
const StartZoneNameY = 80;
const TIMER_ID = 0;
const TIMER_DELAY = 4000;

var int A;

function OnLoad()
{
	RegisterEvent(2420);
	return;
}

function OnEvent(int Event_ID, string param)
{
	local string ZoneName, SubZoneName1, SubZoneName2;

	switch(Event_ID)
	{
		// End:0x7D
		case 2420:
			ParseString(param, "ZoneName", ZoneName);
			ParseString(param, "SubZoneName1", SubZoneName1);
			ParseString(param, "SubZoneName2", SubZoneName2);
			BeginShowZoneName(ZoneName, SubZoneName1, SubZoneName2);
			// End:0x80
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function BeginShowZoneName(string ZoneName, string SubZoneName1, string SubZoneName2)
{
	local int TextWidth, TextHeight, ScreenWidth, ScreenHeight;

	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("textZoneNameBack", ZoneName);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("textZoneNameFront", ZoneName);
	GetZoneNameTextSize(ZoneName, TextWidth, TextHeight);
	GetCurrentResolution(ScreenWidth, ScreenHeight);
	Class'NWindow.UIAPI_WINDOW'.static.SetWindowSize("ZoneTitleWnd", __NFUN_146__(TextWidth, 100), 200);
	Class'NWindow.UIAPI_WINDOW'.static.MoveTo("ZoneTitleWnd", __NFUN_147__(__NFUN_147__(__NFUN_145__(ScreenWidth, 2), __NFUN_145__(TextWidth, 2)), 100), __NFUN_147__(__NFUN_145__(ScreenHeight, 5), 80));
	ShowWindow("ZoneTitleWnd");
	Class'NWindow.UIAPI_WINDOW'.static.SetUITimer("ZoneTitleWnd", 0, 4000);
	return;
}

function OnTimer(int TimerID)
{
	Class'NWindow.UIAPI_WINDOW'.static.KillUITimer("ZoneTitleWnd", TimerID);
	HideWindow("ZoneTitleWnd");
	return;
}
