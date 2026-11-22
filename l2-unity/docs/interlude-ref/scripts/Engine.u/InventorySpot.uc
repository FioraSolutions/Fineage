class InventorySpot extends SmallNavigationPoint
	native
	notplaceable
	hidecategories(Lighting,LightColor,Karma,Force);

cpptext
{
	virtual UBOOL IsIdentifiedAs(FName ActorName);
    virtual AInventorySpot* GetAInventorySpot() { return this; } 

}

var Pickup markedItem;

function Actor GetMoveTargetFor(AIController B, float MaxWait)
{
	// End:0x48
	if(__NFUN_130__(__NFUN_130__(__NFUN_119__(markedItem, none), markedItem.ReadyToPickup(MaxWait)), __NFUN_177__(B.Desireability(markedItem), float(0))))
	{
		return markedItem;
	}
	return self;
	return;
}

event float DetourWeight(Pawn Other, float PathWeight)
{
	// End:0x3E
	if(__NFUN_130__(__NFUN_119__(markedItem, none), markedItem.ReadyToPickup(0.0000000)))
	{
		return markedItem.DetourWeight(Other, PathWeight);
	}
	return;
}

defaultproperties
{
	bCollideWhenPlacing=false
	bHiddenEd=true
}