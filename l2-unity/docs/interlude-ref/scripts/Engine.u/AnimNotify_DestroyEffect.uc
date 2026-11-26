class AnimNotify_DestroyEffect extends AnimNotify
	native
	editinlinenew
	collapsecategories
	hidecategories(Object);

cpptext
{
	// AnimNotify interface.
	virtual void Notify( UMeshInstance *Instance, AActor *Owner );

}

var() name DestroyTag;
var() bool bExpireParticles;

defaultproperties
{
	bExpireParticles=true
}