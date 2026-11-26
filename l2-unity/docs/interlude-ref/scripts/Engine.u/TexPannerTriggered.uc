class TexPannerTriggered extends TexPanner
	native
	editinlinenew
	collapsecategories
	hidecategories(Object,Material);

cpptext
{
	// UTexModifier interface
	virtual FMatrix* GetMatrix(FLOAT TimeSeconds);

}

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
}