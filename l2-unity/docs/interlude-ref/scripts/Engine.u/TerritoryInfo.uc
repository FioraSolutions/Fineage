class TerritoryInfo extends Info
	native
	placeable
	hidecategories(Collision,Lighting,LightColor,Karma,Force);

enum InitalSpawn
{
	All,                            // 0
	random1,                        // 1
	random2                         // 2
};

struct AnywhereNpc
{
	var() name NpcName;
	var() int total;
	var() string respawn;
	var() string nickname;
	var() name ai;
	var() array<NpcPrivate> Privates;
	var() WhenExtinctionCreate when_extinction_create;
	var() bool bWayPointsShow;
	var() array<WayPoint> WayPoints;
};

struct AnywhereNpcMaker
{
	var() bool bGroup;
	var() TerritoryInfo.InitalSpawn inital_spawn;
	var() int maximum_npc;
	var() array<AnywhereNpc> Npc;
};

var() string TerritoryName;
var() int PointNum;
var() float TerritoryHeight;
var() Vector DeltaPoint[64];
var() Color LineColor;
var() array<AnywhereNpcMaker> npcmaker;

defaultproperties
{
	PointNum=4
	TerritoryHeight=200.0000000
	DeltaPoint[0]=(X=-100.0000000,Y=-100.0000000,Z=0.0000000)
	DeltaPoint[1]=(X=100.0000000,Y=-100.0000000,Z=0.0000000)
	DeltaPoint[2]=(X=100.0000000,Y=100.0000000,Z=0.0000000)
	DeltaPoint[3]=(X=-100.0000000,Y=100.0000000,Z=0.0000000)
	LineColor=(R=10,G=10,B=255,A=255)
	bAlwaysVisible=true
	bStatic=true
	Texture=Texture'Engine.S_TerritoryInfo'
	bStaticLighting=true
}