class Material extends Object
	native
	collapsecategories
	noexport
	hidecategories(Object);

var() Material FallbackMaterial;
var Material DefaultMaterial;
var const transient bool UseFallback;
var const transient bool Validated;
var int Reserved;
var byte TextureTranform;
var byte MAX_SAMPLER_NUM;
var byte MAX_TEXMAT_NUM;
var byte MAX_PASS_NUM;
var byte TwoPassRenderState;
var byte AlphaRef;
var int SrcBlend;
var int DestBlend;
var int OverriddenFogColor;
var Matrix matTexMatrix[16];
var int FC_Color1;
var int FC_Color2;
var int FC_FadePeriod;
var int FC_FadePhase;
var int FC_ColorFadeType;
var int BitmapMaterials[16];
var string strTex[16];
var string ShaderCode;

function Reset()
{
	// End:0x1A
	if(__NFUN_119__(FallbackMaterial, none))
	{
		FallbackMaterial.Reset();
	}
	return;
}

function Trigger(Actor Other, Actor EventInstigator)
{
	// End:0x24
	if(__NFUN_119__(FallbackMaterial, none))
	{
		FallbackMaterial.Trigger(Other, EventInstigator);
	}
	return;
}

defaultproperties
{
	DefaultMaterial=Texture'Engine.DefaultTexture'
}