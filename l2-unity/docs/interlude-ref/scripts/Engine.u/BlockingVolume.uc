class BlockingVolume extends Volume
	native
	nativereplication
	notplaceable;

var() bool bClampFluid;

defaultproperties
{
	bClampFluid=true
	bWorldGeometry=true
	bBlockActors=true
	bBlockPlayers=true
	bBlockZeroExtentTraces=false
	bBlockKarma=true
}