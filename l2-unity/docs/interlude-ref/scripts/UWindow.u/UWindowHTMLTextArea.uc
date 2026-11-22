class UWindowHTMLTextArea extends UWindowDynamicTextArea
	config;

var Color TextColor;
var Color BGColor;
var Color LinkColor;
var Color ALinkColor;
var float LastBlinkTime;
var bool bShowBlink;
var bool bReleased;

function SetHTML(string HTML)
{
	Clear();
	ReplaceText(HTML, __NFUN_112__(__NFUN_236__(13), __NFUN_236__(10)), " ");
	ReplaceText(HTML, __NFUN_236__(13), " ");
	ReplaceText(HTML, __NFUN_236__(10), " ");
	AddText(HTML);
	return;
}

function BeforePaint(Canvas C, float X, float Y)
{
	super.BeforePaint(C, X, Y);
	Cursor = Root.NormalCursor;
	return;
}

function Paint(Canvas C, float X, float Y)
{
	C.DrawColor = BGColor;
	DrawStretchedTexture(C, 0.0000000, 0.0000000, WinWidth, WinHeight, Texture'UWindow.WhiteTexture');
	super.Paint(C, X, Y);
	bReleased = false;
	return;
}

function Click(float X, float Y)
{
	super(UWindowWindow).Click(X, Y);
	bReleased = true;
	return;
}

function ProcessURL(string URL)
{
	__NFUN_231__(__NFUN_112__(__NFUN_112__("Clicked Link: >>", URL), "<<"));
	// End:0x58
	if(__NFUN_124__(__NFUN_128__(URL, 7), "mailto:"))
	{
		GetPlayerOwner().ConsoleCommand(__NFUN_112__("start ", URL));
	}
	// End:0x8F
	if(__NFUN_124__(__NFUN_128__(URL, 7), "http://"))
	{
		GetPlayerOwner().ConsoleCommand(__NFUN_112__("start ", URL));
	}
	// End:0xC5
	if(__NFUN_124__(__NFUN_128__(URL, 6), "ftp://"))
	{
		GetPlayerOwner().ConsoleCommand(__NFUN_112__("start ", URL));
	}
	// End:0xFE
	if(__NFUN_124__(__NFUN_128__(URL, 9), "telnet://"))
	{
		GetPlayerOwner().ConsoleCommand(__NFUN_112__("start ", URL));
	}
	// End:0x137
	if(__NFUN_124__(__NFUN_128__(URL, 9), "gopher://"))
	{
		GetPlayerOwner().ConsoleCommand(__NFUN_112__("start ", URL));
	}
	// End:0x172
	if(__NFUN_124__(__NFUN_128__(URL, 4), "www."))
	{
		GetPlayerOwner().ConsoleCommand(__NFUN_112__("start http://", URL));
	}
	// End:0x1AF
	if(__NFUN_124__(__NFUN_128__(URL, 4), "ftp."))
	{
		GetPlayerOwner().ConsoleCommand(__NFUN_112__("start ftp://", URL));		
	}
	else
	{
		// End:0x1D3
		if(__NFUN_124__(__NFUN_128__(URL, 9), "unreal://"))
		{
			LaunchUnrealURL(URL);
		}
	}
	return;
}

function OverURL(string URL)
{
	return;
}

function LaunchUnrealURL(string URL)
{
	GetPlayerOwner().ClientTravel(URL, 0, false);
	return;
}

function TextAreaTextSize(Canvas C, string Text, out float W, out float h)
{
	ReplaceText(Text, "&nbsp;", " ");
	ReplaceText(Text, "&gt;", ">");
	ReplaceText(Text, "&lt;", "<");
	ReplaceText(Text, "&amp;", "&");
	TextSize(C, Text, W, h);
	return;
}

function TextAreaClipText(Canvas C, float DrawX, float DrawY, coerce string Text, optional bool bCheckHotKey)
{
	ReplaceText(Text, "&nbsp;", " ");
	ReplaceText(Text, "&gt;", ">");
	ReplaceText(Text, "&lt;", "<");
	ReplaceText(Text, "&amp;", "&");
	ClipText(C, DrawX, DrawY, Text, bCheckHotKey);
	return;
}

