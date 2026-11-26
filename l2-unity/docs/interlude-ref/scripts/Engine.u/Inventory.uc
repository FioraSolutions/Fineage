class Inventory extends Actor
	abstract
	native
	nativereplication
	notplaceable;

var byte InventoryGroup;
var byte GroupOffset;
var bool bDisplayableInv;
var bool bTossedOut;
var Class<Pickup> PickupClass;
var() travel int Charge;
var(FirstPerson) Vector PlayerViewOffset;
var(FirstPerson) Rotator PlayerViewPivot;
var() bool bDrawingFirstPerson;
var float BobDamping;
var Actor ThirdPersonActor;
var Class<InventoryAttachment> AttachmentClass;
var Material Icon;
var Material StatusIcon;
var localized string ItemName;

replication
{
	// Pos:0x000
	reliable if(__NFUN_130__(__NFUN_130__(bNetOwner, bNetDirty), __NFUN_154__(int(Role), int(ROLE_Authority))))
		Charge, ThirdPersonActor;
}

simulated function AttachToPawn(Pawn P)
{
	local name bonename;

	// End:0x33
	if(__NFUN_114__(ThirdPersonActor, none))
	{
		ThirdPersonActor = __NFUN_278__(AttachmentClass, Owner);
		InventoryAttachment(ThirdPersonActor).InitFor(self);
	}
	bonename = P.GetWeaponBoneFor(self);
	// End:0x86
	if(__NFUN_254__(bonename, 'None'))
	{
		ThirdPersonActor.__NFUN_267__(P.Location);
		ThirdPersonActor.__NFUN_298__(P);		
	}
	else
	{
		P.AttachToBone(ThirdPersonActor, bonename);
	}
	return;
}

exec function updaterelative(int Pitch, int Yaw, int Roll)
{
	local Rotator newRot;

	newRot.Pitch = Pitch;
	newRot.Yaw = Yaw;
	newRot.Roll = Roll;
	ThirdPersonActor.SetRelativeLocation(ThirdPersonActor.default.RelativeLocation);
	ThirdPersonActor.SetRelativeRotation(newRot);
	return;
}

simulated function DetachFromPawn(Pawn P)
{
	// End:0x1E
	if(__NFUN_119__(ThirdPersonActor, none))
	{
		ThirdPersonActor.__NFUN_279__();
		ThirdPersonActor = none;
	}
	return;
}

simulated event RenderOverlays(Canvas Canvas)
{
	// End:0x23
	if(__NFUN_132__(__NFUN_114__(Instigator, none), __NFUN_114__(Instigator.Controller, none)))
	{
		return;
	}
	__NFUN_267__(__NFUN_215__(Instigator.Location, Instigator.CalcDrawOffset(self)));
	__NFUN_299__(Instigator.GetViewRotation());
	Canvas.__NFUN_467__(self, false);
	return;
}

simulated function string GetHumanReadableName()
{
	// End:0x1F
	if(__NFUN_122__(ItemName, ""))
	{
		ItemName = GetItemName(string(Class));
	}
	return ItemName;
	return;
}

function PickupFunction(Pawn Other)
{
	return;
}

simulated function Weapon RecommendWeapon(out float rating)
{
	// End:0x23
	if(__NFUN_119__(Inventory, none))
	{
		return Inventory.RecommendWeapon(rating);		
	}
	else
	{
		rating = -1.0000000;
		return none;
	}
	return;
}

event TravelPreAccept()
{
	super.TravelPreAccept();
	GiveTo(Pawn(Owner));
	return;
}

function TravelPostAccept()
{
	super.TravelPostAccept();
	PickupFunction(Pawn(Owner));
	return;
}

function Destroyed()
{
	// End:0x25
	if(__NFUN_119__(Pawn(Owner), none))
	{
		Pawn(Owner).DeleteInventory(self);
	}
	// End:0x3C
	if(__NFUN_119__(ThirdPersonActor, none))
	{
		ThirdPersonActor.__NFUN_279__();
	}
	return;
}

function GiveTo(Pawn Other)
{
	Instigator = Other;
	Other.AddInventory(self);
	__NFUN_113__('None');
	return;
}

function bool HandlePickupQuery(Pickup item)
{
	// End:0x1A
	if(__NFUN_114__(item.InventoryType, Class))
	{
		return true;
	}
	// End:0x27
	if(__NFUN_114__(Inventory, none))
	{
		return false;
	}
	return Inventory.HandlePickupQuery(item);
	return;
}

function Powerups SelectNext()
{
	// End:0x1E
	if(__NFUN_119__(Inventory, none))
	{
		return Inventory.SelectNext();		
	}
	else
	{
		return none;
	}
	return;
}

function DropFrom(Vector StartLocation)
{
	local Pickup P;

	// End:0x26
	if(__NFUN_119__(Instigator, none))
	{
		DetachFromPawn(Instigator);
		Instigator.DeleteInventory(self);
	}
	SetDefaultDisplayProperties();
	Instigator = none;
	StopAnimating();
	__NFUN_113__('None');
	P = __NFUN_278__(PickupClass,,, StartLocation);
	// End:0x65
	if(__NFUN_114__(P, none))
	{
		__NFUN_279__();
		return;
	}
	P.InitDroppedPickupFor(self);
	P.Velocity = Velocity;
	Velocity = vect(0.0000000, 0.0000000, 0.0000000);
	return;
}

function Use(float Value)
{
	return;
}

simulated function Weapon WeaponChange(byte f)
{
	// End:0x10
	if(__NFUN_114__(Inventory, none))
	{
		return none;		
	}
	else
	{
		return Inventory.WeaponChange(f);
	}
	return;
}

simulated function Weapon PrevWeapon(Weapon CurrentChoice, Weapon CurrentWeapon)
{
	// End:0x14
	if(__NFUN_114__(Inventory, none))
	{
		return CurrentChoice;		
	}
	else
	{
		return Inventory.PrevWeapon(CurrentChoice, CurrentWeapon);
	}
	return;
}

simulated function Weapon NextWeapon(Weapon CurrentChoice, Weapon CurrentWeapon)
{
	// End:0x14
	if(__NFUN_114__(Inventory, none))
	{
		return CurrentChoice;		
	}
	else
	{
		return Inventory.NextWeapon(CurrentChoice, CurrentWeapon);
	}
	return;
}

function OwnerEvent(name EventName)
{
	// End:0x1F
	if(__NFUN_119__(Inventory, none))
	{
		Inventory.OwnerEvent(EventName);
	}
	return;
}

function SetOwnerDisplay()
{
	// End:0x1A
	if(__NFUN_119__(Inventory, none))
	{
		Inventory.SetOwnerDisplay();
	}
	return;
}

static function string StaticItemName()
{
	return default.ItemName;
	return;
}

defaultproperties
{
	BobDamping=0.9600000
	AttachmentClass=Class'Engine.InventoryAttachment'
	DrawType=0
	bHidden=true
	bOnlyOwnerSee=true
	bOnlyRelevantToOwner=true
	bReplicateMovement=false
	bOnlyDirtyReplication=true
	RemoteRole=2
	NetPriority=1.4000000
	bTravel=true
	bClientAnim=true
}