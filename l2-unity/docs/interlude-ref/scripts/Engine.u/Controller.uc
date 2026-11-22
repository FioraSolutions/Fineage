class Controller extends Actor
	abstract
	native
	nativereplication
	config
	notplaceable;

const LATENT_MOVETOWARD = 503;

enum PMoveType
{
	PMT_SLOW,                       // 0
	PMT_FAST                        // 1
};

enum PStopType
{
	STP_SIT,                        // 0
	STP_STAND,                      // 1
	STP_FAKE_DEAD,                  // 2
	STP_FAKE_DEAD_STAND,            // 3
	STP_CHAIR_SIT                   // 4
};

var Pawn Pawn;
var const int PlayerNum;
var float SightCounter;
var float FovAngle;
var globalconfig float Handedness;
var bool bIsPlayer;
var bool bGodMode;
var const bool bLOSflag;
var bool bAdvancedTactics;
var bool bCanOpenDoors;
var bool bCanDoSpecial;
var bool bAdjusting;
var bool bPreparingMove;
var bool bControlAnimations;
var bool bEnemyInfoValid;
var bool bNotifyApex;
var bool bUsePlayerHearing;
var bool bJumpOverWall;
var bool bEnemyAcquired;
var bool bSoaking;
var bool bHuntPlayer;
var bool bAllowedToTranslocate;
var bool bAllowedToImpactJump;
var input byte bRun;
var input byte bDuck;
var input byte bFire;
var input byte bAltFire;
var Vector AdjustLoc;
var const Controller nextController;
var float Stimulus;
var float MoveTimer;
var Actor MoveTarget;
var Vector Destination;
var Vector FocalPoint;
var Actor Focus;
var bool bMoveLocationDone;
var private int SelectedCreatureID;
var private Actor SelectedActor;
var bool bDead;
var bool bAutoAttacking;
var bool bAttackIntend;
var array<Vector> ReservedMovePoint;
var Actor ReservedMoveActor;
var float ReservedMoveDistance;
var Controller.PMoveType MoveType;
var Controller.PStopType WaitType;
var bool bMyController;
var Mover PendingMover;
var Actor GoalList[4];
var NavigationPoint home;
var float MinHitWall;
var float RespawnPredictionTime;
var int AcquisitionYawRate;
var Pawn Enemy;
var Actor Target;
var Vector LastSeenPos;
var Vector LastSeeingPos;
var float LastSeenTime;
var string VoiceType;
var float OldMessageTime;
var Actor RouteCache[16];
var ReachSpec CurrentPath;
var Vector CurrentPathDir;
var Actor RouteGoal;
var float RouteDist;
var float LastRouteFind;
var() Class<PlayerReplicationInfo> PlayerReplicationInfoClass;
var PlayerReplicationInfo PlayerReplicationInfo;
var Class<Pawn> PawnClass;
var Class<Pawn> PreviousPawnClass;
var float GroundPitchTime;
var Vector ViewX;
var Vector ViewY;
var Vector ViewZ;
var NavigationPoint StartSpot;
var Vector MonitorStartLoc;
var Pawn MonitoredPawn;
var float MonitorMaxDistSq;
var AvoidMarker FearSpots[2];
var const Actor LastFailedReach;
var const float FailedReachTime;
var const Vector FailedReachLocation;
var Weapon LastPawnWeapon;

