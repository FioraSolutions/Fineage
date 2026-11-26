class LadderVolume extends PhysicsVolume
	native
	notplaceable;

var() name ClimbingAnimation;
var() name TopAnimation;
var() Rotator WallDir;
var Vector LookDir;
var Vector ClimbDir;
var const Ladder LadderList;
var() bool bNoPhysicalLadder;
var() bool bAutoPath;
var Pawn PendingClimber;

simulated function PostBeginPlay()
{
	local Ladder L, M;
	local Vector Dir;

	super.PostBeginPlay();
	LookDir = Vector(WallDir);
	// End:0x139
	if(__NFUN_130__(__NFUN_129__(bAutoPath), __NFUN_181__(LookDir.Z, float(0))))
	{
		ClimbDir = vect(0.0000000, 0.0000000, 1.0000000);
		L = LadderList;
		J0x50:

		// End:0x102 [Loop If]
		if(__NFUN_119__(L, none))
		{
			M = LadderList;
			J0x66:

			// End:0xEB [Loop If]
			if(__NFUN_119__(M, none))
			{
				// End:0xD4
				if(__NFUN_119__(M, L))
				{
					Dir = __NFUN_226__(__NFUN_216__(M.Location, L.Location));
					// End:0xC8
					if(__NFUN_176__(__NFUN_219__(Dir, ClimbDir), float(0)))
					{
						__NFUN_221__(Dir, float(-1));
					}
					__NFUN_223__(ClimbDir, Dir);
				}
				M = M.LadderList;
				// [Loop Continue]
				goto J0x66;
			}
			L = L.LadderList;
			// [Loop Continue]
			goto J0x50;
		}
		ClimbDir = __NFUN_226__(ClimbDir);
		// End:0x139
		if(__NFUN_176__(__NFUN_219__(ClimbDir, vect(0.0000000, 0.0000000, 1.0000000)), float(0)))
		{
			__NFUN_221__(ClimbDir, float(-1));
		}
	}
	return;
}

function bool InUse(Pawn Ignored)
{
	local Pawn StillClimbing;

	// End:0x4B
	foreach __NFUN_307__(Class'Engine.Pawn', StillClimbing)
	{
		// End:0x4A
		if(__NFUN_130__(__NFUN_130__(__NFUN_119__(StillClimbing, Ignored), StillClimbing.bCollideActors), StillClimbing.bBlockActors))
		{			
			return true;
		}		
	}	
	// End:0xEF
	if(__NFUN_119__(PendingClimber, none))
	{
		// End:0xEF
		if(__NFUN_132__(__NFUN_132__(__NFUN_132__(__NFUN_132__(__NFUN_114__(PendingClimber.Controller, none), __NFUN_129__(PendingClimber.bCollideActors)), __NFUN_129__(PendingClimber.bBlockActors)), __NFUN_114__(Ladder(PendingClimber.Controller.MoveTarget), none)), __NFUN_119__(Ladder(PendingClimber.Controller.MoveTarget).MyLadder, self)))
		{
			PendingClimber = none;
		}
	}
	return __NFUN_130__(__NFUN_119__(PendingClimber, none), __NFUN_119__(PendingClimber, Ignored));
	return;
}

simulated event PawnEnteredVolume(Pawn P)
{
	local Rotator PawnRot;

	super.PawnEnteredVolume(P);
	// End:0x21
	if(__NFUN_129__(P.CanGrabLadder()))
	{
		return;
	}
	PawnRot = P.Rotation;
	PawnRot.Pitch = 0;
	// End:0xAB
	if(__NFUN_132__(__NFUN_177__(__NFUN_219__(Vector(PawnRot), LookDir), 0.9000000), __NFUN_130__(__NFUN_119__(AIController(P.Controller), none), __NFUN_119__(Ladder(P.Controller.MoveTarget), none))))
	{
		P.ClimbLadder(self);		
	}
	else
	{
		// End:0xE2
		if(__NFUN_130__(__NFUN_129__(P.bDeleteMe), __NFUN_119__(P.Controller, none)))
		{
			__NFUN_278__(Class'Engine.PotentialClimbWatcher', P);
		}
	}
	return;
}

simulated event PawnLeavingVolume(Pawn P)
{
	local Controller C;

	// End:0x16
	if(__NFUN_119__(P.OnLadder, self))
	{
		return;
	}
	super.PawnLeavingVolume(P);
	P.OnLadder = none;
	P.EndClimbLadder(self);
	// End:0x57
	if(__NFUN_114__(P, PendingClimber))
	{
		PendingClimber = none;
	}
	// End:0x115
	if(__NFUN_129__(InUse(P)))
	{
		C = Level.ControllerList;
		J0x7B:

		// End:0x115 [Loop If]
		if(__NFUN_119__(C, none))
		{
			// End:0xFE
			if(__NFUN_130__(__NFUN_130__(C.bPreparingMove, __NFUN_119__(Ladder(C.MoveTarget), none)), __NFUN_114__(Ladder(C.MoveTarget).MyLadder, self)))
			{
				C.bPreparingMove = false;
				PendingClimber = C.Pawn;
				return;
			}
			C = C.nextController;
			// [Loop Continue]
			goto J0x7B;
		}
	}
	return;
}

simulated event PhysicsChangedFor(Actor Other)
{
	// End:0x77
	if(__NFUN_132__(__NFUN_132__(__NFUN_132__(__NFUN_132__(__NFUN_154__(int(Other.Physics), int(2)), __NFUN_154__(int(Other.Physics), int(11))), Other.bDeleteMe), __NFUN_114__(Pawn(Other), none)), __NFUN_114__(Pawn(Other).Controller, none)))
	{
		return;
	}
	__NFUN_278__(Class'Engine.PotentialClimbWatcher', Other);
	return;
}

defaultproperties
{
	ClimbDir=(X=0.0000000,Y=0.0000000,Z=1.0000000)
	bAutoPath=true
	RemoteRole=2
}