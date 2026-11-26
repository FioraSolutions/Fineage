class TexPanner extends TexModifier
	native
	editinlinenew
	collapsecategories
	hidecategories(Object,Material);

cpptext
{
	// UTexModifier interface
	virtual FMatrix* GetMatrix(FLOAT TimeSeconds);

}

var() Rotator PanDirection;
var() float PanRate;
var Matrix M;

defaultproperties
{
	PanRate=0.1000000
}