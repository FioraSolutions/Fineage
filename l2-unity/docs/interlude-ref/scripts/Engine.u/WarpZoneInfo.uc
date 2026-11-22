class WarpZoneInfo extends ZoneInfo
	native
	placeable
	hidecategories(Movement,Collision,Lighting,LightColor,Karma,Force);

var() string OtherSideURL;
var() name ThisTag;
var() bool bNoTeleFrag;
var const int iWarpZone;
var const Coords WarpCoords;
var transient WarpZoneInfo OtherSideActor;
var transient Object OtherSideLevel;
var() string Destinations[8];
var int numDestinations;

replication
{
	// Pos:0x000
	reliable if(__NFUN_154__(int(Role), int(ROLE_Authority)))
		OtherSideActor, OtherSideURL, 
		ThisTag;
}

// Export UWarpZoneInfo::execWarp(FFrame&, void* const)
native(314) final function Warp(out Vector Loc, out Vector Vel, out Rotator R);

// Export UWarpZoneInfo::execUnWarp(FFrame&, void* const)
native(315) final function UnWarp(out Vector Loc, out Vector Vel, out Rotator R);

function PreBeginPlay()
{
	super.PreBeginPlay();
	Generate();
	numDestinations = 0;
	J0x13:

	// End:0x46 [Loop If]
	if(__NFUN_150__(numDestinations, 8))
	{
		// End:0x3B
		if(__NFUN_123__(Destinations[numDestinations], ""))
		{
			__NFUN_165__(numDestinations);			
		}
		else
		{
			numDestinations = 8;
		}
		// [Loop Continue]
		goto J0x13;
	}
	// End:0x6C
	if(__NFUN_130__(__NFUN_151__(numDestinations, 0), __NFUN_122__(OtherSideURL, "")))
	{
		OtherSideURL = Destinations[0];
	}
	return;
}

function Trigger(Actor Other, Pawn EventInstigator)
{
	local int nextPick;

	// End:0x0D
	if(__NFUN_154__(numDestinations, 0))
	{
		return;
	}
	nextPick = 0;
	J0x14:

	// End:0x41 [Loop If]
	if(__NFUN_130__(__NFUN_150__(nextPick, 8), __NFUN_123__(Destinations[nextPick], OtherSideURL)))
	{
		__NFUN_165__(nextPick);
		// [Loop Continue]
		goto J0x14;
	}
	__NFUN_165__(nextPick);
	// End:0x6F
	if(__NFUN_132__(__NFUN_151__(nextPick, 7), __NFUN_122__(Destinations[nextPick], "")))
	{
		nextPick = 0;
	}
	OtherSideURL = Destinations[nextPick];
	ForceGenerate();
	return;
}

simulated event Generate()
{
	// End:0x0D
	if(__NFUN_119__(OtherSideLevel, none))
	{
		return;
	}
	ForceGenerate();
	return;
}

simulated event ForceGenerate()
{
	// End:0x21
	if(__NFUN_153__(__NFUN_126__(OtherSideURL, "/"), 0))
	{
		OtherSideLevel = none;
		OtherSideActor = none;		
	}
	else
	{
		OtherSideLevel = XLevel;
		// End:0x67
		foreach __NFUN_304__(Class'Engine.WarpZoneInfo', OtherSideActor)
		{
			// End:0x66
			if(__NFUN_130__(__NFUN_124__(string(OtherSideActor.ThisTag), OtherSideURL), __NFUN_119__(OtherSideActor, self)))
			{
				// End:0x67
				break;
			}			
		}		
	}
	return;
}

simulated function ActorEntered(Actor Other)
{
	local Vector L;
	local Rotator R;
	local Controller P;

	// End:0x241
	if(__NFUN_129__(Other.bJustTeleported))
	{
		Generate();
		// End:0x241
		if(__NFUN_119__(OtherSideActor, none))
		{
			Other.__NFUN_118__('Touch');
			Other.__NFUN_118__('UnTouch');
			L = Other.Location;
			// End:0x83
			if(__NFUN_119__(Pawn(Other), none))
			{
				R = Pawn(Other).GetViewRotation();
			}
			__NFUN_315__(L, Other.Velocity, R);
			OtherSideActor.__NFUN_314__(L, Other.Velocity, R);
			// End:0x1FF
			if(Other.__NFUN_303__('Pawn'))
			{
				Pawn(Other).bWarping = bNoTeleFrag;
				// End:0x1F5
				if(Other.__NFUN_267__(L))
				{
					// End:0x174
					if(__NFUN_154__(int(Role), int(ROLE_Authority)))
					{
						P = Level.ControllerList;
						J0x129:

						// End:0x174 [Loop If]
						if(__NFUN_119__(P, none))
						{
							// End:0x15D
							if(__NFUN_114__(P.Enemy, Other))
							{
								P.__NFUN_514__(Other);
							}
							P = P.nextController;
							// [Loop Continue]
							goto J0x129;
						}
					}
					R.Roll = 0;
					Pawn(Other).SetViewRotation(R);
					Pawn(Other).ClientSetLocation(L, R);
					// End:0x1F2
					if(__NFUN_119__(Pawn(Other).Controller, none))
					{
						Pawn(Other).Controller.MoveTimer = -1.0000000;
					}					
				}
				else
				{
					__NFUN_113__('DelayedWarp');
				}				
			}
			else
			{
				Other.__NFUN_267__(L);
				Other.__NFUN_299__(R);
			}
			Other.__NFUN_117__('Touch');
			Other.__NFUN_117__('UnTouch');
		}
	}
	return;
}

event ActorLeaving(Actor Other)
{
	// End:0x2A
	if(Other.__NFUN_303__('Pawn'))
	{
		Pawn(Other).bWarping = false;
	}
	return;
}

state DelayedWarp
{
	function Tick(float DeltaTime)
	{
		local Controller P;
		local bool bFound;

		P = Level.ControllerList;
		J0x14:

		// End:0x88 [Loop If]
		if(__NFUN_119__(P, none))
		{
			// End:0x71
			if(__NFUN_130__(P.Pawn.bWarping, __NFUN_114__(P.Pawn.Region.Zone, self)))
			{
				bFound = true;
				ActorEntered(P);
			}
			P = P.nextController;
			// [Loop Continue]
			goto J0x14;
		}
		// End:0x9A
		if(__NFUN_129__(bFound))
		{
			__NFUN_113__('None');
		}
		return;
	}
	stop;
}
