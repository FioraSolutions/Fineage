class AIController extends Controller
	native
	config
	notplaceable;

var bool bHunting;
var bool bAdjustFromWalls;
var AIScript MyScript;
var float Skill;

// Export UAIController::execWaitToSeeEnemy(FFrame&, void* const)
native(510) final latent function WaitToSeeEnemy();

event OwnerPossess(Pawn aPawn)
{
	return;
}

event GetDestination(Pawn P)
{
	return;
}

event GetSlaveDestination(Pawn P)
{
	return;
}

event PreBeginPlay()
{
	super.PreBeginPlay();
	// End:0x11
	if(bDeleteMe)
	{
		return;
	}
	// End:0x43
	if(__NFUN_119__(Level.Game, none))
	{
		__NFUN_184__(Skill, Level.Game.GameDifficulty);
	}
	Skill = __NFUN_246__(Skill, 0.0000000, 3.0000000);
	return;
}

function Reset()
{
	super.Reset();
	// End:0x12
	if(bIsPlayer)
	{
		__NFUN_279__();
	}
	return;
}

function Trigger(Actor Other, Pawn EventInstigator)
{
	TriggerScript(Other, EventInstigator);
	return;
}

function bool WeaponFireAgain(float RefireRate, bool bFinishedFire)
{
	// End:0x40
	if(__NFUN_130__(Pawn.PressingFire(), __NFUN_176__(__NFUN_195__(), RefireRate)))
	{
		Pawn.Weapon.BotFire(bFinishedFire);
		return true;
	}
	StopFiring();
	return false;
	return;
}

function bool TriggerScript(Actor Other, Pawn EventInstigator)
{
	// End:0x26
	if(__NFUN_119__(MyScript, none))
	{
		MyScript.Trigger(EventInstigator, Pawn);
		return true;
	}
	return false;
	return;
}

function DisplayDebug(Canvas Canvas, out float YL, out float YPos)
{
	local int i;
	local string t;

	super.DisplayDebug(Canvas, YL, YPos);
	Canvas.DrawColor.B = byte(255);
	Canvas.__NFUN_465__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("     Skill ", string(Skill)), " NAVIGATION MoveTarget "), GetItemName(string(MoveTarget))), " PendingMover "), string(PendingMover)), " MoveTimer "), string(MoveTimer)), false);
	__NFUN_184__(YPos, YL);
	Canvas.SetPos(4.0000000, YPos);
	Canvas.__NFUN_465__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("      Destination ", string(Destination)), " Focus "), GetItemName(string(Focus))), " Preparing Move "), string(bPreparingMove)), false);
	__NFUN_184__(YPos, YL);
	Canvas.SetPos(4.0000000, YPos);
	Canvas.__NFUN_465__(__NFUN_112__(__NFUN_112__(__NFUN_112__("     RouteGoal ", GetItemName(string(RouteGoal))), " RouteDist "), string(RouteDist)), false);
	__NFUN_184__(YPos, YL);
	Canvas.SetPos(4.0000000, YPos);
	i = 0;
	J0x1CA:

	// End:0x25D [Loop If]
	if(__NFUN_150__(i, 16))
	{
		// End:0x222
		if(__NFUN_114__(RouteCache[i], none))
		{
			// End:0x21C
			if(__NFUN_151__(i, 5))
			{
				t = __NFUN_112__(__NFUN_112__(t, "--"), GetItemName(string(RouteCache[__NFUN_147__(i, 1)])));
			}
			// [Explicit Break]
			goto J0x25D;
			// [Explicit Continue]
			goto J0x253;
		}
		// End:0x253
		if(__NFUN_150__(i, 5))
		{
			t = __NFUN_112__(__NFUN_112__(t, GetItemName(string(RouteCache[i]))), "-");
		}
		J0x253:

		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x1CA;
	}
	J0x25D:

	Canvas.__NFUN_465__(__NFUN_112__("     RouteCache: ", t), false);
	__NFUN_184__(YPos, YL);
	Canvas.SetPos(4.0000000, YPos);
	return;
}

function float AdjustDesireFor(Pickup P)
{
	return 0.0000000;
	return;
}

function int GetFacingDirection()
{
	local float strafeMag;
	local Vector Focus2D, Loc2D, Dest2D, Dir, LookDir, Y;

	Focus2D = FocalPoint;
	Focus2D.Z = 0.0000000;
	Loc2D = Pawn.Location;
	Loc2D.Z = 0.0000000;
	Dest2D = Destination;
	Dest2D.Z = 0.0000000;
	LookDir = __NFUN_226__(__NFUN_216__(Focus2D, Loc2D));
	Dir = __NFUN_226__(__NFUN_216__(Dest2D, Loc2D));
	strafeMag = __NFUN_219__(LookDir, Dir);
	Y = __NFUN_220__(LookDir, vect(0.0000000, 0.0000000, 1.0000000));
	// End:0xE6
	if(__NFUN_176__(__NFUN_219__(Y, __NFUN_216__(Dest2D, Loc2D)), float(0)))
	{
		return int(__NFUN_174__(float(49152), __NFUN_171__(float(16384), strafeMag)));		
	}
	else
	{
		return int(__NFUN_175__(float(16384), __NFUN_171__(float(16384), strafeMag)));
	}
	return;
}

