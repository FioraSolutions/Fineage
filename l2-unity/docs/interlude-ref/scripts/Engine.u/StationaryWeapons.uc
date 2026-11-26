class StationaryWeapons extends Pickup
	abstract
	native
	nativereplication
	placeable;

var() int TeamIndex;
var() bool bIgnoreTeammates;
var() StaticMesh Meshes[2];
var byte bActive;
var byte bLastActive;

replication
{
	// Pos:0x000
	reliable if(__NFUN_154__(int(Role), int(ROLE_Authority)))
		TeamIndex, bActive;
}

event PostBeginPlay()
{
	SetTeam(TeamIndex);
	super(Actor).PostBeginPlay();
	return;
}

function SetTeam(int NewTeamIndex)
{
	SetStaticMesh(Meshes[NewTeamIndex]);
	TeamIndex = NewTeamIndex;
	return;
}

simulated event Activated()
{
	return;
}

simulated event Deactivated()
{
	return;
}

function Activate()
{
	bActive = 1;
	// End:0x42
	if(__NFUN_132__(__NFUN_154__(int(Level.NetMode), int(NM_ListenServer)), __NFUN_154__(int(Level.NetMode), int(NM_Standalone))))
	{
		Activated();
	}
	return;
}

function DeActivate()
{
	bActive = 0;
	// End:0x42
	if(__NFUN_132__(__NFUN_154__(int(Level.NetMode), int(NM_ListenServer)), __NFUN_154__(int(Level.NetMode), int(NM_Standalone))))
	{
		Deactivated();
	}
	return;
}

function Explode(Vector HitLocation, Vector HitNormal)
{
	__NFUN_279__();
	return;
}

function float ReduceDamage(int Damage, Class<DamageType> DamageType)
{
	return float(Damage);
	return;
}

function TakeDamage(int Damage, Pawn EventInstigator, Vector HitLocation, Vector Momentum, Class<DamageType> DamageType)
{
	Explode(HitLocation, vect(0.0000000, 0.0000000, 0.0000000));
	return;
}

auto state fucked
{	stop;
}

defaultproperties
{
	bIgnoreTeammates=true
	bOnlyReplicateHidden=false
	DrawType=8
	RemoteRole=2
	bProjTarget=true
	bFixedRotationDir=false
	RotationRate=(Pitch=0,Yaw=0,Roll=0)
	DesiredRotation=(Pitch=0,Yaw=0,Roll=0)
}