class LineOfSightTrigger extends Triggers
	native
	placeable;

var() float MaxViewDist;
var float OldTickTime;
var() bool bEnabled;
var bool bTriggered;
var() name SeenActorTag;
var Actor SeenActor;
var() int MaxViewAngle;
var float RequiredViewDir;

function PostBeginPlay()
{
	super(Actor).PostBeginPlay();
	RequiredViewDir = __NFUN_188__(__NFUN_172__(__NFUN_171__(float(MaxViewAngle), 3.1415927), float(180)));
	// End:0x4B
	if(__NFUN_255__(SeenActorTag, 'None'))
	{
		// End:0x4A
		foreach __NFUN_304__(Class'Engine.Actor', SeenActor, SeenActorTag)
		{
			// End:0x4A
			break;			
		}		
	}
	return;
}

event PlayerSeesMe(PlayerController P)
{
	TriggerEvent(Event, self, P.Pawn);
	bTriggered = true;
	return;
}

function Trigger(Actor Other, Pawn EventInstigator)
{
	bEnabled = true;
	return;
}

defaultproperties
{
	MaxViewDist=3000.0000000
	bEnabled=true
	MaxViewAngle=15
	bCollideActors=false
}