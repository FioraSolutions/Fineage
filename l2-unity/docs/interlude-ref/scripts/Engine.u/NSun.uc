class NSun extends Actor
	native
	placeable;

var(Display) float Radius;
var float Latitude;
var float Longitude;
var(Display) float LimitMaxRadius;
var(Display) float SunScale;
var bool bMakeLightmap;
var Vector Position;

simulated function PostBeginPlay()
{
	super.PostBeginPlay();
	return;
}

defaultproperties
{
	Radius=32768.0000000
	LimitMaxRadius=32768.0000000
	SunScale=1.0000000
	DrawType=13
	bAcceptsProjectors=false
	bNetTemporary=true
	RemoteRole=0
	bIgnoredRange=true
	bGameRelevant=true
}