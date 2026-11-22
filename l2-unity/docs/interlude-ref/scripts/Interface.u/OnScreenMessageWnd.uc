class OnScreenMessageWnd extends UIScript;

var string currentwnd1;
var bool onshowstat1;
var bool onshowstat2;
var int timerset1;
var int globalAlphavalue1;
var int globalAlphavalue2;
var int globalDuration;
var int droprate;
var int moveval;
var int moveval2;
var string MovedWndName;
var int m_TimerCount;
var bool linedivided;

function OnLoad()
{
	RegisterEvent(140);
	RegisterEvent(580);
	ResetAllMessage();
	timerset1 = 0;
	moveval = 0;
	moveval2 = 0;
	globalAlphavalue1 = 0;
	globalAlphavalue2 = 255;
	m_TimerCount = 0;
	return;
}

function OnTick()
{
	// End:0x17
	if(__NFUN_242__(onshowstat1, true))
	{
		FadeIn(currentwnd1);
	}
	// End:0x2E
	if(__NFUN_242__(onshowstat2, true))
	{
		FadeOut(currentwnd1);
	}
	return;
}

function OnTimer(int TimerID)
{
	// End:0x55
	if(__NFUN_151__(m_TimerCount, 0))
	{
		Class'NWindow.UIAPI_WINDOW'.static.KillUITimer("OnScreenMessageWnd1", m_TimerCount);
		__NFUN_166__(m_TimerCount);
		// End:0x55
		if(__NFUN_150__(m_TimerCount, 1))
		{
			m_TimerCount = 0;
			onshowstat2 = true;
		}
	}
	return;
}

function OnHide()
{
	return;
}

function ResetAllMessage()
{
	local int i;
	local Color DefaultColor;
	local string WndName;

	DefaultColor.R = byte(255);
	DefaultColor.G = byte(255);
	DefaultColor.B = byte(255);
	globalAlphavalue1 = 0;
	globalAlphavalue2 = 255;
	currentwnd1 = "";
	onshowstat1 = false;
	onshowstat2 = false;
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("OnScreenMessageWnd1");
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("OnScreenMessageWnd2");
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("OnScreenMessageWnd3");
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("OnScreenMessageWnd4");
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("OnScreenMessageWnd5");
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("OnScreenMessageWnd6");
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("OnScreenMessageWnd7");
	Class'NWindow.UIAPI_WINDOW'.static.HideWindow("OnScreenMessageWnd8");
	i = 1;
	J0x17B:

	// End:0x27C [Loop If]
	if(__NFUN_152__(i, 8))
	{
		WndName = __NFUN_112__("OnScreenMessageWnd", string(i));
		Class'NWindow.UIAPI_TEXTBOX'.static.SetTextColor(__NFUN_112__(__NFUN_112__(WndName, ".TextBox"), string(i)), DefaultColor);
		Class'NWindow.UIAPI_TEXTBOX'.static.SetTextColor(__NFUN_112__(__NFUN_112__(__NFUN_112__(WndName, ".TextBox"), string(i)), "-1"), DefaultColor);
		Class'NWindow.UIAPI_TEXTBOX'.static.SetTextColor(__NFUN_112__(__NFUN_112__(WndName, ".TextBoxsm"), string(i)), DefaultColor);
		Class'NWindow.UIAPI_TEXTBOX'.static.SetTextColor(__NFUN_112__(__NFUN_112__(__NFUN_112__(WndName, ".TextBoxsm"), string(i)), "-1"), DefaultColor);
		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x17B;
	}
	return;
}

