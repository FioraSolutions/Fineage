class KActor extends Actor
	native
	placeable;

cpptext
{
	virtual void Spawned();

}

var(Karma) bool bKTakeShot;
var() array<Sound> ImpactSounds;
var() float ImpactVolume;
var() Class<Actor> ImpactEffect;
var() bool bOrientImpactEffect;
var() float ImpactInterval;
var transient float LastImpactTime;

function TakeDamage(int Damage, Pawn instigatedBy, Vector HitLocation, Vector Momentum, Class<DamageType> DamageType)
{
	local Vector ApplyImpulse;

	// End:0x61
	if(__NFUN_130__(bKTakeShot, __NFUN_177__(DamageType.default.KDamageImpulse, float(0))))
	{
		// End:0x34
		if(__NFUN_176__(__NFUN_225__(Momentum), 0.0010000))
		{
			return;
		}
		ApplyImpulse = __NFUN_212__(__NFUN_226__(Momentum), DamageType.default.KDamageImpulse);
		KAddImpulse(ApplyImpulse, HitLocation);
	}
	return;
}

function Trigger(Actor Other, Pawn EventInstigator)
{
	KWake();
	return;
}

event KImpact(Actor Other, Vector pos, Vector impactVel, Vector impactNorm)
{
	local int numSounds, soundNum;

	// End:0xB9
	if(__NFUN_177__(Level.TimeSeconds, __NFUN_174__(LastImpactTime, ImpactInterval)))
	{
		numSounds = ImpactSounds.Length;
		// End:0x58
		if(__NFUN_151__(numSounds, 0))
		{
			soundNum = __NFUN_167__(numSounds);
			__NFUN_264__(ImpactSounds[soundNum], 4, ImpactVolume);
		}
		// End:0xA5
		if(__NFUN_130__(__NFUN_119__(ImpactEffect, none), EffectIsRelevant(pos, false)))
		{
			// End:0x96
			if(bOrientImpactEffect)
			{
				__NFUN_278__(ImpactEffect, self,, pos, Rotator(impactVel));				
			}
			else
			{
				__NFUN_278__(ImpactEffect, self,, pos);
			}
		}
		LastImpactTime = Level.TimeSeconds;
	}
	return;
}

defaultproperties
{
	bKTakeShot=true
	Physics=13
	DrawType=8
	bNoDelete=true
	RemoteRole=0
	CollisionRadius=1.0000000
	CollisionHeight=1.0000000
	bCollideActors=true
	bBlockActors=true
	bBlockPlayers=true
	bProjTarget=true
	bBlockKarma=true
	bEdShouldSnap=true
}