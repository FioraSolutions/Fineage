class L2SeamlessInfo extends Info
	native
	placeable
	hidecategories(Movement,Collision,Lighting,LightColor,Karma,Force);

var() float AffectRange;
var() int MapX;
var() int MapY;
var bool bLoaded;

defaultproperties
{
	Texture=Texture'Engine.S_L2SeamlessInfo'
}