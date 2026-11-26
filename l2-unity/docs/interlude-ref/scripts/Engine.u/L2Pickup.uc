class L2Pickup extends Actor
	native
	nativereplication
	placeable;

var Rotator TargetRotation;
var Rotator DeltaRotation;
var Sound DropSound;
var bool bPendingDrop;
var Emitter DropEffectActor;
var bool bDropEffectActor;
var Vector CheckLocation;

simulated function Timer()
{
	// End:0x11
	if(bPendingDrop)
	{
		bHidden = false;
	}
	return;
}

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