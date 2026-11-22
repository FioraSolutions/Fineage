class PhysicsVolume extends Volume
	native
	nativereplication
	notplaceable;

var() bool bPainCausing;
var() Vector ZoneVelocity;
var() Vector Gravity;
var() float GroundFriction;
var() float TerminalVelocity;
var() float DamagePerSec;
var() Class<DamageType> DamageType;
var() int Priority;
var() Sound EntrySound;
var() Sound ExitSound;
var() Class<Actor> EntryActor;
var() Class<Actor> ExitActor;
var() float FluidFriction;
var() Vector ViewFlash;
var() Vector ViewFog;
var() bool bDestructive;
var() bool bNoInventory;
var() bool bMoveProjectiles;
var() bool bBounceVelocity;
var() bool bNeutralZone;
var bool bWaterVolume;
var Info PainTimer;
var(VolumeFog) bool bDistanceFog;
var(VolumeFog) Color DistanceFogColor;
var(VolumeFog) float DistanceFogStart;
var(VolumeFog) float DistanceFogEnd;
var(VolumeFog) bool bUseDistanceFogColor;
var(VolumeFog) bool bUseCellophane;
var(VolumeFog) Color CellophaneColor;
var(Karma) float KExtraLinearDamping;
var(Karma) float KExtraAngularDamping;
var(Karma) float KBuoyancy;
var PhysicsVolume NextPhysicsVolume;
var() bool bL2WaterVolume;
var() bool bL2StepVolume;
var(StepSound) int StepSoundID;

replication
{
	// Pos:0x000
	reliable if(__NFUN_130__(bNetDirty, __NFUN_154__(int(Role), int(ROLE_Authority))))
		Gravity;
}

simulated function PostBeginPlay()
{
	super.PostBeginPlay();
	// End:0x18
	if(__NFUN_150__(int(Role), int(ROLE_Authority)))
	{
		return;
	}
	// End:0x30
	if(bPainCausing)
	{
		PainTimer = __NFUN_278__(Class'Engine.VolumeTimer', self);
	}
	return;
}

event PhysicsChangedFor(Actor Other)
{
	return;
}

event ActorEnteredVolume(Actor Other)
{
	return;
}

event ActorLeavingVolume(Actor Other)
{
	return;
}

event PawnEnteredVolume(Pawn Other)
{
	// End:0x23
	if(Other.IsPlayerPawn())
	{
		TriggerEvent(Event, self, Other);
	}
	return;
}

event PawnLeavingVolume(Pawn Other)
{
	// End:0x23
	if(Other.IsPlayerPawn())
	{
		UntriggerEvent(Event, self, Other);
	}
	return;
}

function TimerPop(VolumeTimer t)
{
	local Actor A;

	// End:0x39
	if(__NFUN_114__(t, PainTimer))
	{
		// End:0x1C
		if(__NFUN_129__(bPainCausing))
		{
			return;
		}
		// End:0x38
		foreach __NFUN_307__(Class'Engine.Actor', A)
		{
			CausePainTo(A);			
		}		
	}
	return;
}

function Trigger(Actor Other, Pawn EventInstigator)
{
	// End:0x41
	if(__NFUN_181__(DamagePerSec, float(0)))
	{
		bPainCausing = __NFUN_129__(bPainCausing);
		// End:0x41
		if(__NFUN_130__(bPainCausing, __NFUN_114__(PainTimer, none)))
		{
			PainTimer = __NFUN_278__(Class'Engine.VolumeTimer', self);
		}
	}
	return;
}

