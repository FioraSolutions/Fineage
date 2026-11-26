class KVehicle extends Pawn
	abstract
	native
	config(User)
	placeable;

cpptext
{
#ifdef WITH_KARMA
	virtual void PostNetReceive();
    virtual void PostEditChange();
	virtual void setPhysics(BYTE NewPhysics, AActor *NewFloor, FVector NewFloorV);
	virtual void TickSimulated( FLOAT DeltaSeconds );
	virtual void TickAuthoritative( FLOAT DeltaSeconds );
#endif


}

const FilterFrames = 5;

var() float Steering;
var() float Throttle;
var Pawn Driver;
var() array<Vector> ExitPositions;
var() Vector DrivePos;
var() Rotator DriveRot;
var() Class<Actor> DestroyEffectClass;
var() bool bAutoDrive;
var bool bGetOut;
var KVehicleFactory ParentFactory;
var bool bVehicleIsFiring;
var bool bVehicleIsAltFiring;
var Vector CameraHistory[5];
var int NextHistorySlot;
var bool bHistoryWarmup;

replication
{
	// Pos:0x000
	reliable if(__NFUN_154__(int(Role), int(ROLE_Authority)))
		ClientKDriverEnter, ClientKDriverLeave;

	// Pos:0x00D
	reliable if(__NFUN_150__(int(Role), int(ROLE_Authority)))
		VehicleCeaseFire, VehicleFire;
}

// Export UKVehicle::execGraphData(FFrame&, void* const)
native final function GraphData(string DataName, float DataValue);

function TakeDamage(int Damage, Pawn instigatedBy, Vector HitLocation, Vector Momentum, Class<DamageType> DamageType)
{
	local PlayerController PC;

	__NFUN_162__(Health, Damage);
	// End:0x6E
	if(__NFUN_152__(Health, 0))
	{
		// End:0x6B
		if(__NFUN_119__(Controller, none))
		{
			PC = PlayerController(Controller);
			// End:0x5F
			if(__NFUN_130__(Controller.bIsPlayer, __NFUN_119__(PC, none)))
			{
				ClientKDriverLeave(PC);				
			}
			else
			{
				Controller.__NFUN_279__();
			}
		}
		__NFUN_279__();
	}
	return;
}

event EncroachedBy(Actor Other)
{
	__NFUN_231__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("KVehicle(", string(self)), ") Encroached By: "), string(Other)), "."));
	return;
}

event VehicleStateReceived()
{
	return;
}

simulated event KVehicleUpdateParams()
{
	return;
}

function VehicleFire(bool bWasAltFire)
{
	// End:0x14
	if(bWasAltFire)
	{
		bVehicleIsAltFiring = true;		
	}
	else
	{
		bVehicleIsFiring = true;
	}
	return;
}

function VehicleCeaseFire(bool bWasAltFire)
{
	// End:0x14
	if(bWasAltFire)
	{
		bVehicleIsAltFiring = false;		
	}
	else
	{
		bVehicleIsFiring = false;
	}
	return;
}

function TryToDrive(Pawn P)
{
	local Controller C;

	C = P.Controller;
	// End:0x77
	if(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_114__(Driver, none), __NFUN_119__(C, none)), C.bIsPlayer), __NFUN_129__(C.__NFUN_281__('PlayerDriving'))), P.IsHumanControlled()))
	{
		KDriverEnter(P);
	}
	return;
}

simulated function ClientKDriverEnter(PlayerController PC)
{
	PC.bBehindView = true;
	PC.bFixedCamera = false;
	PC.bFreeCamera = true;
	PC.__NFUN_299__(Rotator(__NFUN_276__(vect(-1.0000000, 0.0000000, 0.0000000), Rotation)));
	return;
}

function KDriverEnter(Pawn P)
{
	local PlayerController PC;

	Driver = P;
	Driver.__NFUN_262__(false, false, false);
	Driver.bCollideWorld = false;
	Driver.bPhysicsAnimUpdate = false;
	Driver.Velocity = vect(0.0000000, 0.0000000, 0.0000000);
	Driver.__NFUN_3970__(0);
	Driver.__NFUN_298__(self);
	PC = PlayerController(P.Controller);
	PC.UnPossess();
	Driver.__NFUN_272__(PC);
	PC.Possess(self);
	PC.ClientSetViewTarget(self);
	PC.__NFUN_113__('PlayerDriving');
	ClientKDriverEnter(PC);
	return;
}

simulated function ClientKDriverLeave(PlayerController PC)
{
	PC.bBehindView = false;
	PC.bFixedCamera = true;
	PC.bFreeCamera = false;
	NextHistorySlot = 0;
	bHistoryWarmup = true;
	return;
}

