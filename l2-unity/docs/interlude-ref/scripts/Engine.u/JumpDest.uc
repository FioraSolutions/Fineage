class JumpDest extends NavigationPoint
	native
	notplaceable
	hidecategories(Lighting,LightColor,Karma,Force);

cpptext
{
	virtual void SetupForcedPath(APawn* Scout, UReachSpec* Path);
	void ClearPaths();
	UBOOL ReviewPath(APawn* Scout);

}

var bool bOptionalJumpDest;
var int NumUpstreamPaths;
var ReachSpec UpstreamPaths[8];
var Vector NeededJump[8];
var float CalculatedGravityZ[8];

function int GetPathIndex(ReachSpec Path)
{
	local int i;

	// End:0x0D
	if(__NFUN_114__(Path, none))
	{
		return 0;
	}
	i = 0;
	J0x14:

	// End:0x45 [Loop If]
	if(__NFUN_150__(i, 4))
	{
		// End:0x3B
		if(__NFUN_114__(UpstreamPaths[i], Path))
		{
			return i;
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x14;
	}
	return 0;
	return;
}

event int SpecialCost(Pawn Other, ReachSpec Path)
{
	local int Num;

	Num = GetPathIndex(Path);
	// End:0x66
	if(__NFUN_179__(__NFUN_186__(__NFUN_172__(Other.JumpZ, Other.PhysicsVolume.Gravity.Z)), __NFUN_186__(__NFUN_172__(NeededJump[Num].Z, CalculatedGravityZ[Num]))))
	{
		return 100;
	}
	return 10000000;
	return;
}

function DoJump(Pawn Other)
{
	Other.bWantsToCrouch = false;
	Other.Controller.MoveTarget = self;
	Other.Controller.Destination = Location;
	Other.bNoJumpAdjust = true;
	Other.Velocity = Other.Controller.__NFUN_523__(0.0000000, Other.GroundSpeed);
	Other.Acceleration = vect(0.0000000, 0.0000000, 0.0000000);
	Other.__NFUN_3970__(2);
	Other.Controller.SetFall();
	Other.DestinationOffset = CollisionRadius;
	return;
}

event bool SuggestMovePreparation(Pawn Other)
{
	local int Num;

	// End:0x16
	if(__NFUN_114__(Other.Controller, none))
	{
		return false;
	}
	Num = GetPathIndex(Other.Controller.CurrentPath);
	// End:0x8D
	if(__NFUN_176__(__NFUN_186__(__NFUN_172__(Other.JumpZ, Other.PhysicsVolume.Gravity.Z)), __NFUN_186__(__NFUN_172__(NeededJump[Num].Z, CalculatedGravityZ[Num]))))
	{
		return false;
	}
	DoJump(Other);
	return false;
	return;
}

defaultproperties
{
	bSpecialForced=true
}