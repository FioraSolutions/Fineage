class TerrainMaterial extends RenderedMaterial
	native
	collapsecategories
	hidecategories(Object);

cpptext
{
	virtual UMaterial* CheckFallback();
	virtual UBOOL HasFallback();

}

struct TerrainMaterialLayer
{
	var Material Texture;
	var BitmapMaterial AlphaWeight;
	var Matrix TextureMatrix;
};

var const array<TerrainMaterialLayer> Layers;
var const byte RenderMethod;
var const bool FirstPass;
