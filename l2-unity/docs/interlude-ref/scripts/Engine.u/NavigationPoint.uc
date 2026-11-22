class NavigationPoint extends Actor
	native
	notplaceable
	hidecategories(Lighting,LightColor,Karma,Force);

var const array<ReachSpec> PathList;
var() name ProscribedPaths[4];
var() name ForcedPaths[4];
var int visitedWeight;
var const int bestPathWeight;
var const NavigationPoint nextNavigationPoint;
var const NavigationPoint nextOrdered;
var const NavigationPoint prevOrdered;
var const NavigationPoint previousPath;
var int cost;
var() int ExtraCost;
var transient int TransientCost;
var transient int FearCost;
var transient bool bEndPoint;
var transient bool bTransientEndPoint;
var bool taken;
var() bool bBlocked;
var() bool bPropagatesSound;
var() bool bOneWayPath;
var() bool bNeverUseStrafing;
var() bool bAlwaysUseStrafing;
var const bool bForceNoStrafing;
var const bool bAutoBuilt;
var bool bSpecialMove;
var bool bNoAutoConnect;
var const bool bNotBased;
var const bool bPathsChanged;
var bool bDestinationOnly;
var bool bSourceOnly;
var bool bSpecialForced;
var bool bMustBeReachable;
var Pickup InventoryCache;
var float InventoryDist;

function PostBeginPlay()
{
	ExtraCost = __NFUN_250__(ExtraCost, 0);
	super.PostBeginPlay();
	return;
}

event int SpecialCost(Pawn Seeker, ReachSpec Path)
{
	return;
}

event bool Accept(Actor Incoming, Actor Source)
{
	taken = Incoming.__NFUN_267__(Location);
	// End:0x4E
	if(taken)
	{
		Incoming.Velocity = vect(0.0000000, 0.0000000, 0.0000000);
		Incoming.__NFUN_299__(Rotation);
	}
	Incoming.PlayTeleportEffect(true, false);
	TriggerEvent(Event, self, Pawn(Incoming));
	return taken;
	return;
}

event float DetourWeight(Pawn Other, float PathWeight)
{
	return;
}

event bool SuggestMovePreparation(Pawn Other)
{
	return false;
	return;
}

function bool ProceedWithMove(Pawn Other)
{
	return true;
	return;
}

function MoverOpened()
{
	return;
}

function MoverClosed()
{
	return;
}

defaultproperties
{
	bPropagatesSound=true
	bStatic=true
	bHidden=true
	bNoDelete=true
	Texture=Texture'Engine.S_NavP'
	bCollideWhenPlacing=true
	CollisionRadius=80.0000000
	CollisionHeight=100.0000000
}