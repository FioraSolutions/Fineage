class AnimNotify_ScreenFade extends AnimNotify
	native
	editinlinenew
	collapsecategories
	hidecategories(Object);

cpptext
{
	// AnimNotify interface.
	virtual void Notify( UMeshInstance *Instance, AActor *Owner );

}

var() float FadeOutDuration;
var() Color FadeOutColor;
var() float BlackOutDuration;
var() float FadeInDuration;

defaultproperties
{
	FadeOutDuration=3000.0000000
	FadeOutColor=(R=255,G=255,B=255,A=255)
	FadeInDuration=1000.0000000
}