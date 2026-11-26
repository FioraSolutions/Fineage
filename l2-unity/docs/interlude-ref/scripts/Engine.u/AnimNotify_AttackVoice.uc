class AnimNotify_AttackVoice extends AnimNotify
	native
	editinlinenew
	collapsecategories
	hidecategories(Object);

cpptext
{
	// AnimNotify interface.
	virtual void Notify( UMeshInstance *Instance, AActor *Owner );

}

var() float Volume;
var() float Radius;
var() int Random;
