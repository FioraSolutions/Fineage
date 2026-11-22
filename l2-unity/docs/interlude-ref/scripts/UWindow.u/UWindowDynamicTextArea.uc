class UWindowDynamicTextArea extends UWindowDialogControl
	config;

var config int MaxLines;
var bool bTopCentric;
var float DefaultTextHeight;
var bool bScrollOnResize;
var bool bVCenter;
var bool bHCenter;
var bool bAutoScrollbar;
var bool bVariableRowHeight;
var float WrapWidth;
var UWindowDynamicTextRow List;
var UWindowVScrollbar VertSB;
var float OldW;
var float OldH;
var bool bDirty;
var int Count;
var int VisibleRows;
var int Font;
var Font AbsoluteFont;
var Color TextColor;
var Class<UWindowDynamicTextRow> RowClass;

function Created()
{
	super.Created();
	VertSB = UWindowVScrollbar(CreateWindow(Class'UWindow.UWindowVScrollbar', __NFUN_175__(WinWidth, float(12)), 0.0000000, 12.0000000, WinHeight));
	VertSB.bAlwaysOnTop = true;
	Cursor = Root.NormalCursor;
	Clear();
	return;
}

function Clear()
{
	bDirty = true;
	// End:0x38
	if(__NFUN_119__(List, none))
	{
		// End:0x29
		if(__NFUN_114__(List.Next, none))
		{
			return;
		}
		List.DestroyList();
	}
	List = new RowClass;
	List.SetupSentinel();
	return;
}

function SetAbsoluteFont(Font f)
{
	AbsoluteFont = f;
	return;
}

function SetFont(int f)
{
	Font = f;
	return;
}

function SetTextColor(Color C)
{
	TextColor = C;
	return;
}

function TextAreaClipText(Canvas C, float DrawX, float DrawY, coerce string S, optional bool bCheckHotKey)
{
	ClipText(C, DrawX, DrawY, S, bCheckHotKey);
	return;
}

function TextAreaTextSize(Canvas C, string Text, out float W, out float h)
{
	TextSize(C, Text, W, h);
	return;
}

function BeforePaint(Canvas C, float X, float Y)
{
	super.BeforePaint(C, X, Y);
	VertSB.WinTop = 0.0000000;
	VertSB.WinHeight = WinHeight;
	VertSB.WinWidth = LookAndFeel.Size_ScrollbarWidth;
	VertSB.WinLeft = __NFUN_175__(WinWidth, LookAndFeel.Size_ScrollbarWidth);
	return;
}

