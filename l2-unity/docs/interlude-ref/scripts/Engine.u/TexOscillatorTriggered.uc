class TexOscillatorTriggered extends TexOscillator
	native
	editinlinenew
	collapsecategories
	hidecategories(Object,Material);

cpptext
{
	// UTexModifier interface
	virtual FMatrix* GetMatrix(FLOAT TimeSeconds);

}

enum ERetriggerAction
{
	RTA_Reverse,                    // 0
	RTA_Reset,                      // 1
	RTA_Ignore,                     // 2
	RTA_Retrigger                   // 3
};

var() TexOscillatorTriggered.ERetriggerAction RetriggerAction;
var() float StopAfterPeriod;
var transient float TriggeredTime;
var transient bool Reverse;
var transient bool Triggered;

function Trigger(Actor Other, Actor EventInstigator)
{
	// End:0x6E
	if(Triggered)
	{
		switch(RetriggerAction)
		{
			// End:0x45
			case 0:
				Triggered = false;
				TriggeredTime = Other.Level.TimeSeconds;
				Reverse = true;
				// End:0x6B
				break;
			// End:0x68
			case 1:
				Triggered = false;
				TriggeredTime = -1.0000000;
				Reverse = true;
				// End:0x6B
				break;
			// End:0xFFFF
			default:
				break;
		}		
	}
	else
	{
		// End:0x86
		if(__NFUN_155__(int(RetriggerAction), int(3)))
		{
			Triggered = true;
		}
		TriggeredTime = Other.Level.TimeSeconds;
		Reverse = false;
	}
	return;
}

function Reset()
{
	Triggered = false;
	TriggeredTime = -1.0000000;
	Reverse = false;
	return;
}

defaultproperties
{
	RetriggerAction=3
	StopAfterPeriod=0.5000000
	UOscillationRate=0.0000000
	VOscillationRate=0.5000000
	VOscillationPhase=0.2500000
	UOscillationAmplitude=0.0000000
	VOscillationAmplitude=0.5000000
}