function WrapRow(Canvas C, UWindowDynamicTextRow L)
{
	local HTMLStyle CurrentStyle;
	local UWindowHTMLTextRow R;
	local string Input, LeftText, HTML, RightText;

	super.WrapRow(C, L);
	R = UWindowHTMLTextRow(L);
	J0x20:

	// End:0x188 [Loop If]
	if(__NFUN_130__(__NFUN_119__(R, none), __NFUN_132__(__NFUN_114__(R, L), __NFUN_114__(R.WrapParent, L))))
	{
		R.DisplayString = "";
		R.StyleString = "";
		CurrentStyle = R.StartStyle;
		Input = R.Text;
		J0xA0:

		// End:0x16C [Loop If]
		if(__NFUN_123__(Input, ""))
		{
			ParseHTML(Input, LeftText, HTML, RightText);
			// End:0x13E
			if(__NFUN_132__(__NFUN_123__(LeftText, ""), __NFUN_122__(R.DisplayString, "")))
			{
				R.DisplayString = __NFUN_112__(R.DisplayString, LeftText);
				R.StyleString = __NFUN_112__(R.StyleString, WriteStyleText(CurrentStyle, __NFUN_125__(LeftText)));
			}
			ProcessInlineHTML(HTML, CurrentStyle);
			SetCanvasStyle(C, CurrentStyle);
			Input = RightText;
			// [Loop Continue]
			goto J0xA0;
		}
		R = UWindowHTMLTextRow(R.Next);
		// [Loop Continue]
		goto J0x20;
	}
	return;
}

function float DrawTextLine(Canvas C, UWindowDynamicTextRow L, float Y)
{
	local float X, W, h, MouseX, MouseY;

	local HTMLStyle CurrentStyle;
	local float RowHeight;
	local Color OldColor;
	local int StylePos, DisplayPos, i;
	local string S;

	RowHeight = 0.0000000;
	CurrentStyle = UWindowHTMLTextRow(L).StartStyle;
	// End:0xB6
	if(CurrentStyle.bCenter)
	{
		W = CalcHTMLTextWidth(C, L.Text, CurrentStyle);
		// End:0x97
		if(VertSB.bWindowVisible)
		{
			X = float(int(__NFUN_172__(__NFUN_175__(__NFUN_175__(WinWidth, VertSB.WinWidth), W), float(2))));			
		}
		else
		{
			X = float(int(__NFUN_172__(__NFUN_175__(WinWidth, W), float(2))));
		}		
	}
	else
	{
		X = 2.0000000;
	}
	// End:0x105
	if(__NFUN_177__(GetEntryLevel().TimeSeconds, __NFUN_174__(LastBlinkTime, 0.5000000)))
	{
		bShowBlink = __NFUN_129__(bShowBlink);
		LastBlinkTime = GetEntryLevel().TimeSeconds;
	}
	// End:0x132
	if(__NFUN_122__(UWindowHTMLTextRow(L).DisplayString, ""))
	{
		SetCanvasStyle(C, CurrentStyle);		
	}
	else
	{
		J0x132:

		// End:0x3DB [Loop If]
		if(__NFUN_150__(DisplayPos, __NFUN_125__(UWindowHTMLTextRow(L).DisplayString)))
		{
			i = ReadStyleText(UWindowHTMLTextRow(L).StyleString, StylePos, CurrentStyle);
			S = __NFUN_127__(UWindowHTMLTextRow(L).DisplayString, DisplayPos, i);
			__NFUN_161__(DisplayPos, i);
			SetCanvasStyle(C, CurrentStyle);
			TextAreaTextSize(C, S, W, h);
			// End:0x1EF
			if(__NFUN_177__(h, RowHeight))
			{
				RowHeight = h;
			}
			// End:0x2CD
			if(CurrentStyle.bLink)
			{
				GetMouseXY(MouseX, MouseY);
				// End:0x2CD
				if(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_176__(X, MouseX), __NFUN_177__(__NFUN_174__(X, W), MouseX)), __NFUN_176__(Y, MouseY)), __NFUN_177__(__NFUN_174__(Y, h), MouseY)))
				{
					Cursor = Root.HandCursor;
					OverURL(CurrentStyle.LinkDestination);
					// End:0x2CD
					if(__NFUN_132__(bMouseDown, bReleased))
					{
						// End:0x2B9
						if(bReleased)
						{
							ProcessURL(CurrentStyle.LinkDestination);
							bReleased = false;							
						}
						else
						{
							C.DrawColor = ALinkColor;
						}
					}
				}
			}
			// End:0x349
			if(CurrentStyle.BGColor != BGColor)
			{
				OldColor = C.DrawColor;
				C.DrawColor = CurrentStyle.BGColor;
				DrawStretchedTexture(C, X, Y, W, h, Texture'UWindow.WhiteTexture');
				C.DrawColor = OldColor;
			}
			// End:0x37E
			if(__NFUN_132__(__NFUN_129__(CurrentStyle.bBlink), bShowBlink))
			{
				TextAreaClipText(C, X, Y, S);
			}
			// End:0x3CC
			if(__NFUN_132__(CurrentStyle.bLink, CurrentStyle.bUnderline))
			{
				DrawStretchedTexture(C, X, __NFUN_175__(__NFUN_174__(Y, h), float(1)), W, 1.0000000, Texture'UWindow.WhiteTexture');
			}
			__NFUN_184__(X, W);
			// [Loop Continue]
			goto J0x132;
		}
	}
	// End:0x400
	if(__NFUN_180__(RowHeight, float(0)))
	{
		TextAreaTextSize(C, "A", W, RowHeight);
	}
	return RowHeight;
	return;
}