function AdjustView(float DeltaTime)
{
	local float TargetYaw, TargetPitch;
	local Rotator OldViewRotation, ViewRotation;

	super.AdjustView(DeltaTime);
	// End:0x21
	if(__NFUN_129__(Pawn.bUpdateEyeheight))
	{
		return;
	}
	ViewRotation = Rotation;
	OldViewRotation = Rotation;
	// End:0x20F
	if(__NFUN_114__(Enemy, none))
	{
		ViewRotation.Roll = 0;
		// End:0x20F
		if(__NFUN_176__(DeltaTime, 0.2000000))
		{
			OldViewRotation.Yaw = __NFUN_156__(OldViewRotation.Yaw, 65535);
			OldViewRotation.Pitch = __NFUN_156__(OldViewRotation.Pitch, 65535);
			TargetYaw = float(__NFUN_156__(Rotation.Yaw, 65535));
			// End:0x104
			if(__NFUN_177__(__NFUN_186__(__NFUN_175__(TargetYaw, float(OldViewRotation.Yaw))), float(32768)))
			{
				// End:0xF6
				if(__NFUN_176__(TargetYaw, float(OldViewRotation.Yaw)))
				{
					__NFUN_184__(TargetYaw, float(65536));					
				}
				else
				{
					__NFUN_185__(TargetYaw, float(65536));
				}
			}
			TargetYaw = __NFUN_174__(__NFUN_171__(float(OldViewRotation.Yaw), __NFUN_175__(float(1), __NFUN_171__(float(5), DeltaTime))), __NFUN_171__(__NFUN_171__(TargetYaw, float(5)), DeltaTime));
			ViewRotation.Yaw = int(TargetYaw);
			TargetPitch = float(__NFUN_156__(Rotation.Pitch, 65535));
			// End:0x1BD
			if(__NFUN_177__(__NFUN_186__(__NFUN_175__(TargetPitch, float(OldViewRotation.Pitch))), float(32768)))
			{
				// End:0x1AF
				if(__NFUN_176__(TargetPitch, float(OldViewRotation.Pitch)))
				{
					__NFUN_184__(TargetPitch, float(65536));					
				}
				else
				{
					__NFUN_185__(TargetPitch, float(65536));
				}
			}
			TargetPitch = __NFUN_174__(__NFUN_171__(float(OldViewRotation.Pitch), __NFUN_175__(float(1), __NFUN_171__(float(5), DeltaTime))), __NFUN_171__(__NFUN_171__(TargetPitch, float(5)), DeltaTime));
			ViewRotation.Pitch = int(TargetPitch);
			__NFUN_299__(ViewRotation);
		}
	}
	return;
}

function SetOrders(name NewOrders, Controller OrderGiver)
{
	return;
}

function Actor GetOrderObject()
{
	return none;
	return;
}

function name GetOrders()
{
	return 'None';
	return;
}

event PrepareForMove(NavigationPoint Goal, ReachSpec Path)
{
	return;
}

function WaitForMover(Mover M)
{
	// End:0x37
	if(__NFUN_130__(__NFUN_119__(Enemy, none), __NFUN_176__(__NFUN_175__(Level.TimeSeconds, LastSeenTime), 3.0000000)))
	{
		Focus = Enemy;
	}
	PendingMover = M;
	bPreparingMove = true;
	Pawn.Acceleration = vect(0.0000000, 0.0000000, 0.0000000);
	return;
}

function MoverFinished()
{
	// End:0x2F
	if(PendingMover.myMarker.ProceedWithMove(Pawn))
	{
		PendingMover = none;
		bPreparingMove = false;
	}
	return;
}

function UnderLift(Mover M)
{
	local NavigationPoint N;

	bPreparingMove = false;
	PendingMover = none;
	// End:0xBC
	if(__NFUN_132__(__NFUN_114__(MoveTarget, none), MoveTarget.__NFUN_303__('LiftCenter')))
	{
		N = Level.NavigationPointList;
		J0x44:

		// End:0xBC [Loop If]
		if(__NFUN_119__(N, none))
		{
			// End:0xA5
			if(__NFUN_130__(__NFUN_130__(N.__NFUN_303__('LiftExit'), __NFUN_254__(LiftExit(N).LiftTag, M.Tag)), __NFUN_520__(N)))
			{
				MoveTarget = N;
				return;
			}
			N = N.nextNavigationPoint;
			// [Loop Continue]
			goto J0x44;
		}
	}
	return;
}

function bool PriorityObjective()
{
	return false;
	return;
}

function Startle(Actor A)
{
	return;
}

defaultproperties
{
	bAdjustFromWalls=true
	bCanOpenDoors=true
	bCanDoSpecial=true
	MinHitWall=-0.5000000
}