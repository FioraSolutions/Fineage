class UWindowWrappedTextArea extends UWindowTextAreaControl;

function Paint(Canvas C, float X, float Y)
{
	local int i, Line, TempHead, TempTail;
	local float XL, YL;

	C.Font = Root.Fonts[Font];
	C.SetDrawColor(byte(255), byte(255), byte(255));
	TextSize(C, "TEST", XL, YL);
	VisibleRows = int(__NFUN_172__(WinHeight, YL));
	// End:0x98
	if(bScrollable)
	{
		VertSB.SetRange(0.0000000, float(Lines), float(VisibleRows));
	}
	TempHead = Head;
	TempTail = Tail;
	Line = TempHead;
	TextArea[Line] = Prompt;
	// End:0x147
	if(bScrollable)
	{
		// End:0x147
		if(__NFUN_177__(__NFUN_175__(VertSB.MaxPos, VertSB.pos), float(0)))
		{
			__NFUN_162__(Line, int(__NFUN_175__(VertSB.MaxPos, VertSB.pos)));
			__NFUN_162__(TempTail, int(__NFUN_175__(VertSB.MaxPos, VertSB.pos)));
		}
	}
	i = 0;
	J0x14E:

	// End:0x1CF [Loop If]
	if(__NFUN_150__(i, VisibleRows))
	{
		WrapClipText(C, 2.0000000, __NFUN_171__(YL, float(__NFUN_147__(__NFUN_147__(VisibleRows, i), 1))), TextArea[__NFUN_147__(Line, 1)]);
		__NFUN_166__(Line);
		// End:0x1AC
		if(__NFUN_154__(TempTail, Line))
		{
			// [Explicit Break]
			goto J0x1CF;
		}
		// End:0x1C5
		if(__NFUN_150__(Line, 0))
		{
			Line = __NFUN_147__(BufSize, 1);
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x14E;
	}
	J0x1CF:

	return;
}