function UWindowDynamicTextRow SplitRowAt(UWindowDynamicTextRow L, int SplitPos)
{
	local UWindowDynamicTextRow N;
	local HTMLStyle CurrentStyle;

	N = super.SplitRowAt(L, SplitPos);
	UWindowHTMLTextRow(N).EndStyle = UWindowHTMLTextRow(L).EndStyle;
	CurrentStyle = UWindowHTMLTextRow(L).StartStyle;
	HTMLUpdateStyle(L.Text, CurrentStyle);
	UWindowHTMLTextRow(L).EndStyle = CurrentStyle;
	UWindowHTMLTextRow(N).StartStyle = CurrentStyle;
	return N;
	return;
}

function RemoveWrap(UWindowDynamicTextRow L)
{
	local UWindowDynamicTextRow N;

	N = UWindowDynamicTextRow(L.Next);
	J0x19:

	// End:0x81 [Loop If]
	if(__NFUN_130__(__NFUN_119__(N, none), __NFUN_114__(N.WrapParent, L)))
	{
		UWindowHTMLTextRow(L).EndStyle = UWindowHTMLTextRow(N).EndStyle;
		N = UWindowDynamicTextRow(N.Next);
		// [Loop Continue]
		goto J0x19;
	}
	super.RemoveWrap(L);
	return;
}

function int GetWrapPos(Canvas C, UWindowDynamicTextRow L, float MaxWidth)
{
	local float LineWidth, NextWordWidth;
	local string Input, NextWord;
	local int WordsThisRow, WrapPos;
	local HTMLStyle CurrentStyle;

	CurrentStyle = UWindowHTMLTextRow(L).StartStyle;
	// End:0x47
	if(__NFUN_178__(CalcHTMLTextWidth(C, L.Text, CurrentStyle), MaxWidth))
	{
		return -1;
	}
	Input = L.Text;
	WordsThisRow = 0;
	LineWidth = 0.0000000;
	WrapPos = 0;
	NextWord = "";
	CurrentStyle = UWindowHTMLTextRow(L).StartStyle;
	J0x95:

	// End:0x13E [Loop If]
	if(__NFUN_132__(__NFUN_123__(Input, ""), __NFUN_123__(NextWord, "")))
	{
		// End:0xE6
		if(__NFUN_122__(NextWord, ""))
		{
			RemoveNextWord(Input, NextWord);
			NextWordWidth = CalcHTMLTextWidth(C, NextWord, CurrentStyle);
		}
		// End:0x112
		if(__NFUN_130__(__NFUN_151__(WordsThisRow, 0), __NFUN_177__(__NFUN_174__(LineWidth, NextWordWidth), MaxWidth)))
		{
			return WrapPos;			
		}
		else
		{
			__NFUN_161__(WrapPos, __NFUN_125__(NextWord));
			__NFUN_184__(LineWidth, NextWordWidth);
			NextWord = "";
			__NFUN_165__(WordsThisRow);
		}
		// [Loop Continue]
		goto J0x95;
	}
	return -1;
	return;
}

