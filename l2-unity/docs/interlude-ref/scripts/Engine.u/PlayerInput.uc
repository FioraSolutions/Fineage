class PlayerInput extends Object within PlayerController
	transient
	native
	config(User);

var globalconfig bool bInvertMouse;
var bool bWasForward;
var bool bWasBack;
var bool bWasLeft;
var bool bWasRight;
var bool bEdgeForward;
var bool bEdgeBack;
var bool bEdgeLeft;
var bool bEdgeRight;
var bool bAdjustSampling;
var globalconfig byte MouseSmoothingMode;
var globalconfig float MouseSmoothingStrength;
var globalconfig float MouseSensitivity;
var globalconfig float MouseSamplingTime;
var globalconfig float MouseAccelThreshold;
var float SmoothedMouse[2];
var float ZeroTime[2];
var float SamplingTime[2];
var float MaybeTime[2];
var float OldSamples[4];
var int MouseSamples[2];
var float DoubleClickTimer;
var globalconfig float DoubleClickTime;

function bool InvertLook()
{
	return;
}

event PlayerInput(float DeltaTime)
{
	local float FOVScale, MouseScale;

	bEdgeForward = __NFUN_131__(bWasForward, __NFUN_177__(Outer.aBaseY, float(0)));
	bEdgeBack = __NFUN_131__(bWasBack, __NFUN_176__(Outer.aBaseY, float(0)));
	bEdgeLeft = __NFUN_131__(bWasLeft, __NFUN_176__(Outer.aStrafe, float(0)));
	bEdgeRight = __NFUN_131__(bWasRight, __NFUN_177__(Outer.aStrafe, float(0)));
	bWasForward = __NFUN_177__(Outer.aBaseY, float(0));
	bWasBack = __NFUN_176__(Outer.aBaseY, float(0));
	bWasLeft = __NFUN_176__(Outer.aStrafe, float(0));
	bWasRight = __NFUN_177__(Outer.aStrafe, float(0));
	FOVScale = __NFUN_171__(Outer.DesiredFOV, 0.0111100);
	MouseScale = __NFUN_171__(MouseSensitivity, FOVScale);
	Outer.aMouseX = SmoothMouse(__NFUN_171__(Outer.aMouseX, MouseScale), DeltaTime, Outer.bXAxis, 0);
	Outer.aMouseY = SmoothMouse(__NFUN_171__(Outer.aMouseY, MouseScale), DeltaTime, Outer.bYAxis, 1);
	Outer.aMouseX = AccelerateMouse(Outer.aMouseX);
	Outer.aMouseY = AccelerateMouse(Outer.aMouseY);
	__NFUN_182__(Outer.aLookUp, FOVScale);
	__NFUN_182__(Outer.aTurn, FOVScale);
	// End:0x257
	if(__NFUN_155__(int(Outer.bStrafe), 0))
	{
		__NFUN_184__(Outer.aStrafe, __NFUN_174__(__NFUN_171__(Outer.aBaseX, 7.5000000), Outer.aMouseX));		
	}
	else
	{
		__NFUN_184__(Outer.aTurn, __NFUN_174__(__NFUN_171__(Outer.aBaseX, FOVScale), Outer.aMouseX));
	}
	Outer.aBaseX = 0.0000000;
	// End:0x32D
	if(__NFUN_130__(__NFUN_154__(int(Outer.bStrafe), 0), __NFUN_132__(Outer.bAlwaysMouseLook, __NFUN_155__(int(Outer.bLook), 0))))
	{
		// End:0x30C
		if(bInvertMouse)
		{
			__NFUN_185__(Outer.aLookUp, Outer.aMouseY);			
		}
		else
		{
			__NFUN_184__(Outer.aLookUp, Outer.aMouseY);
		}		
	}
	else
	{
		__NFUN_184__(Outer.aForward, Outer.aMouseY);
	}
	// End:0x386
	if(__NFUN_155__(int(Outer.bSnapLevel), 0))
	{
		Outer.bCenterView = true;
		Outer.bKeyboardLook = false;		
	}
	else
	{
		// End:0x3C1
		if(__NFUN_181__(Outer.aLookUp, float(0)))
		{
			Outer.bCenterView = false;
			Outer.bKeyboardLook = true;			
		}
		else
		{
			// End:0x40B
			if(__NFUN_130__(Outer.bSnapToLevel, __NFUN_129__(Outer.bAlwaysMouseLook)))
			{
				Outer.bCenterView = true;
				Outer.bKeyboardLook = false;
			}
		}
	}
	// End:0x461
	if(__NFUN_155__(int(Outer.bFreeLook), 0))
	{
		Outer.bKeyboardLook = true;
		__NFUN_184__(Outer.aLookUp, __NFUN_171__(__NFUN_171__(0.5000000, Outer.aBaseY), FOVScale));		
	}
	else
	{
		__NFUN_184__(Outer.aForward, Outer.aBaseY);
	}
	Outer.aBaseY = 0.0000000;
	Outer.HandleWalking();
	return;
}

