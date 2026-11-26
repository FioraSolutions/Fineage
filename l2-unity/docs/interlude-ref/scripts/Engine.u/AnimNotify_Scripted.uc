class AnimNotify_Scripted extends AnimNotify
	abstract
	native
	editinlinenew
	collapsecategories
	hidecategories(Object);

cpptext
{
	// AnimNotify interface.
	virtual void Notify( UMeshInstance *Instance, AActor *Owner );

}

event Notify(Actor Owner)
{
	return;
}
