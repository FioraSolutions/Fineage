class MatAction extends MatObject
	abstract
	native;

var() InterpolationPoint IntPoint;
var() string Comment;
var(Time) float Duration;
var(Sub) export editinline array<export editinline MatSubAction> SubActions;
var(Path) bool bSmoothCorner;
var(Path) Vector StartControlPoint;
var(Path) Vector EndControlPoint;
var(Path) bool bConstantPathVelocity;
var(Path) float PathVelocity;
var float PathLength;
var transient array<Vector> SampleLocations;
var transient float PctStarting;
var transient float PctEnding;
var transient float PctDuration;

defaultproperties
{
	bSmoothCorner=true
	StartControlPoint=(X=800.0000000,Y=800.0000000,Z=0.0000000)
	EndControlPoint=(X=-800.0000000,Y=-800.0000000,Z=0.0000000)
}