event Touch(Actor Other)
{
	super(Actor).Touch(Other);
	// End:0x18
	if(__NFUN_114__(Other, none))
	{
		return;
	}
	// End:0x63
	if(__NFUN_130__(__NFUN_130__(bNoInventory, Other.__NFUN_303__('Inventory')), __NFUN_114__(Other.Owner, none)))
	{
		Other.LifeSpan = 1.5000000;
		return;
	}
	// End:0x108
	if(__NFUN_130__(bMoveProjectiles, __NFUN_218__(ZoneVelocity, vect(0.0000000, 0.0000000, 0.0000000))))
	{
		// End:0xB6
		if(__NFUN_154__(int(Other.Physics), int(6)))
		{
			__NFUN_223__(Other.Velocity, ZoneVelocity);			
		}
		else
		{
			// End:0x108
			if(__NFUN_130__(Other.__NFUN_303__('Effects'), __NFUN_154__(int(Other.Physics), int(0))))
			{
				Other.__NFUN_3970__(6);
				__NFUN_223__(Other.Velocity, ZoneVelocity);
			}
		}
	}
	// End:0x13C
	if(bPainCausing)
	{
		// End:0x131
		if(Other.bDestroyInPainVolume)
		{
			Other.__NFUN_279__();
			return;
		}
		CausePainTo(Other);
	}
	// End:0x164
	if(__NFUN_130__(bL2WaterVolume, Other.CanSplash()))
	{
		PlayEntrySplash(Other);
	}
	return;
}

function PlayEntrySplash(Actor Other)
{
	local float SplashSize;
	local Actor splash;

	SplashSize = __NFUN_246__(__NFUN_171__(__NFUN_171__(0.0000300, Other.Mass), __NFUN_175__(float(250), __NFUN_171__(0.5000000, __NFUN_245__(-600.0000000, Other.Velocity.Z)))), 0.1000000, 1.0000000);
	// End:0x86
	if(__NFUN_119__(EntrySound, none))
	{
		__NFUN_264__(EntrySound, 3, SplashSize);
		// End:0x86
		if(__NFUN_119__(Other.Instigator, none))
		{
			__NFUN_512__(SplashSize);
		}
	}
	// End:0xBE
	if(__NFUN_119__(EntryActor, none))
	{
		splash = __NFUN_278__(EntryActor);
		// End:0xBE
		if(__NFUN_119__(splash, none))
		{
			splash.SetDrawScale(SplashSize);
		}
	}
	return;
}

event UnTouch(Actor Other)
{
	// End:0x28
	if(__NFUN_130__(bL2WaterVolume, Other.CanSplash()))
	{
		PlayExitSplash(Other);
	}
	return;
}

function PlayExitSplash(Actor Other)
{
	local float SplashSize;
	local Actor splash;

	SplashSize = __NFUN_246__(__NFUN_171__(0.0030000, Other.Mass), 0.1000000, 1.0000000);
	// End:0x41
	if(__NFUN_119__(ExitSound, none))
	{
		__NFUN_264__(ExitSound, 3, SplashSize);
	}
	// End:0x79
	if(__NFUN_119__(ExitActor, none))
	{
		splash = __NFUN_278__(ExitActor);
		// End:0x79
		if(__NFUN_119__(splash, none))
		{
			splash.SetDrawScale(SplashSize);
		}
	}
	return;
}

function CausePainTo(Actor Other)
{
	local float Depth;
	local Pawn P;

	Depth = 1.0000000;
	P = Pawn(Other);
	// End:0xCE
	if(__NFUN_177__(DamagePerSec, float(0)))
	{
		// End:0x5C
		if(__NFUN_130__(Region.Zone.bSoftKillZ, __NFUN_155__(int(Other.Physics), int(1))))
		{
			return;
		}
		Other.TakeDamage(int(__NFUN_171__(DamagePerSec, Depth)), none, Location, vect(0.0000000, 0.0000000, 0.0000000), DamageType);
		// End:0xCB
		if(__NFUN_130__(__NFUN_119__(P, none), __NFUN_119__(P.Controller, none)))
		{
			P.Controller.PawnIsInPain(self);
		}		
	}
	else
	{
		// End:0x13B
		if(__NFUN_130__(__NFUN_119__(P, none), __NFUN_150__(P.Health, P.default.Health)))
		{
			P.Health = __NFUN_249__(P.default.Health, int(__NFUN_175__(float(P.Health), __NFUN_171__(Depth, DamagePerSec))));
		}
	}
	return;
}

event GetHitEffectName(out name A, out name B)
{
	return;
}

defaultproperties
{
	Gravity=(X=0.0000000,Y=0.0000000,Z=-1500.0000000)
	GroundFriction=8.0000000
	TerminalVelocity=2500.0000000
	FluidFriction=0.3000000
	KBuoyancy=1.0000000
	StepSoundID=-1
	bAlwaysRelevant=true
}