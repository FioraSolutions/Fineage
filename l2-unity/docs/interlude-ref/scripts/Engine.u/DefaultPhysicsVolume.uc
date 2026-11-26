class DefaultPhysicsVolume extends PhysicsVolume
	native
	notplaceable;

function Destroyed()
{
	__NFUN_231__(__NFUN_112__(string(self), " destroyed!"));
	assert(false);
	return;
}

defaultproperties
{
	bStatic=false
	bNoDelete=false
}