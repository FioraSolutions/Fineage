class UWindowURLTextArea extends UWindowDynamicTextArea
	config;

var bool bReleased;

function BeforePaint(Canvas C, float X, float Y)
{
	super.BeforePaint(C, X, Y);
	Cursor = Root.NormalCursor;
	return;
}

function Paint(Canvas C, float X, float Y)
{
	super.Paint(C, X, Y);
	bReleased = false;
	return;
}

function TextAreaClipText(Canvas C, float DrawX, float DrawY, coerce string S, optional bool bCheckHotKey)
{
	local float X, Y, W, h, ClickX, ClickY;

	local string Text, NextBlock;
	local byte bLink;
	local bool bOverLink;

	Text = S;
	X = DrawX;
	Y = DrawY;
	J0x21:

	// End:0x377 [Loop If]
	if(__NFUN_123__(Text, ""))
	{
		ProcessText(C, Text, NextBlock, W, h, bLink);
		// End:0x78
		if(__NFUN_155__(int(bLink), 0))
		{
			C.SetDrawColor(0, 0, byte(255));			
		}
		else
		{
			C.SetDrawColor(byte(255), byte(255), byte(255));
		}
		GetMouseXY(ClickX, ClickY);
		bOverLink = __NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_155__(int(bLink), 0), __NFUN_176__(DrawX, ClickX)), __NFUN_177__(__NFUN_174__(DrawX, W), ClickX)), __NFUN_176__(DrawY, ClickY)), __NFUN_177__(__NFUN_174__(DrawY, h), ClickY));
		// End:0x123
		if(bOverLink)
		{
			Cursor = Root.HandCursor;
		}
		// End:0x311
		if(__NFUN_130__(bOverLink, __NFUN_132__(bMouseDown, bReleased)))
		{
			// End:0x2E9
			if(bReleased)
			{
				__NFUN_231__(__NFUN_112__(__NFUN_112__("Clicked URL: >>", NextBlock), "<<"));
				// End:0x1A2
				if(__NFUN_124__(__NFUN_128__(NextBlock, 7), "http://"))
				{
					GetPlayerOwner().ConsoleCommand(__NFUN_112__("start ", NextBlock));
				}
				// End:0x1D8
				if(__NFUN_124__(__NFUN_128__(NextBlock, 6), "ftp://"))
				{
					GetPlayerOwner().ConsoleCommand(__NFUN_112__("start ", NextBlock));
				}
				// End:0x211
				if(__NFUN_124__(__NFUN_128__(NextBlock, 9), "telnet://"))
				{
					GetPlayerOwner().ConsoleCommand(__NFUN_112__("start ", NextBlock));
				}
				// End:0x24A
				if(__NFUN_124__(__NFUN_128__(NextBlock, 9), "gopher://"))
				{
					GetPlayerOwner().ConsoleCommand(__NFUN_112__("start ", NextBlock));
				}
				// End:0x285
				if(__NFUN_124__(__NFUN_128__(NextBlock, 4), "www."))
				{
					GetPlayerOwner().ConsoleCommand(__NFUN_112__("start http://", NextBlock));
				}
				// End:0x2C2
				if(__NFUN_124__(__NFUN_128__(NextBlock, 4), "ftp."))
				{
					GetPlayerOwner().ConsoleCommand(__NFUN_112__("start ftp://", NextBlock));					
				}
				else
				{
					// End:0x2E6
					if(__NFUN_124__(__NFUN_128__(NextBlock, 9), "unreal://"))
					{
						LaunchUnrealURL(NextBlock);
					}
				}				
			}
			else
			{
				C.SetDrawColor(byte(255), 0, 0);
			}
			// End:0x311
			if(bReleased)
			{
				bReleased = false;
			}
		}
		// End:0x34E
		if(__NFUN_155__(int(bLink), 0))
		{
			DrawStretchedTexture(C, DrawX, __NFUN_175__(__NFUN_174__(DrawY, h), float(1)), W, 1.0000000, Texture'UWindow.WhiteTexture');
		}
		ClipText(C, DrawX, DrawY, NextBlock);
		__NFUN_184__(DrawX, W);
		// [Loop Continue]
		goto J0x21;
	}
	return;
}

function LaunchUnrealURL(string URL)
{
	GetPlayerOwner().ClientTravel(URL, 0, false);
	return;
}

function Click(float X, float Y)
{
	super(UWindowWindow).Click(X, Y);
	bReleased = true;
	return;
}

function ProcessText(Canvas C, out string Text, out string NextBlock, out float W, out float h, out byte bLink)
{
	local int i, j;

	i = __NFUN_126__(Text, "http://");
	j = __NFUN_126__(Text, "www.");
	// End:0x5E
	if(__NFUN_132__(__NFUN_154__(i, -1), __NFUN_154__(j, -1)))
	{
		i = __NFUN_250__(i, j);		
	}
	else
	{
		i = __NFUN_249__(i, j);
	}
	j = __NFUN_126__(Text, "unreal://");
	// End:0xBD
	if(__NFUN_132__(__NFUN_154__(i, -1), __NFUN_154__(j, -1)))
	{
		i = __NFUN_250__(i, j);		
	}
	else
	{
		i = __NFUN_249__(i, j);
	}
	j = __NFUN_126__(Text, "ftp://");
	// End:0x119
	if(__NFUN_132__(__NFUN_154__(i, -1), __NFUN_154__(j, -1)))
	{
		i = __NFUN_250__(i, j);		
	}
	else
	{
		i = __NFUN_249__(i, j);
	}
	j = __NFUN_126__(Text, "ftp.");
	// End:0x173
	if(__NFUN_132__(__NFUN_154__(i, -1), __NFUN_154__(j, -1)))
	{
		i = __NFUN_250__(i, j);		
	}
	else
	{
		i = __NFUN_249__(i, j);
	}
	j = __NFUN_126__(Text, "telnet://");
	// End:0x1D2
	if(__NFUN_132__(__NFUN_154__(i, -1), __NFUN_154__(j, -1)))
	{
		i = __NFUN_250__(i, j);		
	}
	else
	{
		i = __NFUN_249__(i, j);
	}
	j = __NFUN_126__(Text, "gopher://");
	// End:0x231
	if(__NFUN_132__(__NFUN_154__(i, -1), __NFUN_154__(j, -1)))
	{
		i = __NFUN_250__(i, j);		
	}
	else
	{
		i = __NFUN_249__(i, j);
	}
	bLink = 0;
	// End:0x270
	if(__NFUN_154__(i, -1))
	{
		NextBlock = Text;
		Text = "";		
	}
	else
	{
		// End:0x2DF
		if(__NFUN_154__(i, 0))
		{
			bLink = 1;
			i = __NFUN_126__(Text, " ");
			// End:0x2B8
			if(__NFUN_154__(i, -1))
			{
				NextBlock = Text;
				Text = "";				
			}
			else
			{
				NextBlock = __NFUN_128__(Text, i);
				Text = __NFUN_127__(Text, i);
			}			
		}
		else
		{
			NextBlock = __NFUN_128__(Text, i);
			Text = __NFUN_127__(Text, i);
		}
	}
	TextAreaTextSize(C, NextBlock, W, h);
	return;
}

defaultproperties
{
	bIgnoreLDoubleClick=true
}