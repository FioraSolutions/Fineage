class Mover extends Actor
	native
	nativereplication
	notplaceable;

enum EMoverEncroachType
{
	ME_StopWhenEncroach,            // 0
	ME_ReturnWhenEncroach,          // 1
	ME_CrushWhenEncroach,           // 2
	ME_IgnoreWhenEncroach           // 3
};

enum EMoverGlideType
{
	MV_MoveByTime,                  // 0
	MV_GlideByTime                  // 1
};

enum EBumpType
{
	BT_PlayerBump,                  // 0
	BT_PawnBump,                    // 1
	BT_AnyBump                      // 2
};

var bool bCanBeAttacked;
var int DamageGrade;
var() string MoverName;
var() bool bOpen;
var() StaticMesh MeshGrade[7];
var() float RangeHeight;
var() Vector RangeDelta[4];
var() Vector HitActorNormal;
var(MoverSounds) array<Sound> DamageSounds;
var(MoverSounds) array<Sound> BrokenSounds;
var() Mover.EMoverEncroachType MoverEncroachType;
var() Mover.EMoverGlideType MoverGlideType;
var() Mover.EBumpType BumpType;
var() byte KeyNum;
var byte PrevKeyNum;
var() const byte NumKeys;
var() const byte WorldRaytraceKey;
var() const byte BrushRaytraceKey;
var() float MoveTime;
var() float StayOpenTime;
var() float OtherTime;
var() int EncroachDamage;
var() bool bTriggerOnceOnly;
var() bool bSlave;
var() bool bUseTriggered;
var() bool bDamageTriggered;
var() bool bDynamicLightMover;
var() bool bUseShortestRotation;
var(ReturnGroup) bool bIsLeader;
var() name PlayerBumpEvent;
var() name BumpEvent;
var Actor SavedTrigger;
var() float DamageThreshold;
var int numTriggerEvents;
var Mover Leader;
var Mover Follower;
var(ReturnGroup) name ReturnGroup;
var() float DelayTime;
var(MoverSounds) Sound OpeningSound;
var(MoverSounds) Sound OpenedSound;
var(MoverSounds) Sound ClosingSound;
var(MoverSounds) Sound ClosedSound;
var(MoverSounds) Sound MoveAmbientSound;
var(MoverSounds) Sound LoopSound;
var(MoverEvents) name OpeningEvent;
var(MoverEvents) name OpenedEvent;
var(MoverEvents) name ClosingEvent;
var(MoverEvents) name ClosedEvent;
var(MoverEvents) name LoopEvent;
var Vector KeyPos[8];
var Rotator KeyRot[8];
var Vector BasePos;
var Vector OldPos;
var Vector OldPrePivot;
var Vector SavedPos;
var Rotator BaseRot;
var Rotator OldRot;
var Rotator SavedRot;
var float PhysAlpha;
var float PhysRate;
var NavigationPoint myMarker;
var bool bOpening;
var bool bDelaying;
var bool bClientPause;
var bool bClosed;
var bool bPlayerOnly;
var(ai) bool bAutoDoor;
var(ai) bool bNoAIRelevance;
var Vector SimOldPos;
var int SimOldRotPitch;
var int SimOldRotYaw;
var int SimOldRotRoll;
var Vector SimInterpolate;
var Vector RealPosition;
var Rotator RealRotation;
var int ClientUpdate;
var array<AntiPortalActor> AntiPortals;
var() name AntiPortalTag;

replication
{
	// Pos:0x000
	reliable if(__NFUN_154__(int(Role), int(ROLE_Authority)))
		RealPosition, RealRotation, 
		SimInterpolate, SimOldPos, 
		SimOldRotPitch, SimOldRotRoll, 
		SimOldRotYaw;
}

simulated function StartInterpolation()
{
	__NFUN_113__('None');
	bInterpolating = true;
	__NFUN_3970__(0);
	return;
}

