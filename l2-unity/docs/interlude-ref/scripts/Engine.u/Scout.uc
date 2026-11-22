class Scout extends Pawn
	native
	config(User)
	notplaceable;

var const float MaxLandingVelocity;

function PreBeginPlay()
{
	__NFUN_279__();
	return;
}

defaultproperties
{
	AccelRate=1.0000000
	CollisionRadius=11.0000000
	CollisionHeight=28.0000000
	bCollideActors=false
	bCollideWorld=false
	bBlockActors=false
	bBlockPlayers=false
	bProjTarget=false
	bPathColliding=true
}