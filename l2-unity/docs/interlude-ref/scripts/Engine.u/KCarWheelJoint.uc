class KCarWheelJoint extends KConstraint
	native
	placeable;

cpptext
{
#ifdef WITH_KARMA
    virtual void KUpdateConstraintParams();
	virtual void preKarmaStep(FLOAT DeltaTime);
#endif

}

var(KarmaConstraint) float KSteerAngle;
var(KarmaConstraint) float KProportionalGap;
var(KarmaConstraint) float KMaxSteerTorque;
var(KarmaConstraint) float KMaxSteerSpeed;
var(KarmaConstraint) bool bKSteeringLocked;
var(KarmaConstraint) float KMotorTorque;
var(KarmaConstraint) float KMaxSpeed;
var(KarmaConstraint) float KBraking;
var(KarmaConstraint) float KSuspLowLimit;
var(KarmaConstraint) float KSuspHighLimit;
var(KarmaConstraint) float KSuspStiffness;
var(KarmaConstraint) float KSuspDamping;
var(KarmaConstraint) float KSuspRef;
var const float KWheelHeight;

defaultproperties
{
	KProportionalGap=8200.0000000
	KMaxSteerTorque=1000.0000000
	KMaxSteerSpeed=2600.0000000
	bKSteeringLocked=true
	KMaxSpeed=1310700.0000000
	KSuspLowLimit=-1.0000000
	KSuspHighLimit=1.0000000
	KSuspStiffness=50.0000000
	KSuspDamping=5.0000000
	bNoDelete=false
}