replication
{
	// Pos:0x078
	unreliable if(__NFUN_150__(int(Role), int(ROLE_Authority)))
		SendVoiceMessage, SetPawnClass;

	// Pos:0x000
	reliable if(__NFUN_130__(bNetDirty, __NFUN_154__(int(Role), int(ROLE_Authority))))
		Pawn, PlayerReplicationInfo;

	// Pos:0x018
	reliable if(__NFUN_130__(__NFUN_130__(bNetDirty, __NFUN_154__(int(Role), int(ROLE_Authority))), bNetOwner))
		PawnClass;

	// Pos:0x03B
	reliable if(__NFUN_154__(int(RemoteRole), int(ROLE_AutonomousProxy)))
		ClientDying, ClientGameEnded, 
		ClientSetLocation, ClientSetRotation, 
		ClientSetWeapon, ClientSwitchToBestWeapon;

	// Pos:0x048
	reliable if(__NFUN_130__(__NFUN_132__(__NFUN_129__(bDemoRecording), __NFUN_130__(bClientDemoRecording, bClientDemoNetFunc)), __NFUN_154__(int(Role), int(ROLE_Authority))))
		ClientVoiceMessage;

	// Pos:0x085
	reliable if(__NFUN_150__(int(Role), int(ROLE_Authority)))
		ServerReStartPlayer;
}

// Export UController::execMoveTo(FFrame&, void* const)
native(500) final latent function MoveTo(Vector NewDestination, optional Actor ViewFocus, optional bool bShouldWalk);

// Export UController::execMoveToward(FFrame&, void* const)
native(502) final latent function MoveToward(Actor NewTarget, optional Actor ViewFocus, optional float DestinationOffset, optional bool bUseStrafing, optional bool bShouldWalk);

// Export UController::execFinishRotation(FFrame&, void* const)
native(508) final latent function FinishRotation();

// Export UController::execLineOfSightTo(FFrame&, void* const)
native(514) final function bool LineOfSightTo(Actor Other);

// Export UController::execCanSee(FFrame&, void* const)
native(533) final function bool CanSee(Pawn Other);

// Export UController::execFindPathTo(FFrame&, void* const)
native(518) final function Actor FindPathTo(Vector aPoint);

// Export UController::execFindPathToward(FFrame&, void* const)
native(517) final function Actor FindPathToward(Actor anActor, optional bool bWeightDetours);

// Export UController::execFindPathToIntercept(FFrame&, void* const)
native final function Actor FindPathToIntercept(Pawn P, Actor RouteGoal, optional bool bWeightDetours);

// Export UController::execFindPathTowardNearest(FFrame&, void* const)
native final function Actor FindPathTowardNearest(Class<NavigationPoint> GoalClass, optional bool bWeightDetours);

// Export UController::execFindRandomDest(FFrame&, void* const)
native(525) final function NavigationPoint FindRandomDest();

// Export UController::execEAdjustJump(FFrame&, void* const)
native(523) final function Vector EAdjustJump(float BaseZ, float XYSpeed);

// Export UController::execpointReachable(FFrame&, void* const)
native(521) final function bool pointReachable(Vector aPoint);

// Export UController::execactorReachable(FFrame&, void* const)
native(520) final function bool actorReachable(Actor anActor);

// Export UController::execPickWallAdjust(FFrame&, void* const)
native(526) final function bool PickWallAdjust(Vector HitNormal);

// Export UController::execWaitForLanding(FFrame&, void* const)
native(527) final latent function WaitForLanding();

// Export UController::execFindBestInventoryPath(FFrame&, void* const)
native(540) final function Actor FindBestInventoryPath(out float MinWeight);

// Export UController::execAddController(FFrame&, void* const)
native(529) final function AddController();

// Export UController::execRemoveController(FFrame&, void* const)
native(530) final function RemoveController();

// Export UController::execPickTarget(FFrame&, void* const)
native(531) final function Pawn PickTarget(out float bestAim, out float bestDist, Vector FireDir, Vector projStart, float MaxRange);

// Export UController::execPickAnyTarget(FFrame&, void* const)
native(534) final function Actor PickAnyTarget(out float bestAim, out float bestDist, Vector FireDir, Vector projStart);

// Export UController::execInLatentExecution(FFrame&, void* const)
native final function bool InLatentExecution(int LatentActionNumber);

// Export UController::execStopWaiting(FFrame&, void* const)
native function StopWaiting();

