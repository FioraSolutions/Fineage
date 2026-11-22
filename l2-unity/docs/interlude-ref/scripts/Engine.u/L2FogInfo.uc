class L2FogInfo extends Info
	native
	noexport
	placeable
	hidecategories(Collision,Lighting,LightColor,Karma,Force);

struct L2EnvironmentColorInfo
{
	var() float Time;
	var() Color FogColor;
	var() Color SkyColor;
	var() array<Color> CloudColor;
	var() array<Color> HazeRingColor;
};

var() Range AffectRange;
var() Range FogRange1;
var() Range FogRange2;
var() Range FogRange3;
var() Range FogRange4;
var() Range FogRange5;
var() float TextureDistance;
var() array<L2EnvironmentColorInfo> Colors;
var() Material CloudTexture;

defaultproperties
{
	Texture=Texture'Engine.S_L2FogInfo'
}