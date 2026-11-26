class TrailEmitter extends ParticleEmitter
	native
	editinlinenew;

struct ParticleTrailData
{
	var Vector Location;
	var Color Color;
	var float Size;
	var int DoubleDummy1;
	var int DoubleDummy2;
};

struct ParticleTrailInfo
{
	var int TrailIndex;
	var int NumPoints;
	var Vector LastLocation;
	var int PreDestroyTrailNum;
};

var(Trail) int MaxPointsPerTrail;
var(Trail) Range DistanceRange;
var(Trail) bool UseCrossedSheets;
var(Trail) int MaxTrailTwistAngle;
var transient array<ParticleTrailData> TrailData;
var transient array<ParticleTrailInfo> TrailInfo;
var transient VertexBuffer VertexBuffer;
var transient IndexBuffer IndexBuffer;
var transient int VerticesPerParticle;
var transient int IndicesPerParticle;
var transient int PrimitivesPerParticle;

defaultproperties
{
	MaxPointsPerTrail=50
	DistanceRange=(Min=1.0000000,Max=100.0000000)
	MaxTrailTwistAngle=16384
}