// Export UController::execEndClimbLadder(FFrame&, void* const)
native function EndClimbLadder();

event MayFall()
{
	return;
}

function PendingStasis()
{
	bStasis = true;
	Pawn = none;
	return;
}

function DisplayDebug(Canvas Canvas, out float YL, out float YPos)
{
	// End:0x22
	if(__NFUN_114__(Pawn, none))
	{
		super.DisplayDebug(Canvas, YL, YPos);
		return;
	}
	Canvas.SetDrawColor(byte(255), 0, 0);
	Canvas.__NFUN_465__(__NFUN_112__(__NFUN_112__(__NFUN_112__("CONTROLLER ", GetItemName(string(self))), " Pawn "), GetItemName(string(Pawn))));
	__NFUN_184__(YPos, YL);
	Canvas.SetPos(4.0000000, YPos);
	// End:0xFC
	if(__NFUN_119__(Enemy, none))
	{
		Canvas.__NFUN_465__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("     STATE: ", string(__NFUN_284__())), " Timer: "), string(TimerCounter)), " Enemy "), Enemy.GetHumanReadableName()), false);		
	}
	else
	{
		Canvas.__NFUN_465__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("     STATE: ", string(__NFUN_284__())), " Timer: "), string(TimerCounter)), " NO Enemy "), false);
	}
	__NFUN_184__(YPos, YL);
	Canvas.SetPos(4.0000000, YPos);
	// End:0x1A0
	if(__NFUN_114__(PlayerReplicationInfo, none))
	{
		Canvas.__NFUN_465__("     NO PLAYERREPLICATIONINFO", false);		
	}
	else
	{
		PlayerReplicationInfo.DisplayDebug(Canvas, YL, YPos);
	}
	__NFUN_184__(YPos, YL);
	Canvas.SetPos(4.0000000, YPos);
	return;
}

simulated function string GetHumanReadableName()
{
	// End:0x1A
	if(__NFUN_119__(PlayerReplicationInfo, none))
	{
		return PlayerReplicationInfo.PlayerName;
	}
	return GetItemName(string(self));
	return;
}

function Rotator GetViewRotation()
{
	return Rotation;
	return;
}

function Reset()
{
	super.Reset();
	Enemy = none;
	LastSeenTime = 0.0000000;
	StartSpot = none;
	return;
}

function bool AvoidCertainDeath()
{
	return false;
	return;
}

function ClientSetLocation(Vector NewLocation, Rotator NewRotation)
{
	__NFUN_299__(NewRotation);
	// End:0x8B
	if(__NFUN_130__(__NFUN_151__(Rotation.Pitch, RotationRate.Pitch), __NFUN_150__(Rotation.Pitch, __NFUN_147__(65536, RotationRate.Pitch))))
	{
		// End:0x6F
		if(__NFUN_150__(Rotation.Pitch, 32768))
		{
			NewRotation.Pitch = RotationRate.Pitch;			
		}
		else
		{
			NewRotation.Pitch = __NFUN_147__(65536, RotationRate.Pitch);
		}
	}
	// End:0xC4
	if(__NFUN_119__(Pawn, none))
	{
		NewRotation.Roll = 0;
		Pawn.__NFUN_299__(NewRotation);
		Pawn.__NFUN_267__(NewLocation);
	}
	return;
}

function ClientSetRotation(Rotator NewRotation)
{
	__NFUN_299__(NewRotation);
	// End:0x3C
	if(__NFUN_119__(Pawn, none))
	{
		NewRotation.Pitch = 0;
		NewRotation.Roll = 0;
		Pawn.__NFUN_299__(NewRotation);
	}
	return;
}

function ClientDying(Class<DamageType> DamageType, Vector HitLocation)
{
	// End:0x34
	if(__NFUN_119__(Pawn, none))
	{
		Pawn.PlayDying(DamageType, HitLocation);
		Pawn.__NFUN_113__('Dying');
	}
	return;
}

