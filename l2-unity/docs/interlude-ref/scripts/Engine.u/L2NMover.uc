class L2NMover extends Actor
	native
	notplaceable;

struct NMoverTarget
{
	var int bTarget;
	var int bOwnedTarget;
	var Vector Loc;
	var Actor Target;
};

var bool bLoop;
var bool bMoveStart;
var bool bMoveEnd;
var bool bMovePause;
var float MoveStartDelay;
var Vector OwnerOrigin;
var int TargetIndex;
var array<NMoverTarget> MoverTargets;
var float MoveSpeed;
var float CurMoveSpeed;
var float AccelRate;
var float AccelAccelRate;
var float CurAccelRate;
var float MaxSpeed;
var float MinSpeed;
var float MaxAccelRate;
var float MinAccelRate;
var Actor ForceBounceActor;

defaultproperties
{
	MoveSpeed=100.0000000
	MaxSpeed=2000.0000000
	MinSpeed=1.0000000
	MaxAccelRate=1000.0000000
	MinAccelRate=-1000.0000000
	DrawType=0
	CollisionRadius=1.0000000
	CollisionHeight=1.0000000
}