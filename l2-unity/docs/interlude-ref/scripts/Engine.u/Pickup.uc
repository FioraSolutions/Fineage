class Pickup extends Actor
	abstract
	native
	nativereplication
	placeable;

var float MaxDesireability;
var InventorySpot myMarker;
var NavigationPoint PickupCache;
var() Class<Inventory> InventoryType;
var() bool bInstantRespawn;
var bool bOnlyReplicateHidden;
var(Display) bool bAmbientGlow;
var bool bDropped;
var bool bPredictRespawns;
var() float RespawnTime;
var float RespawnEffectTime;
var() localized string PickupMessage;
var() Sound PickupSound;
var() string PickupForce;

// Export UPickup::execAddToNavigation(FFrame&, void* const)
native final function AddToNavigation();

// Export UPickup::execRemoveFromNavigation(FFrame&, void* const)
native final function RemoveFromNavigation();

static function StaticPrecache(LevelInfo L)
{
	return;
}

function Destroyed()
{
	// End:0x1B
	if(__NFUN_119__(myMarker, none))
	{
		myMarker.markedItem = none;
	}
	// End:0x32
	if(__NFUN_119__(Inventory, none))
	{
		Inventory.__NFUN_279__();
	}
	return;
}

function Reset()
{
	// End:0x11
	if(__NFUN_119__(Inventory, none))
	{
		__NFUN_279__();		
	}
	else
	{
		__NFUN_113__('Pickup');
		super.Reset();
	}
	return;
}

function RespawnEffect()
{
	return;
}

function Pickup Transmogrify(Class<Pickup> NewClass)
{
	local Pickup NewPickup;

	NewPickup = __NFUN_278__(NewClass);
	NewPickup.RespawnTime = RespawnTime;
	// End:0x5C
	if(__NFUN_119__(myMarker, none))
	{
		myMarker.markedItem = NewPickup;
		NewPickup.myMarker = myMarker;
		myMarker = none;
	}
	__NFUN_279__();
	return NewPickup;
	return;
}

function float DetourWeight(Pawn Other, float PathWeight)
{
	return 0.0000000;
	return;
}

function float BotDesireability(Pawn Bot)
{
	local Inventory AlreadyHas;
	local float desire;

	desire = MaxDesireability;
	// End:0x9A
	if(__NFUN_176__(RespawnTime, float(10)))
	{
		AlreadyHas = Bot.FindInventoryType(InventoryType);
		// End:0x9A
		if(__NFUN_119__(AlreadyHas, none))
		{
			// End:0x73
			if(__NFUN_119__(Inventory, none))
			{
				// End:0x70
				if(__NFUN_152__(Inventory.Charge, AlreadyHas.Charge))
				{
					return -1.0000000;
				}				
			}
			else
			{
				// End:0x9A
				if(__NFUN_152__(InventoryType.default.Charge, AlreadyHas.Charge))
				{
					return -1.0000000;
				}
			}
		}
	}
	return desire;
	return;
}

function Inventory SpawnCopy(Pawn Other)
{
	local Inventory Copy;

	// End:0x20
	if(__NFUN_119__(Inventory, none))
	{
		Copy = Inventory;
		Inventory = none;		
	}
	else
	{
		Copy = __NFUN_278__(InventoryType, Other,,, rot(0, 0, 0));
	}
	Copy.GiveTo(Other);
	return Copy;
	return;
}

function StartSleeping()
{
	// End:0x0F
	if(bDropped)
	{
		__NFUN_279__();		
	}
	else
	{
		__NFUN_113__('Sleeping');
	}
	return;
}

function AnnouncePickup(Pawn Receiver)
{
	Receiver.HandlePickup(self);
	__NFUN_264__(PickupSound, 3);
	return;
}

function SetRespawn()
{
	// End:0x25
	if(Level.Game.ShouldRespawn(self))
	{
		StartSleeping();		
	}
	else
	{
		__NFUN_279__();
	}
	return;
}

static function string GetLocalString(optional int Switch, optional PlayerReplicationInfo RelatedPRI_1, optional PlayerReplicationInfo RelatedPRI_2)
{
	return default.PickupMessage;
	return;
}

function InitDroppedPickupFor(Inventory Inv)
{
	__NFUN_3970__(2);
	__NFUN_113__('FallingPickup');
	Inventory = Inv;
	bAlwaysRelevant = false;
	bOnlyReplicateHidden = false;
	bUpdateSimulatedPosition = true;
	bDropped = true;
	LifeSpan = 16.0000000;
	bIgnoreEncroachers = false;
	return;
}