exec function SetSmoothingMode(byte B)
{
	MouseSmoothingMode = B;
	__NFUN_231__(__NFUN_112__("Smoothing mode ", string(MouseSmoothingMode)));
	return;
}

exec function SetSmoothingStrength(float f)
{
	MouseSmoothingStrength = __NFUN_246__(f, 0.0000000, 1.0000000);
	return;
}

function float AccelerateMouse(float aMouse)
{
	local float Accel;

	// End:0x15
	if(__NFUN_180__(__NFUN_186__(aMouse), float(0)))
	{
		return 0.0000000;
	}
	Accel = __NFUN_171__(MouseAccelThreshold, MouseSensitivity);
	// End:0x7A
	if(__NFUN_176__(__NFUN_186__(aMouse), Accel))
	{
		// End:0x5F
		if(__NFUN_176__(__NFUN_186__(aMouse), __NFUN_171__(0.1000000, Accel)))
		{
			__NFUN_182__(aMouse, 0.1000000);			
		}
		else
		{
			aMouse = __NFUN_172__(__NFUN_171__(aMouse, __NFUN_186__(aMouse)), Accel);
		}
	}
	return aMouse;
	return;
}

function float SmoothMouse(float aMouse, float DeltaTime, out byte SampleCount, int Index)
{
	local int i, sum;

	// End:0x13
	if(__NFUN_154__(int(MouseSmoothingMode), 0))
	{
		return aMouse;
	}
	// End:0x11B
	if(__NFUN_180__(aMouse, float(0)))
	{
		__NFUN_184__(ZeroTime[Index], DeltaTime);
		// End:0x7F
		if(__NFUN_176__(ZeroTime[Index], MouseSamplingTime))
		{
			__NFUN_184__(SamplingTime[Index], DeltaTime);
			__NFUN_184__(MaybeTime[Index], DeltaTime);
			aMouse = SmoothedMouse[Index];			
		}
		else
		{
			// End:0xE9
			if(__NFUN_130__(bAdjustSampling, __NFUN_151__(MouseSamples[Index], 9)))
			{
				__NFUN_185__(SamplingTime[Index], MaybeTime[Index]);
				MouseSamplingTime = __NFUN_174__(__NFUN_171__(0.9000000, MouseSamplingTime), __NFUN_172__(__NFUN_171__(0.1000000, SamplingTime[Index]), float(MouseSamples[Index])));
			}
			SamplingTime[Index] = 0.0000000;
			SmoothedMouse[Index] = 0.0000000;
			MouseSamples[Index] = 0;
		}		
	}
	else
	{
		MaybeTime[Index] = 0.0000000;
		// End:0x1C5
		if(__NFUN_181__(SmoothedMouse[Index], float(0)))
		{
			__NFUN_161__(MouseSamples[Index], int(SampleCount));
			// End:0x18E
			if(__NFUN_177__(DeltaTime, __NFUN_171__(MouseSamplingTime, float(__NFUN_146__(int(SampleCount), 1)))))
			{
				__NFUN_184__(SamplingTime[Index], __NFUN_171__(MouseSamplingTime, float(SampleCount)));				
			}
			else
			{
				__NFUN_184__(SamplingTime[Index], DeltaTime);
				aMouse = __NFUN_172__(__NFUN_171__(aMouse, DeltaTime), __NFUN_171__(MouseSamplingTime, float(SampleCount)));
			}			
		}
		else
		{
			SamplingTime[Index] = __NFUN_171__(0.5000000, MouseSamplingTime);
		}
		SmoothedMouse[Index] = __NFUN_172__(aMouse, float(SampleCount));
		ZeroTime[Index] = 0.0000000;
	}
	SampleCount = 0;
	// End:0x31A
	if(__NFUN_151__(int(MouseSmoothingMode), 1))
	{
		// End:0x29E
		if(__NFUN_180__(aMouse, float(0)))
		{
			i = 0;
			J0x231:

			// End:0x28D [Loop If]
			if(__NFUN_150__(i, 3))
			{
				__NFUN_161__(sum, int(__NFUN_171__(float(__NFUN_146__(i, 1)), 0.1000000)));
				__NFUN_184__(aMouse, __NFUN_171__(float(sum), OldSamples[i]));
				OldSamples[i] = 0.0000000;
				__NFUN_165__(i);
				// [Loop Continue]
				goto J0x231;
			}
			OldSamples[3] = 0.0000000;			
		}
		else
		{
			aMouse = __NFUN_171__(0.4000000, aMouse);
			OldSamples[3] = aMouse;
			i = 0;
			J0x2C5:

			// End:0x31A [Loop If]
			if(__NFUN_150__(i, 3))
			{
				__NFUN_184__(aMouse, __NFUN_171__(__NFUN_171__(float(__NFUN_146__(i, 1)), 0.1000000), OldSamples[i]));
				OldSamples[i] = OldSamples[__NFUN_146__(i, 1)];
				__NFUN_165__(i);
				// [Loop Continue]
				goto J0x2C5;
			}
		}
	}
	return aMouse;
	return;
}

