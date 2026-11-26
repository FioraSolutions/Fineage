class UWindowHSliderControl extends UWindowDialogControl;

var float MinValue;
var float MaxValue;
var float Value;
var int Step;
var float SliderWidth;
var float SliderDrawX;
var float SliderDrawY;
var float TrackStart;
var float TrackWidth;
var bool bSliding;
var bool bNoSlidingNotify;

function Created()
{
	super.Created();
	SliderWidth = __NFUN_172__(WinWidth, float(2));
	TrackWidth = 4.0000000;
	return;
}

function SetRange(float Min, float Max, int NewStep)
{
	MinValue = Min;
	MaxValue = Max;
	Step = NewStep;
	Value = CheckValue(Value);
	return;
}

function float GetValue()
{
	return Value;
	return;
}

function SetValue(float NewValue, optional bool bNoNotify)
{
	local float OldValue;

	OldValue = Value;
	Value = CheckValue(NewValue);
	// End:0x40
	if(__NFUN_130__(__NFUN_181__(Value, OldValue), __NFUN_129__(bNoNotify)))
	{
		Notify(1);
	}
	return;
}

function float CheckValue(float test)
{
	local float TempF, NewValue;

	NewValue = test;
	// End:0x47
	if(__NFUN_155__(Step, 0))
	{
		TempF = __NFUN_172__(NewValue, float(Step));
		NewValue = float(__NFUN_144__(int(__NFUN_174__(TempF, 0.5000000)), Step));
	}
	// End:0x61
	if(__NFUN_176__(NewValue, MinValue))
	{
		NewValue = MinValue;
	}
	// End:0x7B
	if(__NFUN_177__(NewValue, MaxValue))
	{
		NewValue = MaxValue;
	}
	return NewValue;
	return;
}

function BeforePaint(Canvas C, float X, float Y)
{
	local float W, h;

	super.BeforePaint(C, X, Y);
	TextSize(C, Text, W, h);
	WinHeight = __NFUN_174__(h, float(1));
	switch(Align)
	{
		// End:0x6B
		case 0:
			SliderDrawX = __NFUN_175__(WinWidth, SliderWidth);
			TextX = 0.0000000;
			// End:0xCB
			break;
		// End:0x90
		case 1:
			SliderDrawX = 0.0000000;
			TextX = __NFUN_175__(WinWidth, W);
			// End:0xCB
			break;
		// End:0xC8
		case 2:
			SliderDrawX = __NFUN_172__(__NFUN_175__(WinWidth, SliderWidth), float(2));
			TextX = __NFUN_172__(__NFUN_175__(WinWidth, W), float(2));
			// End:0xCB
			break;
		// End:0xFFFF
		default:
			break;
	}
	SliderDrawY = __NFUN_172__(__NFUN_175__(WinHeight, float(2)), float(2));
	TextY = __NFUN_172__(__NFUN_175__(WinHeight, h), float(2));
	TrackStart = __NFUN_174__(SliderDrawX, __NFUN_171__(__NFUN_175__(SliderWidth, TrackWidth), __NFUN_172__(__NFUN_175__(Value, MinValue), __NFUN_175__(MaxValue, MinValue))));
	return;
}

function Paint(Canvas C, float X, float Y)
{
	local Texture t;
	local Region R;

	t = GetLookAndFeelTexture();
	// End:0x61
	if(__NFUN_123__(Text, ""))
	{
		C.DrawColor = TextColor;
		ClipText(C, TextX, TextY, Text);
		C.SetDrawColor(byte(255), byte(255), byte(255));
	}
	R = LookAndFeel.HLine;
	DrawStretchedTextureSegment(C, SliderDrawX, SliderDrawY, SliderWidth, float(R.h), float(R.X), float(R.Y), float(R.W), float(R.h), t);
	DrawUpBevel(C, TrackStart, __NFUN_175__(SliderDrawY, float(4)), TrackWidth, 10.0000000, t);
	return;
}

function LMouseUp(float X, float Y)
{
	super(UWindowWindow).LMouseUp(X, Y);
	// End:0x21
	if(bNoSlidingNotify)
	{
		Notify(1);
	}
	return;
}

function LMouseDown(float X, float Y)
{
	super(UWindowWindow).LMouseDown(X, Y);
	// End:0x4E
	if(__NFUN_130__(__NFUN_179__(X, TrackStart), __NFUN_178__(X, __NFUN_174__(TrackStart, TrackWidth))))
	{
		bSliding = true;
		Root.CaptureMouse();
	}
	// End:0xA0
	if(__NFUN_130__(__NFUN_176__(X, TrackStart), __NFUN_177__(X, SliderDrawX)))
	{
		// End:0x90
		if(__NFUN_155__(Step, 0))
		{
			SetValue(__NFUN_175__(Value, float(Step)));			
		}
		else
		{
			SetValue(__NFUN_175__(Value, float(1)));
		}
	}
	// End:0x100
	if(__NFUN_130__(__NFUN_177__(X, __NFUN_174__(TrackStart, TrackWidth)), __NFUN_176__(X, __NFUN_174__(SliderDrawX, SliderWidth))))
	{
		// End:0xF0
		if(__NFUN_155__(Step, 0))
		{
			SetValue(__NFUN_174__(Value, float(Step)));			
		}
		else
		{
			SetValue(__NFUN_174__(Value, float(1)));
		}
	}
	return;
}

function MouseMove(float X, float Y)
{
	super.MouseMove(X, Y);
	// End:0x62
	if(__NFUN_130__(bSliding, bMouseDown))
	{
		SetValue(__NFUN_174__(__NFUN_171__(__NFUN_172__(__NFUN_175__(X, SliderDrawX), __NFUN_175__(SliderWidth, TrackWidth)), __NFUN_175__(MaxValue, MinValue)), MinValue), bNoSlidingNotify);		
	}
	else
	{
		bSliding = false;
	}
	return;
}

function KeyDown(int Key, float X, float Y)
{
	local Interaction C;

	C = GetPlayerOwner().Player.Console;
	switch(Key)
	{
		// End:0x6A
		case int(C.37):
			// End:0x57
			if(__NFUN_155__(Step, 0))
			{
				SetValue(__NFUN_175__(Value, float(Step)));				
			}
			else
			{
				SetValue(__NFUN_175__(Value, float(1)));
			}
			// End:0x106
			break;
		// End:0xAF
		case int(C.39):
			// End:0x9C
			if(__NFUN_155__(Step, 0))
			{
				SetValue(__NFUN_174__(Value, float(Step)));				
			}
			else
			{
				SetValue(__NFUN_174__(Value, float(1)));
			}
			// End:0x106
			break;
		// End:0xCD
		case int(C.36):
			SetValue(MinValue);
			// End:0x106
			break;
		// End:0xEB
		case int(C.35):
			SetValue(MaxValue);
			// End:0x106
			break;
		// End:0xFFFF
		default:
			super.KeyDown(Key, X, Y);
			// End:0x106
			break;
			break;
	}
	return;
}
