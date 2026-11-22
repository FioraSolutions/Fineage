class UWindowTextAreaControl extends UWindowDialogControl;

var string TextArea[750];
var string Prompt;
var int Font;
var Font AbsoluteFont;
var int BufSize;
var int Head;
var int Tail;
var int Lines;
var int VisibleRows;
var bool bCursor;
var bool bScrollable;
var bool bShowCaret;
var bool bScrollOnResize;
var UWindowVScrollbar VertSB;
var float LastDrawTime;

function Created()
{
	super.Created();
	LastDrawTime = GetLevel().TimeSeconds;
	return;
}

function SetScrollable(bool newScrollable)
{
	bScrollable = newScrollable;
	// End:0x5A
	if(newScrollable)
	{
		VertSB = UWindowVScrollbar(CreateWindow(Class'UWindow.UWindowVScrollbar', __NFUN_175__(WinWidth, float(12)), 0.0000000, 12.0000000, WinHeight));
		VertSB.bAlwaysOnTop = true;		
	}
	else
	{
		// End:0x7B
		if(__NFUN_119__(VertSB, none))
		{
			VertSB.Close();
			VertSB = none;
		}
	}
	return;
}

function BeforePaint(Canvas C, float X, float Y)
{
	super.BeforePaint(C, X, Y);
	// End:0x89
	if(__NFUN_119__(VertSB, none))
	{
		VertSB.WinTop = 0.0000000;
		VertSB.WinHeight = WinHeight;
		VertSB.WinWidth = LookAndFeel.Size_ScrollbarWidth;
		VertSB.WinLeft = __NFUN_175__(WinWidth, LookAndFeel.Size_ScrollbarWidth);
	}
	return;
}

function SetAbsoluteFont(Font f)
{
	AbsoluteFont = f;
	return;
}

function Paint(Canvas C, float X, float Y)
{
	local int i, Line, TempHead, TempTail;
	local float XL, YL, W, h;

	// End:0x22
	if(__NFUN_119__(AbsoluteFont, none))
	{
		C.Font = AbsoluteFont;		
	}
	else
	{
		C.Font = Root.Fonts[Font];
	}
	C.SetDrawColor(byte(255), byte(255), byte(255));
	TextSize(C, "TEST", XL, YL);
	VisibleRows = int(__NFUN_172__(WinHeight, YL));
	TempHead = Head;
	TempTail = Tail;
	Line = TempHead;
	TextArea[Line] = Prompt;
	// End:0xEB
	if(__NFUN_122__(Prompt, ""))
	{
		__NFUN_166__(Line);
		// End:0xEB
		if(__NFUN_150__(Line, 0))
		{
			__NFUN_161__(Line, BufSize);
		}
	}
	// End:0x196
	if(bScrollable)
	{
		// End:0x196
		if(__NFUN_179__(__NFUN_175__(VertSB.MaxPos, VertSB.pos), float(0)))
		{
			__NFUN_162__(Line, int(__NFUN_175__(VertSB.MaxPos, VertSB.pos)));
			__NFUN_162__(TempTail, int(__NFUN_175__(VertSB.MaxPos, VertSB.pos)));
			// End:0x17F
			if(__NFUN_150__(Line, 0))
			{
				__NFUN_161__(Line, BufSize);
			}
			// End:0x196
			if(__NFUN_150__(TempTail, 0))
			{
				__NFUN_161__(TempTail, BufSize);
			}
		}
	}
	// End:0x1AC
	if(__NFUN_129__(bCursor))
	{
		bShowCaret = false;		
	}
	else
	{
		// End:0x20B
		if(__NFUN_132__(__NFUN_177__(GetLevel().TimeSeconds, __NFUN_174__(LastDrawTime, 0.3000000)), __NFUN_176__(GetLevel().TimeSeconds, LastDrawTime)))
		{
			LastDrawTime = GetLevel().TimeSeconds;
			bShowCaret = __NFUN_129__(bShowCaret);
		}
	}
	i = 0;
	J0x212:

	// End:0x2F6 [Loop If]
	if(__NFUN_150__(i, __NFUN_146__(VisibleRows, 1)))
	{
		ClipText(C, 2.0000000, __NFUN_175__(WinHeight, __NFUN_171__(YL, float(__NFUN_146__(i, 1)))), TextArea[Line]);
		// End:0x2BC
		if(__NFUN_130__(__NFUN_154__(Line, Head), bShowCaret))
		{
			TextSize(C, TextArea[Line], W, h);
			ClipText(C, W, __NFUN_175__(WinHeight, __NFUN_171__(YL, float(__NFUN_146__(i, 1)))), "|");
		}
		// End:0x2CE
		if(__NFUN_154__(TempTail, Line))
		{
			// [Explicit Break]
			goto J0x2F6;
		}
		__NFUN_166__(Line);
		// End:0x2EC
		if(__NFUN_150__(Line, 0))
		{
			__NFUN_161__(Line, BufSize);
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x212;
	}
	J0x2F6:

	return;
}

function AddText(string NewLine)
{
	TextArea[Head] = NewLine;
	Head = int(__NFUN_173__(float(__NFUN_146__(Head, 1)), float(BufSize)));
	// End:0x56
	if(__NFUN_154__(Head, Tail))
	{
		Tail = int(__NFUN_173__(float(__NFUN_146__(Tail, 1)), float(BufSize)));
	}
	Lines = __NFUN_147__(Head, Tail);
	// End:0x7F
	if(__NFUN_150__(Lines, 0))
	{
		__NFUN_161__(Lines, BufSize);
	}
	// End:0xC7
	if(bScrollable)
	{
		VertSB.SetRange(0.0000000, float(Lines), float(VisibleRows));
		VertSB.pos = VertSB.MaxPos;
	}
	return;
}

function Resized()
{
	// End:0x51
	if(bScrollable)
	{
		VertSB.SetRange(0.0000000, float(Lines), float(VisibleRows));
		// End:0x51
		if(bScrollOnResize)
		{
			VertSB.pos = VertSB.MaxPos;
		}
	}
	return;
}

function SetPrompt(string NewPrompt)
{
	Prompt = NewPrompt;
	return;
}

function Clear()
{
	TextArea[0] = "";
	Head = 0;
	Tail = 0;
	return;
}

defaultproperties
{
	BufSize=750
	bScrollOnResize=true
}