class TexRotator extends TexModifier
	native
	editinlinenew
	collapsecategories
	hidecategories(Object,Material);

cpptext
{
	// UTexModifier interface
	virtual FMatrix* GetMatrix(FLOAT TimeSeconds);
	void PostLoad()
	{
		Super::PostLoad();
		//!!OLDVER
		if( ConstantRotation )
		{
			ConstantRotation = 0;
			TexRotationType = TR_ConstantlyRotating;
		}
	}

}

enum ETexRotationType
{
	TR_FixedRotation,               // 0
	TR_ConstantlyRotating,          // 1
	TR_OscillatingRotation          // 2
};

var() TexRotator.ETexRotationType TexRotationType;
var() Rotator Rotation;
var deprecated bool ConstantRotation;
var() float UOffset;
var() float VOffset;
var() Rotator OscillationRate;
var() Rotator OscillationAmplitude;
var() Rotator OscillationPhase;
var Matrix M;
