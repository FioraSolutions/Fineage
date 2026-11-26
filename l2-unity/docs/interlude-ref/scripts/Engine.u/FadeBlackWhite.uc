class FadeBlackWhite extends CameraEffect
	native
	editinlinenew
	collapsecategories
	noexport;

var() float FadeTime;
var() float FadeDir;
var const float LastAlpha;
var const float LastFrameTime;

defaultproperties
{
	FadeTime=2.0000000
	FadeDir=1.0000000
}