class VolumeTimer extends Info
	notplaceable
	hidecategories(Movement,Collision,Lighting,LightColor,Karma,Force);

var PhysicsVolume V;

function PostBeginPlay()
{
	super(Actor).PostBeginPlay();
	__NFUN_280__(1.0000000, true);
	V = PhysicsVolume(Owner);
	return;
}

function Timer()
{
	V.TimerPop(self);
	return;
}
