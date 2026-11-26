class NProjectile extends Emitter
	native
	placeable;

var float Speed;
var float AccSpeed;
var Actor TargetActor;
var Vector LastTargetLocation;
var Rotator LastTargetRotation;
var Actor TraceActor;
var bool bTrackingCamera;
var bool bPreDestroy;
var(interpolation) bool bHermiteInterpolation;
var(interpolation) Vector VelInitial;
var(interpolation) Vector VelFinal;
var(interpolation) Vector LocInitial;
var(interpolation) float Duration;
var(interpolation) transient float CurTime;
var(interpolation) float Disp;
var transient NMagicInfo MagicInfo;

simulated event ShotNotify()
{
	return;
}

simulated event PreshotNotify(Pawn Attacker)
{
	return;
}

defaultproperties
{
	Speed=10.0000000
	AccSpeed=10.0000000
	bNoDelete=false
	LifeSpan=100.0000000
	CollisionRadius=5.0000000
	CollisionHeight=5.0000000
}