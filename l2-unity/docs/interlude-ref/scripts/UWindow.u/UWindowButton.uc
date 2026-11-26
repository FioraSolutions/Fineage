class UWindowButton extends UWindowDialogControl;

var bool bDisabled;
var bool bStretched;
var Texture UpTexture;
var Texture DownTexture;
var Texture DisabledTexture;
var Texture OverTexture;
var Region UpRegion;
var Region DownRegion;
var Region DisabledRegion;
var Region OverRegion;
var bool bUseRegion;
var float RegionScale;
var string ToolTipString;
var float ImageX;
var float ImageY;
var Sound OverSound;
var Sound DownSound;

function Created()
{
	super.Created();
	ImageX = 0.0000000;
	ImageY = 0.0000000;
	TextX = 0.0000000;
	TextY = 0.0000000;
	RegionScale = 1.0000000;
	return;
}

function BeforePaint(Canvas C, float X, float Y)
{
	C.Font = Root.Fonts[Font];
	return;
}

function Paint(Canvas C, float X, float Y)
{
	C.Font = Root.Fonts[Font];
	// End:0x100
	if(bDisabled)
	{
		// End:0xFD
		if(__NFUN_119__(DisabledTexture, none))
		{
			// End:0xB3
			if(bUseRegion)
			{
				DrawStretchedTextureSegment(C, ImageX, ImageY, __NFUN_171__(float(DisabledRegion.W), RegionScale), __NFUN_171__(float(DisabledRegion.h), RegionScale), float(DisabledRegion.X), float(DisabledRegion.Y), float(DisabledRegion.W), float(DisabledRegion.h), DisabledTexture);				
			}
			else
			{
				// End:0xE3
				if(bStretched)
				{
					DrawStretchedTexture(C, ImageX, ImageY, WinWidth, WinHeight, DisabledTexture);					
				}
				else
				{
					DrawClippedTexture(C, ImageX, ImageY, DisabledTexture);
				}
			}
		}		
	}
	else
	{
		// End:0x1DD
		if(bMouseDown)
		{
			// End:0x1DA
			if(__NFUN_119__(DownTexture, none))
			{
				// End:0x190
				if(bUseRegion)
				{
					DrawStretchedTextureSegment(C, ImageX, ImageY, __NFUN_171__(float(DownRegion.W), RegionScale), __NFUN_171__(float(DownRegion.h), RegionScale), float(DownRegion.X), float(DownRegion.Y), float(DownRegion.W), float(DownRegion.h), DownTexture);					
				}
				else
				{
					// End:0x1C0
					if(bStretched)
					{
						DrawStretchedTexture(C, ImageX, ImageY, WinWidth, WinHeight, DownTexture);						
					}
					else
					{
						DrawClippedTexture(C, ImageX, ImageY, DownTexture);
					}
				}
			}			
		}
		else
		{
			// End:0x2BA
			if(MouseIsOver())
			{
				// End:0x2B7
				if(__NFUN_119__(OverTexture, none))
				{
					// End:0x26D
					if(bUseRegion)
					{
						DrawStretchedTextureSegment(C, ImageX, ImageY, __NFUN_171__(float(OverRegion.W), RegionScale), __NFUN_171__(float(OverRegion.h), RegionScale), float(OverRegion.X), float(OverRegion.Y), float(OverRegion.W), float(OverRegion.h), OverTexture);						
					}
					else
					{
						// End:0x29D
						if(bStretched)
						{
							DrawStretchedTexture(C, ImageX, ImageY, WinWidth, WinHeight, OverTexture);							
						}
						else
						{
							DrawClippedTexture(C, ImageX, ImageY, OverTexture);
						}
					}
				}				
			}
			else
			{
				// End:0x38B
				if(__NFUN_119__(UpTexture, none))
				{
					// End:0x341
					if(bUseRegion)
					{
						DrawStretchedTextureSegment(C, ImageX, ImageY, __NFUN_171__(float(UpRegion.W), RegionScale), __NFUN_171__(float(UpRegion.h), RegionScale), float(UpRegion.X), float(UpRegion.Y), float(UpRegion.W), float(UpRegion.h), UpTexture);						
					}
					else
					{
						// End:0x371
						if(bStretched)
						{
							DrawStretchedTexture(C, ImageX, ImageY, WinWidth, WinHeight, UpTexture);							
						}
						else
						{
							DrawClippedTexture(C, ImageX, ImageY, UpTexture);
						}
					}
				}
			}
		}
	}
	// End:0x3E1
	if(__NFUN_123__(Text, ""))
	{
		C.DrawColor = TextColor;
		ClipText(C, TextX, TextY, Text, true);
		C.SetDrawColor(byte(255), byte(255), byte(255));
	}
	return;
}

function MouseLeave()
{
	super.MouseLeave();
	// End:0x1A
	if(__NFUN_123__(ToolTipString, ""))
	{
		ToolTip("");
	}
	return;
}

simulated function MouseEnter()
{
	super.MouseEnter();
	// End:0x1D
	if(__NFUN_123__(ToolTipString, ""))
	{
		ToolTip(ToolTipString);
	}
	// End:0x49
	if(__NFUN_130__(__NFUN_129__(bDisabled), __NFUN_119__(OverSound, none)))
	{
		GetPlayerOwner().__NFUN_264__(OverSound, 6);
	}
	return;
}

simulated function Click(float X, float Y)
{
	Notify(2);
	// End:0x34
	if(__NFUN_130__(__NFUN_129__(bDisabled), __NFUN_119__(DownSound, none)))
	{
		GetPlayerOwner().__NFUN_264__(DownSound, 3);
	}
	return;
}

function DoubleClick(float X, float Y)
{
	Notify(11);
	return;
}

function RClick(float X, float Y)
{
	Notify(6);
	return;
}

function MClick(float X, float Y)
{
	Notify(5);
	return;
}

function KeyDown(int Key, float X, float Y)
{
	local PlayerController P;

	P = Root.GetPlayerOwner();
	switch(Key)
	{
		// End:0x61
		case int(P.Player.Console.32):
			LMouseDown(X, Y);
			LMouseUp(X, Y);
			// End:0x7C
			break;
		// End:0xFFFF
		default:
			super.KeyDown(Key, X, Y);
			// End:0x7C
			break;
			break;
	}
	return;
}

defaultproperties
{
	bIgnoreLDoubleClick=true
	bIgnoreMDoubleClick=true
	bIgnoreRDoubleClick=true
}