simulated function Timer()
{
	// End:0x21
	if(__NFUN_218__(Velocity, vect(0.0000000, 0.0000000, 0.0000000)))
	{
		bClientPause = false;
		return;
	}
	// End:0xB3
	if(__NFUN_154__(int(Level.NetMode), int(NM_Client)))
	{
		// End:0xA8
		if(__NFUN_154__(ClientUpdate, 0))
		{
			// End:0x8E
			if(bClientPause)
			{
				// End:0x70
				if(__NFUN_177__(__NFUN_225__(__NFUN_216__(RealPosition, Location)), float(3)))
				{
					__NFUN_267__(RealPosition);					
				}
				else
				{
					RealPosition = Location;
				}
				__NFUN_299__(RealRotation);
				bClientPause = false;				
			}
			else
			{
				// End:0xA5
				if(__NFUN_218__(RealPosition, Location))
				{
					bClientPause = true;
				}
			}			
		}
		else
		{
			bClientPause = false;
		}		
	}
	else
	{
		RealPosition = Location;
		RealRotation = Rotation;
	}
	return;
}

final simulated function InterpolateTo(byte NewKeyNum, float Seconds)
{
	NewKeyNum = byte(__NFUN_251__(int(NewKeyNum), 0, __NFUN_147__(8, 1)));
	// End:0x89
	if(__NFUN_130__(__NFUN_154__(int(NewKeyNum), int(PrevKeyNum)), __NFUN_155__(int(KeyNum), int(PrevKeyNum))))
	{
		PhysAlpha = __NFUN_175__(1.0000000, PhysAlpha);
		OldPos = __NFUN_215__(BasePos, KeyPos[int(KeyNum)]);
		OldRot = __NFUN_316__(BaseRot, KeyRot[int(KeyNum)]);		
	}
	else
	{
		OldPos = Location;
		OldRot = Rotation;
		PhysAlpha = 0.0000000;
	}
	__NFUN_3970__(8);
	bInterpolating = true;
	PrevKeyNum = KeyNum;
	KeyNum = NewKeyNum;
	PhysRate = __NFUN_172__(1.0000000, __NFUN_245__(Seconds, 0.0050000));
	__NFUN_165__(ClientUpdate);
	SimOldPos = OldPos;
	SimOldRotYaw = OldRot.Yaw;
	SimOldRotPitch = OldRot.Pitch;
	SimOldRotRoll = OldRot.Roll;
	SimInterpolate.X = __NFUN_171__(100.0000000, PhysAlpha);
	SimInterpolate.Y = __NFUN_171__(100.0000000, __NFUN_245__(0.0100000, PhysRate));
	SimInterpolate.Z = __NFUN_174__(__NFUN_171__(256.0000000, float(PrevKeyNum)), float(KeyNum));
	return;
}

final function SetKeyframe(byte NewKeyNum, Vector NewLocation, Rotator NewRotation)
{
	KeyNum = byte(__NFUN_251__(int(NewKeyNum), 0, __NFUN_147__(8, 1)));
	KeyPos[int(KeyNum)] = NewLocation;
	KeyRot[int(KeyNum)] = NewRotation;
	return;
}

simulated event KeyFrameReached()
{
	local byte OldKeyNum;

	OldKeyNum = PrevKeyNum;
	PrevKeyNum = KeyNum;
	PhysAlpha = 0.0000000;
	__NFUN_166__(ClientUpdate);
	// End:0x64
	if(__NFUN_130__(__NFUN_151__(int(KeyNum), 0), __NFUN_150__(int(KeyNum), int(OldKeyNum))))
	{
		InterpolateTo(byte(__NFUN_147__(int(KeyNum), 1)), MoveTime);		
	}
	else
	{
		// End:0xA9
		if(__NFUN_130__(__NFUN_150__(int(KeyNum), __NFUN_147__(int(NumKeys), 1)), __NFUN_151__(int(KeyNum), int(OldKeyNum))))
		{
			InterpolateTo(byte(__NFUN_146__(int(KeyNum), 1)), MoveTime);			
		}
		else
		{
			// End:0xE5
			if(__NFUN_130__(__NFUN_154__(ClientUpdate, 0), __NFUN_155__(int(Level.NetMode), int(NM_Client))))
			{
				RealPosition = Location;
				RealRotation = Rotation;
			}
		}
	}
	return;
}