function RemoveNextWord(out string Text, out string NextWord)
{
	local int i;
	local bool bInsideTag;
	local string Ch;

	bInsideTag = false;
	i = 0;
	J0x0F:

	// End:0x84 [Loop If]
	if(__NFUN_150__(i, __NFUN_125__(Text)))
	{
		Ch = __NFUN_127__(Text, i, 1);
		// End:0x48
		if(__NFUN_122__(Ch, ">"))
		{
			bInsideTag = false;
		}
		// End:0x5D
		if(__NFUN_122__(Ch, "<"))
		{
			bInsideTag = true;
		}
		// End:0x7A
		if(__NFUN_130__(__NFUN_122__(Ch, " "), __NFUN_129__(bInsideTag)))
		{
			// [Explicit Break]
			goto J0x84;
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x0F;
	}
	J0x84:

	// End:0xA3 [Loop If]
	if(__NFUN_122__(__NFUN_127__(Text, i, 1), " "))
	{
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x84;
	}
	NextWord = __NFUN_128__(Text, i);
	Text = __NFUN_127__(Text, i);
	return;
}

function UWindowDynamicTextRow AddText(string NewLine)
{
	local string Input, Output, LeftText, RightText, HTML, temp;

	local int i;
	local UWindowDynamicTextRow L;
	local HTMLStyle CurrentStyle, StartStyle;

	// End:0xB4
	if(__NFUN_114__(List.Last, List))
	{
		CurrentStyle.BulletLevel = 0;
		CurrentStyle.LinkDestination = "";
		CurrentStyle.TextColor = TextColor;
		CurrentStyle.BGColor = BGColor;
		CurrentStyle.bCenter = bHCenter;
		CurrentStyle.bLink = false;
		CurrentStyle.bUnderline = false;
		CurrentStyle.bNoBR = false;
		CurrentStyle.bHeading = false;
		CurrentStyle.bBold = false;
		CurrentStyle.bBlink = false;		
	}
	else
	{
		CurrentStyle = UWindowHTMLTextRow(List.Last).EndStyle;
	}
	StartStyle = CurrentStyle;
	i = __NFUN_126__(NewLine, "\\n");
	J0xF2:

	// End:0x141 [Loop If]
	if(__NFUN_155__(i, -1))
	{
		NewLine = __NFUN_112__(__NFUN_112__(__NFUN_128__(NewLine, i), "<br>"), __NFUN_127__(NewLine, __NFUN_146__(i, 2)));
		i = __NFUN_126__(NewLine, "\\n");
		// [Loop Continue]
		goto J0xF2;
	}
	Input = NewLine;
	Output = "";
	J0x154:

	// End:0x8E9 [Loop If]
	if(__NFUN_123__(Input, ""))
	{
		ParseHTML(Input, LeftText, HTML, RightText);
		switch(GetTag(HTML))
		{
			// End:0x240
			case "P":
				// End:0x1F2
				if(__NFUN_123__(__NFUN_112__(Output, LeftText), ""))
				{
					L = super.AddText(__NFUN_112__(Output, LeftText));
					Output = "";
					UWindowHTMLTextRow(L).StartStyle = StartStyle;
					UWindowHTMLTextRow(L).EndStyle = CurrentStyle;
				}
				StartStyle = CurrentStyle;
				L = super.AddText("");
				UWindowHTMLTextRow(L).StartStyle = StartStyle;
				UWindowHTMLTextRow(L).EndStyle = CurrentStyle;
				// End:0x8DB
				break;
			// End:0x2A7
			case "BR":
				L = super.AddText(__NFUN_112__(Output, LeftText));
				Output = "";
				UWindowHTMLTextRow(L).StartStyle = StartStyle;
				UWindowHTMLTextRow(L).EndStyle = CurrentStyle;
				StartStyle = CurrentStyle;
				// End:0x8DB
				break;
			// End:0x3CD
			case "BODY":
				temp = GetOption(HTML, "BGCOLOR=");
				// End:0x308
				if(__NFUN_123__(temp, ""))
				{
					BGColor = ParseColor(temp);
					CurrentStyle.BGColor = BGColor;
					StartStyle.BGColor = BGColor;
				}
				temp = GetOption(HTML, "LINK=");
				// End:0x33D
				if(__NFUN_123__(temp, ""))
				{
					LinkColor = ParseColor(temp);
				}
				temp = GetOption(HTML, "ALINK=");
				// End:0x373
				if(__NFUN_123__(temp, ""))
				{
					ALinkColor = ParseColor(temp);
				}
				temp = GetOption(HTML, "TEXT=");
				// End:0x3B8
				if(__NFUN_123__(temp, ""))
				{
					TextColor = ParseColor(temp);
					CurrentStyle.TextColor = TextColor;
				}
				Output = __NFUN_112__(Output, LeftText);
				// End:0x8DB
				break;
			// End:0x458
			case "CENTER":
				// End:0x43D
				if(__NFUN_123__(__NFUN_112__(Output, LeftText), ""))
				{
					L = super.AddText(__NFUN_112__(Output, LeftText));
					Output = "";
					UWindowHTMLTextRow(L).StartStyle = StartStyle;
					UWindowHTMLTextRow(L).EndStyle = CurrentStyle;
				}
				CurrentStyle.bCenter = true;
				StartStyle = CurrentStyle;
				// End:0x8DB
				break;
			// End:0x4D1
			case "/CENTER":
				L = super.AddText(__NFUN_112__(Output, LeftText));
				Output = "";
				UWindowHTMLTextRow(L).StartStyle = StartStyle;
				UWindowHTMLTextRow(L).EndStyle = CurrentStyle;
				CurrentStyle.bCenter = false;
				StartStyle = CurrentStyle;
				// End:0x8DB
				break;
			// End:0x558
			case "H1":
				// End:0x53D
				if(__NFUN_123__(__NFUN_112__(Output, LeftText), ""))
				{
					L = super.AddText(__NFUN_112__(Output, LeftText));
					Output = "";
					UWindowHTMLTextRow(L).StartStyle = StartStyle;
					UWindowHTMLTextRow(L).EndStyle = CurrentStyle;
				}
				CurrentStyle.bHeading = true;
				StartStyle = CurrentStyle;
				// End:0x8DB
				break;
			// End:0x5CD
			case "/H1":
				L = super.AddText(__NFUN_112__(Output, LeftText));
				Output = "";
				UWindowHTMLTextRow(L).StartStyle = StartStyle;
				UWindowHTMLTextRow(L).EndStyle = CurrentStyle;
				CurrentStyle.bHeading = false;
				StartStyle = CurrentStyle;
				// End:0x8DB
				break;
			// End:0x66A
			case "FONT":
				Output = __NFUN_112__(__NFUN_112__(Output, LeftText), HTML);
				temp = GetOption(HTML, "COLOR=");
				// End:0x62A
				if(__NFUN_123__(temp, ""))
				{
					CurrentStyle.TextColor = ParseColor(temp);
				}
				temp = GetOption(HTML, "BGCOLOR=");
				// End:0x667
				if(__NFUN_123__(temp, ""))
				{
					CurrentStyle.BGColor = ParseColor(temp);
				}
				// End:0x8DB
				break;
			// End:0x6B0
			case "/FONT":
				Output = __NFUN_112__(__NFUN_112__(Output, LeftText), HTML);
				CurrentStyle.TextColor = TextColor;
				CurrentStyle.BGColor = BGColor;
				// End:0x8DB
				break;
			// End:0x6DF
			case "B":
				Output = __NFUN_112__(__NFUN_112__(Output, LeftText), HTML);
				CurrentStyle.bBold = true;
				// End:0x8DB
				break;
			// End:0x70F
			case "/B":
				Output = __NFUN_112__(__NFUN_112__(Output, LeftText), HTML);
				CurrentStyle.bBold = false;
				// End:0x8DB
				break;
			// End:0x73E
			case "U":
				Output = __NFUN_112__(__NFUN_112__(Output, LeftText), HTML);
				CurrentStyle.bUnderline = true;
				// End:0x8DB
				break;
			// End:0x76E
			case "/U":
				Output = __NFUN_112__(__NFUN_112__(Output, LeftText), HTML);
				CurrentStyle.bUnderline = false;
				// End:0x8DB
				break;
			// End:0x7BA
			case "A":
				Output = __NFUN_112__(__NFUN_112__(Output, LeftText), HTML);
				CurrentStyle.bLink = true;
				CurrentStyle.LinkDestination = GetOption(HTML, "HREF=");
				// End:0x8DB
				break;
			// End:0x7F7
			case "/A":
				Output = __NFUN_112__(__NFUN_112__(Output, LeftText), HTML);
				CurrentStyle.bLink = false;
				CurrentStyle.LinkDestination = "";
				// End:0x8DB
				break;
			// End:0x829
			case "NOBR":
				Output = __NFUN_112__(__NFUN_112__(Output, LeftText), HTML);
				CurrentStyle.bNoBR = true;
				// End:0x8DB
				break;
			// End:0x85C
			case "/NOBR":
				Output = __NFUN_112__(__NFUN_112__(Output, LeftText), HTML);
				CurrentStyle.bNoBR = false;
				// End:0x8DB
				break;
			// End:0x88F
			case "BLINK":
				Output = __NFUN_112__(__NFUN_112__(Output, LeftText), HTML);
				CurrentStyle.bBlink = true;
				// End:0x8DB
				break;
			// End:0x8C3
			case "/BLINK":
				Output = __NFUN_112__(__NFUN_112__(Output, LeftText), HTML);
				CurrentStyle.bBlink = false;
				// End:0x8DB
				break;
			// End:0xFFFF
			default:
				Output = __NFUN_112__(Output, LeftText);
				// End:0x8DB
				break;
				break;
		}
		Input = RightText;
		// [Loop Continue]
		goto J0x154;
	}
	L = super.AddText(Output);
	UWindowHTMLTextRow(L).StartStyle = StartStyle;
	UWindowHTMLTextRow(L).EndStyle = CurrentStyle;
	return L;
	return;
}

function ParseHTML(string Input, out string LeftText, out string HTML, out string RightText)
{
	local int i;

	i = __NFUN_126__(Input, "<");
	// End:0x3C
	if(__NFUN_154__(i, -1))
	{
		LeftText = Input;
		HTML = "";
		RightText = "";
		return;
	}
	LeftText = __NFUN_128__(Input, i);
	HTML = __NFUN_127__(Input, i);
	i = __NFUN_126__(HTML, ">");
	// End:0x89
	if(__NFUN_154__(i, -1))
	{
		RightText = "";
		return;
	}
	RightText = __NFUN_127__(HTML, __NFUN_146__(i, 1));
	HTML = __NFUN_128__(HTML, __NFUN_146__(i, 1));
	return;
}

function float CalcHTMLTextWidth(Canvas C, string Text, out HTMLStyle CurrentStyle)
{
	local string Input, LeftText, HTML, RightText;
	local float W, h, Width;

	Width = 0.0000000;
	Input = Text;
	J0x16:

	// End:0x90 [Loop If]
	if(__NFUN_123__(Input, ""))
	{
		ParseHTML(Input, LeftText, HTML, RightText);
		SetCanvasStyle(C, CurrentStyle);
		TextAreaTextSize(C, LeftText, W, h);
		__NFUN_184__(Width, W);
		ProcessInlineHTML(HTML, CurrentStyle);
		Input = RightText;
		// [Loop Continue]
		goto J0x16;
	}
	return Width;
	return;
}

function ProcessInlineHTML(string HTML, out HTMLStyle CurrentStyle)
{
	local string temp;

	// End:0x0E
	if(__NFUN_122__(HTML, ""))
	{
		return;
	}
	switch(GetTag(HTML))
	{
		// End:0x32
		case "H1":
			CurrentStyle.bHeading = true;
			// End:0x217
			break;
		// End:0x4A
		case "/H1":
			CurrentStyle.bHeading = false;
			// End:0x217
			break;
		// End:0xCE
		case "FONT":
			temp = GetOption(HTML, "COLOR=");
			// End:0x8E
			if(__NFUN_123__(temp, ""))
			{
				CurrentStyle.TextColor = ParseColor(temp);
			}
			temp = GetOption(HTML, "BGCOLOR=");
			// End:0xCB
			if(__NFUN_123__(temp, ""))
			{
				CurrentStyle.BGColor = ParseColor(temp);
			}
			// End:0x217
			break;
		// End:0xFB
		case "/FONT":
			CurrentStyle.TextColor = TextColor;
			CurrentStyle.BGColor = BGColor;
			// End:0x217
			break;
		// End:0x111
		case "B":
			CurrentStyle.bBold = true;
			// End:0x217
			break;
		// End:0x128
		case "/B":
			CurrentStyle.bBold = false;
			// End:0x217
			break;
		// End:0x13E
		case "U":
			CurrentStyle.bUnderline = true;
			// End:0x217
			break;
		// End:0x155
		case "/U":
			CurrentStyle.bUnderline = false;
			// End:0x217
			break;
		// End:0x188
		case "A":
			CurrentStyle.bLink = true;
			CurrentStyle.LinkDestination = GetOption(HTML, "HREF=");
			// End:0x217
			break;
		// End:0x1AC
		case "/A":
			CurrentStyle.bLink = false;
			CurrentStyle.LinkDestination = "";
			// End:0x217
			break;
		// End:0x1C5
		case "NOBR":
			CurrentStyle.bNoBR = true;
			// End:0x217
			break;
		// End:0x1DF
		case "/NOBR":
			CurrentStyle.bNoBR = false;
			// End:0x217
			break;
		// End:0x1F9
		case "BLINK":
			CurrentStyle.bBlink = true;
			// End:0x217
			break;
		// End:0x214
		case "/BLINK":
			CurrentStyle.bBlink = false;
			// End:0x217
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function HTMLUpdateStyle(string Input, out HTMLStyle CurrentStyle)
{
	local string LeftText, HTML, RightText;

	J0x00:
	// End:0x44 [Loop If]
	if(__NFUN_123__(Input, ""))
	{
		ParseHTML(Input, LeftText, HTML, RightText);
		ProcessInlineHTML(HTML, CurrentStyle);
		Input = RightText;
		// [Loop Continue]
		goto J0x00;
	}
	return;
}

function string GetOption(string HTML, string Option)
{
	local int i, j;
	local string S;

	i = __NFUN_126__(__NFUN_235__(HTML), __NFUN_235__(Option));
	// End:0xD3
	if(__NFUN_132__(__NFUN_154__(i, 1), __NFUN_122__(__NFUN_127__(HTML, __NFUN_147__(i, 1), 1), " ")))
	{
		S = __NFUN_127__(HTML, __NFUN_146__(i, __NFUN_125__(Option)));
		j = FirstMatching(__NFUN_126__(S, ">"), __NFUN_126__(S, " "));
		S = __NFUN_128__(S, j);
		// End:0xA6
		if(__NFUN_122__(__NFUN_128__(S, 1), "\""))
		{
			S = __NFUN_127__(S, 1);
		}
		// End:0xCD
		if(__NFUN_122__(__NFUN_234__(S, 1), "\""))
		{
			S = __NFUN_128__(S, __NFUN_147__(__NFUN_125__(S), 1));
		}
		return S;
	}
	return "";
	return;
}

function string GetTag(string HTML)
{
	local int i;

	// End:0x0F
	if(__NFUN_122__(HTML, ""))
	{
		return "";
	}
	HTML = __NFUN_127__(HTML, 1);
	i = FirstMatching(__NFUN_126__(HTML, ">"), __NFUN_126__(HTML, " "));
	// End:0x57
	if(__NFUN_154__(i, -1))
	{
		return __NFUN_235__(HTML);		
	}
	else
	{
		return __NFUN_235__(__NFUN_128__(HTML, i));
	}
	return;
}

function Color ParseColor(string S)
{
	local Color C;

	// End:0x1E
	if(__NFUN_122__(__NFUN_128__(S, 1), "#"))
	{
		S = __NFUN_127__(S, 1);
	}
	C.R = byte(__NFUN_146__(__NFUN_144__(int(16), GetHexDigit(__NFUN_127__(S, 0, 1))), GetHexDigit(__NFUN_127__(S, 1, 1))));
	C.G = byte(__NFUN_146__(__NFUN_144__(int(16), GetHexDigit(__NFUN_127__(S, 2, 1))), GetHexDigit(__NFUN_127__(S, 3, 1))));
	C.B = byte(__NFUN_146__(__NFUN_144__(int(16), GetHexDigit(__NFUN_127__(S, 4, 1))), GetHexDigit(__NFUN_127__(S, 5, 1))));
	return C;
	return;
}

function int GetHexDigit(string D)
{
	switch(__NFUN_235__(D))
	{
		// End:0x11
		case "0":
			return 0;
		// End:0x19
		case "1":
			return 1;
		// End:0x22
		case "2":
			return 2;
		// End:0x2B
		case "3":
			return 3;
		// End:0x34
		case "4":
			return 4;
		// End:0x3D
		case "5":
			return 5;
		// End:0x46
		case "6":
			return 6;
		// End:0x4F
		case "7":
			return 7;
		// End:0x58
		case "8":
			return 8;
		// End:0x61
		case "9":
			return 9;
		// End:0x6A
		case "A":
			return 10;
		// End:0x73
		case "B":
			return 11;
		// End:0x7C
		case "C":
			return 12;
		// End:0x85
		case "D":
			return 13;
		// End:0x8E
		case "E":
			return 14;
		// End:0x97
		case "F":
			return 15;
		// End:0xFFFF
		default:
			return 0;
			break;
	}
	return;
}

function int FirstMatching(int i, int j)
{
	// End:0x15
	if(__NFUN_154__(i, -1))
	{
		return j;
	}
	// End:0x2D
	if(__NFUN_154__(j, -1))
	{
		return i;		
	}
	else
	{
		return __NFUN_249__(i, j);
	}
	return;
}

function SetCanvasStyle(Canvas C, HTMLStyle CurrentStyle)
{
	// End:0x25
	if(CurrentStyle.bLink)
	{
		C.DrawColor = LinkColor;		
	}
	else
	{
		C.DrawColor = CurrentStyle.TextColor;
	}
	// End:0x6F
	if(CurrentStyle.bHeading)
	{
		C.Font = Root.Fonts[3];		
	}
	else
	{
		// End:0x9F
		if(CurrentStyle.bBold)
		{
			C.Font = Root.Fonts[1];			
		}
		else
		{
			C.Font = Root.Fonts[0];
		}
	}
	return;
}

function string WriteStyleText(HTMLStyle CurrentStyle, int CharCount)
{
	local string Pad, temp, Output;

	Pad = "0000";
	temp = string(CharCount);
	Output = __NFUN_112__(__NFUN_128__(Pad, __NFUN_147__(4, __NFUN_125__(temp))), temp);
	temp = string(__NFUN_125__(CurrentStyle.LinkDestination));
	Output = __NFUN_112__(__NFUN_112__(__NFUN_112__(Output, __NFUN_128__(Pad, __NFUN_147__(4, __NFUN_125__(temp)))), temp), CurrentStyle.LinkDestination);
	temp = string(CurrentStyle.TextColor.R);
	Output = __NFUN_112__(__NFUN_112__(Output, __NFUN_128__(Pad, __NFUN_147__(3, __NFUN_125__(temp)))), temp);
	temp = string(CurrentStyle.TextColor.G);
	Output = __NFUN_112__(__NFUN_112__(Output, __NFUN_128__(Pad, __NFUN_147__(3, __NFUN_125__(temp)))), temp);
	temp = string(CurrentStyle.TextColor.B);
	Output = __NFUN_112__(__NFUN_112__(Output, __NFUN_128__(Pad, __NFUN_147__(3, __NFUN_125__(temp)))), temp);
	temp = string(CurrentStyle.BGColor.R);
	Output = __NFUN_112__(__NFUN_112__(Output, __NFUN_128__(Pad, __NFUN_147__(3, __NFUN_125__(temp)))), temp);
	temp = string(CurrentStyle.BGColor.G);
	Output = __NFUN_112__(__NFUN_112__(Output, __NFUN_128__(Pad, __NFUN_147__(3, __NFUN_125__(temp)))), temp);
	temp = string(CurrentStyle.BGColor.B);
	Output = __NFUN_112__(__NFUN_112__(Output, __NFUN_128__(Pad, __NFUN_147__(3, __NFUN_125__(temp)))), temp);
	// End:0x20D
	if(CurrentStyle.bCenter)
	{
		Output = __NFUN_112__(Output, "T");		
	}
	else
	{
		Output = __NFUN_112__(Output, "F");
	}
	// End:0x23E
	if(CurrentStyle.bLink)
	{
		Output = __NFUN_112__(Output, "T");		
	}
	else
	{
		Output = __NFUN_112__(Output, "F");
	}
	// End:0x26F
	if(CurrentStyle.bUnderline)
	{
		Output = __NFUN_112__(Output, "T");		
	}
	else
	{
		Output = __NFUN_112__(Output, "F");
	}
	// End:0x2A0
	if(CurrentStyle.bNoBR)
	{
		Output = __NFUN_112__(Output, "T");		
	}
	else
	{
		Output = __NFUN_112__(Output, "F");
	}
	// End:0x2D1
	if(CurrentStyle.bHeading)
	{
		Output = __NFUN_112__(Output, "T");		
	}
	else
	{
		Output = __NFUN_112__(Output, "F");
	}
	// End:0x302
	if(CurrentStyle.bBold)
	{
		Output = __NFUN_112__(Output, "T");		
	}
	else
	{
		Output = __NFUN_112__(Output, "F");
	}
	// End:0x333
	if(CurrentStyle.bBlink)
	{
		Output = __NFUN_112__(Output, "T");		
	}
	else
	{
		Output = __NFUN_112__(Output, "F");
	}
	return Output;
	return;
}

function int ReadStyleText(string StyleString, out int StylePos, out HTMLStyle CurrentStyle)
{
	local int CharCount, i;

	CharCount = int(__NFUN_127__(StyleString, StylePos, 4));
	__NFUN_161__(StylePos, 4);
	i = int(__NFUN_127__(StyleString, StylePos, 4));
	__NFUN_161__(StylePos, 4);
	CurrentStyle.LinkDestination = __NFUN_127__(StyleString, StylePos, i);
	__NFUN_161__(StylePos, i);
	CurrentStyle.TextColor.R = byte(int(__NFUN_127__(StyleString, StylePos, 3)));
	__NFUN_161__(StylePos, 3);
	CurrentStyle.TextColor.G = byte(int(__NFUN_127__(StyleString, StylePos, 3)));
	__NFUN_161__(StylePos, 3);
	CurrentStyle.TextColor.B = byte(int(__NFUN_127__(StyleString, StylePos, 3)));
	__NFUN_161__(StylePos, 3);
	CurrentStyle.BGColor.R = byte(int(__NFUN_127__(StyleString, StylePos, 3)));
	__NFUN_161__(StylePos, 3);
	CurrentStyle.BGColor.G = byte(int(__NFUN_127__(StyleString, StylePos, 3)));
	__NFUN_161__(StylePos, 3);
	CurrentStyle.BGColor.B = byte(int(__NFUN_127__(StyleString, StylePos, 3)));
	__NFUN_161__(StylePos, 3);
	CurrentStyle.bCenter = __NFUN_122__(__NFUN_127__(StyleString, __NFUN_165__(StylePos), 1), "T");
	CurrentStyle.bLink = __NFUN_122__(__NFUN_127__(StyleString, __NFUN_165__(StylePos), 1), "T");
	CurrentStyle.bUnderline = __NFUN_122__(__NFUN_127__(StyleString, __NFUN_165__(StylePos), 1), "T");
	CurrentStyle.bNoBR = __NFUN_122__(__NFUN_127__(StyleString, __NFUN_165__(StylePos), 1), "T");
	CurrentStyle.bHeading = __NFUN_122__(__NFUN_127__(StyleString, __NFUN_165__(StylePos), 1), "T");
	CurrentStyle.bBold = __NFUN_122__(__NFUN_127__(StyleString, __NFUN_165__(StylePos), 1), "T");
	CurrentStyle.bBlink = __NFUN_122__(__NFUN_127__(StyleString, __NFUN_165__(StylePos), 1), "T");
	return CharCount;
	return;
}

defaultproperties
{
	TextColor=(R=255,G=255,B=255,A=255)
	BGColor=(R=0,G=0,B=0,A=255)
	LinkColor=(R=0,G=0,B=255,A=255)
	ALinkColor=(R=255,G=0,B=0,A=255)
	bTopCentric=true
	bAutoScrollbar=true
	bVariableRowHeight=true
	RowClass=Class'UWindow.UWindowHTMLTextRow'
	bIgnoreLDoubleClick=true
}