function Paint(Canvas C, float MouseX, float MouseY)
{
	local UWindowDynamicTextRow L;
	local int SkipCount, DrawCount, i;
	local float Y, Junk;
	local bool bWrapped;

	C.DrawColor = TextColor;
	// End:0x36
	if(__NFUN_119__(AbsoluteFont, none))
	{
		C.Font = AbsoluteFont;		
	}
	else
	{
		C.Font = Root.Fonts[Font];
	}
	// End:0xA6
	if(__NFUN_132__(__NFUN_181__(OldW, WinWidth), __NFUN_181__(OldH, WinHeight)))
	{
		WordWrap(C, true);
		OldW = WinWidth;
		OldH = WinHeight;
		bWrapped = true;		
	}
	else
	{
		// End:0xC3
		if(bDirty)
		{
			WordWrap(C, false);
			bWrapped = true;
		}
	}
	// End:0x1BB
	if(bWrapped)
	{
		TextAreaTextSize(C, "A", Junk, DefaultTextHeight);
		VisibleRows = int(__NFUN_172__(WinHeight, DefaultTextHeight));
		Count = List.Count();
		VertSB.SetRange(0.0000000, float(Count), float(VisibleRows));
		// End:0x175
		if(bScrollOnResize)
		{
			// End:0x158
			if(bTopCentric)
			{
				VertSB.pos = 0.0000000;				
			}
			else
			{
				VertSB.pos = VertSB.MaxPos;
			}
		}
		// End:0x1BB
		if(__NFUN_130__(bAutoScrollbar, __NFUN_129__(bVariableRowHeight)))
		{
			// End:0x1AC
			if(__NFUN_152__(Count, VisibleRows))
			{
				VertSB.HideWindow();				
			}
			else
			{
				VertSB.ShowWindow();
			}
		}
	}
	// End:0x397
	if(bTopCentric)
	{
		SkipCount = int(VertSB.pos);
		L = UWindowDynamicTextRow(List.Next);
		i = 0;
		J0x1FA:

		// End:0x239 [Loop If]
		if(__NFUN_130__(__NFUN_150__(i, SkipCount), __NFUN_119__(L, none)))
		{
			L = UWindowDynamicTextRow(L.Next);
			__NFUN_165__(i);
			// [Loop Continue]
			goto J0x1FA;
		}
		// End:0x27B
		if(__NFUN_130__(bVCenter, __NFUN_152__(Count, VisibleRows)))
		{
			Y = float(int(__NFUN_172__(__NFUN_175__(WinHeight, __NFUN_171__(float(Count), DefaultTextHeight)), float(2))));			
		}
		else
		{
			Y = 1.0000000;
		}
		DrawCount = 0;
		J0x28D:

		// End:0x2F5 [Loop If]
		if(__NFUN_176__(Y, WinHeight))
		{
			__NFUN_165__(DrawCount);
			// End:0x2E6
			if(__NFUN_119__(L, none))
			{
				__NFUN_184__(Y, DrawTextLine(C, L, Y));
				L = UWindowDynamicTextRow(L.Next);				
			}
			else
			{
				__NFUN_184__(Y, DefaultTextHeight);
			}
			// [Loop Continue]
			goto J0x28D;
		}
		// End:0x394
		if(bVariableRowHeight)
		{
			VisibleRows = __NFUN_147__(DrawCount, 1);
			J0x30C:

			// End:0x339 [Loop If]
			if(__NFUN_177__(__NFUN_174__(VertSB.pos, float(VisibleRows)), float(Count)))
			{
				__NFUN_166__(VisibleRows);
				// [Loop Continue]
				goto J0x30C;
			}
			VertSB.SetRange(0.0000000, float(Count), float(VisibleRows));
			// End:0x394
			if(bAutoScrollbar)
			{
				// End:0x385
				if(__NFUN_152__(Count, VisibleRows))
				{
					VertSB.HideWindow();					
				}
				else
				{
					VertSB.ShowWindow();
				}
			}
		}		
	}
	else
	{
		SkipCount = __NFUN_250__(0, int(__NFUN_175__(float(Count), __NFUN_174__(float(VisibleRows), VertSB.pos))));
		L = UWindowDynamicTextRow(List.Last);
		i = 0;
		J0x3E2:

		// End:0x425 [Loop If]
		if(__NFUN_130__(__NFUN_150__(i, SkipCount), __NFUN_119__(L, List)))
		{
			L = UWindowDynamicTextRow(L.Prev);
			__NFUN_165__(i);
			// [Loop Continue]
			goto J0x3E2;
		}
		Y = __NFUN_175__(WinHeight, DefaultTextHeight);
		J0x437:

		// End:0x4A9 [Loop If]
		if(__NFUN_130__(__NFUN_130__(__NFUN_119__(L, List), __NFUN_119__(L, none)), __NFUN_177__(Y, __NFUN_169__(DefaultTextHeight))))
		{
			DrawTextLine(C, L, Y);
			Y = __NFUN_175__(Y, DefaultTextHeight);
			L = UWindowDynamicTextRow(L.Prev);
			// [Loop Continue]
			goto J0x437;
		}
	}
	return;
}

function UWindowDynamicTextRow AddText(string NewLine)
{
	local UWindowDynamicTextRow L;
	local string temp;
	local int i;

	bDirty = true;
	i = __NFUN_126__(NewLine, "\\n");
	// End:0x53
	if(__NFUN_155__(i, -1))
	{
		temp = __NFUN_127__(NewLine, __NFUN_146__(i, 2));
		NewLine = __NFUN_128__(NewLine, i);		
	}
	else
	{
		temp = "";
	}
	L = CheckMaxRows();
	// End:0x89
	if(__NFUN_119__(L, none))
	{
		List.AppendItem(L);		
	}
	else
	{
		L = UWindowDynamicTextRow(List.Append(RowClass));
	}
	L.Text = NewLine;
	L.WrapParent = none;
	L.bRowDirty = true;
	// End:0xF4
	if(__NFUN_123__(temp, ""))
	{
		AddText(temp);
	}
	return L;
	return;
}

function UWindowDynamicTextRow CheckMaxRows()
{
	local UWindowDynamicTextRow L;

	L = none;
	J0x07:

	// End:0x7C [Loop If]
	if(__NFUN_130__(__NFUN_130__(__NFUN_151__(MaxLines, 0), __NFUN_151__(List.Count(), __NFUN_147__(MaxLines, 1))), __NFUN_119__(List.Next, none)))
	{
		L = UWindowDynamicTextRow(List.Next);
		RemoveWrap(L);
		L.Remove();
		// [Loop Continue]
		goto J0x07;
	}
	return L;
	return;
}

function WordWrap(Canvas C, bool bForce)
{
	local UWindowDynamicTextRow L;

	L = UWindowDynamicTextRow(List.Next);
	J0x19:

	// End:0x83 [Loop If]
	if(__NFUN_119__(L, none))
	{
		// End:0x67
		if(__NFUN_130__(__NFUN_114__(L.WrapParent, none), __NFUN_132__(L.bRowDirty, bForce)))
		{
			WrapRow(C, L);
		}
		L = UWindowDynamicTextRow(L.Next);
		// [Loop Continue]
		goto J0x19;
	}
	bDirty = false;
	return;
}

