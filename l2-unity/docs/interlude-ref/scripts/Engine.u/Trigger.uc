class Trigger extends Triggers
	native
	placeable;

enum ETriggerType
{
	TT_PlayerProximity,             // 0
	TT_PawnProximity,               // 1
	TT_ClassProximity,              // 2
	TT_AnyProximity,                // 3
	TT_Shoot,                       // 4
	TT_HumanPlayerProximity         // 5
};

var() Trigger.ETriggerType TriggerType;
var() localized string Message;
var() bool bTriggerOnceOnly;
var() bool bInitiallyActive;
var() Class<Actor> ClassProximityType;
var() float RepeatTriggerTime;
var() float ReTriggerDelay;
var float TriggerTime;
var() float DamageThreshold;
var Actor TriggerActor;
var Actor TriggerActor2;
var bool bSavedInitialCollision;
var bool bSavedInitialActive;

function PreBeginPlay()
{
	super(Actor).PreBeginPlay();
	// End:0x65
	if(__NFUN_132__(__NFUN_132__(__NFUN_132__(__NFUN_154__(int(TriggerType), int(0)), __NFUN_154__(int(TriggerType), int(1))), __NFUN_154__(int(TriggerType), int(5))), __NFUN_130__(__NFUN_154__(int(TriggerType), int(2)), __NFUN_258__(ClassProximityType, Class'Engine.Pawn'))))
	{
		OnlyAffectPawns(true);
	}
	return;
}

function PostBeginPlay()
{
	// End:0x11
	if(__NFUN_129__(bInitiallyActive))
	{
		FindTriggerActor();
	}
	// End:0x39
	if(__NFUN_154__(int(TriggerType), int(4)))
	{
		bHidden = false;
		bProjTarget = true;
		SetDrawType(0);
	}
	bSavedInitialActive = bInitiallyActive;
	bSavedInitialCollision = bCollideActors;
	super(Actor).PostBeginPlay();
	return;
}

function Reset()
{
	super(Actor).Reset();
	bInitiallyActive = bSavedInitialActive;
	__NFUN_262__(bSavedInitialCollision, bBlockActors, bBlockPlayers);
	return;
}

