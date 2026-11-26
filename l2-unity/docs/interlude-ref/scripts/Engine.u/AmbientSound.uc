class AmbientSound extends Keypoint
	native
	placeable;

enum ASType
{
	AST_Always,                     // 0
	AST_Day,                        // 1
	AST_Night,                      // 2
	AST_Water                       // 3
};

enum ESoundOcclusion
{
	OCCLUSION_Default,              // 0
	OCCLUSION_None,                 // 1
	OCCLUSION_BSP,                  // 2
	OCCLUSION_StaticMeshes          // 3
};

var(Sound) AmbientSound.ASType AmbientSoundType;
var(Sound) int AmbientRandom;
var float AmbientSoundStartTime;
var(Sound) Sound AmbientSound;
var(Sound) float SoundRadius;
var(Sound) byte SoundVolume;
var(Sound) byte SoundPitch;
var(Sound) AmbientSound.ESoundOcclusion SoundOcclusion;
var(Sound) bool bFullVolume;

defaultproperties
{
	AmbientRandom=100
	SoundRadius=64.0000000
	SoundVolume=190
	SoundPitch=64
	RemoteRole=0
	Texture=Texture'Engine.S_Ambient'
}