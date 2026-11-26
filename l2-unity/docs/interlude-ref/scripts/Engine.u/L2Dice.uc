class L2Dice extends Actor
	native
	nativereplication
	placeable;

struct NActionPtr
{
	var int Ptr;
};

var Rotator TargetRotation;
var Rotator DeltaRotation;
var Sound DropSound;
var Vector CheckLocation;
var NActionPtr Action;
var bool bActionOn;

defaultproperties
{
	DrawType=2
	bNeedCleanup=false
	NoCheatCollision=true
	bOrientOnSlope=true
	bAlwaysRelevant=true
	bCheckChangableLevel=true
	NetUpdateFrequency=8.0000000
	NetPriority=1.4000000
	Texture=Texture'Engine.S_Inventory'
	CollisionRadius=0.1000000
	CollisionHeight=0.1000000
	bCollideActors=true
	bProjTarget=true
	bFixedRotationDir=true
}