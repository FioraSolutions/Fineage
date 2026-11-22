class Emitter extends Actor
	native
	placeable;

var() export editinline array<export editinline ParticleEmitter> Emitters;
var(Global) bool AutoDestroy;
var(Global) bool AutoReset;
var(Global) bool DisableFogging;
var(Global) RangeVector GlobalOffsetRange;
var(Global) Range TimeTillResetRange;
var(Global) bool AutoReplay;
var float SpeedRate;
var(Global) bool bRotEmitter;
var(Global) Rotator RotPerSecond;
var(Global) bool FixedBoundingBox;
var(Global) float FixedBoundingBoxExpand;
var(SpawnSound) array<Sound> SpawnSound;
var(SpawnSound) float SoundRadius;
var(SpawnSound) float SoundVolume;
var(SpawnSound) bool SoundLooping;
var(SpawnSound) float SoundPitchMin;
var(SpawnSound) float SoundPitchMax;
var transient int Initialized;
var transient Box BoundingBox;
var transient float EmitterRadius;
var transient float EmitterHeight;
var transient bool ActorForcesEnabled;
var transient Vector GlobalOffset;
var transient float TimeTillReset;
var transient bool UseParticleProjectors;
var transient ParticleMaterial ParticleMaterial;
var transient bool DeleteParticleEmitters;
var transient float FixedLifeTime;
var transient bool FirstSpawnParticle;
var transient Vector TrailerPrePivot;
var(EmitterLight) bool bUseLight;
var(EmitterLight) byte LightType;
var(EmitterLight) byte LightEffect;
var(EmitterLight) byte LightBrightness;
var(EmitterLight) float LightRadius;
var(EmitterLight) byte LightHue;
var(EmitterLight) byte LightSaturation;
var(EmitterLight) byte EmitterLightingType;
var transient EmitterLight pEmitterLight;
var(EmitterLight) float EL_LifeSpan;
var(EmitterLight) float EL_InitialDelay;
var(EmitterQuake) bool bUseQuake;
var(EmitterQuake) byte ShakeType;
var(EmitterQuake) float ShakeIntensity;
var(EmitterQuake) Vector ShakeVector;
var(EmitterQuake) float ShakeRange;
var(EmitterQuake) int ShakeCount;
var(EmitterQuake) float ShakeTime;
var(EmitterQuake) float EQ_InitialDelay;
var(Global) Range VisibleLimit;
var(Global) float VisibilityInterpRange;
var(Global) bool bSetSizeScale;

// Export UEmitter::execKill(FFrame&, void* const)
native function Kill();

// Export UEmitter::execSetSizeScale(FFrame&, void* const)
native final function SetSizeScale(float NewScale);

simulated function UpdatePrecacheMaterials()
{
	local int i;

	i = 0;
	J0x07:

	// End:0x6F [Loop If]
	if(__NFUN_150__(i, Emitters.Length))
	{
		// End:0x65
		if(__NFUN_119__(Emitters[i], none))
		{
			// End:0x65
			if(__NFUN_119__(Emitters[i].Texture, none))
			{
				Level.AddPrecacheMaterial(Emitters[i].Texture);
			}
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x07;
	}
	return;
}

event Trigger(Actor Other, Pawn EventInstigator)
{
	local int i;

	i = 0;
	J0x07:

	// End:0x47 [Loop If]
	if(__NFUN_150__(i, Emitters.Length))
	{
		// End:0x3D
		if(__NFUN_119__(Emitters[i], none))
		{
			Emitters[i].Trigger();
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x07;
	}
	return;
}

simulated function SetDisabled(bool dis)
{
	local int i;

	i = 0;
	J0x07:

	// End:0x4E [Loop If]
	if(__NFUN_150__(i, Emitters.Length))
	{
		// End:0x44
		if(__NFUN_119__(Emitters[i], none))
		{
			Emitters[i].Disabled = dis;
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x07;
	}
	return;
}

defaultproperties
{
	SpeedRate=1.0000000
	SoundPitchMin=1.0000000
	SoundPitchMax=1.0000000
	VisibilityInterpRange=500.0000000
	bSetSizeScale=true
	DrawType=10
	bNeedCleanup=false
	bNoDelete=true
	bCheckChangableLevel=true
	RemoteRole=0
	Texture=Texture'Engine.S_Emitter'
	Style=8
	bUnlit=true
}