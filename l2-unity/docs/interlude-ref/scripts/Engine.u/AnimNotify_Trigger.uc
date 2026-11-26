class AnimNotify_Trigger extends AnimNotify_Scripted
	editinlinenew
	collapsecategories
	hidecategories(Object);

var() name EventName;

event Notify(Actor Owner)
{
	Owner.TriggerEvent(EventName, Owner, Pawn(Owner));
	return;
}
