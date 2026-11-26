class Powerups extends Inventory
	abstract
	native
	nativereplication
	notplaceable;

var travel int NumCopies;
var() bool bCanHaveMultipleCopies;
var() bool bAutoActivate;
var() bool bActivatable;
var travel bool bActive;
var() localized string ExpireMessage;
var() Sound ActivateSound;
var() Sound DeActivateSound;

replication
{
	// Pos:0x000
	reliable if(__NFUN_130__(__NFUN_130__(bNetOwner, bNetDirty), __NFUN_154__(int(Role), int(ROLE_Authority))))
		NumCopies, bActivatable, 
		bActive;
}

event TravelPreAccept()
{
	super.TravelPreAccept();
	// End:0x15
	if(bActive)
	{
		Activate();
	}
	return;
}

function PickupFunction(Pawn Other)
{
	super.PickupFunction(Other);
	// End:0x3A
	if(__NFUN_130__(bActivatable, __NFUN_114__(Other.SelectedItem, none)))
	{
		Other.SelectedItem = self;
	}
	// End:0x68
	if(__NFUN_130__(__NFUN_130__(bActivatable, bAutoActivate), Other.bAutoActivate))
	{
		Activate();
	}
	return;
}

function Powerups SelectNext()
{
	// End:0x0B
	if(bActivatable)
	{
		return self;
	}
	// End:0x29
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

function Activate()
{
	// End:0x10
	if(bActivatable)
	{
		__NFUN_113__('Activated');
	}
	return;
}

function bool HandlePickupQuery(Pickup item)
{
	// End:0xB3
	if(__NFUN_114__(item.InventoryType, Class))
	{
		// End:0x2B
		if(bCanHaveMultipleCopies)
		{
			__NFUN_165__(NumCopies);			
		}
		else
		{
			// End:0x96
			if(bDisplayableInv)
			{
				// End:0x6F
				if(__NFUN_119__(item.Inventory, none))
				{
					Charge = __NFUN_250__(Charge, item.Inventory.Charge);					
				}
				else
				{
					Charge = __NFUN_250__(Charge, item.InventoryType.default.Charge);
				}				
			}
			else
			{
				return false;
			}
		}
		item.AnnouncePickup(Pawn(Owner));
		return true;
	}
	// End:0xC0
	if(__NFUN_114__(Inventory, none))
	{
		return false;
	}
	return Inventory.HandlePickupQuery(item);
	return;
}

function float UseCharge(float Amount)
{
	return;
}

function FireEffect()
{
	return;
}

function UsedUp()
{
	// End:0xA8
	if(__NFUN_119__(Pawn(Owner), none))
	{
		bActivatable = false;
		Pawn(Owner).NextItem();
		// End:0x87
		if(__NFUN_114__(Pawn(Owner).SelectedItem, self))
		{
			Pawn(Owner).NextItem();
			// End:0x87
			if(__NFUN_114__(Pawn(Owner).SelectedItem, self))
			{
				Pawn(Owner).SelectedItem = none;
			}
		}
		Instigator.ReceiveLocalizedMessage(MessageClass, 0, none, none, self.Class);
	}
	Owner.__NFUN_264__(DeActivateSound, 6);
	__NFUN_279__();
	return;
}

static function string GetLocalString(optional int Switch, optional PlayerReplicationInfo RelatedPRI_1, optional PlayerReplicationInfo RelatedPRI_2)
{
	return default.ExpireMessage;
	return;
}

state Activated
{
	function BeginState()
	{
		bActive = true;
		return;
	}

	function EndState()
	{
		bActive = false;
		return;
	}

	function Activate()
	{
		// End:0x43
		if(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_119__(Pawn(Owner), none), Pawn(Owner).bAutoActivate), bAutoActivate), __NFUN_151__(Charge, 0)))
		{
			return;
		}
		global.Activate();
		return;
	}
	stop;
}
