class MotionBlur extends CameraEffect
	native
	editinlinenew
	collapsecategories
	noexport;

var() byte BlurAlpha;
var const int RenderTargets[2];
var const float LastFrameTime;

defaultproperties
{
	BlurAlpha=128
}