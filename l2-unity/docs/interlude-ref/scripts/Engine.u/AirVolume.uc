class AirVolume extends Volume
	native
	nativereplication
	notplaceable;

var() name EffectName;
var() float FullFadeSeconds;
var() float RelativeOffset;

simulated function PostBeginPlay()
{
	super.PostBeginPlay();
	return;
}

defaultproperties
{
	FullFadeSeconds=60.0000000
	DrawType=14
}