function UpdateSensitivity(float f)
{
	MouseSensitivity = __NFUN_245__(0.0000000, f);
	default.MouseSensitivity = MouseSensitivity;
	Class'Engine.PlayerInput'.static.StaticSaveConfig();
	return;
}

function UpdateAccel(float f)
{
	MouseAccelThreshold = __NFUN_245__(0.0000000, f);
	default.MouseAccelThreshold = MouseAccelThreshold;
	Class'Engine.PlayerInput'.static.StaticSaveConfig();
	return;
}

function UpdateSmoothing(int Mode)
{
	MouseSmoothingMode = byte(Mode);
	default.MouseSmoothingMode = MouseSmoothingMode;
	Class'Engine.PlayerInput'.static.StaticSaveConfig();
	return;
}

function ChangeSnapView(bool B)
{
	Outer.bSnapToLevel = B;
	return;
}

function Actor.EDoubleClickDir CheckForDoubleClickMove(float DeltaTime)
{
	local Actor.EDoubleClickDir DoubleClickMove, OldDoubleClick;

	// End:0x24
	if(__NFUN_154__(int(Outer.DoubleClickDir), int(5)))
	{
		DoubleClickMove = 5;		
	}
	else
	{
		DoubleClickMove = 0;
	}
	// End:0x2CB
	if(__NFUN_177__(DoubleClickTime, 0.0000000))
	{
		// End:0xAB
		if(__NFUN_154__(int(Outer.DoubleClickDir), int(5)))
		{
			// End:0xA8
			if(__NFUN_130__(__NFUN_119__(Outer.Pawn, none), __NFUN_154__(int(Outer.Pawn.Physics), int(1))))
			{
				DoubleClickTimer = 0.0000000;
				Outer.DoubleClickDir = 6;
			}			
		}
		else
		{
			// End:0x202
			if(__NFUN_155__(int(Outer.DoubleClickDir), int(6)))
			{
				OldDoubleClick = Outer.DoubleClickDir;
				Outer.DoubleClickDir = 0;
				// End:0x111
				if(__NFUN_130__(bEdgeForward, bWasForward))
				{
					Outer.DoubleClickDir = 3;					
				}
				else
				{
					// End:0x139
					if(__NFUN_130__(bEdgeBack, bWasBack))
					{
						Outer.DoubleClickDir = 4;						
					}
					else
					{
						// End:0x161
						if(__NFUN_130__(bEdgeLeft, bWasLeft))
						{
							Outer.DoubleClickDir = 1;							
						}
						else
						{
							// End:0x186
							if(__NFUN_130__(bEdgeRight, bWasRight))
							{
								Outer.DoubleClickDir = 2;
							}
						}
					}
				}
				// End:0x1B6
				if(__NFUN_154__(int(Outer.DoubleClickDir), int(0)))
				{
					Outer.DoubleClickDir = OldDoubleClick;					
				}
				else
				{
					// End:0x1EE
					if(__NFUN_155__(int(Outer.DoubleClickDir), int(OldDoubleClick)))
					{
						DoubleClickTimer = __NFUN_174__(DoubleClickTime, __NFUN_171__(0.5000000, DeltaTime));						
					}
					else
					{
						DoubleClickMove = Outer.DoubleClickDir;
					}
				}
			}
		}
		// End:0x262
		if(__NFUN_154__(int(Outer.DoubleClickDir), int(6)))
		{
			DoubleClickTimer = __NFUN_244__(__NFUN_175__(DoubleClickTimer, DeltaTime), 0.0000000);
			// End:0x25F
			if(__NFUN_176__(DoubleClickTimer, -0.3500000))
			{
				Outer.DoubleClickDir = 0;
				DoubleClickTimer = DoubleClickTime;
			}			
		}
		else
		{
			// End:0x2CB
			if(__NFUN_130__(__NFUN_155__(int(Outer.DoubleClickDir), int(0)), __NFUN_155__(int(Outer.DoubleClickDir), int(5))))
			{
				__NFUN_185__(DoubleClickTimer, DeltaTime);
				// End:0x2CB
				if(__NFUN_176__(DoubleClickTimer, float(0)))
				{
					Outer.DoubleClickDir = 0;
					DoubleClickTimer = DoubleClickTime;
				}
			}
		}
	}
	return DoubleClickMove;
	return;
}

defaultproperties
{
	bAdjustSampling=true
	MouseSmoothingMode=1
	MouseSmoothingStrength=0.3000000
	MouseSensitivity=3.0000000
	MouseSamplingTime=0.0083330
	MouseAccelThreshold=100.0000000
	DoubleClickTime=0.2500000
}