class SavedMove extends Info
	native
	notplaceable
	hidecategories(Movement,Collision,Lighting,LightColor,Karma,Force);

var SavedMove NextMove;
var float TimeStamp;
var float Delta;
var bool bRun;
var bool bDuck;
var bool bPressedJump;
var bool bDoubleJump;
var Actor.EDoubleClickDir DoubleClickMove;
var Actor.EPhysics SavedPhysics;
var Vector SavedLocation;
var Vector SavedVelocity;

final function Clear()
{
	TimeStamp = 0.0000000;
	Delta = 0.0000000;
	DoubleClickMove = 0;
	Acceleration = vect(0.0000000, 0.0000000, 0.0000000);
	bRun = false;
	bDuck = false;
	bPressedJump = false;
	bDoubleJump = false;
	return;
}

final function PostUpdate(PlayerController P)
{
	bDoubleJump = __NFUN_132__(P.bDoubleJump, bDoubleJump);
	// End:0x6F
	if(__NFUN_119__(P.Pawn, none))
	{
		SavedLocation = P.Pawn.Location;
		SavedVelocity = P.Pawn.Velocity;
	}
	return;
}

final function SetMoveFor(PlayerController P, float DeltaTime, Vector NewAccel, Actor.EDoubleClickDir InDoubleClick)
{
	// End:0x29
	if(__NFUN_177__(__NFUN_225__(NewAccel), float(3072)))
	{
		NewAccel = __NFUN_213__(float(3072), __NFUN_226__(NewAccel));
	}
	// End:0x67
	if(__NFUN_177__(Delta, float(0)))
	{
		Acceleration = __NFUN_214__(__NFUN_215__(__NFUN_213__(DeltaTime, NewAccel), __NFUN_213__(Delta, Acceleration)), __NFUN_174__(Delta, DeltaTime));		
	}
	else
	{
		// End:0x98
		if(__NFUN_119__(P.Pawn, none))
		{
			SavedPhysics = P.Pawn.Physics;
		}
		Acceleration = NewAccel;
	}
	__NFUN_184__(Delta, DeltaTime);
	// End:0xCA
	if(__NFUN_154__(int(DoubleClickMove), int(0)))
	{
		DoubleClickMove = InDoubleClick;
	}
	bRun = __NFUN_151__(int(P.bRun), 0);
	bDuck = __NFUN_151__(int(P.bDuck), 0);
	bPressedJump = __NFUN_132__(P.bPressedJump, bPressedJump);
	bDoubleJump = __NFUN_132__(P.bDoubleJump, bDoubleJump);
	TimeStamp = Level.TimeSeconds;
	return;
}
