class Light extends Actor
	native
	placeable;

enum ELightType
{
	LT_None,                        // 0
	LT_Steady,                      // 1
	LT_Pulse,                       // 2
	LT_Blink,                       // 3
	LT_Flicker,                     // 4
	LT_Strobe,                      // 5
	LT_BackdropLight,               // 6
	LT_SubtlePulse,                 // 7
	LT_TexturePaletteOnce,          // 8
	LT_TexturePaletteLoop,          // 9
	LT_FadeOut,                     // 10
	LT_Fade                         // 11
};

enum ELightEffect
{
	LE_None,                        // 0
	LE_TorchWaver,                  // 1
	LE_FireWaver,                   // 2
	LE_WateryShimmer,               // 3
	LE_Searchlight,                 // 4
	LE_SlowWave,                    // 5
	LE_FastWave,                    // 6
	LE_CloudCast,                   // 7
	LE_StaticSpot,                  // 8
	LE_Shock,                       // 9
	LE_Disco,                       // 10
	LE_Warp,                        // 11
	LE_Spotlight,                   // 12
	LE_NonIncidence,                // 13
	LE_Shell,                       // 14
	LE_OmniBumpMap,                 // 15
	LE_Interference,                // 16
	LE_Cylinder,                    // 17
	LE_Rotor,                       // 18
	LE_Sunlight,                    // 19
	LE_QuadraticNonIncidence        // 20
};

var(Lighting) Light.ELightType LightType;
var(Lighting) Light.ELightEffect LightEffect;
var(LightColor) float LightBrightness;
var(Lighting) float LightRadius;
var(LightColor) byte LightHue;
var(LightColor) byte LightSaturation;
var(Lighting) byte LightPeriod;
var(Lighting) byte LightPhase;
var(Lighting) byte LightCone;
var bool bSunlightColor;
var(Lighting) bool bTimeLight;
var(Lighting) float LightOnTime;
var(Lighting) float LightOffTime;
var float LightPrevTime;
var float LightLifeTime;
var(Corona) float MinCoronaSize;
var(Corona) float MaxCoronaSize;
var(Corona) float CoronaRotation;
var(Corona) float CoronaRotationOffset;
var(Corona) bool UseOwnFinalBlend;

defaultproperties
{
	LightType=1
	LightBrightness=64.0000000
	LightRadius=64.0000000
	LightSaturation=255
	LightPeriod=32
	LightCone=128
	MaxCoronaSize=1000.0000000
	bStatic=true
	bHidden=true
	bNoDelete=true
	Texture=Texture'Engine.S_Light'
	bMovable=false
	CollisionRadius=24.0000000
	CollisionHeight=24.0000000
}