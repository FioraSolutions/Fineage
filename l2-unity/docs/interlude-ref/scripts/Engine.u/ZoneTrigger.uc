class ZoneTrigger extends Trigger
	placeable;

function Touch(Actor Other)
{
	local ZoneInfo Z;

	// End:0x9F
	if(IsRelevant(Other))
	{
		// End:0x69
		if(__NFUN_255__(Event, 'None'))
		{
			// End:0x68
			foreach __NFUN_304__(Class'Engine.ZoneInfo', Z)
			{
				// End:0x67
				if(__NFUN_254__(Z.ZoneTag, Event))
				{
					Z.Trigger(Other, Other.Instigator);
				}				
			}			
		}
		// End:0x92
		if(__NFUN_123__(Message, ""))
		{
			Other.Instigator.ClientMessage(Message);
		}
		// End:0x9F
		if(bTriggerOnceOnly)
		{
			__NFUN_262__(false);
		}
	}
	return;
}

function UnTouch(Actor Other)
{
	local ZoneInfo Z;

	// End:0x69
	if(IsRelevant(Other))
	{
		// End:0x69
		if(__NFUN_255__(Event, 'None'))
		{
			// End:0x68
			foreach __NFUN_304__(Class'Engine.ZoneInfo', Z)
			{
				// End:0x67
				if(__NFUN_254__(Z.ZoneTag, Event))
				{
					Z.UnTrigger(Other, Other.Instigator);
				}				
			}			
		}
	}
	return;
}
