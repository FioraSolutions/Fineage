class ShadowBitmapMaterial extends BitmapMaterial
	native
	collapsecategories
	hidecategories(Object);

cpptext
{
	virtual void Destroy();

	virtual FBaseTexture* GetRenderInterface();
	virtual UBitmapMaterial* Get(FTime Time,UViewport* Viewport);

}

var const transient int TextureInterfaces[2];
var const transient int ShadowInterface;
var Actor ShadowActor;
var Vector LightDirection;
var float LightDistance;
var float LightFOV;
var bool Dirty;
var bool Invalid;
var bool bBlobShadow;
var float CullDistance;
var byte ShadowDarkness;
var BitmapMaterial BlobShadow;
var bool bDefaultShadow;
var int ShadowIndex;
var int ShadowTextureRevision;

// Export UShadowBitmapMaterial::execDestroy(FFrame&, void* const)
native final function bool Destroy();

defaultproperties
{
	Dirty=true
	ShadowDarkness=255
	BlobShadow=Texture'Engine.BlobTexture'
	ShadowIndex=-1
	ShadowTextureRevision=-1
	Format=5
	UClampMode=1
	VClampMode=1
	UBits=7
	VBits=7
	USize=256
	VSize=256
	UClamp=256
	VClamp=256
}