function FinishNotify()
{
	local Controller C;

	C = Level.ControllerList;
	J0x14:

	// End:0x6F [Loop If]
	if(__NFUN_119__(C, none))
	{
		// End:0x58
		if(__NFUN_130__(__NFUN_119__(C.Pawn, none), __NFUN_114__(C.PendingMover, self)))
		{
			C.MoverFinished();
		}
		C = C.nextController;
		// [Loop Continue]
		goto J0x14;
	}
	return;
}

function FinishedClosing()
{
	local Mover M;

	__NFUN_264__(ClosedSound, 0);
	TriggerEvent(ClosedEvent, self, Instigator);
	// End:0x35
	if(__NFUN_119__(SavedTrigger, none))
	{
		SavedTrigger.EndEvent();
	}
	SavedTrigger = none;
	Instigator = none;
	// End:0x5D
	if(__NFUN_119__(myMarker, none))
	{
		myMarker.MoverClosed();
	}
	bClosed = true;
	FinishNotify();
	M = Leader;
	J0x76:

	// End:0xAE [Loop If]
	if(__NFUN_119__(M, none))
	{
		// End:0x97
		if(__NFUN_129__(M.bClosed))
		{
			return;
		}
		M = M.Follower;
		// [Loop Continue]
		goto J0x76;
	}
	UntriggerEvent(OpeningEvent, self, Instigator);
	return;
}

function FinishedOpening()
{
	__NFUN_264__(OpenedSound, 0);
	TriggerEvent(Event, self, Instigator);
	TriggerEvent(OpenedEvent, self, Instigator);
	// End:0x46
	if(__NFUN_119__(myMarker, none))
	{
		myMarker.MoverOpened();
	}
	FinishNotify();
	return;
}

function DoOpen()
{
	bOpening = true;
	bDelaying = false;
	InterpolateTo(1, MoveTime);
	__NFUN_512__(1.0000000);
	__NFUN_264__(OpeningSound, 0);
	TriggerEvent(OpeningEvent, self, Instigator);
	// End:0x5A
	if(__NFUN_119__(Follower, none))
	{
		Follower.DoOpen();
	}
	return;
}

function DoClose()
{
	bOpening = false;
	bDelaying = false;
	InterpolateTo(byte(__NFUN_250__(0, __NFUN_147__(int(KeyNum), 1))), MoveTime);
	__NFUN_512__(1.0000000);
	__NFUN_264__(ClosingSound, 0);
	UntriggerEvent(Event, self, Instigator);
	TriggerEvent(ClosingEvent, self, Instigator);
	// End:0x78
	if(__NFUN_119__(Follower, none))
	{
		Follower.DoClose();
	}
	return;
}

simulated function BeginPlay()
{
	local AntiPortalActor AntiPortal;

	// End:0x4B
	if(__NFUN_255__(AntiPortalTag, 'None'))
	{
		// End:0x4A
		foreach __NFUN_304__(Class'Engine.AntiPortalActor', AntiPortal, AntiPortalTag)
		{
			AntiPortals.Length = __NFUN_146__(AntiPortals.Length, 1);
			AntiPortals[__NFUN_147__(AntiPortals.Length, 1)] = AntiPortal;			
		}		
	}
	// End:0xA4
	if(__NFUN_155__(int(Level.NetMode), int(NM_Standalone)))
	{
		// End:0x89
		if(__NFUN_154__(int(Level.NetMode), int(NM_Client)))
		{
			__NFUN_280__(4.0000000, true);			
		}
		else
		{
			__NFUN_280__(1.0000000, true);
		}
		// End:0xA4
		if(__NFUN_150__(int(Role), int(ROLE_Authority)))
		{
			return;
		}
	}
	// End:0xD3
	if(__NFUN_155__(int(Level.NetMode), int(NM_Client)))
	{
		RealPosition = Location;
		RealRotation = Rotation;
	}
	super.BeginPlay();
	KeyNum = byte(__NFUN_251__(int(KeyNum), 0, __NFUN_147__(8, 1)));
	PhysAlpha = 0.0000000;
	__NFUN_266__(__NFUN_216__(__NFUN_215__(BasePos, KeyPos[int(KeyNum)]), Location));
	__NFUN_299__(__NFUN_316__(BaseRot, KeyRot[int(KeyNum)]));
	// End:0x14B
	if(__NFUN_254__(ReturnGroup, 'None'))
	{
		ReturnGroup = Tag;
	}
	Leader = none;
	Follower = none;
	InitialState = 'TriggerControl';
	return;
}

