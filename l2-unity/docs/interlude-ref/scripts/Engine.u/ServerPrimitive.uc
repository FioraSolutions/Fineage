class ServerPrimitive extends Info
	native
	placeable
	hidecategories(Collision,Lighting,LightColor,Karma,Force);

struct ServerPointStruct
{
	var() string Name;
	var() Vector point;
	var() Plane Color;
};

struct ServerLineStruct
{
	var() string Name;
	var() Vector Start;
	var() Vector End;
	var() Plane Color;
};

var() array<ServerPointStruct> PointArray;
var() array<ServerLineStruct> LineArray;
var() Color LineColor;
var() string Name;

defaultproperties
{
	LineColor=(R=10,G=10,B=255,A=255)
	bAlwaysVisible=true
	bHidden=false
	bDisableSorting=true
	Texture=Texture'Engine.S_ServerPrimitive'
	bStaticLighting=true
	CollisionRadius=0.0000000
	CollisionHeight=0.0000000
}