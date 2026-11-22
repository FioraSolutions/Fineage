class NMoon extends Actor
	native
	placeable;

var(Display) float Radius;
var float Latitude;
var float Longitude;
var(Display) float LimitMaxRadius;
var(Display) float MoonScale;
var bool bMakeLightmap;
var Vector Position;
var(Display) bool bMoonLight;
var(Display) int EnvType;
var(Display) Texture Flame[12];
var(LightColor) byte LightHue;
var(LightColor) byte LightSaturation;
var(LightColor) float LightBrightness;

defaultproperties
{
	Radius=32768.0000000
	LimitMaxRadius=32768.0000000
	MoonScale=1.0000000
	DrawType=13
	bAcceptsProjectors=false
	bNetTemporary=true
	RemoteRole=0
	bIgnoredRange=true
	bGameRelevant=true
	bDirectional=true
}