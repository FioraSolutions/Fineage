class UWindowProgressBar extends UWindowWindow;

const BlockWidth = 7;

var float Percent;

function SetPercent(float NewPercent)
{
	Percent = NewPercent;
	return;
}

function Paint(Canvas C, float X, float Y)
{
	local float BlockX, BlockW;

	DrawMiscBevel(C, 0.0000000, 0.0000000, WinWidth, WinHeight, LookAndFeel.Misc, 2);
	C.SetDrawColor(192, 192, 192);
	DrawStretchedTextureSegment(C, float(LookAndFeel.MiscBevelL[2].W), float(LookAndFeel.MiscBevelT[2].h), __NFUN_175__(__NFUN_175__(WinWidth, float(LookAndFeel.MiscBevelL[2].W)), float(LookAndFeel.MiscBevelR[2].W)), __NFUN_175__(__NFUN_175__(WinHeight, float(LookAndFeel.MiscBevelT[2].h)), float(LookAndFeel.MiscBevelB[2].h)), 0.0000000, 0.0000000, 1.0000000, 1.0000000, Texture'UWindow.WhiteTexture');
	C.SetDrawColor(0, 0, byte(255));
	BlockX = float(__NFUN_146__(LookAndFeel.MiscBevelL[2].W, 1));
	J0x142:

	// End:0x286 [Loop If]
	if(__NFUN_176__(BlockX, __NFUN_174__(float(__NFUN_146__(1, LookAndFeel.MiscBevelL[2].W)), __NFUN_172__(__NFUN_171__(Percent, __NFUN_175__(__NFUN_175__(__NFUN_175__(WinWidth, float(LookAndFeel.MiscBevelL[2].W)), float(LookAndFeel.MiscBevelR[2].W)), float(2))), float(100)))))
	{
		BlockW = float(__NFUN_249__(7, int(__NFUN_175__(__NFUN_175__(__NFUN_175__(WinWidth, float(LookAndFeel.MiscBevelR[2].W)), BlockX), float(1)))));
		DrawStretchedTextureSegment(C, BlockX, float(__NFUN_146__(LookAndFeel.MiscBevelT[2].h, 1)), BlockW, __NFUN_175__(__NFUN_175__(__NFUN_175__(WinHeight, float(LookAndFeel.MiscBevelT[2].h)), float(LookAndFeel.MiscBevelB[2].h)), float(1)), 0.0000000, 0.0000000, 1.0000000, 1.0000000, Texture'UWindow.WhiteTexture');
		__NFUN_184__(BlockX, float(__NFUN_146__(7, 1)));
		// [Loop Continue]
		goto J0x142;
	}
	C.SetDrawColor(byte(255), byte(255), byte(255));
	return;
}
