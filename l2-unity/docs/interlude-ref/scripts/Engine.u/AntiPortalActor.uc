class AntiPortalActor extends Actor
	native
	placeable;

state() TriggerControl
{
	event Trigger(Actor Other, Pawn EventInstigator)
	{
		SetDrawType(0);
		return;
	}

	event UnTrigger(Actor Other, Pawn EventInstigator)
	{
		SetDrawType(11);
		return;
	}
	stop;
}

state() TriggerToggle
{
	event Trigger(Actor Other, Pawn EventInstigator)
	{
		// End:0x1B
		if(__NFUN_154__(int(DrawType), int(11)))
		{
			SetDrawType(0);			
		}
		else
		{
			// End:0x33
			if(__NFUN_154__(int(DrawType), int(0)))
			{
				SetDrawType(11);
			}
		}
		return;
	}
	stop;
}

defaultproperties
{
	DrawType=11
	bNoDelete=true
	RemoteRole=0
	bEdShouldSnap=true
}