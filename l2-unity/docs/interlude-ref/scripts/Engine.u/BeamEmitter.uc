class BeamEmitter extends ParticleEmitter
	native
	editinlinenew;

enum EBeamEndPointType
{
	PTEP_Velocity,                  // 0
	PTEP_Distance,                  // 1
	PTEP_Offset,                    // 2
	PTEP_Actor,                     // 3
	PTEP_TraceOffset,               // 4
	PTEP_OffsetAsAbsolute           // 5
};

struct ParticleBeamData
{
	var Vector Location;
	var float t;
};

struct ParticleBeamEndPoint
{
	var() name ActorTag;
	var() RangeVector offset;
	var() float Weight;
};

struct ParticleBeamScale
{
	var() Vector FrequencyScale;
	var() float RelativeLength;
};

var(Beam) Range BeamDistanceRange;
var(Beam) array<ParticleBeamEndPoint> BeamEndPoints;
var(Beam) BeamEmitter.EBeamEndPointType DetermineEndPointBy;
var(Beam) float BeamTextureUScale;
var(Beam) float BeamTextureVScale;
var(Beam) int RotatingSheets;
var(Beam) bool TriggerEndpoint;
var(BeamNoise) RangeVector LowFrequencyNoiseRange;
var(BeamNoise) int LowFrequencyPoints;
var(BeamNoise) RangeVector HighFrequencyNoiseRange;
var(BeamNoise) int HighFrequencyPoints;
var(BeamNoise) array<ParticleBeamScale> LFScaleFactors;
var(BeamNoise) array<ParticleBeamScale> HFScaleFactors;
var(BeamNoise) float LFScaleRepeats;
var(BeamNoise) float HFScaleRepeats;
var(BeamNoise) bool UseHighFrequencyScale;
var(BeamNoise) bool UseLowFrequencyScale;
var(BeamNoise) bool NoiseDeterminesEndPoint;
var(BeamNoise) RangeVector DynamicHFNoiseRange;
var(BeamNoise) Range DynamicHFNoisePointsRange;
var(BeamNoise) Range DynamicTimeBetweenNoiseRange;
var(BeamBranching) bool UseBranching;
var(BeamBranching) Range BranchProbability;
var(BeamBranching) Range BranchHFPointsRange;
var(BeamBranching) int BranchEmitter;
var(BeamBranching) Range BranchSpawnAmountRange;
var(BeamBranching) bool LinkupLifetime;
var transient int SheetsUsed;
var transient int VerticesPerParticle;
var transient int IndicesPerParticle;
var transient int PrimitivesPerParticle;
var transient float BeamValueSum;
var transient array<ParticleBeamData> HFPoints;
var transient array<Vector> LFPoints;
var transient array<Actor> HitActors;
var transient float TimeSinceLastDynamicNoise;

defaultproperties
{
	BeamTextureUScale=1.0000000
	BeamTextureVScale=1.0000000
	LowFrequencyPoints=3
	HighFrequencyPoints=10
	BranchEmitter=-1
}