event AIHearSound(Actor Actor, int Id, Sound S, Vector SoundLocation, Vector Parameters, bool Attenuate)
{
	return;
}

event SoakStop(string problem)
{
	return;
}

function Possess(Pawn aPawn)
{
	aPawn.PossessedBy(self);
	Pawn = aPawn;
	// End:0x45
	if(__NFUN_119__(PlayerReplicationInfo, none))
	{
		PlayerReplicationInfo.bIsFemale = Pawn.bIsFemale;
	}
	FocalPoint = __NFUN_215__(Pawn.Location, __NFUN_213__(float(512), Vector(Pawn.Rotation)));
	Restart();
	return;
}

function WasKilledBy(Controller Other)
{
	return;
}

function Weapon GetLastWeapon()
{
	// End:0x27
	if(__NFUN_132__(__NFUN_114__(Pawn, none), __NFUN_114__(Pawn.Weapon, none)))
	{
		return LastPawnWeapon;
	}
	return Pawn.Weapon;
	return;
}

function PawnDied(Pawn P)
{
	// End:0x11
	if(__NFUN_119__(Pawn, P))
	{
		return;
	}
	// End:0x3C
	if(__NFUN_119__(Pawn, none))
	{
		__NFUN_267__(Pawn.Location);
		Pawn.UnPossessed();
	}
	Pawn = none;
	PendingMover = none;
	// End:0x6A
	if(bIsPlayer)
	{
		// End:0x67
		if(__NFUN_129__(__NFUN_281__('GameEnded')))
		{
			__NFUN_113__('Dead');
		}		
	}
	else
	{
		__NFUN_279__();
	}
	return;
}

function Restart()
{
	Enemy = none;
	return;
}

event LongFall()
{
	return;
}

event bool NotifyPhysicsVolumeChange(PhysicsVolume NewVolume)
{
	return;
}

event bool NotifyHeadVolumeChange(PhysicsVolume NewVolume)
{
	return;
}

event bool NotifyLanded(Vector HitNormal)
{
	return;
}

event bool NotifyHitWall(Vector HitNormal, Actor Wall)
{
	return;
}

event bool NotifyBump(Actor Other)
{
	return;
}

event NotifyHitMover(Vector HitNormal, Mover Wall)
{
	return;
}

event NotifyJumpApex()
{
	return;
}

event NotifyMissedJump()
{
	return;
}

function NotifyAddInventory(Inventory NewItem)
{
	return;
}

function NotifyTakeHit(Pawn instigatedBy, Vector HitLocation, int Damage, Class<DamageType> DamageType, Vector Momentum)
{
	// End:0x2E
	if(__NFUN_130__(__NFUN_119__(instigatedBy, none), __NFUN_119__(instigatedBy, Pawn)))
	{
		damageAttitudeTo(instigatedBy, float(Damage));
	}
	return;
}

function SetFall()
{
	return;
}

function PawnIsInPain(PhysicsVolume PainVolume)
{
	return;
}

event PreBeginPlay()
{
	// End:0x0E
	if(__NFUN_303__('LineagePlayerController'))
	{
		__NFUN_529__();
	}
	super.PreBeginPlay();
	// End:0x1F
	if(bDeleteMe)
	{
		return;
	}
	SightCounter = __NFUN_171__(0.2000000, __NFUN_195__());
	return;
}

event PostBeginPlay()
{
	super.PostBeginPlay();
	// End:0x51
	if(__NFUN_130__(bIsPlayer, __NFUN_154__(int(Role), int(ROLE_Authority))))
	{
		PlayerReplicationInfo = __NFUN_278__(PlayerReplicationInfoClass, self,, vect(0.0000000, 0.0000000, 0.0000000), rot(0, 0, 0));
		InitPlayerReplicationInfo();
	}
	return;
}

