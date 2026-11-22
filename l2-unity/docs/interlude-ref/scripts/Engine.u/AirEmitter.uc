class AirEmitter extends Emitter
	native
	placeable;

var() bool bAttachToPawn;
var() bool bByOpacity;
var() int NumSteps;
var() Range FadeRange;
var() transient float FadeAlpha;
var() transient int FadeAlphaByNum;
var() transient AirVolume RecentVolume;
var() transient float RecentFadeSeconds;
var(Vanish) float BoostVanishCoef;
var(Vanish) float VanishCoef;
var() int IndexPE;

function bool IsInAirVolume()
{
	return __NFUN_130__(__NFUN_119__(RecentVolume, none), __NFUN_255__(RecentVolume.EffectName, 'None'));
	return;
}

simulated function Destroyed()
{
	super(Actor).Destroyed();
	ClearL2Game();
	return;
}

event ClearL2Game()
{
	// End:0x4D
	if(__NFUN_130__(__NFUN_130__(__NFUN_119__(Owner, none), __NFUN_119__(LineagePlayerController(Owner), none)), __NFUN_114__(LineagePlayerController(Owner).AirEffect, self)))
	{
		LineagePlayerController(Owner).AirEffect = none;
	}
	__NFUN_298__(none);
	return;
}

event ForceKill()
{
	// End:0x14
	if(__NFUN_255__(__NFUN_284__(), '_killpending'))
	{
		__NFUN_113__('_killpending');
	}
	return;
}

function TouchVolume()
{
	return;
}

function UnTouchVolume()
{
	return;
}

event SetAirVolume(AirVolume NewVolume)
{
	RecentVolume = NewVolume;
	// End:0x28
	if(IsInAirVolume())
	{
		RecentFadeSeconds = RecentVolume.FullFadeSeconds;
	}
	return;
}

function Timer()
{
	// End:0x12
	if(IsInAirVolume())
	{
		TouchVolume();		
	}
	else
	{
		UnTouchVolume();
	}
	return;
}

function ApplyFadeAlpha()
{
	// End:0x26
	if(bByOpacity)
	{
		Emitters[IndexPE].Opacity = FadeAlpha;		
	}
	else
	{
		Emitters[IndexPE].MaxActiveParticles = int(FadeAlpha);
	}
	return;
}

function PostBeginPlay()
{
	super(Actor).PostBeginPlay();
	FadeAlpha = 0.0000000;
	// End:0x3A
	if(__NFUN_129__(bByOpacity))
	{
		NumSteps = int(__NFUN_175__(FadeRange.Max, FadeRange.Min));
	}
	return;
}

state() dummyinitial
{
	ignores ApplyFadeAlpha;

	simulated function EndState()
	{
		super(Object).EndState();
		FadeAlpha = FadeRange.Min;
		return;
	}
Begin:

	__NFUN_256__(1.0000000);
	// End:0x18
	if(IsInAirVolume())
	{
		__NFUN_113__('active');
	}
	stop;			
}

state() active
{
	simulated function BeginState()
	{
		super(Object).BeginState();
		__NFUN_280__(__NFUN_172__(RecentFadeSeconds, float(NumSteps)), true);
		ApplyFadeAlpha();
		return;
	}

	function TouchVolume()
	{
		FadeAlpha = __NFUN_246__(__NFUN_174__(__NFUN_172__(__NFUN_175__(FadeRange.Max, FadeRange.Min), float(NumSteps)), FadeAlpha), FadeRange.Min, FadeRange.Max);
		ApplyFadeAlpha();
		return;
	}

	function UnTouchVolume()
	{
		__NFUN_113__('vanishing');
		return;
	}
	stop;
}

state() vanishing
{
	simulated function BeginState()
	{
		__NFUN_280__(__NFUN_172__(RecentFadeSeconds, __NFUN_171__(VanishCoef, float(NumSteps))), true);
		return;
	}

	function TouchVolume()
	{
		__NFUN_113__('active');
		return;
	}

	function UnTouchVolume()
	{
		// End:0x55
		if(IsInAirVolume())
		{
			FadeAlpha = __NFUN_246__(__NFUN_175__(FadeAlpha, __NFUN_172__(__NFUN_171__(BoostVanishCoef, __NFUN_175__(FadeRange.Max, FadeRange.Min)), float(NumSteps))), FadeRange.Min, FadeRange.Max);			
		}
		else
		{
			FadeAlpha = __NFUN_246__(__NFUN_175__(FadeAlpha, __NFUN_172__(__NFUN_175__(FadeRange.Max, FadeRange.Min), float(NumSteps))), FadeRange.Min, FadeRange.Max);
		}
		ApplyFadeAlpha();
		// End:0xB8
		if(__NFUN_180__(FadeAlpha, FadeRange.Min))
		{
			__NFUN_113__('_killpending');
		}
		return;
	}
	stop;
}

state() _killpending
{
	ignores UnTouchVolume, TouchVolume, SetAirVolume;

	simulated function BeginState()
	{
		super(Object).BeginState();
		__NFUN_280__(0.0000000, false);
		// End:0x4F
		if(__NFUN_130__(__NFUN_119__(LineagePlayerController(Owner), none), __NFUN_114__(LineagePlayerController(Owner).AirEffect, self)))
		{
			LineagePlayerController(Owner).AirEffect = none;
		}
		Kill();
		return;
	}
	stop;
}

defaultproperties
{
	NumSteps=1
	FadeRange=(Min=0.0000000,Max=1.0000000)
	BoostVanishCoef=1.5000000
	VanishCoef=3.0000000
	AutoDestroy=true
	InitialState="dummyinitial"
}