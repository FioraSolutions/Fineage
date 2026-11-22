class WeaponPickup extends Pickup
	abstract
	placeable;

var bool bWeaponStay;
var() bool bThrown;

function PostBeginPlay()
{
	super(Actor).PostBeginPlay();
	SetWeaponStay();
	MaxDesireability = __NFUN_171__(1.2000000, Class<Weapon>(InventoryType).default.AIRating);
	return;
}

function SetWeaponStay()
{
	bWeaponStay = __NFUN_130__(bWeaponStay, Level.Game.bWeaponStay);
	return;
}

function StartSleeping()
{
	// End:0x0F
	if(bDropped)
	{
		__NFUN_279__();		
	}
	else
	{
		// End:0x21
		if(__NFUN_129__(bWeaponStay))
		{
			__NFUN_113__('Sleeping');
		}
	}
	return;
}

function bool AllowRepeatPickup()
{
	return __NFUN_132__(__NFUN_129__(bWeaponStay), __NFUN_130__(bDropped, __NFUN_129__(bThrown)));
	return;
}

function float DetourWeight(Pawn Other, float PathWeight)
{
	local Weapon AlreadyHas;

	AlreadyHas = Weapon(Other.FindInventoryType(InventoryType));
	// End:0x3B
	if(__NFUN_130__(__NFUN_119__(AlreadyHas, none), bWeaponStay))
	{
		return 0.0000000;
	}
	// End:0x97
	if(__NFUN_130__(AIController(Other.Controller).PriorityObjective(), __NFUN_132__(__NFUN_177__(Other.Weapon.AIRating, 0.5000000), __NFUN_177__(PathWeight, float(400)))))
	{
		return 0.0000000;
	}
	// End:0xE1
	if(__NFUN_177__(Class<Weapon>(InventoryType).default.AIRating, Other.Weapon.AIRating))
	{
		return __NFUN_172__(Class<Weapon>(InventoryType).default.AIRating, PathWeight);
	}
	return __NFUN_172__(Class<Weapon>(InventoryType).default.AIRating, PathWeight);
	return;
}

function float BotDesireability(Pawn Bot)
{
	local Weapon AlreadyHas;
	local float desire;

	desire = __NFUN_174__(MaxDesireability, Bot.Controller.AdjustDesireFor(self));
	AlreadyHas = Weapon(Bot.FindInventoryType(InventoryType));
	// End:0x17A
	if(__NFUN_119__(AlreadyHas, none))
	{
		// End:0x71
		if(Bot.Controller.bHuntPlayer)
		{
			return 0.0000000;
		}
		// End:0x82
		if(__NFUN_129__(AllowRepeatPickup()))
		{
			return 0.0000000;
		}
		// End:0xEC
		if(__NFUN_130__(__NFUN_176__(RespawnTime, float(10)), __NFUN_132__(__NFUN_132__(bHidden, __NFUN_114__(AlreadyHas.AmmoType, none)), __NFUN_150__(AlreadyHas.AmmoType.AmmoAmount, AlreadyHas.AmmoType.MaxAmmo))))
		{
			return 0.0000000;
		}
		// End:0x174
		if(AlreadyHas.HasAmmo())
		{
			return __NFUN_245__(__NFUN_171__(0.2500000, desire), __NFUN_171__(AlreadyHas.AmmoType.PickupClass.default.MaxDesireability, __NFUN_244__(1.0000000, __NFUN_172__(__NFUN_171__(0.1500000, float(AlreadyHas.AmmoType.MaxAmmo)), float(AlreadyHas.AmmoType.AmmoAmount)))));			
		}
		else
		{
			return 0.0500000;
		}
	}
	// End:0x1CC
	if(__NFUN_130__(Bot.Controller.bHuntPlayer, __NFUN_176__(__NFUN_171__(MaxDesireability, 0.8330000), __NFUN_175__(Bot.Weapon.AIRating, 0.1000000))))
	{
		return 0.0000000;
	}
	// End:0x210
	if(__NFUN_132__(__NFUN_114__(Bot.Weapon, none), __NFUN_176__(Bot.Weapon.AIRating, 0.5000000)))
	{
		return __NFUN_171__(2.0000000, desire);
	}
	return desire;
	return;
}

function float GetRespawnTime()
{
	// End:0x41
	if(__NFUN_132__(__NFUN_155__(int(Level.NetMode), int(NM_Standalone)), __NFUN_177__(Level.Game.GameDifficulty, float(3))))
	{
		return RespawnTime;
	}
	return __NFUN_171__(RespawnTime, __NFUN_174__(0.3300000, __NFUN_171__(0.2200000, Level.Game.GameDifficulty)));
	return;
}

defaultproperties
{
	bWeaponStay=true
	MaxDesireability=0.5000000
	bAmbientGlow=true
	bPredictRespawns=true
	RespawnTime=30.0000000
	PickupMessage="You got a weapon"
	Physics=5
	Texture=Texture'Engine.S_Weapon'
	AmbientGlow=255
	CollisionRadius=36.0000000
	CollisionHeight=30.0000000
	RotationRate=(Pitch=0,Yaw=32768,Roll=0)
}