function InitPlayerReplicationInfo()
{
	// End:0x32
	if(__NFUN_122__(PlayerReplicationInfo.PlayerName, ""))
	{
		PlayerReplicationInfo.SetPlayerName(Class'Engine.GameInfo'.default.DefaultPlayerName);
	}
	return;
}

function bool SameTeamAs(Controller C)
{
	// End:0x39
	if(__NFUN_132__(__NFUN_132__(__NFUN_114__(PlayerReplicationInfo, none), __NFUN_114__(C.PlayerReplicationInfo, none)), __NFUN_114__(PlayerReplicationInfo.Team, none)))
	{
		return false;
	}
	return Level.Game.IsOnTeam(C, PlayerReplicationInfo.Team.TeamIndex);
	return;
}

function HandlePickup(Pickup pick)
{
	// End:0x6D
	if(__NFUN_114__(MoveTarget, pick))
	{
		// End:0x62
		if(__NFUN_119__(pick.myMarker, none))
		{
			MoveTarget = pick.myMarker;
			Pawn.Anchor = pick.myMarker;
			MoveTimer = 0.5000000;			
		}
		else
		{
			MoveTimer = -1.0000000;
		}
	}
	return;
}

simulated event Destroyed()
{
	// End:0x18
	if(__NFUN_150__(int(Role), int(ROLE_Authority)))
	{
		super.Destroyed();
		return;
	}
	__NFUN_530__();
	// End:0x53
	if(__NFUN_130__(bIsPlayer, __NFUN_119__(Level.Game, none)))
	{
		Level.Game.Logout(self);
	}
	// End:0xAD
	if(__NFUN_119__(PlayerReplicationInfo, none))
	{
		// End:0xA1
		if(__NFUN_130__(__NFUN_129__(PlayerReplicationInfo.bOnlySpectator), __NFUN_119__(PlayerReplicationInfo.Team, none)))
		{
			PlayerReplicationInfo.Team.RemoveFromTeam(self);
		}
		PlayerReplicationInfo.__NFUN_279__();
	}
	super.Destroyed();
	return;
}

event bool AllowDetourTo(NavigationPoint N)
{
	return true;
	return;
}

function AdjustView(float DeltaTime)
{
	local Controller C;

	C = Level.ControllerList;
	J0x14:

	// End:0x6B [Loop If]
	if(__NFUN_119__(C, none))
	{
		// End:0x54
		if(__NFUN_130__(C.__NFUN_303__('PlayerController'), __NFUN_114__(PlayerController(C).ViewTarget, Pawn)))
		{
			return;
		}
		C = C.nextController;
		// [Loop Continue]
		goto J0x14;
	}
	Pawn.bUpdateEyeheight = false;
	Pawn.EyeHeight = Pawn.BaseEyeHeight;
	return;
}

function bool WantsSmoothedView()
{
	return __NFUN_130__(__NFUN_130__(__NFUN_119__(Pawn, none), __NFUN_132__(__NFUN_154__(int(Pawn.Physics), int(1)), __NFUN_154__(int(Pawn.Physics), int(9)))), __NFUN_129__(Pawn.bJustLanded));
	return;
}

function GameHasEnded()
{
	// End:0x1C
	if(__NFUN_119__(Pawn, none))
	{
		Pawn.bNoWeaponFiring = true;
	}
	__NFUN_113__('GameEnded');
	return;
}

function ClientGameEnded()
{
	__NFUN_113__('GameEnded');
	return;
}

simulated event RenderOverlays(Canvas Canvas)
{
	return;
}

function int GetFacingDirection()
{
	return 0;
	return;
}

function byte GetMessageIndex(name PhraseName)
{
	return 0;
	return;
}

function SendMessage(PlayerReplicationInfo Recipient, name messagetype, byte MessageID, float Wait, name BroadcastType)
{
	SendVoiceMessage(PlayerReplicationInfo, Recipient, messagetype, MessageID, BroadcastType);
	return;
}

