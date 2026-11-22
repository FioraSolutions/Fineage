class MessagingSpectator extends PlayerController
	abstract
	config(User)
	notplaceable;

function PostBeginPlay()
{
	super.PostBeginPlay();
	bIsPlayer = false;
	return;
}
