class Texture extends BitmapMaterial
	native
	noexport
	safereplace
	hidecategories(Object);

enum EEnvMapTransformType
{
	EMTT_ViewSpace,                 // 0
	EMTT_WorldSpace,                // 1
	EMTT_LightSpace                 // 2
};

enum ELODSet
{
	LODSET_None,                    // 0
	LODSET_World,                   // 1
	LODSET_PlayerSkin,              // 2
	LODSET_WeaponSkin,              // 3
	LODSET_Terrain,                 // 4
	LODSET_Interface,               // 5
	LODSET_RenderMap,               // 6
	LODSET_Lightmap                 // 7
};

var() Palette Palette;
var() Material Detail;
var() float DetailScale;
var const Color MipZero;
var const Color MaxColor;
var const int InternalTime[2];
var deprecated Texture DetailTexture;
var deprecated Texture EnvironmentMap;
var deprecated Texture.EEnvMapTransformType EnvMapTransformType;
var deprecated float Specular;
var(Surface) bool bMasked;
var(Surface) bool bAlphaTexture;
var(Surface) bool bTwoSided;
var(Quality) private bool bHighColorQuality;
var(Quality) private bool bHighTextureQuality;
var private bool bRealtime;
var private bool bParametric;
var private transient bool bRealtimeChanged;
var private const editconst bool bHasComp;
var() Texture.ELODSet LODSet;
var() int NormalLOD;
var int MinLOD;
var transient int MaxLOD;
var(Animation) Texture AnimNext;
var transient Texture AnimCurrent;
var(Animation) byte PrimeCount;
var transient byte PrimeCurrent;
var(Animation) float MinFrameRate;
var(Animation) float MaxFrameRate;
var(Animation) int TotalFrameNum;
var transient float Accumulator;
var native const array<int> Mips;
var const editconst BitmapMaterial.ETextureFormat CompFormat;
var const transient int RenderInterface;
var const transient int __LastUpdateTime[2];

defaultproperties
{
	DetailScale=8.0000000
	MipZero=(R=64,G=128,B=64,A=0)
	MaxColor=(R=255,G=255,B=255,A=255)
	LODSet=1
}