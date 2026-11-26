class BitmapMaterial extends RenderedMaterial
	abstract
	native
	collapsecategories
	noexport
	hidecategories(Object);

enum ETextureFormat
{
	TEXF_P8,                        // 0
	TEXF_RGBA7,                     // 1
	TEXF_RGB16,                     // 2
	TEXF_DXT1,                      // 3
	TEXF_RGB8,                      // 4
	TEXF_RGBA8,                     // 5
	TEXF_NODATA,                    // 6
	TEXF_DXT3,                      // 7
	TEXF_DXT5,                      // 8
	TEXF_L8,                        // 9
	TEXF_G16,                       // 10
	TEXF_RRRGGGBBB                  // 11
};

enum ETexClampMode
{
	TC_Wrap,                        // 0
	TC_Clamp                        // 1
};

var(TextureFormat) const editconst BitmapMaterial.ETextureFormat Format;
var(Texture) BitmapMaterial.ETexClampMode UClampMode;
var(Texture) BitmapMaterial.ETexClampMode VClampMode;
var const byte UBits;
var const byte VBits;
var const int USize;
var const int VSize;
var(Texture) const int UClamp;
var(Texture) const int VClamp;
var int LossDetail;
