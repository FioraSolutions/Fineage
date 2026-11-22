class TexMatrix extends TexModifier
	native
	collapsecategories
	hidecategories(Object,Material);

cpptext
{
	// UTexModifier interface
	virtual FMatrix* GetMatrix(FLOAT TimeSeconds) { return &Matrix; }

}

var Matrix Matrix;
