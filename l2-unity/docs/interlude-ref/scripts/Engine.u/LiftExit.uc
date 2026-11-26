class LiftExit extends NavigationPoint
	native
	placeable
	hidecategories(Lighting,LightColor,Karma,Force);

var() name LiftTag;
var Mover MyLift;
var() byte SuggestedKeyFrame;
var byte KeyFrame;

function bool SuggestMovePreparation(Pawn Other)
{
	local Controller C;

	// End:0x23
	if(__NFUN_132__(__NFUN_114__(MyLift, none), __NFUN_114__(Other.Controller, none)))
	{
		return false;
	}
	// End:0x131
	if(__NFUN_132__(__NFUN_114__(Other.Base, MyLift), __NFUN_130__(__NFUN_130__(__NFUN_119__(LiftCenter(Other.Anchor), none), __NFUN_114__(LiftCenter(Other.Anchor).MyLift, MyLift)), Other.ReachedDestination(Other.Anchor))))
	{
		// End:0xE9
		if(__NFUN_130__(__NFUN_176__(Location.Z, __NFUN_174__(Other.Location.Z, Other.CollisionHeight)), Other.LineOfSightTo(self)))
		{
			return false;
		}
		Other.DesiredRotation = Rotator(__NFUN_216__(Location, Other.Location));
		Other.Controller.WaitForMover(MyLift);
		return true;		
	}
	else
	{
		C = Level.ControllerList;
		J0x145:

		// End:0x21A [Loop If]
		if(__NFUN_119__(C, none))
		{
			// End:0x203
			if(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_119__(C.Pawn, none), __NFUN_114__(C.PendingMover, MyLift)), C.SameTeamAs(Other.Controller)), C.Pawn.ReachedDestination(self)))
			{
				Other.DesiredRotation = Rotator(__NFUN_216__(Location, Other.Location));
				Other.Controller.WaitForMover(MyLift);
				return true;
			}
			C = C.nextController;
			// [Loop Continue]
			goto J0x145;
		}
	}
	return false;
	return;
}

defaultproperties
{
	SuggestedKeyFrame=255
	bNeverUseStrafing=true
	bForceNoStrafing=true
	bSpecialMove=true
	Texture=Texture'Engine.S_LiftExit'
}