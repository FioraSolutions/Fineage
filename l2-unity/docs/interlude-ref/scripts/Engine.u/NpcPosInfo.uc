class NpcPosInfo extends Info
	native
	placeable
	hidecategories(Collision,Lighting,LightColor,Karma,Force);

struct NpcPos
{
	var() Vector Delta;
	var() int Yaw;
	var() int Percent;
};

var() array<NpcPos> pos;
var() Color LineColor;
var() name NpcName;
var() string nickname;
var() name ai;
var() array<NpcPrivate> Privates;
var() WhenExtinctionCreate when_extinction_create;
var() bool bWayPointsShow;
var() array<WayPoint> WayPoints;

defaultproperties
{
	LineColor=(R=10,G=255,B=255,A=255)
	bStatic=true
	Texture=Texture'Engine.S_TerritoryInfo'
	bStaticLighting=true
}