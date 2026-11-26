class MovableStaticMeshActor extends StaticMeshActor
	native
	placeable;

struct L2RotatorTime
{
	var() config float PitchTime;
	var() config float RollTime;
	var() config float YawTime;
};

var Rotator L2OrgRotator;
var L2RotatorTime L2CycleDeltaTime;
var L2RotatorTime L2CurrentCycleTime;
var L2RotatorTime L2CurrentMax;
var bool bL2InitMove;
var(L2Movement) array<name> L2MovementTag;
var(L2Movement) L2RotatorTime L2AccelRatio;
var(L2Movement) bool bUseL2RotatorMaxRandom;
var(L2Movement) bool bUseL2RotatorRandomStart;
var(L2Movement) Rotator L2RotatorRate;
var(L2Movement) Rotator L2RotatorMax;

defaultproperties
{
	bUseL2RotatorRandomStart=true
	Physics=19
	bStatic=false
}