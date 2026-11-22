class AnimNotify extends Object
	abstract
	native
	editinlinenew
	collapsecategories
	hidecategories(Object);

cpptext
{
	// AnimNotify interface.
	virtual void Notify( UMeshInstance *Instance, AActor *Owner ) {};
	virtual FString ExportToScript();
	// UObject interface.
	virtual void PostEditChange();

}

var transient int Revision;
