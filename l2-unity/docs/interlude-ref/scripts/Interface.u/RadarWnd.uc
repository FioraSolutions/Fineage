class RadarWnd extends UIScript;

var bool onshowstat1;
var bool onshowstat2;
var int globalAlphavalue1;
var int globalyloc;
var float numberstrange;
var int global_move_val;

function SetRadarColor(Color a_RadarColor, float a_Seconds)
{
	Class'NWindow.UIAPI_RADAR'.static.SetRadarColor("RadarContainerWnd.Radar", a_RadarColor, a_Seconds);
	return;
}

function OnLoad()
{
	RegisterEvent(110);
	onshowstat1 = false;
	onshowstat2 = false;
	globalAlphavalue1 = 0;
	globalyloc = 0;
	numberstrange = 0.0000000;
	global_move_val = 0;
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("movingtext");
	HideAllIcons();
	init_textboxmove();
	return;
}

function OnShow()
{
	HideAllIcons();
	return;
}

function OnTick()
{
	// End:0x12
	if(__NFUN_242__(onshowstat2, true))
	{
		FadeIn();
	}
	// End:0x24
	if(__NFUN_242__(onshowstat1, true))
	{
		FadeOut();
	}
	return;
}

function FadeIn()
{
	globalAlphavalue1 = __NFUN_146__(globalAlphavalue1, 3);
	// End:0x3E
	if(__NFUN_150__(globalAlphavalue1, 255))
	{
		Class'NWindow.UIAPI_WINDOW'.static.SetAlpha("movingtext", globalAlphavalue1);		
	}
	else
	{
		Class'NWindow.UIAPI_WINDOW'.static.SetAlpha("movingtext", 255);
		onshowstat1 = true;
		onshowstat2 = false;
	}
	return;
}

function FadeOut()
{
	globalAlphavalue1 = __NFUN_147__(globalAlphavalue1, 2);
	globalyloc = __NFUN_146__(globalyloc, 1);
	// End:0x4B
	if(__NFUN_151__(globalAlphavalue1, 1))
	{
		Class'NWindow.UIAPI_WINDOW'.static.SetAlpha("movingtext", globalAlphavalue1);		
	}
	else
	{
		Class'NWindow.UIAPI_WINDOW'.static.SetAlpha("movingtext", 0);
		Class'NWindow.UIAPI_WINDOW'.static.HideWindow("movingtext");
		globalyloc = 0;
		global_move_val = 0;
		Debug(__NFUN_112__("thisisrunningright:", string(__NFUN_145__(globalyloc, 2))));
		globalAlphavalue1 = 0;
		onshowstat1 = false;
		onshowstat2 = false;
	}
	return;
}

function int move_value()
{
	local int movevalue;

	numberstrange = __NFUN_174__(numberstrange, 0.5000000);
	// End:0x2C
	if(__NFUN_176__(numberstrange, float(1)))
	{
		movevalue = 0;
		return movevalue;
	}
	// End:0x55
	if(__NFUN_180__(numberstrange, float(1)))
	{
		movevalue = -1;
		numberstrange = 0.0000000;
		return movevalue;
	}
	return;
}

function resetanimloc()
{
	onshowstat1 = false;
	onshowstat2 = false;
	globalyloc = 0;
	global_move_val = 0;
	globalAlphavalue1 = 0;
	return;
}

function HideAllIcons()
{
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("RadarWnd.icon1");
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("RadarWnd.icon2");
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("RadarWnd.icon3");
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("RadarWnd.icon4");
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("RadarWnd.icon5");
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("RadarWnd.icon6");
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("RadarWnd.icon7");
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("RadarWnd.icon8");
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("RadarWnd.icon9");
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("RadarWnd");
	return;
}

function OnEvent(int Event_ID, string a_Param)
{
	local int Type;
	local Color Red, Blue, Grey, Orange, Green;

	Red.R = 50;
	Red.G = 0;
	Red.B = 0;
	Blue.R = 0;
	Blue.G = 0;
	Blue.B = 50;
	Grey.R = 30;
	Grey.G = 30;
	Grey.B = 30;
	Orange.R = 60;
	Orange.G = 30;
	Orange.B = 0;
	Green.R = 0;
	Green.G = 50;
	Green.B = 0;
	// End:0x3A0
	if(__NFUN_154__(Event_ID, 110))
	{
		ParseInt(a_Param, "ZoneCode", Type);
		resetanimloc();
		onshowstat2 = true;
		Class'NWindow.UIAPI_WINDOW'.static.SetAlpha("movingtext", 0);
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("movingtext");
		switch(Type)
		{
			// End:0x185
			case 15:
				HideAllIcons();
				Class'NWindow.UIAPI_TEXTBOX'.static.SetText("movingtext.textboxmove", GetSystemString(1284));
				SetRadarColor(Grey, 2.0000000);
				// End:0x3A0
				break;
			// End:0x1D5
			case 12:
				HideAllIcons();
				Class'NWindow.UIAPI_TEXTBOX'.static.SetText("movingtext.textboxmove", GetSystemString(1285));
				SetRadarColor(Blue, 2.0000000);
				// End:0x3A0
				break;
			// End:0x225
			case 11:
				HideAllIcons();
				Class'NWindow.UIAPI_TEXTBOX'.static.SetText("movingtext.textboxmove", GetSystemString(1286));
				SetRadarColor(Orange, 2.0000000);
				// End:0x3A0
				break;
			// End:0x275
			case 9:
				HideAllIcons();
				Class'NWindow.UIAPI_TEXTBOX'.static.SetText("movingtext.textboxmove", GetSystemString(1287));
				SetRadarColor(Red, 2.0000000);
				// End:0x3A0
				break;
			// End:0x2C5
			case 8:
				HideAllIcons();
				Class'NWindow.UIAPI_TEXTBOX'.static.SetText("movingtext.textboxmove", GetSystemString(1288));
				SetRadarColor(Red, 2.0000000);
				// End:0x3A0
				break;
			// End:0x34D
			case 13:
				HideAllIcons();
				Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("RadarWnd");
				Class'NWindow.UIAPI_WINDOW'.static.ShowWindow("RadarWnd.icon6");
				Class'NWindow.UIAPI_TEXTBOX'.static.SetText("movingtext.textboxmove", GetSystemString(1289));
				SetRadarColor(Grey, 2.0000000);
				// End:0x3A0
				break;
			// End:0x39D
			case 14:
				HideAllIcons();
				Class'NWindow.UIAPI_TEXTBOX'.static.SetText("movingtext.textboxmove", GetSystemString(1290));
				SetRadarColor(Green, 2.0000000);
				// End:0x3A0
				break;
			// End:0xFFFF
			default:
				break;
		}
	}
	return;
}

function init_textboxmove()
{
	Class'NWindow.UIAPI_TEXTBOX'.static.SetText("movingtext.textboxmove", "");
	return;
}
