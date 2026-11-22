class Ammunition extends Inventory
	abstract
	native
	nativereplication
	notplaceable;

var travel int MaxAmmo;
var travel int AmmoAmount;
var travel int PickupAmmo;
var bool bRecommendSplashDamage;
var bool bTossed;
var bool bTrySplash;
var bool bLeadTarget;
var bool bInstantHit;
var bool bSplashDamage;
var Class<Projectile> ProjectileClass;
var Class<DamageType> MyDamageType;
var float WarnTargetPct;
var float RefireRate;
var Sound FireSound;
var float MaxAutoAimRange;

replication
{
	// Pos:0x000
	reliable if(__NFUN_130__(__NFUN_130__(bNetOwner, bNetDirty), __NFUN_154__(int(Role), int(ROLE_Authority))))
		AmmoAmount;
}

simulated function bool HasAmmo()
{
	return __NFUN_151__(AmmoAmount, 0);
	return;
}

function float RateSelf(Pawn shooter, out name RecommendedFiringMode)
{
	return 0.5000000;
	return;
}

function WarnTarget(Actor Target, Pawn P, Vector FireDir)
{
	// End:0x0B
	if(bInstantHit)
	{
		return;
	}
	// End:0x79
	if(__NFUN_130__(__NFUN_130__(__NFUN_176__(__NFUN_195__(), WarnTargetPct), __NFUN_119__(Pawn(Target), none)), __NFUN_119__(Pawn(Target).Controller, none)))
	{
		Pawn(Target).Controller.ReceiveWarning(P, ProjectileClass.default.Speed, FireDir);
	}
	return;
}

function SpawnProjectile(Vector Start, Rotator Dir)
{
	__NFUN_162__(AmmoAmount, 1);
	__NFUN_278__(ProjectileClass,,, Start, Dir);
	return;
}

function ProcessTraceHit(Weapon W, Actor Other, Vector HitLocation, Vector HitNormal, Vector X, Vector Y, Vector Z)
{
	__NFUN_162__(AmmoAmount, 1);
	return;
}

simulated function DisplayDebug(Canvas Canvas, out float YL, out float YPos)
{
	Canvas.__NFUN_465__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("Ammunition ", GetItemName(string(self))), " amount "), string(AmmoAmount)), " Max "), string(MaxAmmo)));
	__NFUN_184__(YPos, YL);
	Canvas.SetPos(4.0000000, YPos);
	return;
}

function bool HandlePickupQuery(Pickup item)
{
	// End:0x6C
	if(__NFUN_114__(Class, item.InventoryType))
	{
		// End:0x29
		if(__NFUN_154__(AmmoAmount, MaxAmmo))
		{
			return true;
		}
		item.AnnouncePickup(Pawn(Owner));
		AddAmmo(Ammo(item).AmmoAmount);
		item.SetRespawn();
		return true;
	}
	// End:0x79
	if(__NFUN_114__(Inventory, none))
	{
		return false;
	}
	return Inventory.HandlePickupQuery(item);
	return;
}

function bool AddAmmo(int AmmoToAdd)
{
	AmmoAmount = __NFUN_249__(MaxAmmo, __NFUN_146__(AmmoAmount, AmmoToAdd));
	return true;
	return;
}

function float GetDamageRadius()
{
	// End:0x1A
	if(__NFUN_119__(ProjectileClass, none))
	{
		return ProjectileClass.default.DamageRadius;
	}
	return 0.0000000;
	return;
}

defaultproperties
{
	MyDamageType=Class'Engine.DamageType'
	WarnTargetPct=0.5000000
	RefireRate=0.5000000
	MaxAutoAimRange=5000.0000000
}