function bool KDriverLeave(bool bForceLeave)
{
	local PlayerController PC;
	local int i;
	local bool havePlaced;
	local Vector HitLocation, HitNormal, tryPlace;

	// End:0x0D
	if(__NFUN_114__(Driver, none))
	{
		return false;
	}
	Driver.bCollideWorld = true;
	Driver.__NFUN_262__(true, true, true);
	havePlaced = false;
	i = 0;
	J0x3C:

	// End:0xC6 [Loop If]
	if(__NFUN_130__(__NFUN_150__(i, ExitPositions.Length), __NFUN_242__(havePlaced, false)))
	{
		tryPlace = __NFUN_215__(Location, __NFUN_276__(ExitPositions[i], Rotation));
		// End:0x9B
		if(__NFUN_119__(__NFUN_277__(HitLocation, HitNormal, tryPlace, Location, false), none))
		{
			// [Explicit Continue]
			goto J0xBC;
		}
		// End:0xB4
		if(__NFUN_129__(Driver.__NFUN_267__(tryPlace)))
		{
			// [Explicit Continue]
			goto J0xBC;
		}
		havePlaced = true;
		J0xBC:

		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x3C;
	}
	// End:0x11B
	if(__NFUN_130__(__NFUN_129__(havePlaced), __NFUN_129__(bForceLeave)))
	{
		__NFUN_231__("Could not place driver.");
		Driver.bCollideWorld = false;
		Driver.__NFUN_262__(false, false, false);
		return false;
	}
	PC = PlayerController(Controller);
	ClientKDriverLeave(PC);
	PC.UnPossess();
	PC.Possess(Driver);
	PC.ClientSetViewTarget(Driver);
	Controller = none;
	Driver.PlayWaiting();
	Driver.bPhysicsAnimUpdate = Driver.default.bPhysicsAnimUpdate;
	Driver.Acceleration = vect(0.0000000, 0.0000000, 24000.0000000);
	Driver.__NFUN_3970__(2);
	Driver.__NFUN_298__(none);
	Driver = none;
	Throttle = 0.0000000;
	Steering = 0.0000000;
	bVehicleIsFiring = false;
	bVehicleIsAltFiring = false;
	return true;
	return;
}

simulated function bool SpecialCalcView(out Actor ViewActor, out Vector CameraLocation, out Rotator CameraRotation)
{
	local Vector CamLookAt, HitLocation, HitNormal;
	local PlayerController PC;
	local int i, averageOver;

	PC = PlayerController(Controller);
	// End:0x33
	if(__NFUN_132__(__NFUN_114__(PC, none), __NFUN_119__(PC.ViewTarget, self)))
	{
		return false;
	}
	ViewActor = self;
	CamLookAt = __NFUN_215__(Location, __NFUN_276__(vect(-100.0000000, 0.0000000, 100.0000000), Rotation));
	CameraHistory[NextHistorySlot] = CamLookAt;
	__NFUN_165__(NextHistorySlot);
	// End:0x8B
	if(bHistoryWarmup)
	{
		averageOver = NextHistorySlot;		
	}
	else
	{
		averageOver = 5;
	}
	CamLookAt = vect(0.0000000, 0.0000000, 0.0000000);
	i = 0;
	J0xAD:

	// End:0xD8 [Loop If]
	if(__NFUN_150__(i, averageOver))
	{
		__NFUN_223__(CamLookAt, CameraHistory[i]);
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0xAD;
	}
	__NFUN_222__(CamLookAt, float(averageOver));
	// End:0x101
	if(__NFUN_154__(NextHistorySlot, 5))
	{
		NextHistorySlot = 0;
		bHistoryWarmup = false;
	}
	CameraLocation = __NFUN_215__(CamLookAt, __NFUN_276__(vect(-600.0000000, 0.0000000, 0.0000000), CameraRotation));
	// End:0x159
	if(__NFUN_119__(__NFUN_277__(HitLocation, HitNormal, CameraLocation, CamLookAt, false, vect(10.0000000, 10.0000000, 10.0000000)), none))
	{
		CameraLocation = HitLocation;
	}
	return true;
	return;
}

simulated function Destroyed()
{
	// End:0x17
	if(__NFUN_119__(Driver, none))
	{
		Driver.__NFUN_279__();
	}
	// End:0x32
	if(__NFUN_119__(ParentFactory, none))
	{
		__NFUN_166__(ParentFactory.VehicleCount);
	}
	// End:0x51
	if(__NFUN_119__(DestroyEffectClass, none))
	{
		__NFUN_278__(DestroyEffectClass,,, Location, Rotation);
	}
	super.Destroyed();
	return;
}

simulated event Tick(float DeltaSeconds)
{
	local bool gotOut;

	// End:0x54
	if(__NFUN_130__(bGetOut, __NFUN_154__(int(Role), int(ROLE_Authority))))
	{
		gotOut = KDriverLeave(false);
		// End:0x54
		if(__NFUN_129__(gotOut))
		{
			__NFUN_231__("Couldn't Leave - staying in!");
		}
	}
	bGetOut = false;
	return;
}

defaultproperties
{
	ExitPositions[0]=()
	bHistoryWarmup=true
	bCanBeBaseForPawns=true
	bSpecialCalcView=true
	Physics=13
	bAcceptsProjectors=true
	bAlwaysRelevant=true
	bNetInitialRotation=true
	CollisionRadius=1.0000000
	CollisionHeight=1.0000000
	bCollideWorld=false
	bBlockKarma=true
	bEdShouldSnap=true
}