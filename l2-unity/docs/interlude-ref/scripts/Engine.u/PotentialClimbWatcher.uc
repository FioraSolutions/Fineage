class PotentialClimbWatcher extends Info
	native
	notplaceable
	hidecategories(Movement,Collision,Lighting,LightColor,Karma,Force);

simulated function Tick(float DeltaTime)
{
	local Rotator PawnRot;
	local LadderVolume L;
	local bool bFound;

	// End:0x3F
	if(__NFUN_132__(__NFUN_132__(__NFUN_114__(Owner, none), Owner.bDeleteMe), __NFUN_129__(Pawn(Owner).CanGrabLadder())))
	{
		__NFUN_279__();
		return;
	}
	PawnRot = Owner.Rotation;
	PawnRot.Pitch = 0;
	// End:0xDB
	foreach Owner.__NFUN_307__(Class'Engine.LadderVolume', L)
	{
		// End:0xDA
		if(L.Encompasses(Owner))
		{
			// End:0xD2
			if(__NFUN_177__(__NFUN_219__(Vector(PawnRot), L.LookDir), 0.9000000))
			{
				Pawn(Owner).ClimbLadder(L);
				__NFUN_279__();				
				return;
				// End:0xDA
				continue;
			}
			bFound = true;
		}		
	}	
	// End:0xEA
	if(__NFUN_129__(bFound))
	{
		__NFUN_279__();
	}
	return;
}
