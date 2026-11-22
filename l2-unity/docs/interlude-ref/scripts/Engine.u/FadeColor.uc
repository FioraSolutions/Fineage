class FadeColor extends ConstantMaterial
	native
	editinlinenew
	collapsecategories
	hidecategories(Object);

cpptext
{
	//
	// UConstantMaterial interface
	//
	virtual FColor GetColor(FLOAT TimeSeconds);

}

enum EColorFadeType
{
	FC_Linear,                      // 0
	FC_Sinusoidal                   // 1
};

var() Color Color1;
var() Color Color2;
var() float FadePeriod;
var() float FadePhase;
var() FadeColor.EColorFadeType ColorFadeType;
