class LocationInfo extends Info
	native
	placeable
	hidecategories(Collision,Lighting,LightColor,Karma,Force);

var() bool bMovable;

defaultproperties
{
	bStatic=true
	Texture=Texture'Engine.S_LocationInfo'
	bStaticLighting=true
}