class TexCoordSource extends TexModifier
	native
	editinlinenew
	collapsecategories
	hidecategories(Object,Material);

cpptext
{
	void PostEditChange();

}

var() int SourceChannel;

defaultproperties
{
	TexCoordSource=0
}