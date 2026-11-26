class SubActionCameraEffect extends MatSubAction
	native
	editinlinenew
	collapsecategories
	noexport;

var() editinline CameraEffect CameraEffect;
var() float StartAlpha;
var() float EndAlpha;
var() bool DisableAfterDuration;

defaultproperties
{
	EndAlpha=1.0000000
	Icon=Texture'Engine.SubActionFade'
	Desc="Camera effect"
}