function FindTriggerActor()
{
	local Actor A;

	TriggerActor = none;
	TriggerActor2 = none;
	// End:0x5E
	foreach __NFUN_304__(Class'Engine.Actor', A)
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

function Actor SpecialHandling(Pawn Other)
{
	local Actor A;

	// End:0x18
	if(__NFUN_130__(bTriggerOnceOnly, __NFUN_129__(bCollideActors)))
	{
		return none;
	}
	// End:0x40
	if(__NFUN_130__(__NFUN_154__(int(TriggerType), int(5)), __NFUN_129__(Other.IsHumanControlled())))
	{
		return none;
	}
	// End:0x68
	if(__NFUN_130__(__NFUN_154__(int(TriggerType), int(0)), __NFUN_129__(Other.IsPlayerPawn())))
	{
		return none;
	}
	// End:0xF2
	if(__NFUN_129__(bInitiallyActive))
	{
		// End:0x84
		if(__NFUN_114__(TriggerActor, none))
		{
			FindTriggerActor();
		}
		// End:0x91
		if(__NFUN_114__(TriggerActor, none))
		{
			return none;
		}
		// End:0xEC
		if(__NFUN_130__(__NFUN_119__(TriggerActor2, none), __NFUN_176__(__NFUN_225__(__NFUN_216__(TriggerActor2.Location, Other.Location)), __NFUN_225__(__NFUN_216__(TriggerActor.Location, Other.Location)))))
		{
			return TriggerActor2;			
		}
		else
		{
			return TriggerActor;
		}
	}
	// End:0x113
	if(__NFUN_154__(int(TriggerType), int(4)))
	{
		return Other.ShootSpecial(self);
	}
	// End:0x14F
	if(IsRelevant(Other))
	{
		// End:0x14C
		foreach __NFUN_307__(Class'Engine.Actor', A)
		{
			// End:0x14B
			if(__NFUN_114__(A, Other))
			{
				Touch(Other);
			}			
		}		
		return self;
	}
	return self;
	return;
}

function CheckTouchList()
{
	local Actor A;

	// End:0x1C
	foreach __NFUN_307__(Class'Engine.Actor', A)
	{
		Touch(A);		
	}	
	return;
}

function bool IsRelevant(Actor Other)
{
	// End:0x0D
	if(__NFUN_129__(bInitiallyActive))
	{
		return false;
	}
	switch(TriggerType)
	{
		// End:0x40
		case 5:
			return __NFUN_130__(__NFUN_119__(Pawn(Other), none), Pawn(Other).IsHumanControlled());
		// End:0x85
		case 0:
			return __NFUN_130__(__NFUN_119__(Pawn(Other), none), __NFUN_132__(Pawn(Other).IsPlayerPawn(), Pawn(Other).WasPlayerPawn()));
		// End:0xB2
		case 1:
			return __NFUN_130__(__NFUN_119__(Pawn(Other), none), Pawn(Other).CanTrigger(self));
		// End:0xCE
		case 2:
			return __NFUN_258__(Other.Class, ClassProximityType);
		// End:0xD5
		case 3:
			return true;
		// End:0x107
		case 4:
			return __NFUN_130__(__NFUN_119__(Projectile(Other), none), __NFUN_179__(Projectile(Other).Damage, DamageThreshold));
		// End:0xFFFF
		default:
			return;
			break;
	}
}

function Touch(Actor Other)
{
	local int i;

	// End:0x166
	if(IsRelevant(Other))
	{
		// End:0x50
		if(__NFUN_177__(ReTriggerDelay, float(0)))
		{
			// End:0x3C
			if(__NFUN_176__(__NFUN_175__(Level.TimeSeconds, TriggerTime), ReTriggerDelay))
			{
				return;
			}
			TriggerTime = Level.TimeSeconds;
		}
		TriggerEvent(Event, self, Other.Instigator);
		// End:0x101
		if(__NFUN_130__(__NFUN_119__(Pawn(Other), none), __NFUN_119__(Pawn(Other).Controller, none)))
		{
			i = 0;
			J0x9C:

			// End:0x101 [Loop If]
			if(__NFUN_150__(i, 4))
			{
				// End:0xF7
				if(__NFUN_114__(Pawn(Other).Controller.GoalList[i], self))
				{
					Pawn(Other).Controller.GoalList[i] = none;
					// [Explicit Break]
					goto J0x101;
				}
				__NFUN_165__(i);
				// [Loop Continue]
				goto J0x9C;
			}
		}
		J0x101:

		// End:0x140
		if(__NFUN_130__(__NFUN_123__(Message, ""), __NFUN_119__(Other.Instigator, none)))
		{
			Other.Instigator.ClientMessage(Message);
		}
		// End:0x150
		if(bTriggerOnceOnly)
		{
			__NFUN_262__(false);			
		}
		else
		{
			// End:0x166
			if(__NFUN_177__(RepeatTriggerTime, float(0)))
			{
				__NFUN_280__(RepeatTriggerTime, false);
			}
		}
	}
	return;
}

function Timer()
{
	local bool bKeepTiming;
	local Actor A;

	bKeepTiming = false;
	// End:0x3A
	foreach __NFUN_307__(Class'Engine.Actor', A)
	{
		// End:0x39
		if(IsRelevant(A))
		{
			bKeepTiming = true;
			Touch(A);
		}		
	}	
	// End:0x4D
	if(bKeepTiming)
	{
		__NFUN_280__(RepeatTriggerTime, false);
	}
	return;
}

function TakeDamage(int Damage, Pawn instigatedBy, Vector HitLocation, Vector Momentum, Class<DamageType> DamageType)
{
	// End:0xC4
	if(__NFUN_130__(__NFUN_130__(__NFUN_130__(bInitiallyActive, __NFUN_154__(int(TriggerType), int(4))), __NFUN_179__(float(Damage), DamageThreshold)), __NFUN_119__(instigatedBy, none)))
	{
		// End:0x7D
		if(__NFUN_177__(ReTriggerDelay, float(0)))
		{
			// End:0x69
			if(__NFUN_176__(__NFUN_175__(Level.TimeSeconds, TriggerTime), ReTriggerDelay))
			{
				return;
			}
			TriggerTime = Level.TimeSeconds;
		}
		TriggerEvent(Event, self, instigatedBy);
		// End:0xB7
		if(__NFUN_123__(Message, ""))
		{
			instigatedBy.Instigator.ClientMessage(Message);
		}
		// End:0xC4
		if(bTriggerOnceOnly)
		{
			__NFUN_262__(false);
		}
	}
	return;
}

function UnTouch(Actor Other)
{
	// End:0x28
	if(IsRelevant(Other))
	{
		UntriggerEvent(Event, self, Other.Instigator);
	}
	return;
}

state() NormalTrigger
{	stop;
}

state() OtherTriggerToggles
{
	function Trigger(Actor Other, Pawn EventInstigator)
	{
		bInitiallyActive = __NFUN_129__(bInitiallyActive);
		// End:0x1E
		if(bInitiallyActive)
		{
			CheckTouchList();
		}
		return;
	}
	stop;
}

state() OtherTriggerTurnsOn
{
	function Trigger(Actor Other, Pawn EventInstigator)
	{
		local bool bWasActive;

		bWasActive = bInitiallyActive;
		bInitiallyActive = true;
		// End:0x26
		if(__NFUN_129__(bWasActive))
		{
			CheckTouchList();
		}
		return;
	}
	stop;
}

state() OtherTriggerTurnsOff
{
	function Trigger(Actor Other, Pawn EventInstigator)
	{
		bInitiallyActive = false;
		return;
	}
	stop;
}

defaultproperties
{
	bInitiallyActive=true
	InitialState="NormalTrigger"
	Texture=Texture'Engine.S_Trigger'
}