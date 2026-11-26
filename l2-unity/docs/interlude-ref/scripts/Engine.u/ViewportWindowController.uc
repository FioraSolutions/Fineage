class ViewportWindowController extends Actor
	native
	notplaceable;

enum ECALCSTEP
{
	STEP_TARGET_PLAYER,             // 0
	STEP_MOVE_BACK,                 // 1
	STEP_ROTATE_FLOAT,              // 2
	STEP_TARGET_FLOAT,              // 3
	STEP_MOVE_FLOAT                 // 4
};

var SkeletalMeshInstance TargetSMInst;
var int BoneIndex;
var Vector TargetLocation;
var Rotator TargetRotation;
var array<Vector> CameraPath;
var int CalcStep;
var Vector OrgLocation;
var int EffectType;
var float fEffectElapsedTime;

defaultproperties
{
	DrawType=0
}