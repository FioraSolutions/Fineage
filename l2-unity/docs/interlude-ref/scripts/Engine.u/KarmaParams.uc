class KarmaParams extends KarmaParamsCollision
	native
	editinlinenew;

cpptext
{
#ifdef WITH_KARMA
    void PostEditChange();
#endif

}

var const transient int KAng3;
var const transient int KTriList;
var const transient float KLastVel;
var() float KMass;
var() float KLinearDamping;
var() float KAngularDamping;
var() float KBuoyancy;
var() bool KStartEnabled;
var() Vector KStartLinVel;
var() Vector KStartAngVel;
var() bool bKNonSphericalInertia;
var() float KActorGravScale;
var() float KVelDropBelowThreshold;
var() bool bHighDetailOnly;
var bool bClientOnly;
var() const bool bKDoubleTickRate;
var() bool bKStayUpright;
var() bool bKAllowRotate;
var bool bDestroyOnSimError;

defaultproperties
{
	KMass=1.0000000
	KLinearDamping=0.2000000
	KAngularDamping=0.2000000
	KActorGravScale=1.0000000
	KVelDropBelowThreshold=1000000.0000000
	bHighDetailOnly=true
	bClientOnly=true
	bDestroyOnSimError=true
}