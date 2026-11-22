class CarriedObject extends Actor
	abstract
	native
	nativereplication
	notplaceable;

cpptext
{
	INT* GetOptimizedRepList( BYTE* Recent, FPropertyRetirement* Retire, INT* Ptr, UPackageMap* Map, UActorChannel* Channel );

}

var bool bHome;
var bool bHeld;
var PlayerReplicationInfo HolderPRI;
var Pawn Holder;
var const NavigationPoint LastAnchor;
var float LastValidAnchorTime;

replication
{
	// Pos:0x000
	reliable if(__NFUN_154__(int(Role), int(ROLE_Authority)))
		HolderPRI, bHeld, 
		bHome;
}

function Actor Position()
{
	// End:0x0F
	if(bHeld)
	{
		return Holder;
	}
	return self;
	return;
}

defaultproperties
{
	DrawType=2
	bOrientOnSlope=true
	bAlwaysZeroBoneOffset=true
	bUseCylinderCollision=true
}