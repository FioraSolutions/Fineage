class Ammo extends Pickup
	abstract
	native
	placeable;

var() int AmmoAmount;

function float DetourWeight(Pawn Other, float PathWeight)
{
	local Ammunition AlreadyHas;

	// End:0x27
	if(__NFUN_179__(Other.Weapon.AIRating, 0.5000000))
	{
		return 0.0000000;
	}
	AlreadyHas = Ammunition(Other.FindInventoryType(InventoryType));
	// End:0x57
	if(__NFUN_114__(AlreadyHas, none))
	{
		return 0.0000000;
	}
	// End:0x78
	if(__NFUN_154__(AlreadyHas.AmmoAmount, 0))
	{
		return __NFUN_172__(MaxDesireability, PathWeight);
	}
	return 0.0000000;
	return;
}

function float BotDesireability(Pawn Bot)
{
	local Ammunition AlreadyHas;

	// End:0x21
	if(Bot.Controller.bHuntPlayer)
	{
		return 0.0000000;
	}
	AlreadyHas = Ammunition(Bot.FindInventoryType(InventoryType));
	// End:0x58
	if(__NFUN_114__(AlreadyHas, none))
	{
		return __NFUN_171__(0.3500000, MaxDesireability);
	}
	// End:0x72
	if(__NFUN_154__(AlreadyHas.AmmoAmount, 0))
	{
		return MaxDesireability;
	}
	// End:0x99
	if(__NFUN_153__(AlreadyHas.AmmoAmount, AlreadyHas.MaxAmmo))
	{
		return -1.0000000;
	}
	return __NFUN_171__(MaxDesireability, __NFUN_244__(1.0000000, __NFUN_172__(__NFUN_171__(0.1500000, float(AmmoAmount)), float(AlreadyHas.AmmoAmount))));
	return;
}

function Inventory SpawnCopy(Pawn Other)
{
	local Inventory Copy;

	Copy = super.SpawnCopy(Other);
	Ammunition(Copy).AmmoAmount = AmmoAmount;
	return Copy;
	return;
}

defaultproperties
{
	MaxDesireability=0.2000000
	RespawnTime=30.0000000
	PickupMessage="You picked up some ammo."
	Texture=Texture'Engine.S_Ammo'
	AmbientGlow=255
}