function ShowMsg(int WndNum, string TextValue, int Duration, int Animation, int FontType, int BackgroundType, int ColorR, int ColorG, int ColorB)
{
	local string WndName, TextBoxName, ShadowBoxName, TextBoxName2, ShadowBoxName2, TextValue1,
		TextValue2, CurText, SmallBoxName1, SmallBoxName2;

	local Color FontColor;
	local int i, j, LengthTotal, TotalLength, TextOffsetTotal1;

	j = 1;
	TotalLength = __NFUN_125__(TextValue);
	TextValue1 = "";
	TextValue2 = "";
	linedivided = false;
	FontColor.R = byte(ColorR);
	FontColor.G = byte(ColorG);
	FontColor.B = byte(ColorB);
	Debug(__NFUN_168__("totalval", TextValue));
	i = 1;
	J0x80:

	// End:0x135 [Loop If]
	if(__NFUN_152__(i, TotalLength))
	{
		LengthTotal = __NFUN_147__(__NFUN_125__(TextValue), 1);
		CurText = __NFUN_128__(TextValue, 1);
		TextValue = __NFUN_234__(TextValue, LengthTotal);
		// End:0xD4
		if(__NFUN_122__(CurText, "`"))
		{
			CurText = "";
		}
		// End:0xF9
		if(__NFUN_122__(CurText, "#"))
		{
			CurText = "";
			j = 2;
			linedivided = true;
		}
		// End:0x119
		if(__NFUN_154__(j, 1))
		{
			TextValue1 = __NFUN_112__(TextValue1, CurText);
			// [Explicit Continue]
			goto J0x12B;
		}
		TextValue2 = __NFUN_112__(TextValue2, CurText);
		J0x12B:

		__NFUN_163__(i);
		// [Loop Continue]
		goto J0x80;
	}
	Debug(TextValue1);
	Debug(TextValue2);
	WndName = __NFUN_112__("OnScreenMessageWnd", string(WndNum));
	TextBoxName = __NFUN_112__(__NFUN_112__(WndName, ".TextBox"), string(WndNum));
	ShadowBoxName = __NFUN_112__(__NFUN_112__(__NFUN_112__(WndName, ".TextBox"), string(WndNum)), "-0");
	TextBoxName2 = __NFUN_112__(__NFUN_112__(__NFUN_112__(WndName, ".TextBox"), string(WndNum)), "-1");
	ShadowBoxName2 = __NFUN_112__(__NFUN_112__(__NFUN_112__(WndName, ".TextBox"), string(WndNum)), "-1-0");
	SmallBoxName1 = __NFUN_112__(__NFUN_112__(WndName, ".TextBoxsm"), string(WndNum));
	SmallBoxName2 = __NFUN_112__(__NFUN_112__(__NFUN_112__(WndName, ".TextBoxsm"), string(WndNum)), "-1");
	currentwnd1 = WndName;
	Class'NWindow.UIAPI_TEXTBOX'.static.SetTextColor(TextBoxName, FontColor);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetTextColor(TextBoxName2, FontColor);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetTextColor(SmallBoxName1, FontColor);
	Class'NWindow.UIAPI_TEXTBOX'.static.SetTextColor(SmallBoxName2, FontColor);
	// End:0x36D
	if(__NFUN_154__(FontType, 0))
	{
		Class'NWindow.UIAPI_WINDOW'.static.ShowWindow(currentwnd1);
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText(ShadowBoxName, TextValue1);
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText(TextBoxName, TextValue1);
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText(ShadowBoxName2, TextValue2);
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText(TextBoxName2, TextValue2);
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText(SmallBoxName1, "");
		Class'NWindow.UIAPI_TEXTBOX'.static.SetText(SmallBoxName2, "");		
	}
	else
	{
		// End:0x416
		if(__NFUN_154__(FontType, 1))
		{
			Class'NWindow.UIAPI_WINDOW'.static.ShowWindow(currentwnd1);
			Class'NWindow.UIAPI_TEXTBOX'.static.SetText(ShadowBoxName, "");
			Class'NWindow.UIAPI_TEXTBOX'.static.SetText(TextBoxName, "");
			Class'NWindow.UIAPI_TEXTBOX'.static.SetText(ShadowBoxName2, "");
			Class'NWindow.UIAPI_TEXTBOX'.static.SetText(TextBoxName2, "");
			Class'NWindow.UIAPI_TEXTBOX'.static.SetText(SmallBoxName1, TextValue1);
			Class'NWindow.UIAPI_TEXTBOX'.static.SetText(SmallBoxName2, TextValue2);
		}
	}
	// End:0x4A6
	if(__NFUN_154__(WndNum, 2))
	{
		// End:0x42D
		if(__NFUN_155__(moveval, 0))
		{
		}
		MovedWndName = WndName;
		moveval2 = __NFUN_144__(__NFUN_145__(TextOffsetTotal1, 2), 29);
		// End:0x47E
		if(__NFUN_154__(BackgroundType, 1))
		{
			Class'NWindow.UIAPI_WINDOW'.static.ShowWindow(__NFUN_112__(MovedWndName, ".texturetype1"));			
		}
		else
		{
			Class'NWindow.UIAPI_WINDOW'.static.HideWindow(__NFUN_112__(MovedWndName, ".texturetype1"));
		}		
	}
	else
	{
		// End:0x4CB
		if(__NFUN_154__(WndNum, 5))
		{
			// End:0x4BD
			if(__NFUN_155__(moveval, 0))
			{
			}
			MovedWndName = WndName;			
		}
		else
		{
			// End:0x4F0
			if(__NFUN_154__(WndNum, 7))
			{
				// End:0x4E2
				if(__NFUN_155__(moveval, 0))
				{
				}
				MovedWndName = WndName;				
			}
			else
			{
				moveval = 0;
			}
		}
	}
	onshowstat1 = true;
	onshowstat2 = false;
	globalDuration = Duration;
	switch(Animation)
	{
		// End:0x528
		case 0:
			droprate = 255;
			// End:0x56A
			break;
		// End:0x537
		case 1:
			droprate = 25;
			// End:0x56A
			break;
		// End:0x547
		case 11:
			droprate = 15;
			// End:0x56A
			break;
		// End:0x557
		case 12:
			droprate = 25;
			// End:0x56A
			break;
		// End:0x567
		case 13:
			droprate = 35;
			// End:0x56A
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function FadeIn(string WndName)
{
	globalAlphavalue1 = __NFUN_146__(globalAlphavalue1, droprate);
	// End:0x3A
	if(__NFUN_150__(globalAlphavalue1, 255))
	{
		Class'NWindow.UIAPI_WINDOW'.static.SetAlpha(WndName, globalAlphavalue1);		
	}
	else
	{
		Class'NWindow.UIAPI_WINDOW'.static.SetAlpha(WndName, 255);
		globalAlphavalue1 = 0;
		onshowstat1 = false;
		__NFUN_165__(m_TimerCount);
		Class'NWindow.UIAPI_WINDOW'.static.SetUITimer("OnScreenMessageWnd1", m_TimerCount, globalDuration);
	}
	return;
}

function FadeOut(string WndName)
{
	globalAlphavalue2 = __NFUN_147__(globalAlphavalue2, droprate);
	// End:0x39
	if(__NFUN_151__(globalAlphavalue2, 1))
	{
		Class'NWindow.UIAPI_WINDOW'.static.SetAlpha(WndName, globalAlphavalue2);		
	}
	else
	{
		Class'NWindow.UIAPI_WINDOW'.static.SetAlpha(WndName, 0);
		globalAlphavalue2 = 255;
		onshowstat2 = false;
		ResetAllMessage();
		moveval2 = 0;
		moveval = 0;
	}
	return;
}

function OnEvent(int a_EventID, string a_Param)
{
	local int MsgType, MsgNo, WindowType, FontSize, FontType, MsgColor,
		msgcolorR, msgcolorG, msgcolorB, shadowtype, BackgroundType,
		LifeTime, AnimationType, SystemMsgIndex;

	local string msgtext, ParamString1, ParamString2;

	// End:0x23D
	if(__NFUN_154__(a_EventID, 140))
	{
		ParseInt(a_Param, "MsgType", MsgType);
		ParseInt(a_Param, "MsgNo", MsgNo);
		ParseInt(a_Param, "WindowType", WindowType);
		ParseInt(a_Param, "FontSize", FontSize);
		ParseInt(a_Param, "FontType", FontType);
		ParseInt(a_Param, "MsgColor", MsgColor);
		// End:0xCE
		if(__NFUN_129__(ParseInt(a_Param, "MsgColorR", msgcolorR)))
		{
			msgcolorR = 255;
		}
		// End:0xF6
		if(__NFUN_129__(ParseInt(a_Param, "MsgColorG", msgcolorG)))
		{
			msgcolorG = 255;
		}
		// End:0x11E
		if(__NFUN_129__(ParseInt(a_Param, "MsgColorB", msgcolorB)))
		{
			msgcolorB = 255;
		}
		ParseInt(a_Param, "ShadowType", shadowtype);
		ParseInt(a_Param, "BackgroundType", BackgroundType);
		ParseInt(a_Param, "LifeTime", LifeTime);
		ParseInt(a_Param, "AnimationType", AnimationType);
		ParseString(a_Param, "Msg", msgtext);
		ResetAllMessage();
		switch(MsgType)
		{
			// End:0x1EF
			case 1:
				ShowMsg(WindowType, msgtext, LifeTime, AnimationType, FontType, BackgroundType, msgcolorR, msgcolorG, msgcolorB);
				// End:0x23D
				break;
			// End:0x23A
			case 0:
				msgtext = GetSystemMessage(MsgNo);
				ShowMsg(WindowType, msgtext, LifeTime, AnimationType, FontType, BackgroundType, msgcolorR, msgcolorG, msgcolorB);
				// End:0x23D
				break;
			// End:0xFFFF
			default:
				break;
		}
	}
	// End:0x2A8
	if(__NFUN_154__(a_EventID, 580))
	{
		ParseInt(a_Param, "Index", SystemMsgIndex);
		ParseString(a_Param, "Param1", ParamString1);
		ParseString(a_Param, "Param2", ParamString2);
		ValidateSystemMsg(SystemMsgIndex, ParamString1, ParamString2);
	}
	return;
}

function ValidateSystemMsg(int Index, string StringTxt1, string StringTxt2)
{
	local SystemMsgData SystemMsgCurrent;
	local int WindowType, FontType, BackgroundType, LifeTime, AnimationType;

	local string msgtext;
	local Color TextColor;

	GetSystemMsgInfo(Index, SystemMsgCurrent);
	// End:0x1CE
	if(__NFUN_155__(SystemMsgCurrent.WindowType, 0))
	{
		WindowType = SystemMsgCurrent.WindowType;
		msgtext = SystemMsgCurrent.OnScrMsg;
		msgtext = MakeFullSystemMsg(msgtext, StringTxt1, StringTxt2);
		LifeTime = __NFUN_144__(SystemMsgCurrent.LifeTime, 1000);
		AnimationType = SystemMsgCurrent.AnimationType;
		FontType = SystemMsgCurrent.FontType;
		BackgroundType = SystemMsgCurrent.BackgroundType;
		TextColor = SystemMsgCurrent.Color;
		// End:0x11C
		if(__NFUN_130__(__NFUN_130__(__NFUN_154__(int(TextColor.R), 0), __NFUN_154__(int(TextColor.G), 0)), __NFUN_154__(int(TextColor.B), 0)))
		{
			TextColor.R = byte(255);
			TextColor.G = byte(255);
			TextColor.B = byte(255);			
		}
		else
		{
			// End:0x186
			if(__NFUN_130__(__NFUN_130__(__NFUN_154__(int(TextColor.R), 176), __NFUN_154__(int(TextColor.G), 155)), __NFUN_154__(int(TextColor.B), 121)))
			{
				TextColor.R = byte(255);
				TextColor.G = byte(255);
				TextColor.B = byte(255);
			}
		}
		ShowMsg(WindowType, msgtext, LifeTime, AnimationType, FontType, BackgroundType, int(TextColor.R), int(TextColor.G), int(TextColor.B));
	}
	return;
}
