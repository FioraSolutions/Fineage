class KVehicleFactory extends Actor
	placeable;

var() Class<KVehicle> VehicleClass;
var() int MaxVehicleCount;
var int VehicleCount;

event Trigger(Actor Other, Pawn EventInstigator)
{
	local KVehicle CreatedVehicle;

	// End:0x11
	if(__NFUN_153__(VehicleCount, MaxVehicleCount))
	{
		return;
	}
	// End:0x4D
	if(__NFUN_119__(VehicleClass, none))
	{
		CreatedVehicle = __NFUN_278__(VehicleClass,,, Location, Rotation);
		__NFUN_165__(VehicleCount);
		CreatedVehicle.ParentFactory = self;
	}
	return;
}

defaultproperties
{
	bHidden=true
	bNoDelete=true
	RemoteRole=0
	Texture=Texture'Engine.S_KVehFact'
	bDirectional=true
}