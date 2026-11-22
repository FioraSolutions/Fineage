class WeaponAttachment extends InventoryAttachment
	native
	nativereplication
	notplaceable;

var byte FlashCount;
var bool bAutoFire;
var name FiringMode;
var float FiringSpeed;

replication
{
	// Pos:0x000
	reliable if(__NFUN_130__(__NFUN_130__(bNetDirty, __NFUN_129__(bNetOwner)), __NFUN_154__(int(Role), int(ROLE_Authority))))
		FiringMode, FlashCount, 
		bAutoFire;
}

simulated event ThirdPersonEffects()
{
	// End:0x24
	if(__NFUN_119__(Instigator, none))
	{
		Instigator.PlayFiring(1.0000000, FiringMode);
	}
	return;
}

defaultproperties
{
	FiringSpeed=1.0000000
	bActorShadows=true
	bReplicateInstigator=true
}