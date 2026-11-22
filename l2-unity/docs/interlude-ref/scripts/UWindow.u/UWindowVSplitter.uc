class UWindowVSplitter extends UWindowWindow;

var UWindowWindow TopClientWindow;
var UWindowWindow BottomClientWindow;
var bool bSizing;
var float SplitPos;
var float MinWinHeight;
var float MaxSplitPos;
var float OldWinHeight;
var bool bBottomGrow;
var bool bSizable;

function Created()
{
	super.Created();
	bAlwaysBehind = true;
	SplitPos = __NFUN_172__(WinHeight, float(2));
	MinWinHeight = 24.0000000;
	OldWinHeight = WinHeight;
	return;
}

function Paint(Canvas C, float X, float Y)
{
	local Texture t;

	t = GetLookAndFeelTexture();
	DrawUpBevel(C, 0.0000000, SplitPos, WinWidth, 7.0000000, t);
	return;
}

function BeforePaint(Canvas C, float X, float Y)
{
	local float NewW, NewH;

	// End:0x35
	if(__NFUN_130__(__NFUN_181__(OldWinHeight, WinHeight), __NFUN_129__(bBottomGrow)))
	{
		SplitPos = __NFUN_175__(__NFUN_174__(SplitPos, WinHeight), OldWinHeight);
	}
	SplitPos = __NFUN_246__(SplitPos, MinWinHeight, __NFUN_175__(__NFUN_175__(WinHeight, float(7)), MinWinHeight));
	// End:0x7D
	if(__NFUN_181__(MaxSplitPos, float(0)))
	{
		SplitPos = __NFUN_246__(SplitPos, 0.0000000, MaxSplitPos);
	}
	NewW = WinWidth;
	NewH = SplitPos;
	// End:0xDE
	if(__NFUN_132__(__NFUN_181__(NewW, TopClientWindow.WinWidth), __NFUN_181__(NewH, TopClientWindow.WinHeight)))
	{
		TopClientWindow.SetSize(NewW, NewH);
	}
	NewH = __NFUN_175__(__NFUN_175__(WinHeight, SplitPos), float(7));
	// End:0x141
	if(__NFUN_132__(__NFUN_181__(NewW, BottomClientWindow.WinWidth), __NFUN_181__(NewH, BottomClientWindow.WinHeight)))
	{
		BottomClientWindow.SetSize(NewW, NewH);
	}
	BottomClientWindow.WinTop = __NFUN_174__(SplitPos, float(7));
	BottomClientWindow.WinLeft = 0.0000000;
	OldWinHeight = WinHeight;
	return;
}

function LMouseDown(float X, float Y)
{
	super.LMouseDown(X, Y);
	// End:0x58
	if(__NFUN_130__(__NFUN_130__(bSizable, __NFUN_179__(Y, SplitPos)), __NFUN_178__(Y, __NFUN_174__(SplitPos, float(7)))))
	{
		bSizing = true;
		Root.CaptureMouse();
	}
	return;
}

function MouseMove(float X, float Y)
{
	// End:0x48
	if(__NFUN_130__(__NFUN_130__(bSizable, __NFUN_179__(Y, SplitPos)), __NFUN_178__(Y, __NFUN_174__(SplitPos, float(7)))))
	{
		Cursor = Root.VSplitCursor;		
	}
	else
	{
		Cursor = Root.NormalCursor;
	}
	// End:0x7E
	if(__NFUN_130__(bSizing, bMouseDown))
	{
		SplitPos = Y;		
	}
	else
	{
		bSizing = false;
	}
	return;
}

defaultproperties
{
	bSizable=true
}