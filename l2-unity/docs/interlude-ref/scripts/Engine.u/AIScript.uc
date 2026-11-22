class AIScript extends Keypoint
	native
	placeable;

cpptext
{
	virtual INT AddMyMarker(AActor *S);
	virtual void ClearMarker();

}

var() Class<AIController> ControllerClass;
var bool bNavigate;
var bool bLoggingEnabled;
var AIMarker myMarker;

function SpawnControllerFor(Pawn P)
{
	local AIController C;

	// End:0x59
	if(__NFUN_114__(ControllerClass, none))
	{
		// End:0x21
		if(__NFUN_114__(P.ControllerClass, none))
		{
			return;
		}
		C = __NFUN_278__(P.ControllerClass,,, P.Location, P.Rotation);		
	}
	else
	{
		C = __NFUN_278__(ControllerClass,,, P.Location, P.Rotation);
	}
	C.MyScript = self;
	C.Possess(P);
	return;
}

function Actor GetMoveTarget()
{
	// End:0x11
	if(__NFUN_119__(myMarker, none))
	{
		return myMarker;
	}
	return self;
	return;
}

function TakeOver(Pawn P)
{
	return;
}

defaultproperties
{
	Texture=Texture'Engine.S_AIScript'
	DrawScale=0.5000000
}