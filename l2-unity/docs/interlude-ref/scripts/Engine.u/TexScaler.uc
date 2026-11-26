class TexScaler extends TexModifier
	native
	editinlinenew
	collapsecategories
	hidecategories(Object,Material);

cpptext
{
	// UTexModifier interface
	virtual FMatrix* GetMatrix(FLOAT TimeSeconds);

}

var() float UScale;
var() float VScale;
var() float UOffset;
var() float VOffset;
var Matrix M;

defaultproperties
{
	UScale=1.0000000
	VScale=1.0000000
}