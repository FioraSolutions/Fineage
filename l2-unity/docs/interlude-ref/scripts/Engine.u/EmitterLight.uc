class EmitterLight extends Light
	native
	placeable;

enum EEmitterLightType
{
	EEL_PawnOnly,                   // 0
	EEL_WorldOnly,                  // 1
	EEL_All                         // 2
};

var() EmitterLight.EEmitterLightType EmitterLightType;

defaultproperties
{
	bStatic=false
	bNoDelete=false
	bDynamicLight=true
}