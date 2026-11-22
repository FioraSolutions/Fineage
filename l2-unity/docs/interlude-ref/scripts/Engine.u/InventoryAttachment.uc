class InventoryAttachment extends Actor
	native
	nativereplication
	notplaceable;

function InitFor(Inventory i)
{
	Instigator = i.Instigator;
	return;
}

defaultproperties
{
	DrawType=2
	bOnlyDrawIfAttached=true
	bOnlyDirtyReplication=true
	RemoteRole=2
	NetUpdateFrequency=10.0000000
	bUseLightingFromBase=true
}