function PostBeginPlay()
{
	local Mover M;

	// End:0x51
	if(__NFUN_129__(bSlave))
	{
		// End:0x50
		foreach __NFUN_313__(Class'Engine.Mover', M, Tag)
		{
			// End:0x4F
			if(M.bSlave)
			{
				M.__NFUN_113__('None');
				M.__NFUN_298__(self);
			}			
		}		
	}
	// End:0xCA
	if(bIsLeader)
	{
		Leader = self;
		// End:0xC6
		foreach __NFUN_313__(Class'Engine.Mover', M)
		{
			// End:0xC5
			if(__NFUN_130__(__NFUN_119__(M, self), __NFUN_254__(M.ReturnGroup, ReturnGroup)))
			{
				M.Leader = self;
				M.Follower = Follower;
				Follower = M;
			}			
		}				
	}
	else
	{
		// End:0x116
		if(__NFUN_114__(Leader, none))
		{
			// End:0x10E
			foreach __NFUN_313__(Class'Engine.Mover', M)
			{
				// End:0x10D
				if(__NFUN_130__(__NFUN_119__(M, self), __NFUN_254__(M.ReturnGroup, ReturnGroup)))
				{					
					return;
				}				
			}			
			Leader = self;
		}
	}
	return;
}

function MakeGroupStop()
{
	bInterpolating = false;
	__NFUN_113__(, 'None');
	// End:0x2A
	if(__NFUN_119__(Follower, none))
	{
		Follower.MakeGroupStop();
	}
	return;
}

function MakeGroupReturn()
{
	bInterpolating = false;
	// End:0x44
	if(__NFUN_132__(bIsLeader, __NFUN_114__(Leader, self)))
	{
		// End:0x3C
		if(__NFUN_150__(int(KeyNum), int(PrevKeyNum)))
		{
			__NFUN_113__(, 'Open');			
		}
		else
		{
			__NFUN_113__(, 'Close');
		}
	}
	// End:0x5E
	if(__NFUN_119__(Follower, none))
	{
		Follower.MakeGroupReturn();
	}
	return;
}

function bool EncroachingOn(Actor Other)
{
	local Pawn P;

	// End:0x0D
	if(__NFUN_114__(Other, none))
	{
		return false;
	}
	// End:0x85
	if(__NFUN_132__(__NFUN_130__(__NFUN_119__(Pawn(Other), none), __NFUN_114__(Pawn(Other).Controller, none)), Other.__NFUN_303__('Decoration')))
	{
		Other.TakeDamage(10000, none, Other.Location, vect(0.0000000, 0.0000000, 0.0000000), Class'Engine.Crushed');
		return false;
	}
	// End:0xD1
	if(Other.__NFUN_303__('Pickup'))
	{
		// End:0xCF
		if(__NFUN_130__(__NFUN_129__(Other.bAlwaysRelevant), __NFUN_114__(Other.Owner, none)))
		{
			Other.__NFUN_279__();
		}
		return false;
	}
	// End:0x11F
	if(__NFUN_132__(__NFUN_132__(Other.__NFUN_303__('Fragment'), Other.__NFUN_303__('Gib')), Other.__NFUN_303__('Projectile')))
	{
		Other.__NFUN_279__();
		return false;
	}
	// End:0x163
	if(__NFUN_155__(EncroachDamage, 0))
	{
		Other.TakeDamage(EncroachDamage, Instigator, Other.Location, vect(0.0000000, 0.0000000, 0.0000000), Class'Engine.Crushed');
	}
	P = Pawn(Other);
	// End:0x231
	if(__NFUN_130__(__NFUN_130__(__NFUN_119__(P, none), __NFUN_119__(P.Controller, none)), P.IsPlayerPawn()))
	{
		// End:0x1C2
		if(__NFUN_255__(PlayerBumpEvent, 'None'))
		{
			Bump(Other);
		}
		// End:0x231
		if(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_119__(P, none), __NFUN_119__(P.Controller, none)), __NFUN_119__(P.Base, self)), __NFUN_114__(P.Controller.PendingMover, self)))
		{
			P.Controller.UnderLift(self);
		}
	}
	return false;
	return;
}

