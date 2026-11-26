class MaterialSequence extends Modifier
	native
	editinlinenew
	collapsecategories
	hidecategories(Object,Material,Modifier);

cpptext
{
	virtual void PostEditChange();
	virtual UBOOL CheckCircularReferences( TArray<class UMaterial*>& History );
	virtual void PreSetMaterial(FLOAT TimeSeconds);
	virtual void Serialize(FArchive& Ar);
	void	Reset();		
	void	Trigger();		

}

enum EMaterialSequenceAction
{
	MSA_ShowMaterial,               // 0
	MSA_FadeToMaterial              // 1
};

enum EMaterialSequenceTriggerActon
{
	MSTA_Ignore,                    // 0
	MSTA_Reset,                     // 1
	MSTA_Pause,                     // 2
	MSTA_Stop                       // 3
};

struct MaterialSequenceItem
{
	var() editinlineuse Material Material;
	var() float Time;
	var() MaterialSequence.EMaterialSequenceAction Action;
};

var() array<MaterialSequenceItem> SequenceItems;
var() MaterialSequence.EMaterialSequenceTriggerActon TriggerAction;
var() bool Loop;
var() bool Paused;
var transient float CurrentTime;
var transient float LastTime;
var float TotalTime;

function Reset()
{
	CurrentTime = 0.0000000;
	LastTime = 0.0000000;
	Paused = default.Paused;
	return;
}

function Trigger(Actor Other, Actor EventInstigator)
{
	switch(TriggerAction)
	{
		// End:0x25
		case 1:
			CurrentTime = 0.0000000;
			LastTime = 0.0000000;
			// End:0x4F
			break;
		// End:0x3C
		case 2:
			Paused = __NFUN_129__(Paused);
			// End:0x4F
			break;
		// End:0x4C
		case 3:
			Paused = true;
			// End:0x4F
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

defaultproperties
{
	Loop=true
}