function bool AllowVoiceMessage(name messagetype)
{
	// End:0x23
	if(__NFUN_176__(__NFUN_175__(Level.TimeSeconds, OldMessageTime), float(10)))
	{
		return false;		
	}
	else
	{
		OldMessageTime = Level.TimeSeconds;
	}
	return true;
	return;
}

function SendVoiceMessage(PlayerReplicationInfo Sender, PlayerReplicationInfo Recipient, name messagetype, byte MessageID, name BroadcastType)
{
	local Controller P;

	// End:0x12
	if(__NFUN_129__(AllowVoiceMessage(messagetype)))
	{
		return;
	}
	P = Level.ControllerList;
	J0x26:

	// End:0x14C [Loop If]
	if(__NFUN_119__(P, none))
	{
		// End:0xE5
		if(__NFUN_119__(PlayerController(P), none))
		{
			// End:0x95
			if(__NFUN_132__(__NFUN_254__(BroadcastType, 'Global'), __NFUN_129__(Level.Game.bTeamGame)))
			{
				P.ClientVoiceMessage(Sender, Recipient, messagetype, MessageID);				
			}
			else
			{
				// End:0xE2
				if(__NFUN_114__(Sender.Team, P.PlayerReplicationInfo.Team))
				{
					P.ClientVoiceMessage(Sender, Recipient, messagetype, MessageID);
				}
			}			
		}
		else
		{
			// End:0x135
			if(__NFUN_130__(__NFUN_254__(messagetype, 'ORDER'), __NFUN_132__(__NFUN_114__(Recipient, none), __NFUN_114__(Recipient, P.PlayerReplicationInfo))))
			{
				P.BotVoiceMessage(messagetype, MessageID, self);
			}
		}
		P = P.nextController;
		// [Loop Continue]
		goto J0x26;
	}
	return;
}

function ClientVoiceMessage(PlayerReplicationInfo Sender, PlayerReplicationInfo Recipient, name messagetype, byte MessageID)
{
	return;
}

function BotVoiceMessage(name messagetype, byte MessageID, Controller Sender)
{
	return;
}

function bool WouldReactToNoise(float Loudness, Actor NoiseMaker)
{
	return false;
	return;
}

function bool WouldReactToSeeing(Pawn Seen)
{
	return false;
	return;
}

function Vector AdjustToss(float TSpeed, Vector Start, Vector End, bool bNormalize)
{
	local Vector Dest2D, Result, Vel2D;
	local float Dist2D;

	// End:0xEA
	if(__NFUN_177__(Start.Z, __NFUN_174__(End.Z, float(64))))
	{
		Dest2D = End;
		Dest2D.Z = Start.Z;
		Dist2D = __NFUN_225__(__NFUN_216__(Dest2D, Start));
		__NFUN_182__(TSpeed, __NFUN_172__(Dist2D, __NFUN_225__(__NFUN_216__(End, Start))));
		Result = SuggestFallVelocity(Dest2D, Start, TSpeed, TSpeed);
		Vel2D = Result;
		Vel2D.Z = 0.0000000;
		Result.Z = __NFUN_174__(Result.Z, __NFUN_172__(__NFUN_171__(__NFUN_175__(End.Z, Start.Z), __NFUN_225__(Vel2D)), Dist2D));		
	}
	else
	{
		Result = SuggestFallVelocity(End, Start, TSpeed, TSpeed);
	}
	// End:0x125
	if(bNormalize)
	{
		return __NFUN_213__(TSpeed, __NFUN_226__(Result));		
	}
	else
	{
		return Result;
	}
	return;
}

event PrepareForMove(NavigationPoint Goal, ReachSpec Path)
{
	return;
}

function WaitForMover(Mover M)
{
	return;
}

function MoverFinished()
{
	return;
}

function UnderLift(Mover M)
{
	return;
}

