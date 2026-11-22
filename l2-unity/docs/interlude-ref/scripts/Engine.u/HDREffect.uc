class HDREffect extends CameraEffect
	native
	editinlinenew
	collapsecategories
	noexport;

var const int RenderTargets[7];
var float HDRTimeStamp;
var int idxCurLum;
var float GrayLum;
var float FinalCoef;
var float ExpBase;
var float ExpCoef;
var float ClampMin;
var float ClampMax;

defaultproperties
{
	idxCurLum=5
	GrayLum=0.6000000
	FinalCoef=3.5000000
	ExpBase=0.9800000
	ExpCoef=30.0000000
	ClampMin=0.3000000
	ClampMax=0.7500000
}