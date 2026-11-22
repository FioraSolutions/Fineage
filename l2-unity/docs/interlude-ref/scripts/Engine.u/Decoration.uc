class Decoration extends Actor
	abstract
	native
	placeable;

var() Class<Actor> EffectWhenDestroyed;
var() bool bPushable;
var() bool bDamageable;
var bool bPushSoundPlaying;
var bool bSplash;
var() Sound PushSound;
var() Sound EndPushSound;
var const int numLandings;
var() Class<Inventory> contents;
var() int NumFrags;
var() Texture FragSkin;
var() Class<Fragment> FragType;
var Vector FragMomentum;
var() int Health;
var() float SplashTime;

function bool CanSplash()
{
	// End:0x5E
	if(__NFUN_130__(__NFUN_130__(__NFUN_177__(__NFUN_175__(Level.TimeSeconds, SplashTime), 0.2500000), __NFUN_154__(int(Physics), int(2))), __NFUN_177__(__NFUN_186__(Velocity.Z), float(100))))
	{
		SplashTime = Level.TimeSeconds;
		return true;
	}
	return false;
	return;
}

function Drop(Vector newVel)
{
	return;
}

function Landed(Vector HitNormal)
{
	local Rotator newRot;

	// End:0x40
	if(__NFUN_176__(Velocity.Z, float(-500)))
	{
		TakeDamage(100, Pawn(Owner), HitNormal, __NFUN_212__(HitNormal, float(10000)), Class'Engine.Crushed');
	}
	Velocity = vect(0.0000000, 0.0000000, 0.0000000);
	newRot = Rotation;
	newRot.Pitch = 0;
	newRot.Roll = 0;
	__NFUN_299__(newRot);
	return;
}

function HitWall(Vector HitNormal, Actor Wall)
{
	Landed(HitNormal);
	return;
}

function TakeDamage(int NDamage, Pawn instigatedBy, Vector HitLocation, Vector Momentum, Class<DamageType> DamageType)
{
	Instigator = instigatedBy;
	// End:0x25
	if(__NFUN_132__(__NFUN_129__(bDamageable), __NFUN_150__(Health, 0)))
	{
		return;
	}
	// End:0x38
	if(__NFUN_119__(Instigator, none))
	{
		__NFUN_512__(1.0000000);
	}
	__NFUN_162__(Health, NDamage);
	FragMomentum = Momentum;
	// End:0x60
	if(__NFUN_150__(Health, 0))
	{
		__NFUN_279__();		
	}
	else
	{
		__NFUN_3970__(2);
		Momentum.Z = 1000.0000000;
		Velocity = __NFUN_214__(Momentum, Mass);
	}
	return;
}

singular function PhysicsVolumeChange(PhysicsVolume NewVolume)
{
	// End:0x8D
	if(NewVolume.bWaterVolume)
	{
		// End:0x8D
		if(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_130__(bSplash, __NFUN_129__(PhysicsVolume.bWaterVolume)), __NFUN_178__(Mass, Buoyancy)), __NFUN_132__(__NFUN_176__(__NFUN_186__(Velocity.Z), float(100)), __NFUN_180__(Mass, float(0)))), __NFUN_176__(__NFUN_195__(), 0.0500000)), __NFUN_129__(__NFUN_532__())))
		{
			bSplash = false;
			__NFUN_3970__(0);
		}
	}
	// End:0x103
	if(__NFUN_130__(PhysicsVolume.bWaterVolume, __NFUN_177__(Buoyancy, Mass)))
	{
		// End:0xDB
		if(__NFUN_177__(Buoyancy, __NFUN_171__(1.1000000, Mass)))
		{
			Buoyancy = __NFUN_171__(0.9500000, Buoyancy);			
		}
		else
		{
			// End:0x103
			if(__NFUN_177__(Buoyancy, __NFUN_171__(1.0300000, Mass)))
			{
				Buoyancy = __NFUN_171__(0.9900000, Buoyancy);
			}
		}
	}
	return;
}

function Trigger(Actor Other, Pawn EventInstigator)
{
	Instigator = EventInstigator;
	TakeDamage(1000, Instigator, Location, __NFUN_212__(vect(0.0000000, 0.0000000, 1.0000000), float(900)), Class'Engine.Crushed');
	return;
}

