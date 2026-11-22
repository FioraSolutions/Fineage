class UWindowSmallButton extends UWindowButton;

function Created()
{
	bNoKeyboard = true;
	super.Created();
	ToolTipString = "";
	SetText("");
	SetFont(0);
	WinHeight = 16.0000000;
	return;
}

function AutoWidth(Canvas C)
{
	local float W, h;

	C.Font = Root.Fonts[Font];
	TextSize(C, RemoveAmpersand(Text), W, h);
	// End:0x69
	if(__NFUN_176__(WinWidth, __NFUN_174__(W, float(10))))
	{
		WinWidth = __NFUN_174__(W, float(10));
	}
	return;
}

function BeforePaint(Canvas C, float X, float Y)
{
	local float W, h;

	C.Font = Root.Fonts[Font];
	TextSize(C, RemoveAmpersand(Text), W, h);
	TextX = __NFUN_172__(__NFUN_175__(WinWidth, W), float(2));
	TextY = __NFUN_172__(__NFUN_175__(WinHeight, h), float(2));
	// End:0x90
	if(bMouseDown)
	{
		__NFUN_184__(TextX, float(1));
		__NFUN_184__(TextY, float(1));
	}
	return;
}

function Paint(Canvas C, float X, float Y)
{
	LookAndFeel.Button_DrawSmallButton(self, C);
	super.Paint(C, X, Y);
	return;
}
