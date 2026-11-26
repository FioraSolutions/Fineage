class Door extends NavigationPoint
	native
	placeable
	hidecategories(Lighting,LightColor,Karma,Force);

var() name DoorTag;
var Mover MyDoor;
var() name DoorTrigger;
var Actor RecommendedTrigger;
var() bool bInitiallyClosed;
var() bool bBlockedWhenClosed;
var bool bDoorOpen;
var bool bTempNoCollide;

function PostBeginPlay()
{
	local Vector dist;

	// End:0x9F
	if(__NFUN_255__(DoorTrigger, 'None'))
	{
		// End:0x28
		foreach __NFUN_304__(Class'Engine.Actor', RecommendedTrigger, DoorTrigger)
		{
			// End:0x28
			break;			
		}		
		// End:0x9F
		if(__NFUN_119__(RecommendedTrigger, none))
		{
			dist = __NFUN_216__(Location, RecommendedTrigger.Location);
			// End:0x9F
			if(__NFUN_176__(__NFUN_186__(dist.Z), RecommendedTrigger.CollisionHeight))
			{
				dist.Z = 0.0000000;
				// End:0x9F
				if(__NFUN_176__(__NFUN_225__(dist), RecommendedTrigger.CollisionRadius))
				{
					RecommendedTrigger = none;
				}
			}
		}
	}
	bBlocked = __NFUN_130__(bInitiallyClosed, bBlockedWhenClosed);
	bDoorOpen = __NFUN_129__(bInitiallyClosed);
	super.PostBeginPlay();
	return;
}

function MoverOpened()
{
	bBlocked = __NFUN_130__(__NFUN_129__(bInitiallyClosed), bBlockedWhenClosed);
	bDoorOpen = bInitiallyClosed;
	return;
}

function MoverClosed()
{
	bBlocked = __NFUN_130__(bInitiallyClosed, bBlockedWhenClosed);
	bDoorOpen = __NFUN_129__(bInitiallyClosed);
	return;
}

function Actor SpecialHandling(Pawn Other)
{
	// End:0x0D
	if(__NFUN_114__(MyDoor, none))
	{
		return self;
	}
	// End:0x3E
	if(__NFUN_130__(__NFUN_154__(int(MyDoor.BumpType), int(0)), __NFUN_129__(Other.IsPlayerPawn())))
	{
		return none;
	}
	// End:0x79
	if(__NFUN_242__(bInitiallyClosed, __NFUN_132__(__NFUN_132__(bDoorOpen, MyDoor.bOpening), MyDoor.bDelaying)))
	{
		return self;
	}
	// End:0x8A
	if(__NFUN_119__(RecommendedTrigger, none))
	{
		return RecommendedTrigger;
	}
	return self;
	return;
}

function bool ProceedWithMove(Pawn Other)
{
	// End:0x4D
	if(__NFUN_130__(MyDoor.bDamageTriggered, __NFUN_114__(Other.Controller.Focus, MyDoor)))
	{
		Other.Controller.StopFiring();
	}
	// End:0x6E
	if(__NFUN_132__(bDoorOpen, __NFUN_129__(MyDoor.bDamageTriggered)))
	{
		return true;
	}
	Other.__NFUN_299__(Rotator(__NFUN_216__(MyDoor.Location, Other.Location)));
	Other.Controller.Focus = MyDoor;
	Other.ShootSpecial(MyDoor);
	MyDoor.Trigger(Other, Other);
	Other.Controller.WaitForMover(MyDoor);
	return false;
	return;
}

event bool SuggestMovePreparation(Pawn Other)
{
	// End:0x0B
	if(bDoorOpen)
	{
		return false;
	}
	// End:0x50
	if(__NFUN_132__(MyDoor.bOpening, MyDoor.bDelaying))
	{
		Other.Controller.WaitForMover(MyDoor);
		return true;
	}
	// End:0xF7
	if(MyDoor.bDamageTriggered)
	{
		Other.__NFUN_299__(Rotator(__NFUN_216__(MyDoor.Location, Other.Location)));
		Other.Controller.Focus = MyDoor;
		Other.ShootSpecial(MyDoor);
		MyDoor.Trigger(Other, Other);
		Other.Controller.WaitForMover(MyDoor);
		return true;
	}
	return false;
	return;
}

defaultproperties
{
	bInitiallyClosed=true
	ExtraCost=100
	bSpecialMove=true
	RemoteRole=0
	Texture=Texture'Engine.S_Door'
}