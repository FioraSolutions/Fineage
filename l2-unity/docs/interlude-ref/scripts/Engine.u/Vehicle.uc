class Vehicle extends Pawn
	abstract
	native
	config(User)
	placeable;

var Class<VehiclePart> PartClass[16];
var VehiclePart VehicleParts[16];
var Vector PartOffset[16];
var int NumParts;
var bool bActivated;
var bool bUpdating;
var StaticMesh StaticVehicleMast;
var bool bTrip;
var Vector TripTarget;
var float MoveSpeed;
var int RotationSpeed;

function PostBeginPlay()
{
	local int i;
	local Vector RotX, RotY, RotZ;

	super.PostBeginPlay();
	__NFUN_229__(Rotation, RotX, RotY, RotZ);
	i = 0;
	J0x23:

	// End:0x139 [Loop If]
	if(__NFUN_150__(i, 16))
	{
		// End:0x12C
		if(__NFUN_119__(PartClass[i], none))
		{
			VehicleParts[i] = __NFUN_278__(PartClass[i], self,, __NFUN_215__(__NFUN_215__(__NFUN_215__(Location, __NFUN_213__(PartOffset[i].X, RotX)), __NFUN_213__(PartOffset[i].Y, RotY)), __NFUN_213__(PartOffset[i].Z, RotZ)));
			// End:0xF8
			if(__NFUN_114__(VehicleParts[i], none))
			{
				__NFUN_231__(__NFUN_112__(__NFUN_112__(__NFUN_112__("WARNING - ", string(PartClass[i])), " failed to spawn for "), string(self)));
			}
			VehicleParts[i].__NFUN_299__(Rotation);
			VehicleParts[i].__NFUN_298__(self);
			__NFUN_165__(NumParts);
			// [Explicit Continue]
			goto J0x12F;
		}
		// [Explicit Break]
		goto J0x139;
		J0x12F:

		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x23;
	}
	J0x139:

	return;
}

simulated function bool PointOfView()
{
	return true;
	return;
}

function Tick(float DeltaTime)
{
	local int i;

	bUpdating = false;
	i = 0;
	J0x0F:

	// End:0x75 [Loop If]
	if(__NFUN_150__(i, NumParts))
	{
		// End:0x6B
		if(__NFUN_130__(__NFUN_119__(VehicleParts[i], none), VehicleParts[i].bUpdating))
		{
			VehicleParts[i].Update(DeltaTime);
			bUpdating = true;
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x0F;
	}
	// End:0x93
	if(bUpdating)
	{
		// End:0x93
		if(__NFUN_154__(int(Physics), int(0)))
		{
			__NFUN_3970__(5);
		}
	}
	return;
}

auto state Startup
{
	function Tick(float DeltaTime)
	{
		local int i;

		bUpdating = false;
		i = 0;
		J0x0F:

		// End:0x75 [Loop If]
		if(__NFUN_150__(i, NumParts))
		{
			// End:0x6B
			if(__NFUN_130__(__NFUN_119__(VehicleParts[i], none), VehicleParts[i].bUpdating))
			{
				VehicleParts[i].Update(DeltaTime);
				bUpdating = true;
			}
			__NFUN_165__(i);
			// [Loop Continue]
			goto J0x0F;
		}
		return;
	}
Begin:

	__NFUN_113__('None');
	stop;				
}

defaultproperties
{
	bAsTreatWorld=true
	ControllerClass=none
	Physics=4
	bOwnerNoSee=false
}