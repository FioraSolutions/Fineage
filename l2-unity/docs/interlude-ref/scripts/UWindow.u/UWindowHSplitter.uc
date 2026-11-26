class UWindowHSplitter extends UWindowWindow;

var UWindowWindow LeftClientWindow;
var UWindowWindow RightClientWindow;
var bool bSizing;
var float SplitPos;
var float MinWinWidth;
var float OldWinWidth;
var float MaxSplitPos;
var bool bRightGrow;
var bool bSizable;

function Created()
{
	super.Created();
	bAlwaysBehind = true;
	SplitPos = __NFUN_172__(WinWidth, float(2));
	MinWinWidth = 24.0000000;
	OldWinWidth = WinWidth;
	return;
}

function Paint(Canvas C, float X, float Y)
{
	local Texture t;

	t = GetLookAndFeelTexture();
	DrawUpBevel(C, SplitPos, 0.0000000, 7.0000000, WinHeight, t);
	return;
}

function BeforePaint(Canvas C, float X, float Y)
{
	local float NewW, NewH;

	// End:0x35
	if(__NFUN_130__(__NFUN_181__(OldWinWidth, WinWidth), __NFUN_129__(bRightGrow)))
	{
		SplitPos = __NFUN_175__(__NFUN_174__(SplitPos, WinWidth), OldWinWidth);
	}
	SplitPos = __NFUN_246__(SplitPos, MinWinWidth, __NFUN_175__(__NFUN_175__(WinWidth, float(7)), MinWinWidth));
	// End:0x7D
	if(__NFUN_181__(MaxSplitPos, float(0)))
	{
		SplitPos = __NFUN_246__(SplitPos, 0.0000000, MaxSplitPos);
	}
	NewW = SplitPos;
	NewH = WinHeight;
	// End:0xDE
	if(__NFUN_132__(__NFUN_181__(NewH, LeftClientWindow.WinHeight), __NFUN_181__(NewW, LeftClientWindow.WinWidth)))
	{
		LeftClientWindow.SetSize(NewW, NewH);
	}
	LeftClientWindow.WinTop = 0.0000000;
	LeftClientWindow.WinLeft = 0.0000000;
	NewW = __NFUN_175__(__NFUN_175__(WinWidth, SplitPos), float(7));
	// End:0x169
	if(__NFUN_132__(__NFUN_181__(NewH, RightClientWindow.WinHeight), __NFUN_181__(NewW, RightClientWindow.WinWidth)))
	{
		RightClientWindow.SetSize(NewW, NewH);
	}
	RightClientWindow.WinTop = 0.0000000;
	RightClientWindow.WinLeft = __NFUN_174__(SplitPos, float(7));
	OldWinWidth = WinWidth;
	return;
}

function LMouseDown(float X, float Y)
{
	super.LMouseDown(X, Y);
	// End:0x58
	if(__NFUN_130__(__NFUN_130__(bSizable, __NFUN_179__(X, SplitPos)), __NFUN_178__(X, __NFUN_174__(SplitPos, float(7)))))
	{
		bSizing = true;
		Root.CaptureMouse();
	}
	return;
}

function MouseMove(float X, float Y)
{
	// End:0x48
	if(__NFUN_130__(__NFUN_130__(bSizable, __NFUN_179__(X, SplitPos)), __NFUN_178__(X, __NFUN_174__(SplitPos, float(7)))))
	{
		Cursor = Root.HSplitCursor;		
	}
	else
	{
		Cursor = Root.NormalCursor;
	}
	// End:0x7E
	if(__NFUN_130__(bSizing, bMouseDown))
	{
		SplitPos = X;		
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