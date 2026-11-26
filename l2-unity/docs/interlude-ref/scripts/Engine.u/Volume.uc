class Volume extends Brush
	native
	notplaceable;

var Actor AssociatedActor;
var() name AssociatedActorTag;
var() int LocationPriority;
var() localized string LocationName;
var() edfindable DecorationList DecoList;

// Export UVolume::execEncompasses(FFrame&, void* const)
native function bool Encompasses(Actor Other);

function PostBeginPlay()
{
	super(Actor).PostBeginPlay();
	// End:0x2F
	if(__NFUN_255__(AssociatedActorTag, 'None'))
	{
		// End:0x2E
		foreach __NFUN_304__(Class'Engine.Actor', AssociatedActor, AssociatedActorTag)
		{
			// End:0x2E
			break;			
		}		
	}
	// End:0x4A
	if(__NFUN_119__(AssociatedActor, none))
	{
		__NFUN_113__('AssociatedTouch');
		InitialState = __NFUN_284__();
	}
	return;
}

function DisplayDebug(Canvas Canvas, out float YL, out float YPos)
{
	super(Actor).DisplayDebug(Canvas, YL, YPos);
	Canvas.__NFUN_465__(__NFUN_112__("AssociatedActor ", string(AssociatedActor)), false);
	__NFUN_184__(YPos, YL);
	Canvas.SetPos(4.0000000, YPos);
	return;
}

state AssociatedTouch
{
	event Touch(Actor Other)
	{
		AssociatedActor.Touch(Other);
		return;
	}

	event UnTouch(Actor Other)
	{
		AssociatedActor.UnTouch(Other);
		return;
	}

	function BeginState()
	{
		local Actor A;

		// End:0x1C
		foreach __NFUN_307__(Class'Engine.Actor', A)
		{
			Touch(A);			
		}		
		return;
	}
	stop;
}

defaultproperties
{
	LocationName="unspecified"
	bSkipActorPropertyReplication=true
	bCollideActors=true
}