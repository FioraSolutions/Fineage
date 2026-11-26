class ConstantMaterial extends RenderedMaterial
	abstract
	native
	editinlinenew
	collapsecategories
	hidecategories(Object);

cpptext
{
	//
	// UConstantMaterial interface
	//
	virtual FColor GetColor(FLOAT TimeSeconds) { return FColor(0,0,0,0); }

}
