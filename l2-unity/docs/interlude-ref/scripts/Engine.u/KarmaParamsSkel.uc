class KarmaParamsSkel extends KarmaParams
	native
	editinlinenew;

var() string KSkeleton;
var() bool bKDoConvulsions;
var() Range KConvulseSpacing;
var transient Vector KShotStart;
var transient Vector KShotEnd;
var transient float KShotStrength;
var transient bool bKImportantRagdoll;

defaultproperties
{
	KConvulseSpacing=(Min=0.5000000,Max=1.5000000)
}