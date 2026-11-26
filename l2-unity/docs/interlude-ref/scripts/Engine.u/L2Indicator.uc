class L2Indicator extends Emitter
	native
	nativereplication
	placeable;

var int Type;

defaultproperties
{
	DrawType=2
	NoCheatCollision=true
	bOrientOnSlope=true
	bAlwaysRelevant=true
	bCheckChangableLevel=false
	RemoteRole=1
	NetUpdateFrequency=8.0000000
	NetPriority=1.4000000
	Texture=Texture'Engine.S_Inventory'
	CollisionRadius=0.1000000
	CollisionHeight=0.1000000
	bCollideActors=true
	bProjTarget=true
	bFixedRotationDir=true
}