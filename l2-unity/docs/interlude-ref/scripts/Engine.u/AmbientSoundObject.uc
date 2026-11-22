class AmbientSoundObject extends LevelObject
	native;

enum ASType1
{
	AST1_Always,                    // 0
	AST1_Day,                       // 1
	AST1_Night,                     // 2
	AST1_Water                      // 3
};

var(Sound) AmbientSoundObject.ASType1 AmbientSoundType;
var(Sound) int AmbientRandom;
var float AmbientSoundStartTime;
var(Sound) Sound AmbientSound;
var(Sound) float SoundRadius;
var(Sound) byte SoundVolume;
var(Sound) byte SoundPitch;

defaultproperties
{
	AmbientRandom=100
	SoundRadius=64.0000000
	SoundVolume=190
	SoundPitch=64
	Texture=Texture'Engine.S_Ambient'
}