function Bump(Actor Other)
{
	local Pawn P;

	P = Pawn(Other);
	// End:0x79
	if(__NFUN_130__(__NFUN_130__(__NFUN_130__(bUseTriggered, __NFUN_119__(P, none)), __NFUN_129__(P.IsHumanControlled())), P.IsPlayerPawn()))
	{
		Trigger(P, P);
		P.Controller.WaitForMover(self);
	}
	// End:0x98
	if(__NFUN_130__(__NFUN_155__(int(BumpType), int(2)), __NFUN_114__(P, none)))
	{
		return;
	}
	TriggerEvent(BumpEvent, self, P);
	// End:0xD9
	if(__NFUN_130__(__NFUN_119__(P, none), P.IsPlayerPawn()))
	{
		TriggerEvent(PlayerBumpEvent, self, P);
	}
	return;
}

function TakeDamage(int Damage, Pawn instigatedBy, Vector HitLocation, Vector Momentum, Class<DamageType> DamageType)
{
	// End:0x7D
	if(__NFUN_130__(bDamageTriggered, __NFUN_179__(float(Damage), DamageThreshold)))
	{
		// End:0x6C
		if(__NFUN_130__(__NFUN_119__(AIController(instigatedBy.Controller), none), __NFUN_114__(instigatedBy.Controller.Focus, self)))
		{
			instigatedBy.Controller.StopFiring();
		}
		self.Trigger(self, instigatedBy);
	}
	return;
}

event GetEffTargetLocation(out Vector LocVector)
{
	// End:0x93
	if(__NFUN_130__(__NFUN_130__(__NFUN_180__(LocVector.X, float(0)), __NFUN_180__(LocVector.Y, float(0))), __NFUN_180__(LocVector.Z, float(0))))
	{
		LocVector = __NFUN_215__(Location, __NFUN_214__(__NFUN_215__(__NFUN_215__(__NFUN_215__(RangeDelta[0], RangeDelta[1]), RangeDelta[2]), RangeDelta[3]), float(4)));
		LocVector.Z = __NFUN_174__(LocVector.Z, __NFUN_172__(RangeHeight, float(2)));
	}
	return;
}

state OpenTimedMover
{Open:

	bClosed = false;
	DisableTrigger();
	// End:0x2B
	if(__NFUN_177__(DelayTime, float(0)))
	{
		bDelaying = true;
		__NFUN_256__(DelayTime);
	}
	DoOpen();
	__NFUN_301__();
	FinishedOpening();
	__NFUN_256__(StayOpenTime);
	// End:0x52
	if(bTriggerOnceOnly)
	{
		__NFUN_113__('None');
	}
Close:


	DoClose();
	__NFUN_301__();
	FinishedClosing();
	EnableTrigger();
	stop;				
}

state() StandOpenTimed extends OpenTimedMover
{
	function Attach(Actor Other)
	{
		local Pawn P;

		P = Pawn(Other);
		// End:0x2F
		if(__NFUN_130__(__NFUN_155__(int(BumpType), int(2)), __NFUN_114__(P, none)))
		{
			return;
		}
		SavedTrigger = none;
		__NFUN_113__('StandOpenTimed', 'Open');
		return;
	}

	function DisableTrigger()
	{
		__NFUN_118__('Attach');
		return;
	}

	function EnableTrigger()
	{
		__NFUN_117__('Attach');
		return;
	}
	stop;
}

