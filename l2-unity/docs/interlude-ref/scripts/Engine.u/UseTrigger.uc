class UseTrigger extends Triggers
	placeable;

var() localized string Message;

function UsedBy(Pawn User)
{
	TriggerEvent(Event, self, User);
	return;
}

function Touch(Actor Other)
{
	// End:0x3F
	if(__NFUN_130__(__NFUN_123__(Message, ""), __NFUN_119__(Other.Instigator, none)))
	{
		Other.Instigator.ClientMessage(Message);
	}
	return;
}
