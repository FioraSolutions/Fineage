class ScriptedTexture extends BitmapMaterial
	native
	collapsecategories
	hidecategories(Object);

cpptext
{
	void Render(FRenderInterface* RI);

	virtual UBitmapMaterial* Get(FTime Time,UViewport* Viewport);
	virtual FBaseTexture* GetRenderInterface();
	virtual void Destroy();
	virtual void PostEditChange();

}

var const transient int RenderTarget;
var const transient Viewport RenderViewport;
var Actor Client;
var transient int Revision;
var const transient int OldRevision;

// Export UScriptedTexture::execSetSize(FFrame&, void* const)
native final function SetSize(int Width, int Height);

// Export UScriptedTexture::execDrawText(FFrame&, void* const)
native final function DrawText(int StartX, int StartY, coerce string Text, Font Font, Color Color);

// Export UScriptedTexture::execTextSize(FFrame&, void* const)
native final function TextSize(coerce string Text, Font Font, out int Width, out int Height);

// Export UScriptedTexture::execDrawTile(FFrame&, void* const)
native final function DrawTile(float X, float Y, float XL, float YL, float U, float V, float UL, float VL, Material Material, Color Color);

// Export UScriptedTexture::execDrawPortal(FFrame&, void* const)
native final function DrawPortal(int X, int Y, int Width, int Height, Actor CamActor, Vector CamLocation, Rotator CamRotation, optional int FOV, optional bool ClearZ);
