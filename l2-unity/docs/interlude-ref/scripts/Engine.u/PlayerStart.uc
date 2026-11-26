class PlayerStart extends SmallNavigationPoint
	native
	placeable
	hidecategories(Lighting,LightColor,Karma,Force);

var() byte TeamNumber;
var() bool bSinglePlayerStart;
var() bool bCoopStart;
var() bool bEnabled;
var() bool bPrimaryStart;
var() float LastSpawnCampTime;

defaultproperties
{
	bSinglePlayerStart=true
	bCoopStart=true
	bEnabled=true
	bPrimaryStart=true
	Texture=Texture'Engine.S_Player'
	bDirectional=true
}