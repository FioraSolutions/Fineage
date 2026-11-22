class KTire extends KActor
	abstract
	native
	placeable;

var KCarWheelJoint WheelJoint;
var float RollFriction;
var float LateralFriction;
var float RollSlip;
var float LateralSlip;
var float MinSlip;
var float SlipRate;
var float Softness;
var float Adhesion;
var float Restitution;
var const bool bTireOnGround;
var const float GroundSlipVel;
var const Vector GroundSlipVec;
var const float SpinSpeed;
var const Material GroundMaterial;
var KRigidBodyState ReceiveState;
var bool bReceiveStateNew;

event bool KUpdateState(out KRigidBodyState NewState)
{
	// End:0x0D
	if(__NFUN_129__(bReceiveStateNew))
	{
		return false;
	}
	NewState = ReceiveState;
	bReceiveStateNew = false;
	return true;
	return;
}

function TakeDamage(int Damage, Pawn instigatedBy, Vector HitLocation, Vector Momentum, Class<DamageType> DamageType)
{
	return;
}

defaultproperties
{
	RollFriction=0.3000000
	LateralFriction=0.3000000
	RollSlip=0.0850000
	LateralSlip=0.0600000
	MinSlip=0.0010000
	SlipRate=0.0005000
	Softness=0.0002000
	Restitution=0.1000000
	bNoDelete=false
	bDisturbFluidSurface=true
}