function bool ReadyToPickup(float MaxWait)
{
	return false;
	return;
}

event Landed(Vector HitNormal)
{
	__NFUN_113__('Pickup');
	return;
}

function float GetRespawnTime()
{
	return RespawnTime;
	return;
}

state FallingPickup
{
	function Timer()
	{
		__NFUN_113__('FadeOut');
		return;
	}

	function BeginState()
	{
		__NFUN_280__(8.0000000, false);
		return;
	}
	stop;
}

auto state Pickup
{
	function bool ReadyToPickup(float MaxWait)
	{
		return true;
		return;
	}

	function bool ValidTouch(Actor Other)
	{
		// End:0x48
		if(__NFUN_132__(__NFUN_132__(__NFUN_114__(Pawn(Other), none), __NFUN_129__(Pawn(Other).bCanPickupInventory)), __NFUN_152__(Pawn(Other).Health, 0)))
		{
			return false;
		}
		// End:0x65
		if(__NFUN_129__(__NFUN_548__(Other.Location, Location)))
		{
			return false;
		}
		// End:0xA3
		if(Level.Game.PickupQuery(Pawn(Other), self))
		{
			TriggerEvent(Event, self, Pawn(Other));
			return true;
		}
		return false;
		return;
	}

	function Touch(Actor Other)
	{
		local Inventory Copy;

		// End:0x53
		if(ValidTouch(Other))
		{
			Copy = SpawnCopy(Pawn(Other));
			AnnouncePickup(Pawn(Other));
			SetRespawn();
			Copy.PickupFunction(Pawn(Other));
		}
		return;
	}

	function CheckTouching()
	{
		local Pawn P;

		// End:0x1C
		foreach __NFUN_307__(Class'Engine.Pawn', P)
		{
			Touch(P);			
		}		
		return;
	}

	function Timer()
	{
		// End:0x10
		if(bDropped)
		{
			__NFUN_113__('FadeOut');
		}
		return;
	}

	function BeginState()
	{
		// End:0x18
		if(bDropped)
		{
			AddToNavigation();
			__NFUN_280__(8.0000000, false);
		}
		return;
	}

	function EndState()
	{
		// End:0x0F
		if(bDropped)
		{
			RemoveFromNavigation();
		}
		return;
	}
Begin:

	CheckTouching();
	stop;	
}

state FadeOut extends Pickup
{
	function Tick(float DeltaTime)
	{
		SetDrawScale(__NFUN_245__(0.0100000, __NFUN_175__(DrawScale, __NFUN_171__(default.DrawScale, DeltaTime))));
		return;
	}

	function BeginState()
	{
		RotationRate.Yaw = 60000;
		__NFUN_3970__(5);
		LifeSpan = 1.0000000;
		return;
	}

	function EndState()
	{
		LifeSpan = 0.0000000;
		SetDrawScale(default.DrawScale);
		// End:0x2B
		if(__NFUN_154__(int(Physics), int(5)))
		{
			__NFUN_3970__(0);
		}
		return;
	}
	stop;
}

state Sleeping
{
	function bool ReadyToPickup(float MaxWait)
	{
		return __NFUN_130__(bPredictRespawns, __NFUN_176__(LatentFloat, MaxWait));
		return;
	}

	function StartSleeping()
	{
		return;
	}

	function BeginState()
	{
		bHidden = true;
		return;
	}

	function EndState()
	{
		bHidden = false;
		return;
	}
Begin:

	__NFUN_256__(__NFUN_175__(GetRespawnTime(), RespawnEffectTime));
	RespawnEffect();
	__NFUN_256__(RespawnEffectTime);
	__NFUN_113__('Pickup');
	stop;		
}

defaultproperties
{
	MaxDesireability=0.1000000
	bOnlyReplicateHidden=true
	RespawnEffectTime=0.5000000
	PickupMessage="Snagged an item."
	DrawType=2
	bOrientOnSlope=true
	bIgnoreEncroachers=true
	bAlwaysRelevant=true
	bOnlyDirtyReplication=true
	bCheckChangableLevel=true
	NetUpdateFrequency=8.0000000
	NetPriority=1.4000000
	Texture=Texture'Engine.S_Inventory'
	bShouldBaseAtStartup=true
	bCollideActors=true
	bCollideWorld=true
	bUseCylinderCollision=true
	bFixedRotationDir=true
	RotationRate=(Pitch=0,Yaw=5000,Roll=0)
	DesiredRotation=(Pitch=0,Yaw=30000,Roll=0)
}