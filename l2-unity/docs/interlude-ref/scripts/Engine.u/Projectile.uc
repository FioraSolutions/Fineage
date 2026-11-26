class Projectile extends Actor
	abstract
	native
	notplaceable;

var float Speed;
var float MaxSpeed;
var float TossZ;
var bool bSwitchToZeroCollision;
var float Damage;
var float DamageRadius;
var float MomentumTransfer;
var Class<DamageType> MyDamageType;
var Sound SpawnSound;
var Sound ImpactSound;
var Class<Projector> ExplosionDecal;
var float ExploWallOut;

function bool EncroachingOn(Actor Other)
{
	// End:0x28
	if(__NFUN_132__(__NFUN_119__(Other.Brush, none), __NFUN_119__(Brush(Other), none)))
	{
		return true;
	}
	return false;
	return;
}

singular simulated function Touch(Actor Other)
{
	local Actor HitActor;
	local Vector HitLocation, HitNormal, VelDir;
	local bool bBeyondOther;
	local float BackDist, DirZ;

	// End:0x0D
	if(__NFUN_114__(Other, none))
	{
		return;
	}
	// End:0x1EF
	if(__NFUN_132__(Other.bProjTarget, __NFUN_130__(Other.bBlockActors, Other.bBlockPlayers)))
	{
		// End:0x86
		if(__NFUN_132__(__NFUN_217__(Velocity, vect(0.0000000, 0.0000000, 0.0000000)), Other.__NFUN_303__('Mover')))
		{
			ProcessTouch(Other, Location);
			return;
		}
		bBeyondOther = __NFUN_177__(__NFUN_219__(Velocity, __NFUN_216__(Location, Other.Location)), float(0));
		VelDir = __NFUN_226__(Velocity);
		DirZ = __NFUN_193__(__NFUN_186__(VelDir.Z));
		BackDist = __NFUN_174__(__NFUN_171__(Other.CollisionRadius, __NFUN_175__(float(1), DirZ)), __NFUN_171__(Other.CollisionHeight, DirZ));
		// End:0x130
		if(bBeyondOther)
		{
			__NFUN_184__(BackDist, __NFUN_225__(__NFUN_216__(Location, Other.Location)));			
		}
		else
		{
			__NFUN_185__(BackDist, __NFUN_225__(__NFUN_216__(Location, Other.Location)));
		}
		HitActor = __NFUN_277__(HitLocation, HitNormal, Location, __NFUN_216__(Location, __NFUN_213__(__NFUN_171__(1.1000000, BackDist), VelDir)), true);
		// End:0x1A3
		if(__NFUN_114__(HitActor, Other))
		{
			ProcessTouch(Other, HitLocation);			
		}
		else
		{
			// End:0x1DF
			if(bBeyondOther)
			{
				ProcessTouch(Other, __NFUN_216__(Other.Location, __NFUN_213__(Other.CollisionRadius, VelDir)));				
			}
			else
			{
				ProcessTouch(Other, Location);
			}
		}
	}
	return;
}

simulated function ProcessTouch(Actor Other, Vector HitLocation)
{
	// End:0x31
	if(__NFUN_119__(Other, Instigator))
	{
		Explode(HitLocation, __NFUN_226__(__NFUN_216__(HitLocation, Other.Location)));
	}
	return;
}

simulated function HitWall(Vector HitNormal, Actor Wall)
{
	// End:0x5B
	if(__NFUN_154__(int(Role), int(ROLE_Authority)))
	{
		// End:0x53
		if(__NFUN_119__(Mover(Wall), none))
		{
			Wall.TakeDamage(int(Damage), Instigator, Location, __NFUN_213__(MomentumTransfer, __NFUN_226__(Velocity)), MyDamageType);
		}
		__NFUN_512__(1.0000000);
	}
	Explode(__NFUN_215__(Location, __NFUN_213__(ExploWallOut, HitNormal)), HitNormal);
	// End:0xB7
	if(__NFUN_130__(__NFUN_119__(ExplosionDecal, none), __NFUN_155__(int(Level.NetMode), int(NM_DedicatedServer))))
	{
		__NFUN_278__(ExplosionDecal, self,, Location, Rotator(__NFUN_211__(HitNormal)));
	}
	return;
}

simulated function BlowUp(Vector HitLocation)
{
	HurtRadius(Damage, DamageRadius, MyDamageType, MomentumTransfer, HitLocation);
	// End:0x37
	if(__NFUN_154__(int(Role), int(ROLE_Authority)))
	{
		__NFUN_512__(1.0000000);
	}
	return;
}

simulated function Explode(Vector HitLocation, Vector HitNormal)
{
	__NFUN_279__();
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

static function Vector GetTossVelocity(Pawn P, Rotator R)
{
	local Vector V;

	V = Vector(R);
	__NFUN_221__(V, __NFUN_174__(__NFUN_171__(__NFUN_219__(P.Velocity, V), 0.4000000), default.Speed));
	__NFUN_184__(V.Z, default.TossZ);
	return V;
	return;
}

defaultproperties
{
	MaxSpeed=2000.0000000
	TossZ=100.0000000
	DamageRadius=220.0000000
	MyDamageType=Class'Engine.DamageType'
	Physics=6
	DrawType=2
	bAcceptsProjectors=false
	bNetTemporary=true
	bReplicateInstigator=true
	bNetInitialRotation=true
	bCheckChangableLevel=true
	RemoteRole=2
	NetPriority=2.5000000
	LifeSpan=14.0000000
	Texture=Texture'Engine.S_Camera'
	bUnlit=true
	bGameRelevant=true
	bDisturbFluidSurface=true
	CollisionRadius=0.0000000
	CollisionHeight=0.0000000
	bCollideActors=true
	bCollideWorld=true
	bUseCylinderCollision=true
}