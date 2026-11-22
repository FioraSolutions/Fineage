class Teleporter extends SmallNavigationPoint
	native
	placeable
	hidecategories(Lighting,LightColor,Karma,Force);

var() string URL;
var() name ProductRequired;
var() bool bChangesVelocity;
var() bool bChangesYaw;
var() bool bReversesX;
var() bool bReversesY;
var() bool bReversesZ;
var() bool bEnabled;
var() Vector TargetVelocity;
var Actor TriggerActor;
var Actor TriggerActor2;
var float LastFired;

replication
{
	// Pos:0x000
	reliable if(__NFUN_154__(int(Role), int(ROLE_Authority)))
		URL, bEnabled;

	// Pos:0x00D
	reliable if(__NFUN_130__(bNetInitial, __NFUN_154__(int(Role), int(ROLE_Authority))))
		TargetVelocity, bChangesVelocity, 
		bChangesYaw, bReversesX, 
		bReversesY, bReversesZ;
}

function PostBeginPlay()
{
	// End:0x12
	if(__NFUN_124__(URL, ""))
	{
		__NFUN_262__(false, false, false);
	}
	// End:0x23
	if(__NFUN_129__(bEnabled))
	{
		FindTriggerActor();
	}
	super(NavigationPoint).PostBeginPlay();
	return;
}

function FindTriggerActor()
{
	local Actor A;

	TriggerActor = none;
	TriggerActor2 = none;
	// End:0x5E
	foreach __NFUN_313__(Class'Engine.Actor', A)
	{
		// End:0x5D
		if(__NFUN_254__(A.Event, Tag))
		{
			// End:0x4F
			if(__NFUN_114__(TriggerActor, none))
			{
				TriggerActor = A;
				// End:0x5D
				continue;
			}
			TriggerActor2 = A;			
			return;
		}		
	}	
	return;
}

simulated function bool Accept(Actor Incoming, Actor Source)
{
	local Rotator newRot, OldRot;
	local float mag;
	local Vector oldDir;
	local Controller P;

	__NFUN_118__('Touch');
	newRot = Incoming.Rotation;
	// End:0x93
	if(bChangesYaw)
	{
		OldRot = Incoming.Rotation;
		newRot.Yaw = Rotation.Yaw;
		// End:0x93
		if(__NFUN_119__(Source, none))
		{
			__NFUN_161__(newRot.Yaw, __NFUN_147__(__NFUN_146__(32768, Incoming.Rotation.Yaw), Source.Rotation.Yaw));
		}
	}
	// End:0x268
	if(__NFUN_119__(Pawn(Incoming), none))
	{
		// End:0x112
		if(__NFUN_154__(int(Role), int(ROLE_Authority)))
		{
			P = Level.ControllerList;
			J0xC7:

			// End:0x112 [Loop If]
			if(__NFUN_119__(P, none))
			{
				// End:0xFB
				if(__NFUN_114__(P.Enemy, Incoming))
				{
					P.__NFUN_514__(Incoming);
				}
				P = P.nextController;
				// [Loop Continue]
				goto J0xC7;
			}
		}
		// End:0x156
		if(__NFUN_129__(Pawn(Incoming).__NFUN_267__(Location)))
		{
			__NFUN_231__(__NFUN_112__(__NFUN_112__(string(self), " Teleport failed for "), string(Incoming)));
			return false;
		}
		// End:0x1EF
		if(__NFUN_132__(__NFUN_154__(int(Role), int(ROLE_Authority)), __NFUN_177__(__NFUN_175__(Level.TimeSeconds, LastFired), 0.5000000)))
		{
			newRot.Roll = 0;
			Pawn(Incoming).__NFUN_299__(newRot);
			Pawn(Incoming).SetViewRotation(newRot);
			Pawn(Incoming).ClientSetRotation(newRot);
			LastFired = Level.TimeSeconds;
		}
		// End:0x254
		if(__NFUN_119__(Pawn(Incoming).Controller, none))
		{
			Pawn(Incoming).Controller.MoveTimer = -1.0000000;
			Pawn(Incoming).Anchor = self;
			Pawn(Incoming).SetMoveTarget(self);
		}
		Incoming.PlayTeleportEffect(false, true);		
	}
	else
	{
		// End:0x287
		if(__NFUN_129__(Incoming.__NFUN_267__(Location)))
		{
			__NFUN_117__('Touch');
			return false;
		}
		// End:0x2A1
		if(bChangesYaw)
		{
			Incoming.__NFUN_299__(newRot);
		}
	}
	__NFUN_117__('Touch');
	// End:0x2C8
	if(bChangesVelocity)
	{
		Incoming.Velocity = TargetVelocity;		
	}
	else
	{
		// End:0x362
		if(bChangesYaw)
		{
			// End:0x2F6
			if(__NFUN_154__(int(Incoming.Physics), int(1)))
			{
				OldRot.Pitch = 0;
			}
			oldDir = Vector(OldRot);
			mag = __NFUN_219__(Incoming.Velocity, oldDir);
			Incoming.Velocity = __NFUN_215__(__NFUN_216__(Incoming.Velocity, __NFUN_213__(mag, oldDir)), __NFUN_213__(mag, Vector(Incoming.Rotation)));
		}
		// End:0x385
		if(bReversesX)
		{
			__NFUN_182__(Incoming.Velocity.X, -1.0000000);
		}
		// End:0x3A8
		if(bReversesY)
		{
			__NFUN_182__(Incoming.Velocity.Y, -1.0000000);
		}
		// End:0x3CB
		if(bReversesZ)
		{
			__NFUN_182__(Incoming.Velocity.Z, -1.0000000);
		}
	}
	return true;
	return;
}

