class UWindowLabelControl extends UWindowDialogControl;

function Created()
{
	TextX = 0.0000000;
	TextY = 0.0000000;
	return;
}

function BeforePaint(Canvas C, float X, float Y)
{
	local float W, h;

	super.BeforePaint(C, X, Y);
	TextSize(C, Text, W, h);
	WinHeight = __NFUN_174__(h, float(1));
	TextY = __NFUN_172__(__NFUN_175__(WinHeight, h), float(2));
	switch(Align)
	{
		// End:0x66
		case 0:
			// End:0xA3
			break;
		// End:0x86
		case 2:
			TextX = __NFUN_172__(__NFUN_175__(WinWidth, W), float(2));
			// End:0xA3
			break;
		// End:0xA0
		case 1:
			TextX = __NFUN_175__(WinWidth, W);
			// End:0xA3
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function Paint(Canvas C, float X, float Y)
{
	// End:0x91
	if(__NFUN_123__(Text, ""))
	{
		C.SetDrawColor(TextColor.R, TextColor.G, TextColor.B);
		C.Font = Root.Fonts[Font];
		ClipText(C, TextX, TextY, Text);
		C.SetDrawColor(byte(255), byte(255), byte(255));
	}
	return;
}
