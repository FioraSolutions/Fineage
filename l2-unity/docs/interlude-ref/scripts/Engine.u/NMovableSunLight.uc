class NMovableSunLight extends Light
	native
	placeable;

defaultproperties
{
	LightEffect=19
	bSunlightColor=true
	bStatic=false
	Texture=Texture'Engine.SunIcon'
	bIgnoredRange=true
	bMovable=true
	bDirectional=true
}