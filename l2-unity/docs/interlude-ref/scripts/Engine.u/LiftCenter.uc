class LiftCenter extends NavigationPoint
	native
	placeable
	hidecategories(Lighting,LightColor,Karma,Force);

var() name LiftTag;
var Mover MyLift;
var() name LiftTrigger;
var Trigger RecommendedTrigger;
var float MaxDist2D;
var Vector LiftOffset;

function PostBeginPlay()
{
	// End:0x29
	if(__NFUN_255__(LiftTrigger, 'None'))
	{
		// End:0x28
		foreach __NFUN_313__(Class'Engine.Trigger', RecommendedTrigger, LiftTrigger)
		{
			// End:0x28
			break;			
		}		
	}
	super.PostBeginPlay();
	return;
}

function Actor SpecialHandling(Pawn Other)
{
	// End:0x0D
	if(__NFUN_114__(MyLift, none))
	{
		return self;
	}
	// End:0x4B
	if(__NFUN_129__(MyLift.__NFUN_281__('StandOpenTimed')))
	{
		// End:0x48
		if(__NFUN_130__(MyLift.bClosed, __NFUN_119__(RecommendedTrigger, none)))
		{
			return RecommendedTrigger;
		}		
	}
	else
	{
		// End:0x7C
		if(__NFUN_130__(__NFUN_154__(int(MyLift.BumpType), int(0)), __NFUN_129__(Other.IsPlayerPawn())))
		{
			return none;
		}
	}
	return self;
	return;
}

function bool SuggestMovePreparation(Pawn Other)
{
	// End:0x1A
	if(__NFUN_114__(Other.Base, MyLift))
	{
		return false;
	}
	__NFUN_267__(__NFUN_215__(MyLift.Location, LiftOffset));
	__NFUN_298__(MyLift);
	// End:0x7D
	if(__NFUN_132__(MyLift.bInterpolating, __NFUN_129__(ProceedWithMove(Other))))
	{
		Other.Controller.WaitForMover(MyLift);
		return true;
	}
	return false;
	return;
}

function bool ProceedWithMove(Pawn Other)
{
	local LiftExit Start;
	local float Dist2D;
	local Vector Dir;

	Start = LiftExit(Other.Anchor);
	// End:0x67
	if(__NFUN_130__(__NFUN_119__(Start, none), __NFUN_155__(int(Start.KeyFrame), 255)))
	{
		// End:0x64
		if(__NFUN_154__(int(MyLift.KeyNum), int(Start.KeyFrame)))
		{
			return true;
		}		
	}
	else
	{
		Dir = __NFUN_216__(Location, Other.Location);
		Dir.Z = 0.0000000;
		Dist2D = __NFUN_225__(Dir);
		// End:0x136
		if(__NFUN_130__(__NFUN_130__(__NFUN_176__(__NFUN_175__(Location.Z, CollisionHeight), __NFUN_174__(__NFUN_175__(Other.Location.Z, Other.CollisionHeight), 10.0000000)), __NFUN_177__(__NFUN_175__(Location.Z, CollisionHeight), __NFUN_175__(__NFUN_175__(Other.Location.Z, Other.CollisionHeight), float(1200)))), __NFUN_176__(Dist2D, MaxDist2D)))
		{
			return true;
		}
	}
	// End:0x164
	if(MyLift.bClosed)
	{
		Other.SetMoveTarget(SpecialHandling(Other));
		return true;
	}
	return false;
	return;
}

defaultproperties
{
	MaxDist2D=400.0000000
	ExtraCost=400
	bNeverUseStrafing=true
	bForceNoStrafing=true
	bSpecialMove=true
	bNoAutoConnect=true
	bStatic=false
	RemoteRole=0
	Texture=Texture'Engine.S_LiftCenter'
}