function WrapRow(Canvas C, UWindowDynamicTextRow L)
{
	local UWindowDynamicTextRow CurrentRow, N;
	local float MaxWidth;
	local int WrapPos;

	// End:0x56
	if(__NFUN_180__(WrapWidth, float(0)))
	{
		// End:0x48
		if(__NFUN_132__(VertSB.bWindowVisible, bAutoScrollbar))
		{
			MaxWidth = __NFUN_175__(WinWidth, VertSB.WinWidth);			
		}
		else
		{
			MaxWidth = WinWidth;
		}		
	}
	else
	{
		MaxWidth = WrapWidth;
	}
	L.bRowDirty = false;
	N = UWindowDynamicTextRow(L.Next);
	// End:0xD1
	if(__NFUN_132__(__NFUN_114__(N, none), __NFUN_119__(N.WrapParent, L)))
	{
		// End:0xD1
		if(__NFUN_154__(GetWrapPos(C, L, MaxWidth), -1))
		{
			return;
		}
	}
	RemoveWrap(L);
	CurrentRow = L;
	J0xE7:

	// End:0x131 [Loop If]
	if(true)
	{
		WrapPos = GetWrapPos(C, CurrentRow, MaxWidth);
		// End:0x118
		if(__NFUN_154__(WrapPos, -1))
		{
			// [Explicit Break]
			goto J0x131;
		}
		CurrentRow = SplitRowAt(CurrentRow, WrapPos);
		// [Loop Continue]
		goto J0xE7;
	}
	J0x131:

	return;
}

function float DrawTextLine(Canvas C, UWindowDynamicTextRow L, float Y)
{
	local float X, W, h;

	// End:0x8C
	if(bHCenter)
	{
		TextAreaTextSize(C, L.Text, W, h);
		// End:0x6D
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
	TextAreaClipText(C, X, Y, L.Text);
	return DefaultTextHeight;
	return;
}

function int GetWrapPos(Canvas C, UWindowDynamicTextRow L, float MaxWidth)
{
	local float W, h, LineWidth, NextWordWidth;
	local string Input, NextWord;
	local int WordsThisRow, WrapPos;

	TextAreaTextSize(C, L.Text, W, h);
	// End:0x38
	if(__NFUN_178__(W, MaxWidth))
	{
		return -1;
	}
	Input = L.Text;
	WordsThisRow = 0;
	LineWidth = 0.0000000;
	WrapPos = 0;
	NextWord = "";
	J0x6D:

	// End:0x115 [Loop If]
	if(__NFUN_132__(__NFUN_123__(Input, ""), __NFUN_123__(NextWord, "")))
	{
		// End:0xBD
		if(__NFUN_122__(NextWord, ""))
		{
			RemoveNextWord(Input, NextWord);
			TextAreaTextSize(C, NextWord, NextWordWidth, h);
		}
		// End:0xE9
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
		goto J0x6D;
	}
	return -1;
	return;
}

function UWindowDynamicTextRow SplitRowAt(UWindowDynamicTextRow L, int SplitPos)
{
	local UWindowDynamicTextRow N;

	N = UWindowDynamicTextRow(L.InsertAfter(RowClass));
	// End:0x4A
	if(__NFUN_114__(L.WrapParent, none))
	{
		N.WrapParent = L;		
	}
	else
	{
		N.WrapParent = L.WrapParent;
	}
	N.Text = __NFUN_127__(L.Text, SplitPos);
	L.Text = __NFUN_128__(L.Text, SplitPos);
	return N;
	return;
}

function RemoveNextWord(out string Text, out string NextWord)
{
	local int i;

	i = __NFUN_126__(Text, " ");
	// End:0x35
	if(__NFUN_154__(i, -1))
	{
		NextWord = Text;
		Text = "";		
	}
	else
	{
		J0x35:

		// End:0x54 [Loop If]
		if(__NFUN_122__(__NFUN_127__(Text, i, 1), " "))
		{
			__NFUN_165__(i);
			// [Loop Continue]
			goto J0x35;
		}
		NextWord = __NFUN_128__(Text, i);
		Text = __NFUN_127__(Text, i);
	}
	return;
}

function RemoveWrap(UWindowDynamicTextRow L)
{
	local UWindowDynamicTextRow N;

	N = UWindowDynamicTextRow(L.Next);
	J0x19:

	// End:0x96 [Loop If]
	if(__NFUN_130__(__NFUN_119__(N, none), __NFUN_114__(N.WrapParent, L)))
	{
		L.Text = __NFUN_112__(L.Text, N.Text);
		N.Remove();
		N = UWindowDynamicTextRow(L.Next);
		// [Loop Continue]
		goto J0x19;
	}
	return;
}

defaultproperties
{
	bScrollOnResize=true
	TextColor=(R=255,G=255,B=255,A=255)
	RowClass=Class'UWindow.UWindowDynamicTextRow'
	bNoKeyboard=true
}