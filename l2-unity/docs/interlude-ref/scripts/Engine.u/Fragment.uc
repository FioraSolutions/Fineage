class Fragment extends Effects
	notplaceable;

var() Mesh Fragments[11];
var int numFragmentTypes;
var bool bFirstHit;
var() Sound ImpactSound;
var() Sound AltImpactSound;
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

simulated function CalcVelocity(Vector Momentum)
{
	local float ExplosionSize;

	ExplosionSize = __NFUN_171__(0.0110000, __NFUN_225__(Momentum));
	Velocity = __NFUN_215__(__NFUN_213__(0.0033000, Momentum), __NFUN_212__(__NFUN_213__(0.7000000, __NFUN_252__()), __NFUN_174__(__NFUN_174__(ExplosionSize, __NFUN_171__(__NFUN_195__(), 100.0000000)), 100.0000000)));
	__NFUN_184__(Velocity.Z, __NFUN_171__(0.5000000, ExplosionSize));
	return;
}

simulated function HitWall(Vector HitNormal, Actor HitWall)
{
	local float Speed;

	Velocity = __NFUN_213__(0.5000000, __NFUN_215__(__NFUN_212__(__NFUN_213__(__NFUN_219__(Velocity, HitNormal), HitNormal), -2.0000000), Velocity));
	Speed = __NFUN_225__(Velocity);
	// End:0x9F
	if(__NFUN_130__(bFirstHit, __NFUN_176__(Speed, float(400))))
	{
		bFirstHit = false;
		bRotateToDesired = true;
		bFixedRotationDir = false;
		DesiredRotation.Pitch = 0;
		DesiredRotation.Yaw = int(__NFUN_171__(__NFUN_195__(), float(65536)));
		DesiredRotation.Roll = 0;
	}
	RotationRate.Yaw = int(__NFUN_171__(float(RotationRate.Yaw), 0.7500000));
	RotationRate.Roll = int(__NFUN_171__(float(RotationRate.Roll), 0.7500000));
	RotationRate.Pitch = int(__NFUN_171__(float(RotationRate.Pitch), 0.7500000));
	// End:0x13A
	if(__NFUN_130__(__NFUN_176__(Speed, float(60)), __NFUN_177__(HitNormal.Z, 0.7000000)))
	{
		__NFUN_3970__(0);
		bBounce = false;
		__NFUN_113__('Dying');		
	}
	else
	{
		// End:0x19B
		if(__NFUN_177__(Speed, float(80)))
		{
			// End:0x179
			if(__NFUN_176__(__NFUN_195__(), 0.5000000))
			{
				__NFUN_264__(ImpactSound, 0,,, 300.0000000, __NFUN_174__(0.8500000, __NFUN_171__(__NFUN_195__(), 0.3000000)), true);				
			}
			else
			{
				__NFUN_264__(AltImpactSound, 0,,, 300.0000000, __NFUN_174__(0.8500000, __NFUN_171__(__NFUN_195__(), 0.3000000)), true);
			}
		}
	}
	return;
}

final simulated function RandSpin(float spinRate)
{
	DesiredRotation = __NFUN_320__();
	RotationRate.Yaw = int(__NFUN_175__(__NFUN_171__(__NFUN_171__(spinRate, float(2)), __NFUN_195__()), spinRate));
	RotationRate.Pitch = int(__NFUN_175__(__NFUN_171__(__NFUN_171__(spinRate, float(2)), __NFUN_195__()), spinRate));
	RotationRate.Roll = int(__NFUN_175__(__NFUN_171__(__NFUN_171__(spinRate, float(2)), __NFUN_195__()), spinRate));
	return;
}

auto state Flying
{
	simulated function Timer()
	{
		__NFUN_113__('Dying');
		return;
	}

	singular simulated function PhysicsVolumeChange(PhysicsVolume NewVolume)
	{
		// End:0x8F
		if(NewVolume.bWaterVolume)
		{
			Velocity = __NFUN_213__(0.2000000, Velocity);
			// End:0x75
			if(bFirstHit)
			{
				bFirstHit = false;
				bRotateToDesired = true;
				bFixedRotationDir = false;
				DesiredRotation.Pitch = 0;
				DesiredRotation.Yaw = int(__NFUN_171__(__NFUN_195__(), float(65536)));
				DesiredRotation.Roll = 0;
			}
			RotationRate = __NFUN_288__(0.2000000, RotationRate);
			__NFUN_113__('Dying');
		}
		return;
	}

	simulated function BeginState()
	{
		RandSpin(125000.0000000);
		// End:0x35
		if(__NFUN_176__(__NFUN_186__(float(RotationRate.Pitch)), float(10000)))
		{
			RotationRate.Pitch = 10000;
		}
		// End:0x5F
		if(__NFUN_176__(__NFUN_186__(float(RotationRate.Roll)), float(10000)))
		{
			RotationRate.Roll = 10000;
		}
		LinkMesh(Fragments[int(__NFUN_171__(__NFUN_195__(), float(numFragmentTypes)))]);
		// End:0xA6
		if(__NFUN_154__(int(Level.NetMode), int(NM_Standalone)))
		{
			LifeSpan = __NFUN_174__(20.0000000, __NFUN_171__(float(40), __NFUN_195__()));
		}
		__NFUN_280__(5.0000000, true);
		return;
	}
	stop;
}

state Dying
{
	function TakeDamage(int Dam, Pawn instigatedBy, Vector HitLocation, Vector Momentum, Class<DamageType> DamageType)
	{
		__NFUN_279__();
		return;
	}

	simulated function Timer()
	{
		// End:0x0B
		if(__NFUN_129__(__NFUN_532__()))
		{
			__NFUN_279__();
		}
		return;
	}

	simulated function BeginState()
	{
		__NFUN_280__(__NFUN_174__(1.0000000, __NFUN_195__()), true);
		__NFUN_262__(true, false, false);
		return;
	}
	stop;
}

defaultproperties
{
	bFirstHit=true
	Physics=2
	DrawType=2
	LifeSpan=20.0000000
	bDestroyInPainVolume=true
	CollisionRadius=18.0000000
	CollisionHeight=4.0000000
	bCollideWorld=true
	bBounce=true
	bFixedRotationDir=true
}