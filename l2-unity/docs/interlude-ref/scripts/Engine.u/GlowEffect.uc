class GlowEffect extends CameraEffect
	native
	editinlinenew
	collapsecategories
	noexport;

var const int RenderTargets[5];
var float Luminance;
var float MiddleGray;
var float WhiteCutoff;
var float Threshold;
var float BloomScale;
var int BlurNum;
var int GlowType;
var float RGBCutoff;
var int FinalBlendBlurType;
var int FinalBlendOpacity;
var int AspectBlendOpacity;

defaultproperties
{
	Luminance=0.0800000
	MiddleGray=0.1800000
	WhiteCutoff=0.8000000
	Threshold=0.3500000
	BloomScale=1.5000000
	BlurNum=2
	GlowType=1
	RGBCutoff=0.8000000
	FinalBlendBlurType=1
	FinalBlendOpacity=85
	AspectBlendOpacity=192
}