state() BumpOpenTimed extends OpenTimedMover
{
	function Bump(Actor Other)
	{
		// End:0x24
		if(__NFUN_130__(__NFUN_155__(int(BumpType), int(2)), __NFUN_114__(Pawn(Other), none)))
		{
			return;
		}
		global.Bump(Other);
		SavedTrigger = none;
		Instigator = Pawn(Other);
		Instigator.Controller.WaitForMover(self);
		__NFUN_113__('BumpOpenTimed', 'Open');
		return;
	}

	function DisableTrigger()
	{
		__NFUN_118__('Bump');
		return;
	}

	function EnableTrigger()
	{
		__NFUN_117__('Bump');
		return;
	}
	stop;
}

state() TriggerOpenTimed extends OpenTimedMover
{
	function Trigger(Actor Other, Pawn EventInstigator)
	{
		SavedTrigger = Other;
		Instigator = EventInstigator;
		// End:0x30
		if(__NFUN_119__(SavedTrigger, none))
		{
			SavedTrigger.BeginEvent();
		}
		__NFUN_113__('TriggerOpenTimed', 'Open');
		return;
	}

	function DisableTrigger()
	{
		__NFUN_118__('Trigger');
		return;
	}

	function EnableTrigger()
	{
		__NFUN_117__('Trigger');
		return;
	}
	stop;
}

state() TriggerToggle
{
	function Trigger(Actor Other, Pawn EventInstigator)
	{
		SavedTrigger = Other;
		Instigator = EventInstigator;
		// End:0x30
		if(__NFUN_119__(SavedTrigger, none))
		{
			SavedTrigger.BeginEvent();
		}
		// End:0x61
		if(__NFUN_132__(__NFUN_154__(int(KeyNum), 0), __NFUN_150__(int(KeyNum), int(PrevKeyNum))))
		{
			__NFUN_113__('TriggerToggle', 'Open');			
		}
		else
		{
			__NFUN_113__('TriggerToggle', 'Close');
		}
		return;
	}
Open:

	bClosed = false;
	// End:0x25
	if(__NFUN_177__(DelayTime, float(0)))
	{
		bDelaying = true;
		__NFUN_256__(DelayTime);
	}
	DoOpen();
	__NFUN_301__();
	FinishedOpening();
	// End:0x4E
	if(__NFUN_119__(SavedTrigger, none))
	{
		SavedTrigger.EndEvent();
	}
	stop;
Close:


	DoClose();
	__NFUN_301__();
	FinishedClosing();
	stop;	
}

state() TriggerControl
{
	function Trigger(Actor Other, Pawn EventInstigator)
	{
		__NFUN_165__(numTriggerEvents);
		SavedTrigger = Other;
		Instigator = EventInstigator;
		// End:0x37
		if(__NFUN_119__(SavedTrigger, none))
		{
			SavedTrigger.BeginEvent();
		}
		__NFUN_113__('TriggerControl', 'Open');
		return;
	}

	function UnTrigger(Actor Other, Pawn EventInstigator)
	{
		__NFUN_166__(numTriggerEvents);
		// End:0x4A
		if(__NFUN_152__(numTriggerEvents, 0))
		{
			numTriggerEvents = 0;
			SavedTrigger = Other;
			Instigator = EventInstigator;
			SavedTrigger.BeginEvent();
			__NFUN_113__('TriggerControl', 'Close');
		}
		return;
	}

	function BeginState()
	{
		numTriggerEvents = 0;
		return;
	}
Open:

	bClosed = false;
	// End:0x25
	if(__NFUN_177__(DelayTime, float(0)))
	{
		bDelaying = true;
		__NFUN_256__(DelayTime);
	}
	DoOpen();
	__NFUN_301__();
	FinishedOpening();
	SavedTrigger.EndEvent();
	// End:0x53
	if(bTriggerOnceOnly)
	{
		__NFUN_113__('None');
	}
	stop;
Close:


	DoClose();
	__NFUN_301__();
	FinishedClosing();
	stop;				
}