function Trigger(Actor Other, Pawn EventInstigator)
{
	local Actor A;

	bEnabled = __NFUN_129__(bEnabled);
	// End:0x35
	if(bEnabled)
	{
		// End:0x34
		foreach __NFUN_307__(Class'Engine.Actor', A)
		{
			PostTouch(A);			
		}		
	}
	return;
}

event Touch(Actor Other)
{
	// End:0x1A
	if(__NFUN_132__(__NFUN_129__(bEnabled), __NFUN_114__(Other, none)))
	{
		return;
	}
	// End:0x68
	if(__NFUN_130__(Other.bCanTeleport, __NFUN_242__(Other.PreTeleport(self), false)))
	{
		PendingTouch = Other.PendingTouch;
		Other.PendingTouch = self;
	}
	return;
}

simulated function PostTouch(Actor Other)
{
	local Teleporter D, Dest[16];
	local int i;

	// End:0x95
	if(__NFUN_132__(__NFUN_153__(__NFUN_126__(URL, "/"), 0), __NFUN_153__(__NFUN_126__(URL, "#"), 0)))
	{
		// End:0x92
		if(__NFUN_130__(__NFUN_130__(__NFUN_154__(int(Role), int(ROLE_Authority)), __NFUN_119__(Pawn(Other), none)), Pawn(Other).IsHumanControlled()))
		{
			Level.Game.SendPlayer(PlayerController(Pawn(Other).Controller), URL);
		}		
	}
	else
	{
		// End:0xF4
		foreach __NFUN_304__(Class'Engine.Teleporter', D)
		{
			// End:0xF3
			if(__NFUN_130__(__NFUN_124__(string(D.Tag), URL), __NFUN_119__(D, self)))
			{
				Dest[i] = D;
				__NFUN_165__(i);
				// End:0xF3
				if(__NFUN_151__(i, 16))
				{
					// End:0xF4
					break;
				}
			}			
		}		
		i = __NFUN_167__(i);
		// End:0x179
		if(__NFUN_119__(Dest[i], none))
		{
			// End:0x138
			if(Other.__NFUN_303__('Pawn'))
			{
				Other.PlayTeleportEffect(false, true);
			}
			Dest[i].Accept(Other, self);
			// End:0x179
			if(__NFUN_119__(Pawn(Other), none))
			{
				TriggerEvent(Event, self, Pawn(Other));
			}
		}
	}
	return;
}

function Actor SpecialHandling(Pawn Other)
{
	local Vector Dist2D;

	// End:0xF3
	if(__NFUN_130__(__NFUN_130__(bEnabled, __NFUN_119__(Teleporter(Other.Controller.RouteCache[1]), none)), __NFUN_124__(string(Other.Controller.RouteCache[1].Tag), URL)))
	{
		// End:0xF1
		if(__NFUN_176__(__NFUN_186__(__NFUN_175__(Location.Z, Other.Location.Z)), __NFUN_174__(CollisionHeight, Other.CollisionHeight)))
		{
			Dist2D = __NFUN_216__(Location, Other.Location);
			Dist2D.Z = 0.0000000;
			// End:0xF1
			if(__NFUN_176__(__NFUN_225__(Dist2D), __NFUN_174__(CollisionRadius, Other.CollisionRadius)))
			{
				PostTouch(Other);
			}
		}
		return self;
	}
	// End:0x111
	if(__NFUN_114__(TriggerActor, none))
	{
		FindTriggerActor();
		// End:0x111
		if(__NFUN_114__(TriggerActor, none))
		{
			return none;
		}
	}
	// End:0x169
	if(__NFUN_130__(__NFUN_119__(TriggerActor2, none), __NFUN_176__(__NFUN_225__(__NFUN_216__(TriggerActor2.Location, Other.Location)), __NFUN_225__(__NFUN_216__(TriggerActor.Location, Other.Location)))))
	{
		return TriggerActor2;
	}
	return TriggerActor;
	return;
}

defaultproperties
{
	bChangesYaw=true
	bEnabled=true
	RemoteRole=2
	Texture=Texture'Engine.S_Teleport'
	bCollideActors=true
	bDirectional=true
}