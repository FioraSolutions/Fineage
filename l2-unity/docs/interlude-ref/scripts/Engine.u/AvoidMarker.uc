class AvoidMarker extends Triggers
	native
	notplaceable;

function Touch(Actor Other)
{
	// End:0x49
	if(__NFUN_130__(__NFUN_119__(Pawn(Other), none), __NFUN_119__(Pawn(Other).Controller, none)))
	{
		Pawn(Other).Controller.FearThisSpot(self);
	}
	return;
}

function StartleBots()
{
	local Pawn P;

	// End:0x4D
	foreach __NFUN_321__(Class'Engine.Pawn', P, CollisionRadius)
	{
		// End:0x4C
		if(__NFUN_119__(AIController(P.Controller), none))
		{
			AIController(P.Controller).Startle(self);
		}		
	}	
	return;
}

defaultproperties
{
	RemoteRole=0
	CollisionRadius=100.0000000
}