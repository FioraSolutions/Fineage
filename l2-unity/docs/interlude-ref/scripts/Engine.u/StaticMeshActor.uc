class StaticMeshActor extends Actor
	native
	placeable;

struct AccessoryType
{
	var() int Depth;
	var() StaticMesh Mesh;
};

var(Agit) bool bAgitDefaultStaticMesh;
var(Agit) int AgitID;
var(Agit) int AccessoryIndex;
var(Agit) int AgitStatus;
var(Agit) transient int CurrAccessoryType;
var(Agit) array<AccessoryType> AccessoryTypeList;
var(TimeReactor) bool bTimeReactor;
var(TimeReactor) float ShowTime;
var(TimeReactor) float HideTime;
var(Sound) Sound StepSound_1;
var(Sound) Sound StepSound_2;
var(Sound) Sound StepSound_3;
var() bool bExactProjectileCollision;

defaultproperties
{
	bExactProjectileCollision=true
	DrawType=8
	bStatic=true
	bWorldGeometry=true
	bShadowCast=true
	bStaticLighting=true
	CollisionRadius=1.0000000
	CollisionHeight=1.0000000
	bCollideActors=true
	bBlockActors=true
	bBlockPlayers=true
	bBlockKarma=true
	bEdShouldSnap=true
}