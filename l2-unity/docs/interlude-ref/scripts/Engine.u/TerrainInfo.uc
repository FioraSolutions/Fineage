class TerrainInfo extends Info
	native
	noexport
	placeable;

enum ETexMapAxis
{
	TEXMAPAXIS_XY,                  // 0
	TEXMAPAXIS_XZ,                  // 1
	TEXMAPAXIS_YZ                   // 2
};

enum ESortOrder
{
	SORT_NoSort,                    // 0
	SORT_BackToFront,               // 1
	SORT_FrontToBack                // 2
};

struct NormalPair
{
	var Vector Normal1;
	var Vector Normal2;
};

struct TerrainLayer
{
	var() Material Texture;
	var() Texture AlphaMap;
	var() float UScale;
	var() float VScale;
	var() float UPan;
	var() float VPan;
	var() TerrainInfo.ETexMapAxis TextureMapAxis;
	var() float TextureRotation;
	var() Rotator LayerRotation;
	var Matrix TerrainMatrix;
	var() float KFriction;
	var() float KRestitution;
	var Texture LayerWeightMap;
	var Vector Scale;
	var Vector ToWorld[4];
	var Vector ToMaskmap[4];
	var() bool bUseAlpha;
};

struct DecorationLayer
{
	var() int ShowOnTerrain;
	var() Texture ScaleMap;
	var() Texture DensityMap;
	var() Texture ColorMap;
	var() StaticMesh StaticMesh;
	var() RangeVector ScaleMultiplier;
	var() Range FadeoutRadius;
	var() Range DensityMultiplier;
	var() int MaxPerQuad;
	var() int Seed;
	var() int AlignToTerrain;
	var() TerrainInfo.ESortOrder DrawOrder;
	var() int ShowOnInvisibleTerrain;
	var() int LitDirectional;
	var() int DisregardTerrainLighting;
	var() int RandomYaw;
	var() int bForceRender;
};

struct DecoInfo
{
	var Vector Location;
	var Rotator Rotation;
	var Vector Scale;
	var Vector TempScale;
	var Color Color;
	var int Distance;
	var Rotator OrigRotation;
	var int VertexPos;
	var float Speed;
	var float Accel;
};

struct DecoSectorInfo
{
	var array<DecoInfo> DecoInfo;
	var Vector Location;
	var float Radius;
	var int bDecoGenerated;
};

struct DecorationLayerData
{
	var array<DecoSectorInfo> Sectors;
};

struct TerrainIntensityMap
{
	var float Time;
	var array<byte> Intensity;
};

var() int TerrainSectorSize;
var() Texture TerrainMap;
var() Vector TerrainScale;
var() TerrainLayer Layers[32];
var() array<DecorationLayer> DecoLayers;
var() float DecoLayerOffset;
var() bool Inverted;
var() bool bKCollisionHalfRes;
var transient int JustLoaded;
var native const array<DecorationLayerData> DecoLayerData;
var native const array<TerrainSector> Sectors;
var native const array<Vector> Vertices;
var native const int HeightmapX;
var native const int HeightmapY;
var native const int SectorsX;
var native const int SectorsY;
var native const TerrainPrimitive Primitive;
var native const array<NormalPair> FaceNormals;
var native const Vector ToWorld[4];
var native const Vector ToHeightmap[4];
var native const array<int> SelectedVertices;
var native const int ShowGrid;
var const array<int> QuadVisibilityBitmap;
var const array<int> EdgeTurnBitmap;
var const array<Material> QuadDomMaterialBitmap;
var native const array<int> RenderCombinations;
var native const array<int> VertexStreams;
var native const array<Color> VertexColors;
var native const array<Color> PaintedColor;
var native const Texture OldTerrainMap;
var native const array<byte> OldHeightmap;
var() int BaseHeight;
var int VTGruop;
var int VTGroupOrig;
var int MapX;
var int MapY;
var int bUpdatedHEdge;
var int bUpdatedVEdge;
var int bUpdatedZ;
var array<int> SectorsOrig;
var native const Vector ToHeightmapOrig[4];
var const array<int> QuadVisibilityBitmapOrig;
var const array<int> EdgeTurnBitmapOrig;
var int GeneratedSectorCounter;
var int NumIntMap;
var bool bAutoTimeGeneration;
var int NightMapStart;
var int DayMapStart;
var array<TerrainIntensityMap> TIntMap;
var float TickTime;

defaultproperties
{
	TerrainSectorSize=16
	TerrainScale=(X=64.0000000,Y=64.0000000,Z=64.0000000)
	bStatic=true
	bWorldGeometry=true
	Texture=Texture'Engine.S_TerrainInfo'
	bStaticLighting=true
}