function FearThisSpot(AvoidMarker aSpot)
{
	local int i;

	// End:0x0D
	if(__NFUN_114__(Pawn, none))
	{
		return;
	}
	// End:0x1C
	if(__NFUN_129__(__NFUN_514__(aSpot)))
	{
		return;
	}
	i = 0;
	J0x23:

	// End:0x5D [Loop If]
	if(__NFUN_150__(i, 2))
	{
		// End:0x53
		if(__NFUN_114__(FearSpots[i], none))
		{
			FearSpots[i] = aSpot;
			return;
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x23;
	}
	i = 0;
	J0x64:

	// End:0xD8 [Loop If]
	if(__NFUN_150__(i, 2))
	{
		// End:0xCE
		if(__NFUN_177__(__NFUN_225__(__NFUN_216__(Pawn.Location, FearSpots[i].Location)), __NFUN_225__(__NFUN_216__(Pawn.Location, aSpot.Location))))
		{
			FearSpots[i] = aSpot;
			return;
		}
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x64;
	}
	return;
}

event float Desireability(Pickup P)
{
	return P.BotDesireability(Pawn);
	return;
}

event SetupSpecialPathAbilities()
{
	return;
}

event HearNoise(float Loudness, Actor NoiseMaker)
{
	return;
}

event SeePlayer(Pawn Seen)
{
	return;
}

event SeeMonster(Pawn Seen)
{
	return;
}

event EnemyNotVisible()
{
	return;
}

function ShakeView(float ShakeTime, float RollMag, Vector OffsetMag, float RollRate, Vector OffsetRate, float OffsetTime)
{
	return;
}

function NotifyKilled(Controller Killer, Controller Killed, Pawn Other)
{
	// End:0x16
	if(__NFUN_114__(Enemy, Other))
	{
		Enemy = none;
	}
	return;
}

function damageAttitudeTo(Pawn Other, float Damage)
{
	return;
}

function float AdjustDesireFor(Pickup P)
{
	return;
}

function bool FireWeaponAt(Actor A)
{
	return;
}

function StopFiring()
{
	bFire = 0;
	bAltFire = 0;
	return;
}

simulated function float RateWeapon(Weapon W)
{
	return 0.0000000;
	return;
}

function float WeaponPreference(Weapon W)
{
	return 0.0000000;
	return;
}

function Rotator AdjustAim(Ammunition FiredAmmunition, Vector projStart, int aimerror)
{
	return Rotation;
	return;
}

function ReceiveWarning(Pawn shooter, float projSpeed, Vector FireDir)
{
	return;
}

exec function SwitchToBestWeapon()
{
	local float rating;

	// End:0x23
	if(__NFUN_132__(__NFUN_114__(Pawn, none), __NFUN_114__(Pawn.Inventory, none)))
	{
		return;
	}
	// End:0xB8
	if(__NFUN_132__(__NFUN_114__(Pawn.PendingWeapon, none), __NFUN_119__(AIController(self), none)))
	{
		Pawn.PendingWeapon = Pawn.Inventory.RecommendWeapon(rating);
		// End:0xA2
		if(__NFUN_114__(Pawn.PendingWeapon, Pawn.Weapon))
		{
			Pawn.PendingWeapon = none;
		}
		// End:0xB8
		if(__NFUN_114__(Pawn.PendingWeapon, none))
		{
			return;
		}
	}
	StopFiring();
	// End:0xE4
	if(__NFUN_114__(Pawn.Weapon, none))
	{
		Pawn.ChangedWeapon();		
	}
	else
	{
		// End:0x11D
		if(__NFUN_119__(Pawn.Weapon, Pawn.PendingWeapon))
		{
			Pawn.Weapon.PutDown();
		}
	}
	return;
}

function ClientSwitchToBestWeapon()
{
	SwitchToBestWeapon();
	return;
}

function ClientSetWeapon(Class<Weapon> WeaponClass)
{
	local Inventory Inv;
	local int Count;

	Inv = Pawn.Inventory;
	J0x14:

	// End:0xFB [Loop If]
	if(__NFUN_119__(Inv, none))
	{
		__NFUN_165__(Count);
		// End:0x37
		if(__NFUN_151__(Count, 1000))
		{
			return;
		}
		// End:0x55
		if(__NFUN_129__(__NFUN_258__(Inv.Class, WeaponClass)))
		{			
		}
		else
		{
			// End:0x94
			if(__NFUN_114__(Pawn.Weapon, none))
			{
				Pawn.PendingWeapon = Weapon(Inv);
				Pawn.ChangedWeapon();				
			}
			else
			{
				// End:0xE2
				if(__NFUN_119__(Pawn.Weapon, Weapon(Inv)))
				{
					Pawn.PendingWeapon = Weapon(Inv);
					Pawn.Weapon.PutDown();
				}
			}
			return;
		}
		Inv = Inv.Inventory;
		// [Loop Continue]
		goto J0x14;
	}
	return;
}

function SetPawnClass(string InClass, string InCharacter)
{
	local Class<Pawn> pClass;

	pClass = Class<Pawn>(DynamicLoadObject(InClass, Class'Core.Class'));
	// End:0x31
	if(__NFUN_119__(pClass, none))
	{
		PawnClass = pClass;
	}
	return;
}

function bool CheckFutureSight(float DeltaTime)
{
	return true;
	return;
}

function ChangedWeapon()
{
	return;
}

function ServerReStartPlayer()
{
	// End:0x1B
	if(__NFUN_154__(int(Level.NetMode), int(NM_Client)))
	{
		return;
	}
	// End:0x2C
	if(__NFUN_119__(Pawn, none))
	{
		ServerGivePawn();
	}
	return;
}

function ServerGivePawn()
{
	return;
}

event MonitoredPawnAlert()
{
	return;
}

function StartMonitoring(Pawn P, float MaxDist)
{
	MonitoredPawn = P;
	MonitorStartLoc = P.Location;
	MonitorMaxDistSq = __NFUN_171__(MaxDist, MaxDist);
	return;
}

function bool AutoTaunt()
{
	return false;
	return;
}

function bool DontReuseTaunt(int t)
{
	return false;
	return;
}

state Dead
{
	ignores KilledBy;

	function PawnDied(Pawn P)
	{
		// End:0x36
		if(__NFUN_155__(int(Level.NetMode), int(NM_Client)))
		{
			__NFUN_232__(__NFUN_112__(string(self), " Pawndied while dead"));
		}
		return;
	}

	function ServerReStartPlayer()
	{
		// End:0x1B
		if(__NFUN_154__(int(Level.NetMode), int(NM_Client)))
		{
			return;
		}
		Level.Game.RestartPlayer(self);
		return;
	}
	stop;
}

state GameEnded
{
	ignores ReceiveWarning, TakeDamage, KilledBy;

	function BeginState()
	{
		// End:0x9A
		if(__NFUN_119__(Pawn, none))
		{
			Pawn.bPhysicsAnimUpdate = false;
			Pawn.StopAnimating();
			Pawn.SimAnim.AnimRate = 0;
			Pawn.__NFUN_262__(true, false, false);
			Pawn.Velocity = vect(0.0000000, 0.0000000, 0.0000000);
			Pawn.__NFUN_3970__(0);
			Pawn.UnPossessed();
			Pawn.bIgnoreForces = true;
		}
		// End:0xA8
		if(__NFUN_129__(bIsPlayer))
		{
			__NFUN_279__();
		}
		return;
	}
	stop;
}

defaultproperties
{
	FovAngle=90.0000000
	MoveType=1
	WaitType=1
	MinHitWall=-1.0000000
	AcquisitionYawRate=20000
	PlayerReplicationInfoClass=Class'Engine.PlayerReplicationInfo'
	bNeedCleanup=false
	bHidden=true
	RotationRate=(Pitch=3072,Yaw=30000,Roll=2048)
	bHiddenEd=true
}