class MaterialTrigger extends Triggers
	placeable;

var() array<Material> MaterialsToTrigger;

function PostBeginPlay()
{
	local int i;

	i = 0;
	J0x07:

	// End:0x47 [Loop If]
	if(__NFUN_150__(i, MaterialsToTrigger.Length))
	{
		// End:0x3D
		if(__NFUN_119__(MaterialsToTrigger[i], none))
		{
			MaterialsToTrigger[i].Reset();
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x07;
	}
	return;
}

function Trigger(Actor Other, Pawn EventInstigator)
{
	local int i;

	// End:0x12
	if(__NFUN_114__(Other, none))
	{
		Other = self;
	}
	i = 0;
	J0x19:

	// End:0x63 [Loop If]
	if(__NFUN_150__(i, MaterialsToTrigger.Length))
	{
		// End:0x59
		if(__NFUN_119__(MaterialsToTrigger[i], none))
		{
			MaterialsToTrigger[i].Trigger(Other, EventInstigator);
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x19;
	}
	return;
}

defaultproperties
{
	Texture=Texture'Engine.S_MaterialTrigger'
	bCollideActors=false
}