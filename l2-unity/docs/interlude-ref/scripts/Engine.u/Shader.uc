class Shader extends RenderedMaterial
	native
	editinlinenew
	collapsecategories
	hidecategories(Object);

enum EOutputBlending
{
	OB_Normal,                      // 0
	OB_Masked,                      // 1
	OB_Modulate,                    // 2
	OB_Translucent,                 // 3
	OB_Invisible,                   // 4
	OB_Brighten,                    // 5
	OB_Darken                       // 6
};

var() editinlineuse Material Diffuse;
var() editinlineuse Material Opacity;
var() editinlineuse Material Specular;
var() editinlineuse Material SpecularityMask;
var() editinlineuse Material SelfIllumination;
var() editinlineuse Material SelfIlluminationMask;
var() editinlineuse Material Detail;
var() float DetailScale;
var() Shader.EOutputBlending OutputBlending;
var() bool TwoSided;
var() bool Wireframe;
var bool ModulateStaticLighting2X;
var() bool PerformLightingOnSpecularPass;
var() bool TreatAsTwoSided;
var() bool ZWrite;
var() bool AlphaTest;
var() byte AlphaRef;
var() editinlineuse Material NormalMap;
var() float BumpOffsetScaleFactor;
var() float BumpOffsetBiasFactor;

function Reset()
{
	// End:0x1A
	if(__NFUN_119__(Diffuse, none))
	{
		Diffuse.Reset();
	}
	// End:0x34
	if(__NFUN_119__(Opacity, none))
	{
		Opacity.Reset();
	}
	// End:0x4E
	if(__NFUN_119__(Specular, none))
	{
		Specular.Reset();
	}
	// End:0x68
	if(__NFUN_119__(SpecularityMask, none))
	{
		SpecularityMask.Reset();
	}
	// End:0x82
	if(__NFUN_119__(SelfIllumination, none))
	{
		SelfIllumination.Reset();
	}
	// End:0x9C
	if(__NFUN_119__(SelfIlluminationMask, none))
	{
		SelfIlluminationMask.Reset();
	}
	// End:0xB6
	if(__NFUN_119__(FallbackMaterial, none))
	{
		FallbackMaterial.Reset();
	}
	// End:0xD0
	if(__NFUN_119__(NormalMap, none))
	{
		NormalMap.Reset();
	}
	return;
}

function Trigger(Actor Other, Actor EventInstigator)
{
	// End:0x24
	if(__NFUN_119__(Diffuse, none))
	{
		Diffuse.Trigger(Other, EventInstigator);
	}
	// End:0x48
	if(__NFUN_119__(Opacity, none))
	{
		Opacity.Trigger(Other, EventInstigator);
	}
	// End:0x6C
	if(__NFUN_119__(Specular, none))
	{
		Specular.Trigger(Other, EventInstigator);
	}
	// End:0x90
	if(__NFUN_119__(SpecularityMask, none))
	{
		SpecularityMask.Trigger(Other, EventInstigator);
	}
	// End:0xB4
	if(__NFUN_119__(SelfIllumination, none))
	{
		SelfIllumination.Trigger(Other, EventInstigator);
	}
	// End:0xD8
	if(__NFUN_119__(SelfIlluminationMask, none))
	{
		SelfIlluminationMask.Trigger(Other, EventInstigator);
	}
	// End:0xFC
	if(__NFUN_119__(FallbackMaterial, none))
	{
		FallbackMaterial.Trigger(Other, EventInstigator);
	}
	// End:0x120
	if(__NFUN_119__(NormalMap, none))
	{
		NormalMap.Trigger(Other, EventInstigator);
	}
	return;
}

defaultproperties
{
	DetailScale=8.0000000
}