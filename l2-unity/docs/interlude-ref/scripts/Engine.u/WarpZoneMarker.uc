class WarpZoneMarker extends SmallNavigationPoint
	native
	notplaceable
	hidecategories(Lighting,LightColor,Karma,Force);

var WarpZoneInfo markedWarpZone;
var Actor TriggerActor;
var Actor TriggerActor2;

function PostBeginPlay()
{
	// End:0x1A
	if(__NFUN_151__(markedWarpZone.numDestinations, 1))
	{
		FindTriggerActor();
	}
	super(NavigationPoint).PostBeginPlay();
	return;
}

function FindTriggerActor()
{
	local ZoneTrigger Z;

	// End:0x40
	foreach __NFUN_304__(Class'Engine.ZoneTrigger', Z)
	{
		// End:0x3F
		if(__NFUN_254__(Z.Event, markedWarpZone.ZoneTag))
		{
			TriggerActor = Z;			
			return;
		}		
	}	
	return;
}

function Actor SpecialHandling(Pawn Other)
{
	// End:0x31
	if(__NFUN_114__(Other.Region.Zone, markedWarpZone))
	{
		markedWarpZone.ActorEntered(Other);
	}
	return self;
	return;
}

defaultproperties
{
	bCollideWhenPlacing=false
	bHiddenEd=true
}