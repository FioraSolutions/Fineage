class RibbonEmitter extends ParticleEmitter
	native
	editinlinenew;

enum EGetPointAxis
{
	PAXIS_OwnerX,                   // 0
	PAXIS_OwnerY,                   // 1
	PAXIS_OwnerZ,                   // 2
	PAXIS_BoneNormal,               // 3
	PAXIS_StartBoneDirection,       // 4
	PAXIS_AxisNormal                // 5
};

enum EAccDrop
{
	ADRP_NONE,                      // 0
	ADRP_BYPOINT,                   // 1
	ADRP_BYTIME,                    // 2
	ADRP_BYTIME_DUAL                // 3
};

struct RibbonPoint
{
	var() Vector Location;
	var() Vector LocationUnder;
	var() Vector MovingDirection;
	var() Vector MovingDirectionUnder;
	var() float Width;
};

var(Ribbon) float SampleRate;
var(Ribbon) float DecayRate;
var(Ribbon) int NumPoints;
var(RibbonAccDrop) RibbonEmitter.EAccDrop AccDrop;
var(RibbonAccDrop) int PointsDropRate;
var(RibbonAccDrop) float ThresholdTime;
var(RibbonAccDrop) int MinPoints;
var(RibbonAccDrop) float ThresholdTime2;
var(Ribbon) float RibbonWidth;
var(Ribbon) RibbonEmitter.EGetPointAxis GetPointAxisFrom;
var(Ribbon) Vector AxisNormal;
var(Ribbon) float MinSampleDist;
var(Ribbon) float MinSampleDot;
var(Ribbon) float PointOriginOffset;
var(RibbonTexture) float RibbonTextureUScale;
var(RibbonTexture) float RibbonTextureVScale;
var(RibbonSheets) int NumSheets;
var(RibbonSheets) array<float> SheetScale;
var(RibbonBones) Vector StartBoneOffset;
var(RibbonBones) Vector EndBoneOffset;
var(RibbonBones) name BoneNameStart;
var(RibbonBones) name BoneNameEnd;
var(Ribbon) array<RibbonPoint> RibbonPoints;
var(RibbonInterpolation) bool bUseInterpolation;
var(RibbonInterpolation) int CntrPoints;
var(RibbonInterpolation) float ScaleRatio;
var(Ribbon) bool bSamplePoints;
var(Ribbon) bool bDecayPoints;
var(Ribbon) bool bDecayPointsWhenStopped;
var(Ribbon) bool bSyncDecayWhenKilled;
var(RibbonTexture) bool bLengthBasedTextureU;
var(RibbonSheets) bool bUseSheetScale;
var(RibbonBones) bool bUseBones;
var(RibbonBones) bool bUseBoneDistance;
var transient float SampleTimer;
var transient float DecayTimer;
var transient float RealSampleRate;
var transient float RealDecayRate;
var transient int SheetsUsed;
var transient RibbonPoint LastSampledPoint;
var transient bool bKilled;
var transient bool bDecaying;

defaultproperties
{
	SampleRate=0.0500000
	NumPoints=20
	RibbonWidth=20.0000000
	GetPointAxisFrom=5
	AxisNormal=(X=0.0000000,Y=0.0000000,Z=1.0000000)
	MinSampleDist=1.0000000
	MinSampleDot=0.9950000
	PointOriginOffset=0.5000000
	RibbonTextureUScale=1.0000000
	RibbonTextureVScale=1.0000000
	CntrPoints=5
	ScaleRatio=3.0000000
	bSamplePoints=true
	bDecayPoints=true
	MaxParticles=1
	UseRegularSizeScale=false
	StartSizeRange=(X=(Min=1.0000000,Max=1.0000000),Y=(Min=1.0000000,Max=1.0000000),Z=(Min=1.0000000,Max=1.0000000))
	InitialParticlesPerSecond=10000.0000000
	AutomaticInitialSpawning=false
}