state() TriggerPound
{
	function Trigger(Actor Other, Pawn EventInstigator)
	{
		__NFUN_165__(numTriggerEvents);
		SavedTrigger = Other;
		Instigator = EventInstigator;
		__NFUN_113__('TriggerPound', 'Open');
		return;
	}

	function UnTrigger(Actor Other, Pawn EventInstigator)
	{
		__NFUN_166__(numTriggerEvents);
		// End:0x33
		if(__NFUN_152__(numTriggerEvents, 0))
		{
			numTriggerEvents = 0;
			SavedTrigger = none;
			Instigator = none;
			__NFUN_113__('TriggerPound', 'Close');
		}
		return;
	}

	function BeginState()
	{
		numTriggerEvents = 0;
		return;
	}
Open:

	bClosed = false;
	// End:0x25
	if(__NFUN_177__(DelayTime, float(0)))
	{
		bDelaying = true;
		__NFUN_256__(DelayTime);
	}
	DoOpen();
	__NFUN_301__();
	__NFUN_256__(OtherTime);
Close:


	DoClose();
	__NFUN_301__();
	__NFUN_256__(StayOpenTime);
	// End:0x57
	if(bTriggerOnceOnly)
	{
		__NFUN_113__('None');
	}
	// End:0x68
	if(__NFUN_119__(SavedTrigger, none))
	{
		goto 'Open';
	}
	stop;			
}

state() BumpButton
{
	function Bump(Actor Other)
	{
		// End:0x24
		if(__NFUN_130__(__NFUN_155__(int(BumpType), int(2)), __NFUN_114__(Pawn(Other), none)))
		{
			return;
		}
		global.Bump(Other);
		SavedTrigger = Other;
		Instigator = Pawn(Other);
		Instigator.Controller.WaitForMover(self);
		__NFUN_113__('BumpButton', 'Open');
		return;
	}

	function BeginEvent()
	{
		bSlave = true;
		return;
	}

	function EndEvent()
	{
		bSlave = false;
		Instigator = none;
		__NFUN_113__('BumpButton', 'Close');
		return;
	}
Open:

	bClosed = false;
	__NFUN_118__('Bump');
	// End:0x2C
	if(__NFUN_177__(DelayTime, float(0)))
	{
		bDelaying = true;
		__NFUN_256__(DelayTime);
	}
	DoOpen();
	__NFUN_301__();
	FinishedOpening();
	// End:0x4B
	if(bTriggerOnceOnly)
	{
		__NFUN_113__('None');
	}
	// End:0x55
	if(bSlave)
	{
		stop;
	}
Close:


	DoClose();
	__NFUN_301__();
	FinishedClosing();
	__NFUN_117__('Bump');
	stop;				
}

defaultproperties
{
	RangeHeight=150.0000000
	RangeDelta[0]=(X=-80.0000000,Y=-80.0000000,Z=0.0000000)
	RangeDelta[1]=(X=80.0000000,Y=-80.0000000,Z=0.0000000)
	RangeDelta[2]=(X=80.0000000,Y=80.0000000,Z=0.0000000)
	RangeDelta[3]=(X=-80.0000000,Y=80.0000000,Z=0.0000000)
	MoverEncroachType=1
	MoverGlideType=1
	NumKeys=2
	MoveTime=1.0000000
	StayOpenTime=4.0000000
	bClosed=true
	Physics=8
	bNoDelete=true
	bAlwaysRelevant=true
	bOnlyDirtyReplication=true
	RemoteRole=2
	NetPriority=2.7000000
	InitialState="BumpOpenTimed"
	bShadowCast=true
	TransientSoundVolume=1.0000000
	CollisionRadius=160.0000000
	CollisionHeight=160.0000000
	bCollideActors=true
	bBlockActors=true
	bBlockPlayers=true
	bEdShouldSnap=true
	bPathColliding=true
}