singular function BaseChange()
{
	// End:0x4F
	if(__NFUN_176__(Velocity.Z, float(-500)))
	{
		TakeDamage(int(__NFUN_175__(float(1), __NFUN_172__(Velocity.Z, float(30)))), Instigator, Location, vect(0.0000000, 0.0000000, 0.0000000), Class'Engine.Crushed');
	}
	// End:0x8A
	if(__NFUN_114__(Base, none))
	{
		// End:0x87
		if(__NFUN_130__(__NFUN_130__(__NFUN_129__(bInterpolating), bPushable), __NFUN_154__(int(Physics), int(0))))
		{
			__NFUN_3970__(2);
		}		
	}
	else
	{
		// End:0x140
		if(__NFUN_119__(Pawn(Base), none))
		{
			Base.TakeDamage(int(__NFUN_172__(__NFUN_171__(float(int(__NFUN_175__(float(1), __NFUN_172__(Velocity.Z, float(400))))), Mass), Base.Mass)), Instigator, Location, __NFUN_213__(0.5000000, Velocity), Class'Engine.Crushed');
			Velocity.Z = 100.0000000;
			// End:0x128
			if(__NFUN_176__(__NFUN_195__(), 0.5000000))
			{
				__NFUN_184__(Velocity.X, float(70));				
			}
			else
			{
				__NFUN_184__(Velocity.Y, float(70));
			}
			__NFUN_3970__(2);			
		}
		else
		{
			// End:0x207
			if(__NFUN_130__(__NFUN_119__(Decoration(Base), none), __NFUN_176__(Velocity.Z, float(-500))))
			{
				Base.TakeDamage(int(__NFUN_175__(float(1), __NFUN_172__(__NFUN_171__(__NFUN_172__(Mass, Base.Mass), Velocity.Z), float(30)))), Instigator, Location, __NFUN_213__(0.2000000, Velocity), Class'Engine.Crushed');
				Velocity.Z = 100.0000000;
				// End:0x1EF
				if(__NFUN_176__(__NFUN_195__(), 0.5000000))
				{
					__NFUN_184__(Velocity.X, float(70));					
				}
				else
				{
					__NFUN_184__(Velocity.Y, float(70));
				}
				__NFUN_3970__(2);				
			}
			else
			{
				Instigator = none;
			}
		}
	}
	return;
}

simulated function Destroyed()
{
	local Inventory dropped;
	local int i;
	local Fragment S;
	local float BaseSize;

	// End:0x73
	if(__NFUN_154__(int(Role), int(ROLE_Authority)))
	{
		// End:0x53
		if(__NFUN_130__(__NFUN_119__(contents, none), __NFUN_129__(Level.bStartup)))
		{
			dropped = __NFUN_278__(contents);
			dropped.DropFrom(Location);
		}
		TriggerEvent(Event, self, none);
		// End:0x73
		if(bPushSoundPlaying)
		{
			__NFUN_264__(EndPushSound, 1);
		}
	}
	// End:0x17F
	if(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_155__(int(Level.NetMode), int(NM_DedicatedServer)), __NFUN_129__(PhysicsVolume.bDestructive)), __NFUN_151__(NumFrags, 0)), __NFUN_119__(FragType, none)))
	{
		BaseSize = __NFUN_172__(__NFUN_171__(0.8000000, __NFUN_193__(__NFUN_171__(CollisionRadius, CollisionHeight))), float(NumFrags));
		i = 0;
		J0xE7:

		// End:0x17F [Loop If]
		if(__NFUN_150__(i, NumFrags))
		{
			S = __NFUN_278__(FragType, Owner,, __NFUN_215__(Location, __NFUN_213__(CollisionRadius, __NFUN_252__())));
			S.CalcVelocity(FragMomentum);
			// End:0x14F
			if(__NFUN_119__(FragSkin, none))
			{
				S.Skins[0] = FragSkin;
			}
			S.SetDrawScale(__NFUN_171__(BaseSize, __NFUN_174__(0.5000000, __NFUN_171__(0.7000000, __NFUN_195__()))));
			__NFUN_165__(i);
			// [Loop Continue]
			goto J0xE7;
		}
	}
	super.Destroyed();
	return;
}

function Timer()
{
	__NFUN_264__(EndPushSound, 1);
	bPushSoundPlaying = false;
	return;
}

function Bump(Actor Other)
{
	local float Speed, OldZ;

	// End:0xF8
	if(__NFUN_130__(__NFUN_130__(bPushable, __NFUN_119__(Pawn(Other), none)), __NFUN_177__(Other.Mass, float(40))))
	{
		OldZ = Velocity.Z;
		Speed = __NFUN_225__(Other.Velocity);
		Velocity = __NFUN_214__(__NFUN_212__(Other.Velocity, __NFUN_244__(120.0000000, __NFUN_174__(20.0000000, Speed))), Speed);
		// End:0xCA
		if(__NFUN_154__(int(Physics), int(0)))
		{
			Velocity.Z = 25.0000000;
			// End:0xC7
			if(__NFUN_129__(bPushSoundPlaying))
			{
				__NFUN_264__(PushSound, 1);
				bPushSoundPlaying = true;
			}			
		}
		else
		{
			Velocity.Z = OldZ;
		}
		__NFUN_3970__(2);
		__NFUN_280__(0.3000000, false);
		Instigator = Pawn(Other);
	}
	return;
}

defaultproperties
{
	DrawType=2
	bStatic=true
	bStasis=true
	bOrientOnSlope=true
	NetUpdateFrequency=10.0000000
	bShouldBaseAtStartup=true
}