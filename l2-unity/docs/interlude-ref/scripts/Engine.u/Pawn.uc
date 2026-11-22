class Pawn extends Actor
	abstract
	native
	nativereplication
	config(User)
	placeable;

enum NActionList
{
	NACT_NONE,                      // 0
	NACT_MELLEATTACK,               // 1
	NACT_BOWATTACK,                 // 2
	NACT_PICITEM,                   // 3
	NACT_CHANGEITEM,                // 4
	NACT_SKILLUSE,                  // 5
	NACT_SKILLINPUTWAIT,            // 6
	NACT_THROWATTACK,               // 7
	NACT_SHOTATTACK,                // 8
	NACT_PRIVATESTORE,              // 9
	NACT_OBSERVING                  // 10
};

enum EUpdateMovementAnimType
{
	UM_NONE,                        // 0
	UM_ONCE,                        // 1
	UM_FORCE                        // 2
};

enum WeaponType
{
	WT_HAND,                        // 0
	WT_1HS,                         // 1
	WT_2HS,                         // 2
	WT_DUAL,                        // 3
	WT_POLE,                        // 4
	WT_BOW,                         // 5
	WT_THROW,                       // 6
	WT_DUALFIST                     // 7
};

enum RideType
{
	RD_NONE,                        // 0
	RD_STRIDER,                     // 1
	RD_WYVERN                       // 2
};

enum FishingType
{
	FST_NONE,                       // 0
	FST_WAIT,                       // 1
	FST_BATTLE                      // 2
};

enum ESamePoseStat
{
	SPS_NONE,                       // 0
	SPS_ONCE,                       // 1
	SPS_TWICE                       // 2
};

struct NActionPtr
{
	var int Ptr;
};

struct NAttackActionParam
{
	var int Damage;
	var bool bMiss;
	var bool bCritical;
	var bool bShieldDefense;
	var bool bSpirit;
	var int SoulshotGrade;
	var Actor ActionTarget;
};

struct NPrimeActionParam
{
	var int ActionID;
	var int Damage;
	var bool bMiss;
	var bool bCritical;
	var bool bShieldDefense;
	var bool bSpirit;
	var int SoulshotGrade;
	var Actor ActionTarget;
	var int MaxAtkShotNum;
	var int CurAtkShotNum;
	var int SkillID;
	var array<NAttackActionParam> NAssociatedAttackParam;
};

struct NAtkConsumeItemParam
{
	var int BowItemID;
	var int BowUseItemID;
};

struct NSilhouetteParam
{
	var byte Type;
	var byte R;
	var byte G;
	var byte B;
};

struct NPawnLightPtr
{
	var int Ptr;
};

struct NAbnormalStatPtr
{
	var int Ptr;
};

struct NAppendixEffectPtr
{
	var int Ptr;
};

struct NWeaponEffectPtr
{
	var int Ptr;
};

struct NCursedWeaponEffectPtr
{
	var int Ptr;
};

struct NBoneScalerStatPtr
{
	var int Ptr;
};

var Mesh SubMeshes[9];
var Mesh RightHandMesh;
var Mesh LeftHandMesh;
var Mesh RightArmMesh;
var Mesh LeftArmMesh;
var Mesh CloakMesh;
var Mesh CloakCoverMesh;
var transient MeshInstance SubMeshInstances[9];
var transient MeshInstance RightHandMeshInstance;
var transient MeshInstance LeftHandMeshInstance;
var transient MeshInstance RightArmMeshInstance;
var transient MeshInstance LeftArmMeshInstance;
var transient MeshInstance CloakMeshInstance;
var transient MeshInstance CloakCoverMeshInstance;
var name RightHandBone;
var name LeftHandBone;
var name RightArmBone;
var name LeftArmBone;
var name SpineBone;
var name LowbodyBone;
var name CapeBone;
var name HeadBone;
var name RightFootBone;
var name LeftFootBone;
var name WingBone;
var int EffectSpawnBoneIdx;
var config Material SubSkins[9];
var config Material RightHandSkins;
var config Material LeftHandSkins;
var config Material RightArmSkins;
var config Material LeftArmSkins[3];
var config Material CloakSkins[2];
var(NpcPos) string nickname;
var(NpcPos) name ai;
var(NpcPos) array<NpcPrivate> Privates;
var(NpcPos) WhenExtinctionCreate when_extinction_create;
var(NpcPos) bool bWayPointsShow;
var(NpcPos) array<WayPoint> WayPoints;
var float WeaponScale;
var transient NMagicInfo MagicInfo;
var Mesh SubMeshesBuffer[9];
var Material SubSkinsBuffer[9];
var Mesh RightHandMeshBuffer;
var Material RightHandSkinsBuffer;
var Mesh LeftHandMeshBuffer;
var Material LeftHandSkinsBuffer;
var Mesh RightArmMeshBuffer;
var Material RightArmSkinsBuffer;
var Mesh LeftArmMeshBuffer;
var Material LeftArmSkinsBuffer[3];
var Mesh CloakMeshBuffer;
var Material CloakSkinsBuffer[2];
var Mesh CloakCoverMeshBuffer;
var NSilhouetteParam NSilhouetteData;
var NPrimeActionParam NPrimeAction;
var array<NActionPtr> NActions;
var NAtkConsumeItemParam NAtkConsumeItem;
var array<ExtraMeshData> ExtraMeshDatas;
var bool bPhysicInit;
var bool bAsTreatWorld;
var bool bIgnorePhysics;
var bool CanBeIngnoredCollision;
var bool bPlayingSpecialAnim;
var Rotator LastNeckRot;
var Rotator LastBodyRot;
var bool bFaceRotation;
var bool bEnableFaceRotation;
var bool bNpc;
var int CharClassID;
var int NpcClassID;
var int AttackItemClassID;
var int DefenseItemClassID;
var int ShieldItemClassID;
var bool bGetOnVehicle;
var float AttackSpeedRate;
var float NonAttackSpeedRate;
var float SkillSpeedRate;
var float PhysicalSkillSpeedRate;
var float MoveTimer;
var float MoveToPawnAdjust;
var int MoveEmergencyLevel;
var int WalkAnimFrameInMove;
var float ValidateLocationMoveTime;
var float ValidateLocationSpeed;
var int WantedYaw;
var bool bWantChangeYaw;
var bool bTurning;
var int TurningDir;
var bool bNotRotationMoving;
var Class<Emitter> DamageEffect;
var float GroundMaxSpeed;
var float GroundMinSpeed;
var float WaterMaxSpeed;
var float WaterMinSpeed;
var float AirMaxSpeed;
var float AirMinSpeed;
var float HoverMaxSpeed;
var float HoverMinSpeed;
var bool bUpdateMovementAnim;
var Pawn.EUpdateMovementAnimType UpdateMovementAnimType;
var bool bRide;
var Pawn RidePawn;
var Pawn.RideType CurRideType;
var Pawn.WeaponType CurWeaponType;
var bool bFish;
var bool bBrightFloat;
var Pawn.FishingType CurFishingType;
var Actor FishLine;
var L2Float FishFloat;
var ViewportWindowController VWController;
var float SkinNotifyElapsedTime;
var float SkinNotifyNextStartTime;
var bool bDemageAct;
var int bDemageStatus;
var float DemageDist;
var bool bSpineRotation;
var bool bTargetSpineRotation;
var transient Vector EffTargetLocation;
var transient int LoadingResourceRef;
var transient bool bCreatedResource;
var(CurveProjectile) int Curvature;
var(CurveProjectile) float WaistAngle;
var Vector WarpDest;
var bool IsWarpDest;
var bool bReadyToWarp;
var bool bIgnoreToWarp;
var transient int Environment;
var transient bool bHitGroundInWater;
var transient bool bSwimAfloat;
var transient Rotator LastFootRot;
var localized name TurnAnimName[8];
var localized name WalkAnimName[8];
var localized name RunAnimName[8];
var localized name FallAnimName[8];
var localized name DamageFlyAnimName[8];
var localized name JumpAnimName[8];
var localized name BJumpAnimName[8];
var localized name WaitAnimName[8];
var localized name AtkWaitAnimName[8];
var localized name SitAnimName[8];
var localized name SitWaitAnimName[8];
var localized name StandAnimName[8];
var localized name DeathStandAnimName[8];
var localized name ChairSitAnimName[8];
var localized name ChairWaitAnimName[8];
var localized name ChairStandAnimName[8];
var localized name StunAnimName[8];
var localized name Atk01AnimName[8];
var localized name Atk02AnimName[8];
var localized name Atk03AnimName[8];
var localized name AtkUpAnimName[8];
var localized name AtkDownAnimName[8];
var localized name SpAtk01AnimName[8];
var localized name SpAtk02AnimName[8];
var localized name SpAtk03AnimName[8];
var localized name SpAtk04AnimName[8];
var localized name SpAtk05AnimName[8];
var localized name SpAtk06AnimName[8];
var localized name SpAtk07AnimName[8];
var localized name SpAtk08AnimName[8];
var localized name SpAtk09AnimName[8];
var localized name SpAtk10AnimName[8];
var localized name SpAtk11AnimName[8];
var localized name SpAtk12AnimName[8];
var localized name SpAtk13AnimName[8];
var localized name SpAtk14AnimName[8];
var localized name SpAtk15AnimName[8];
var localized name SpAtk16AnimName[8];
var localized name SpAtk17AnimName[8];
var localized name SpAtk18AnimName[8];
var localized name SpAtk19AnimName[8];
var localized name SpAtk20AnimName[8];
var localized name SpAtk21AnimName[8];
var localized name SpAtk22AnimName[8];
var localized name SpAtk23AnimName[8];
var localized name SpAtk24AnimName[8];
var localized name SpAtk25AnimName[8];
var localized name SpAtk26AnimName[8];
var localized name SpAtk27AnimName[8];
var localized name SpAtk28AnimName[8];
var localized name ShieldAtkAnimName[8];
var localized name DefenceAnimName[8];
var localized name DodgeAnimName[8];
var localized name DeathAnimName[8];
var localized name DeathWaitAnimName[8];
var localized name SitDeathAnimName[8];
var localized name SitDeathWaitAnimName[8];
var localized name DamageAnimName[8];
var localized name CastShortAnimName[8];
var localized name CastMidAnimName[8];
var localized name CastLongAnimName[8];
var localized name CastEndAnimName[8];
var localized name MagicThrowAnimName[8];
var localized name MagicShotAnimName[8];
var localized name MagicNoTargetAnimName[8];
var localized name MagicFriendAnimName[8];
var localized name PicItemAnimName[8];
var localized name ThrowAnimName[8];
var localized name PcSocialAnimName[20];
var localized name NpcSocialAnimName[5];
var localized name RiderWaitAnimName[3];
var localized name RiderAtkAnimName[3];
var localized name RiderRunAnimName[3];
var localized name RiderDeathWaitAnimName[3];
var localized name RiderDeathAnimName[3];
var localized name SwimDeathAnimName[8];
var localized name SwimAnimName[8];
var localized name SwimWaitAnimName[8];
var localized name SwimDeathWaitAnimName[8];
var localized name SwimAttackWaitAnimName[8];
var localized name FishStartAnimName;
var localized name FishWaitAnimName;
var localized name FishControlAnimName;
var localized name FishPullAnimName;
var localized name FishEndAnimName;
var localized float WalkAnimRate[8];
var localized float RunAnimRate[8];
var localized float Atk01AnimRate[8];
var localized float Atk02AnimRate[8];
var localized float Atk03AnimRate[8];
var localized float AtkUpAnimRate[8];
var localized float AtkDownAnimRate[8];
var localized float SpAtk01AnimRate[8];
var localized float SpAtk02AnimRate[8];
var localized float SpAtk03AnimRate[8];
var localized float SpAtk04AnimRate[8];
var localized float ShieldAtkAnimRate[8];
var localized float CastShortAnimRate[8];
var localized float CastMidAnimRate[8];
var localized float CastLongAnimRate[8];
var localized float CastShortEndAnimRate[8];
var localized float MagicThrowAnimRate[8];
var localized float MagicShotAnimRate[8];
var localized float MagicNoTargetAnimRate[8];
var localized float MagicFriendAnimRate[8];
var localized float ThrowAnimRate[8];
var localized float SitAnimRate;
var localized float StandAnimRate;
var localized int PcSocialHideRightWeapon[20];
var localized int PcSocialHideLeftWeapon[20];
var localized int NpcSocialHideRightWeapon[5];
var localized int NpcSocialHideLeftWeapon[5];
var localized int PcCastHideRightWeapon;
var localized int PcCastHideLeftWeapon;
var Controller Controller;
var float NetRelevancyTime;
var PlayerController LastRealViewer;
var Actor LastViewer;
var bool bJustLanded;
var bool bUpAndOut;
var bool bIsWalking;
var bool bWarping;
var bool bWantsToCrouch;
var const bool bIsCrouched;
var const bool bTryToUncrouch;
var() bool bCanCrouch;
var bool bCrawler;
var const bool bReducedSpeed;
var bool bJumpCapable;
var bool bCanJump;
var bool bCanWalk;
var bool bCanSwim;
var bool bCanFly;
var bool bCanClimbLadders;
var bool bCanStrafe;
var bool bCanDoubleJump;
var bool bAvoidLedges;
var bool bStopAtLedges;
var bool bNoJumpAdjust;
var bool bCountJumps;
var const bool bSimulateGravity;
var bool bUpdateEyeheight;
var bool bIgnoreForces;
var const bool bNoVelocityUpdate;
var bool bCanWalkOffLedges;
var bool bSteadyFiring;
var bool bCanBeBaseForPawns;
var bool bClientCollision;
var const bool bSimGravityDisabled;
var bool bDirectHitWall;
var bool bThumped;
var bool bInvulnerableBody;
var bool bIsFemale;
var bool bAutoActivate;
var bool bCanPickupInventory;
var bool bUpdatingDisplay;
var bool bAmbientCreature;
var(ai) bool bLOSHearing;
var(ai) bool bSameZoneHearing;
var(ai) bool bAdjacentZoneHearing;
var(ai) bool bMuffledHearing;
var(ai) bool bAroundCornerHearing;
var(ai) bool bDontPossess;
var bool bAutoFire;
var bool bRollToDesired;
var bool bIgnorePlayFiring;
var bool bCachedRelevant;
var bool bUseCompressedPosition;
var globalconfig bool bWeaponBob;
var bool bHideRegularHUD;
var bool bSpecialHUD;
var bool bSpecialCalcView;
var bool bNoWeaponFiring;
var bool bIsTyping;
var byte FlashCount;
var byte Visibility;
var float DesiredSpeed;
var float MaxDesiredSpeed;
var(ai) name AIScriptTag;
var(ai) float HearingThreshold;
var(ai) float Alertness;
var(ai) float SightRadius;
var(ai) float PeripheralVision;
var() float SkillModifier;
var const float AvgPhysicsTime;
var float MeleeRange;
var NavigationPoint Anchor;
var const NavigationPoint LastAnchor;
var float FindAnchorFailedTime;
var float LastValidAnchorTime;
var float DestinationOffset;
var float NextPathRadius;
var Vector SerpentineDir;
var float SerpentineDist;
var float SerpentineTime;
var const float UncrouchTime;
var float SpawnTime;
var float GroundSpeed;
var float WaterSpeed;
var float AirSpeed;
var float LadderSpeed;
var float AccelRate;
var float JumpZ;
var float AirControl;
var float WalkingPct;
var float CrouchedPct;
var float MaxFallSpeed;
var Vector ConstantAcceleration;
var string OwnerName;
var travel Weapon Weapon;
var Weapon PendingWeapon;
var travel Powerups SelectedItem;
var float BaseEyeHeight;
var float EyeHeight;
var const Vector Floor;
var float SplashTime;
var float CrouchHeight;
var float CrouchRadius;
var float OldZ;
var PhysicsVolume HeadVolume;
var travel int Health;
var float BreathTime;
var float UnderWaterTime;
var float LastPainTime;
var Class<DamageType> ReducedDamageType;
var float HeadScale;
var PhysicsVolume FootVolume;
var const Vector noise1spot;
var const float noise1time;
var const Pawn noise1other;
var const float noise1loudness;
var const Vector noise2spot;
var const float noise2time;
var const Pawn noise2other;
var const float noise2loudness;
var float LastPainSound;
var globalconfig float Bob;
var float LandBob;
var float AppliedBob;
var float bobtime;
var Vector WalkBob;
var float SoundDampening;
var float DamageScaling;
var localized string MenuName;
var Projector Shadow;
var ShadowProjector Shadow1;
var bool bIgnoreShadowClipping;
var bool bSameAnim;
var bool bLongDeathWaitAnim;
var array<NPawnLightPtr> NPawnLight;
var array<NAbnormalStatPtr> NAbnormalStat;
var array<NBoneScalerStatPtr> NBoneScalerStat;
var array<NCubics> NCubicArray;
var L2Alarm OverHeadAlarm;
var bool bShowArrow;
var Vector ArrowTarget;
var array<NAppendixEffectPtr> NAppendixEffect;
var array<NWeaponEffectPtr> NWeaponEffect;
var Emitter NHeroEffect;
var NCursedWeaponEffectPtr NCursedWeaponEffect;
var array<Emitter> NDecoEffect;
var Emitter NSpoilEffect;
var Pawn.ESamePoseStat SamePoseStat;
var Class<Effects> BloodEffect;
var Class<Effects> LowGoreBlood;
var Class<AIController> ControllerClass;
var PlayerReplicationInfo PlayerReplicationInfo;
var LadderVolume OnLadder;
var name LandMovementState;
var name WaterMovementState;
var PlayerStart LastStartSpot;
var float LastStartTime;
var int ComboAnimPlayNum;
var name ComboAnimPlayName[8];
var int ComboAnimPlayCount;
var name AnimAction;
var Vector TakeHitLocation;
var Class<DamageType> HitDamageType;
var Vector TearOffMomentum;
var bool bPhysicsAnimUpdate;
var bool bWasCrouched;
var bool bWasWalking;
var bool bWasOnGround;
var bool bInitializeAnimation;
var bool bPlayedDeath;
var Actor.EPhysics OldPhysics;
var float OldRotYaw;
var Vector OldAcceleration;
var float BaseMovementRate;
var name MovementAnims[4];
var name TurnLeftAnim;
var name TurnRightAnim;
var(AnimTweaks) float BlendChangeTime;
var float MovementBlendStartTime;
var float ForwardStrafeBias;
var float BackwardStrafeBias;
var bool bNeedTurnAnim;
var MarkProjector Mark;
var Rotator TargetDirSpineRot;
var string ChatMsg;
var Vector HitLocation;
var float HitWaterEffectTimer;
var transient float RecentHitWaterInterval;
var string PrivateStoreChatMsg;
var string PrivateBuyChatMsg;
var string RecipeShopMsg;
var int AbnormalVisualEffectType;
var int AbnormalVisualEffectStopMoveFlag;
var float AbnormalVisualEffectLifeTime;
var int AppendixEffectType;
var bool bPlayerCharacter;
var int PlayerSlotNum;
var float BoundRadius;
var bool bLobbyCharacter;
var name LobbyWaitAnim;
var float NameOffset;
var float DeadNameOffset;
var bool bRendered;
var bool bShowGuilty;
var float fBlinkTime;
var float fMaxAirSpeed;
var float fMinAirSpeed;
var float fAirSpeedAccel;
var int CameraWalkingAccelMode;
var float fMaxAirSpeedIncRate;
var bool bUseDarkEffect;
var float CastingEffectScale;
var bool bIsHero;
var bool bHeroWeapon;
var bool bIsCursedWeapon;
var int CursedWeaponLevel;
var int SoundableBootsClassID;
var int LastAttackItemClassID;
var int AttackItemEnchantedValue;
var int AttackItemVariationOption1;
var int AttackItemVariationOption2;
var transient CompressedPosition PawnPosition;
var bool bColosseumSelected;
var int ColosseumTeam;
var int ColosseumPartyNum;
var(Collision) const float OriCollisionRadius;
var(Collision) const float OriCollisionHeight;
var Vector HelmMeshOrigin;
var Rotator HelmMeshRotOrigin;
var Vector HairAcceMeshOrigin[2];
var Rotator HairAcceMeshRotOrigin[2];
var array<int> NQuestList;
var bool bNeedPostSetPawnResourceProcess;
var float fAutoTargetPendingTime;
var Pawn AutoTargetPawn;
var bool bNeedSetPawnResource;

replication
{
	// Pos:0x000
	reliable if(__NFUN_130__(bNetDirty, __NFUN_154__(int(Role), int(ROLE_Authority))))
		AnimAction, HeadScale, 
		HitDamageType, PlayerReplicationInfo, 
		TakeHitLocation, bIsCrouched, 
		bIsTyping, bIsWalking, 
		bSimulateGravity;

	// Pos:0x018
	reliable if(__NFUN_130__(__NFUN_130__(bTearOff, bNetDirty), __NFUN_154__(int(Role), int(ROLE_Authority))))
		TearOffMomentum;

	// Pos:0x03B
	reliable if(__NFUN_130__(__NFUN_130__(bNetDirty, __NFUN_129__(bNetOwner)), __NFUN_154__(int(Role), int(ROLE_Authority))))
		bSteadyFiring;

	// Pos:0x060
	reliable if(__NFUN_130__(__NFUN_130__(bNetDirty, bNetOwner), __NFUN_154__(int(Role), int(ROLE_Authority))))
		AccelRate, AirControl, 
		AirSpeed, Controller, 
		GroundSpeed, JumpZ, 
		SelectedItem, WaterSpeed;

	// Pos:0x083
	reliable if(__NFUN_130__(bNetDirty, __NFUN_154__(int(Role), int(ROLE_Authority))))
		Health;

	// Pos:0x09B
	reliable if(__NFUN_130__(__NFUN_129__(bNetOwner), __NFUN_154__(int(Role), int(ROLE_Authority))))
		PawnPosition;

	// Pos:0x0B5
	reliable if(__NFUN_150__(int(Role), int(ROLE_Authority)))
		ServerChangedWeapon;
}

function name GetTurnAnimName()
{
	return TurnAnimName[int(CurWeaponType)];
	return;
}

function name GetWalkAnimName()
{
	return WalkAnimName[int(CurWeaponType)];
	return;
}

function name GetRunAnimName()
{
	return RunAnimName[int(CurWeaponType)];
	return;
}

function name GetJumpAnimName()
{
	return JumpAnimName[int(CurWeaponType)];
	return;
}

function name GetBJumpAnimName()
{
	return BJumpAnimName[int(CurWeaponType)];
	return;
}

function name GetWaitAnimName()
{
	return WaitAnimName[int(CurWeaponType)];
	return;
}

function name GetAtkWaitAnimName()
{
	return AtkWaitAnimName[int(CurWeaponType)];
	return;
}

function name GetSitAnimName()
{
	return SitAnimName[int(CurWeaponType)];
	return;
}

function name GetSitWaitAnimName()
{
	return SitWaitAnimName[int(CurWeaponType)];
	return;
}

function name GetStandAnimName()
{
	return StandAnimName[int(CurWeaponType)];
	return;
}

function name GetDeathStandAnimName()
{
	return DeathStandAnimName[int(CurWeaponType)];
	return;
}

function name GetChairSitAnimName()
{
	return ChairSitAnimName[int(CurWeaponType)];
	return;
}

function name GetChairWaitAnimName()
{
	return ChairWaitAnimName[int(CurWeaponType)];
	return;
}

function name GetChairStandAnimName()
{
	return ChairStandAnimName[int(CurWeaponType)];
	return;
}

function name GetStunAnimName()
{
	return StunAnimName[int(CurWeaponType)];
	return;
}

function name GetAtk01AnimName()
{
	return Atk01AnimName[int(CurWeaponType)];
	return;
}

function name GetAtk02AnimName()
{
	return Atk02AnimName[int(CurWeaponType)];
	return;
}

function name GetAtk03AnimName()
{
	return Atk03AnimName[int(CurWeaponType)];
	return;
}

function name GetAtkUpAnimName()
{
	return AtkUpAnimName[int(CurWeaponType)];
	return;
}

function name GetAtkDownAnimName()
{
	return AtkDownAnimName[int(CurWeaponType)];
	return;
}

function name GetSpAtk01AnimName()
{
	return SpAtk01AnimName[int(CurWeaponType)];
	return;
}

function name GetSpAtk02AnimName()
{
	return SpAtk02AnimName[int(CurWeaponType)];
	return;
}

function name GetSpAtk03AnimName()
{
	return SpAtk03AnimName[int(CurWeaponType)];
	return;
}

function name GetSpAtk04AnimName()
{
	return SpAtk04AnimName[int(CurWeaponType)];
	return;
}

function name GetSpAtk05AnimName()
{
	return SpAtk05AnimName[int(CurWeaponType)];
	return;
}

function name GetSpAtk06AnimName()
{
	return SpAtk06AnimName[int(CurWeaponType)];
	return;
}

function name GetSpAtk07AnimName()
{
	return SpAtk07AnimName[int(CurWeaponType)];
	return;
}

function name GetSpAtk08AnimName()
{
	return SpAtk08AnimName[int(CurWeaponType)];
	return;
}

function name GetSpAtk09AnimName()
{
	return SpAtk09AnimName[int(CurWeaponType)];
	return;
}

function name GetSpAtk10AnimName()
{
	return SpAtk10AnimName[int(CurWeaponType)];
	return;
}

function name GetSpAtk11AnimName()
{
	return SpAtk11AnimName[int(CurWeaponType)];
	return;
}

function name GetSpAtk12AnimName()
{
	return SpAtk12AnimName[int(CurWeaponType)];
	return;
}

function name GetSpAtk13AnimName()
{
	return SpAtk13AnimName[int(CurWeaponType)];
	return;
}

function name GetSpAtk14AnimName()
{
	return SpAtk14AnimName[int(CurWeaponType)];
	return;
}

function name GetSpAtk15AnimName()
{
	return SpAtk15AnimName[int(CurWeaponType)];
	return;
}

function name GetSpAtk16AnimName()
{
	return SpAtk16AnimName[int(CurWeaponType)];
	return;
}

function name GetSpAtk17AnimName()
{
	return SpAtk17AnimName[int(CurWeaponType)];
	return;
}

function name GetSpAtk18AnimName()
{
	return SpAtk18AnimName[int(CurWeaponType)];
	return;
}

function name GetSpAtk19AnimName()
{
	return SpAtk19AnimName[int(CurWeaponType)];
	return;
}

function name GetSpAtk20AnimName()
{
	return SpAtk20AnimName[int(CurWeaponType)];
	return;
}

function name GetSpAtk21AnimName()
{
	return SpAtk21AnimName[int(CurWeaponType)];
	return;
}

function name GetSpAtk22AnimName()
{
	return SpAtk22AnimName[int(CurWeaponType)];
	return;
}

function name GetSpAtk23AnimName()
{
	return SpAtk23AnimName[int(CurWeaponType)];
	return;
}

function name GetSpAtk24AnimName()
{
	return SpAtk24AnimName[int(CurWeaponType)];
	return;
}

function name GetSpAtk25AnimName()
{
	return SpAtk25AnimName[int(CurWeaponType)];
	return;
}

function name GetSpAtk26AnimName()
{
	return SpAtk26AnimName[int(CurWeaponType)];
	return;
}

function name GetSpAtk27AnimName()
{
	return SpAtk27AnimName[int(CurWeaponType)];
	return;
}

function name GetSpAtk28AnimName()
{
	return SpAtk28AnimName[int(CurWeaponType)];
	return;
}

function name GetShieldAtkAnimName()
{
	return ShieldAtkAnimName[int(CurWeaponType)];
	return;
}

function name GetDefenceAnimName()
{
	return DefenceAnimName[int(CurWeaponType)];
	return;
}

function name GetDodgeAnimName()
{
	return DodgeAnimName[int(CurWeaponType)];
	return;
}

function name GetDeathAnimName()
{
	// End:0x3A
	if(__NFUN_130__(__NFUN_154__(int(Physics), int(3)), __NFUN_255__(SwimDeathAnimName[int(CurWeaponType)], 'None')))
	{
		return SwimDeathAnimName[int(CurWeaponType)];		
	}
	else
	{
		// End:0x87
		if(__NFUN_130__(__NFUN_130__(__NFUN_119__(Controller, none), __NFUN_154__(int(Controller.WaitType), int(0))), __NFUN_255__(SitDeathAnimName[int(CurWeaponType)], 'None')))
		{
			return SitDeathAnimName[int(CurWeaponType)];
		}
	}
	return DeathAnimName[int(CurWeaponType)];
	return;
}

function name GetDeathWaitAnimName()
{
	// End:0x3A
	if(__NFUN_130__(__NFUN_154__(int(Physics), int(3)), __NFUN_255__(SwimDeathWaitAnimName[int(CurWeaponType)], 'None')))
	{
		return SwimDeathWaitAnimName[int(CurWeaponType)];		
	}
	else
	{
		// End:0x8A
		if(__NFUN_130__(__NFUN_130__(__NFUN_119__(Controller, none), __NFUN_154__(int(Controller.WaitType), int(0))), __NFUN_255__(SitDeathWaitAnimName[int(CurWeaponType)], 'None')))
		{
			return SitDeathWaitAnimName[int(CurWeaponType)];			
		}
		else
		{
			return DeathWaitAnimName[int(CurWeaponType)];
		}
	}
	return;
}

function name GetDamageAnimName()
{
	return DamageAnimName[int(CurWeaponType)];
	return;
}

function name GetCastShortAnimName()
{
	return CastShortAnimName[int(CurWeaponType)];
	return;
}

function name GetCastMidAnimName()
{
	return CastMidAnimName[int(CurWeaponType)];
	return;
}

function name GetCastLongAnimName()
{
	return CastLongAnimName[int(CurWeaponType)];
	return;
}

function name GetCastEndAnimName()
{
	return CastEndAnimName[int(CurWeaponType)];
	return;
}

function name GetMagicThrowAnimName()
{
	return MagicThrowAnimName[int(CurWeaponType)];
	return;
}

function name GetMagicShotAnimName()
{
	return MagicShotAnimName[int(CurWeaponType)];
	return;
}

function name GetMagicNoTargetAnimName()
{
	return MagicNoTargetAnimName[int(CurWeaponType)];
	return;
}

function name GetMagicFriendAnimName()
{
	return MagicFriendAnimName[int(CurWeaponType)];
	return;
}

function name GetPicItemAnimName()
{
	return PicItemAnimName[int(CurWeaponType)];
	return;
}

function name GetThrowAnimName()
{
	return ThrowAnimName[int(CurWeaponType)];
	return;
}

function name GetRiderWaitAnimName()
{
	// End:0x11
	if(__NFUN_129__(bRide))
	{
		return 'None';
	}
	return RiderWaitAnimName[int(CurRideType)];
	return;
}

function name GetRiderAtkAnimName()
{
	// End:0x11
	if(__NFUN_129__(bRide))
	{
		return 'None';
	}
	return RiderAtkAnimName[int(CurRideType)];
	return;
}

function name GetRiderRunAnimName()
{
	// End:0x11
	if(__NFUN_129__(bRide))
	{
		return 'None';
	}
	return RiderRunAnimName[int(CurRideType)];
	return;
}

function name GetRiderDeathWaitAnimName()
{
	// End:0x11
	if(__NFUN_129__(bRide))
	{
		return 'None';
	}
	return RiderDeathWaitAnimName[int(CurRideType)];
	return;
}

function name GetRiderDeathAnimName()
{
	// End:0x11
	if(__NFUN_129__(bRide))
	{
		return 'None';
	}
	return RiderDeathAnimName[int(CurRideType)];
	return;
}

simulated event SetHeadScale(float NewScale)
{
	return;
}

simulated event AnimBegin(name SequenceName)
{
	return;
}

// Export UPawn::execReachedDestination(FFrame&, void* const)
native function bool ReachedDestination(Actor Goal);

// Export UPawn::execForceCrouch(FFrame&, void* const)
native function ForceCrouch();

// Export UPawn::execPlayAnimUM(FFrame&, void* const)
native final function PlayAnimUM(name Sequence, optional int UpdateMovementAnim, optional float Rate, optional float TweenTime, optional int Channel);

function Reset()
{
	// End:0x25
	if(__NFUN_132__(__NFUN_114__(Controller, none), Controller.bIsPlayer))
	{
		__NFUN_279__();		
	}
	else
	{
		super.Reset();
	}
	return;
}

function Fire(optional float f)
{
	// End:0x1F
	if(__NFUN_119__(Weapon, none))
	{
		Weapon.Fire(f);
	}
	return;
}

function DrawHUD(Canvas Canvas)
{
	return;
}

function bool SpecialCalcView(out Actor ViewActor, out Vector CameraLocation, out Rotator CameraRotation)
{
	return;
}

simulated function string GetHumanReadableName()
{
	// End:0x1A
	if(__NFUN_119__(PlayerReplicationInfo, none))
	{
		return PlayerReplicationInfo.PlayerName;
	}
	return MenuName;
	return;
}

function PlayTeleportEffect(bool bOut, bool bSound)
{
	__NFUN_512__(1.0000000);
	return;
}

function PossessedBy(Controller C)
{
	Controller = C;
	NetPriority = 3.0000000;
	// End:0x52
	if(__NFUN_119__(C.PlayerReplicationInfo, none))
	{
		PlayerReplicationInfo = C.PlayerReplicationInfo;
		OwnerName = PlayerReplicationInfo.PlayerName;
	}
	// End:0x90
	if(C.__NFUN_303__('PlayerController'))
	{
		// End:0x87
		if(__NFUN_155__(int(Level.NetMode), int(NM_Standalone)))
		{
			RemoteRole = ROLE_AutonomousProxy;
		}
		BecomeViewTarget();		
	}
	else
	{
		RemoteRole = default.RemoteRole;
	}
	__NFUN_272__(Controller);
	EyeHeight = BaseEyeHeight;
	ChangeAnimation();
	return;
}

function UnPossessed()
{
	PlayerReplicationInfo = none;
	__NFUN_272__(none);
	Controller = none;
	return;
}

simulated function bool PointOfView()
{
	return false;
	return;
}

function BecomeViewTarget()
{
	bUpdateEyeheight = true;
	return;
}

function DropToGround()
{
	bCollideWorld = true;
	bInterpolating = false;
	// End:0x3F
	if(__NFUN_151__(Health, 0))
	{
		__NFUN_262__(true, true, true);
		__NFUN_3970__(2);
		// End:0x3F
		if(IsHumanControlled())
		{
			Controller.__NFUN_113__(LandMovementState);
		}
	}
	return;
}

function bool CanGrabLadder()
{
	return __NFUN_130__(__NFUN_130__(__NFUN_130__(bCanClimbLadders, __NFUN_119__(Controller, none)), __NFUN_155__(int(Physics), int(11))), __NFUN_132__(__NFUN_155__(int(Physics), int(2)), __NFUN_178__(__NFUN_186__(Velocity.Z), JumpZ)));
	return;
}

event SetWalking(bool bNewIsWalking)
{
	// End:0x24
	if(__NFUN_243__(bNewIsWalking, bIsWalking))
	{
		bIsWalking = bNewIsWalking;
		ChangeAnimation();
	}
	return;
}

function bool CanSplash()
{
	// End:0x70
	if(__NFUN_130__(__NFUN_130__(__NFUN_177__(__NFUN_175__(Level.TimeSeconds, SplashTime), 0.2500000), __NFUN_132__(__NFUN_154__(int(Physics), int(2)), __NFUN_154__(int(Physics), int(4)))), __NFUN_177__(__NFUN_186__(Velocity.Z), float(100))))
	{
		SplashTime = Level.TimeSeconds;
		return true;
	}
	return false;
	return;
}

function EndClimbLadder(LadderVolume OldLadder)
{
	// End:0x1A
	if(__NFUN_119__(Controller, none))
	{
		Controller.EndClimbLadder();
	}
	// End:0x2F
	if(__NFUN_154__(int(Physics), int(11)))
	{
		__NFUN_3970__(2);
	}
	return;
}

function ClimbLadder(LadderVolume L)
{
	OnLadder = L;
	__NFUN_299__(OnLadder.WallDir);
	__NFUN_3970__(11);
	// End:0x3A
	if(IsHumanControlled())
	{
		Controller.__NFUN_113__('PlayerClimbing');
	}
	return;
}

simulated function DisplayDebug(Canvas Canvas, out float YL, out float YPos)
{
	local string t;

	super.DisplayDebug(Canvas, YL, YPos);
	Canvas.SetDrawColor(byte(255), byte(255), byte(255));
	Canvas.__NFUN_465__(__NFUN_112__(__NFUN_112__(__NFUN_112__("Animation Action ", string(AnimAction)), " Health "), string(Health)));
	__NFUN_184__(YPos, YL);
	Canvas.SetPos(4.0000000, YPos);
	Canvas.__NFUN_465__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("Anchor ", string(Anchor)), " Serpentine Dist "), string(SerpentineDist)), " Time "), string(SerpentineTime)));
	__NFUN_184__(YPos, YL);
	Canvas.SetPos(4.0000000, YPos);
	t = __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("Floor ", string(Floor)), " DesiredSpeed "), string(DesiredSpeed)), " Crouched "), string(bIsCrouched)), " Try to uncrouch "), string(UncrouchTime));
	// End:0x1AE
	if(__NFUN_132__(__NFUN_119__(OnLadder, none), __NFUN_154__(int(Physics), int(11))))
	{
		t = __NFUN_112__(__NFUN_112__(t, " on ladder "), string(OnLadder));
	}
	Canvas.__NFUN_465__(t);
	__NFUN_184__(YPos, YL);
	Canvas.SetPos(4.0000000, YPos);
	Canvas.__NFUN_465__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("EyeHeight ", string(EyeHeight)), " BaseEyeHeight "), string(BaseEyeHeight)), " Physics Anim "), string(bPhysicsAnimUpdate)));
	__NFUN_184__(YPos, YL);
	Canvas.SetPos(4.0000000, YPos);
	// End:0x2C7
	if(__NFUN_114__(Controller, none))
	{
		Canvas.SetDrawColor(byte(255), 0, 0);
		Canvas.__NFUN_465__("NO CONTROLLER");
		__NFUN_184__(YPos, YL);
		Canvas.SetPos(4.0000000, YPos);		
	}
	else
	{
		Controller.DisplayDebug(Canvas, YL, YPos);
	}
	// End:0x346
	if(__NFUN_114__(Weapon, none))
	{
		Canvas.SetDrawColor(0, byte(255), 0);
		Canvas.__NFUN_465__("NO WEAPON");
		__NFUN_184__(YPos, YL);
		Canvas.SetPos(4.0000000, YPos);		
	}
	else
	{
		Weapon.DisplayDebug(Canvas, YL, YPos);
	}
	return;
}

simulated function Vector CalcDrawOffset(Inventory Inv)
{
	local Vector DrawOffset;

	// End:0x38
	if(__NFUN_114__(Controller, none))
	{
		return __NFUN_215__(__NFUN_276__(Inv.PlayerViewOffset, Rotation), __NFUN_213__(BaseEyeHeight, vect(0.0000000, 0.0000000, 1.0000000)));
	}
	DrawOffset = __NFUN_276__(__NFUN_213__(__NFUN_171__(__NFUN_172__(0.9000000, Weapon.DisplayFOV), float(100)), ModifiedPlayerViewOffset(Inv)), GetViewRotation());
	// End:0x8E
	if(__NFUN_129__(IsLocallyControlled()))
	{
		__NFUN_184__(DrawOffset.Z, BaseEyeHeight);		
	}
	else
	{
		__NFUN_184__(DrawOffset.Z, EyeHeight);
		// End:0xC3
		if(bWeaponBob)
		{
			__NFUN_223__(DrawOffset, WeaponBob(Inv.BobDamping));
		}
	}
	return DrawOffset;
	return;
}

function Vector ModifiedPlayerViewOffset(Inventory Inv)
{
	return Inv.PlayerViewOffset;
	return;
}

function Vector WeaponBob(float BobDamping)
{
	local Vector WBob;

	WBob = __NFUN_213__(BobDamping, WalkBob);
	WBob.Z = __NFUN_171__(__NFUN_174__(0.4500000, __NFUN_171__(0.5500000, BobDamping)), WalkBob.Z);
	return WBob;
	return;
}

function CheckBob(float DeltaTime, Vector Y)
{
	local float Speed2D;

	// End:0x2B
	if(__NFUN_129__(bWeaponBob))
	{
		bobtime = 0.0000000;
		WalkBob = vect(0.0000000, 0.0000000, 0.0000000);
		return;
	}
	Bob = __NFUN_246__(Bob, -0.0100000, 0.0100000);
	// End:0x19B
	if(__NFUN_154__(int(Physics), int(1)))
	{
		Speed2D = __NFUN_225__(Velocity);
		// End:0x83
		if(__NFUN_176__(Speed2D, float(10)))
		{
			__NFUN_184__(bobtime, __NFUN_171__(0.2000000, DeltaTime));			
		}
		else
		{
			__NFUN_184__(bobtime, __NFUN_171__(DeltaTime, __NFUN_174__(0.3000000, __NFUN_172__(__NFUN_171__(0.7000000, Speed2D), GroundSpeed))));
		}
		WalkBob = __NFUN_212__(__NFUN_212__(__NFUN_212__(Y, Bob), Speed2D), __NFUN_187__(__NFUN_171__(8.0000000, bobtime)));
		AppliedBob = __NFUN_171__(AppliedBob, __NFUN_175__(float(1), __NFUN_244__(1.0000000, __NFUN_171__(16.0000000, DeltaTime))));
		WalkBob.Z = AppliedBob;
		// End:0x151
		if(__NFUN_177__(Speed2D, float(10)))
		{
			WalkBob.Z = __NFUN_174__(WalkBob.Z, __NFUN_171__(__NFUN_171__(__NFUN_171__(0.7500000, Bob), Speed2D), __NFUN_187__(__NFUN_171__(16.0000000, bobtime))));
		}
		// End:0x198
		if(__NFUN_177__(LandBob, 0.0100000))
		{
			__NFUN_184__(AppliedBob, __NFUN_171__(__NFUN_244__(1.0000000, __NFUN_171__(16.0000000, DeltaTime)), LandBob));
			__NFUN_182__(LandBob, __NFUN_175__(float(1), __NFUN_171__(float(8), DeltaTime)));
		}		
	}
	else
	{
		// End:0x254
		if(__NFUN_154__(int(Physics), int(3)))
		{
			Speed2D = __NFUN_193__(__NFUN_174__(__NFUN_171__(Velocity.X, Velocity.X), __NFUN_171__(Velocity.Y, Velocity.Y)));
			WalkBob = __NFUN_212__(__NFUN_212__(__NFUN_212__(__NFUN_212__(Y, Bob), 0.5000000), Speed2D), __NFUN_187__(__NFUN_171__(4.0000000, Level.TimeSeconds)));
			WalkBob.Z = __NFUN_171__(__NFUN_171__(__NFUN_171__(Bob, 1.5000000), Speed2D), __NFUN_187__(__NFUN_171__(8.0000000, Level.TimeSeconds)));			
		}
		else
		{
			bobtime = 0.0000000;
			WalkBob = __NFUN_212__(WalkBob, __NFUN_175__(float(1), __NFUN_244__(1.0000000, __NFUN_171__(8.0000000, DeltaTime))));
		}
	}
	return;
}

simulated function bool IsPlayerPawn()
{
	return __NFUN_130__(__NFUN_119__(Controller, none), Controller.bIsPlayer);
	return;
}

simulated function bool WasPlayerPawn()
{
	return false;
	return;
}

simulated function bool IsHumanControlled()
{
	return __NFUN_119__(PlayerController(Controller), none);
	return;
}

simulated function bool IsLocallyControlled()
{
	// End:0x1B
	if(__NFUN_154__(int(Level.NetMode), int(NM_Standalone)))
	{
		return true;
	}
	// End:0x28
	if(__NFUN_114__(Controller, none))
	{
		return false;
	}
	// End:0x3A
	if(__NFUN_114__(PlayerController(Controller), none))
	{
		return true;
	}
	return __NFUN_119__(Viewport(PlayerController(Controller).Player), none);
	return;
}

simulated function bool IsFirstPerson()
{
	local PlayerController PC;

	PC = PlayerController(Controller);
	return __NFUN_130__(__NFUN_130__(__NFUN_119__(PC, none), __NFUN_129__(PC.bBehindView)), __NFUN_119__(Viewport(PC.Player), none));
	return;
}

simulated function Rotator GetViewRotation()
{
	// End:0x11
	if(__NFUN_114__(Controller, none))
	{
		return Rotation;
	}
	return Controller.GetViewRotation();
	return;
}

simulated function SetViewRotation(Rotator NewRotation)
{
	// End:0x1C
	if(__NFUN_119__(Controller, none))
	{
		Controller.__NFUN_299__(NewRotation);
	}
	return;
}

final function bool InGodMode()
{
	return __NFUN_130__(__NFUN_119__(Controller, none), Controller.bGodMode);
	return;
}

function bool NearMoveTarget()
{
	// End:0x23
	if(__NFUN_132__(__NFUN_114__(Controller, none), __NFUN_114__(Controller.MoveTarget, none)))
	{
		return false;
	}
	return ReachedDestination(Controller.MoveTarget);
	return;
}

final simulated function bool PressingFire()
{
	return __NFUN_130__(__NFUN_119__(Controller, none), __NFUN_155__(int(Controller.bFire), 0));
	return;
}

final simulated function bool PressingAltFire()
{
	return __NFUN_130__(__NFUN_119__(Controller, none), __NFUN_155__(int(Controller.bAltFire), 0));
	return;
}

function Actor GetMoveTarget()
{
	// End:0x0D
	if(__NFUN_114__(Controller, none))
	{
		return none;
	}
	return Controller.MoveTarget;
	return;
}

function SetMoveTarget(Actor NewTarget)
{
	// End:0x1F
	if(__NFUN_119__(Controller, none))
	{
		Controller.MoveTarget = NewTarget;
	}
	return;
}

function bool LineOfSightTo(Actor Other)
{
	return __NFUN_130__(__NFUN_119__(Controller, none), Controller.__NFUN_514__(Other));
	return;
}

final simulated function Rotator AdjustAim(Ammunition FiredAmmunition, Vector projStart, int aimerror)
{
	// End:0x11
	if(__NFUN_114__(Controller, none))
	{
		return Rotation;
	}
	return Controller.AdjustAim(FiredAmmunition, projStart, aimerror);
	return;
}

function Actor ShootSpecial(Actor A)
{
	// End:0x23
	if(__NFUN_132__(__NFUN_129__(Controller.bCanDoSpecial), __NFUN_114__(Weapon, none)))
	{
		return none;
	}
	Controller.FireWeaponAt(A);
	Controller.bFire = 0;
	return A;
	return;
}

function float AdjustedStrength()
{
	return 0.0000000;
	return;
}

function HandlePickup(Pickup pick)
{
	__NFUN_512__(0.2000000);
	// End:0x27
	if(__NFUN_119__(Controller, none))
	{
		Controller.HandlePickup(pick);
	}
	return;
}

function ReceiveLocalizedMessage(Class<LocalMessage> Message, optional int Switch, optional PlayerReplicationInfo RelatedPRI_1, optional PlayerReplicationInfo RelatedPRI_2, optional Object OptionalObject)
{
	// End:0x3D
	if(__NFUN_119__(PlayerController(Controller), none))
	{
		PlayerController(Controller).ReceiveLocalizedMessage(Message, Switch, RelatedPRI_1, RelatedPRI_2, OptionalObject);
	}
	return;
}

event ClientMessage(coerce string S, optional name Type)
{
	// End:0x2E
	if(__NFUN_119__(PlayerController(Controller), none))
	{
		PlayerController(Controller).ClientMessage(S, Type);
	}
	return;
}

function Trigger(Actor Other, Pawn EventInstigator)
{
	// End:0x24
	if(__NFUN_119__(Controller, none))
	{
		Controller.Trigger(Other, EventInstigator);
	}
	return;
}

function bool CanTrigger(Trigger t)
{
	return true;
	return;
}

function GiveWeapon(string aClassName)
{
	local Class<Weapon> WeaponClass;
	local Weapon newWeapon;

	WeaponClass = Class<Weapon>(DynamicLoadObject(aClassName, Class'Core.Class'));
	// End:0x2E
	if(__NFUN_119__(FindInventoryType(WeaponClass), none))
	{
		return;
	}
	newWeapon = __NFUN_278__(WeaponClass);
	// End:0x57
	if(__NFUN_119__(newWeapon, none))
	{
		newWeapon.GiveTo(self);
	}
	return;
}

function SetDisplayProperties(Actor.ERenderStyle NewStyle, Material NewTexture, bool bLighting)
{
	Style = NewStyle;
	Texture = NewTexture;
	bUnlit = bLighting;
	// End:0x4D
	if(__NFUN_119__(Weapon, none))
	{
		Weapon.SetDisplayProperties(Style, Texture, bUnlit);
	}
	// End:0x7C
	if(__NFUN_130__(__NFUN_129__(bUpdatingDisplay), __NFUN_119__(Inventory, none)))
	{
		bUpdatingDisplay = true;
		Inventory.SetOwnerDisplay();
	}
	bUpdatingDisplay = false;
	return;
}

function SetDefaultDisplayProperties()
{
	Style = default.Style;
	Texture = default.Texture;
	bUnlit = default.bUnlit;
	// End:0x3D
	if(__NFUN_119__(Weapon, none))
	{
		Weapon.SetDefaultDisplayProperties();
	}
	// End:0x6C
	if(__NFUN_130__(__NFUN_129__(bUpdatingDisplay), __NFUN_119__(Inventory, none)))
	{
		bUpdatingDisplay = true;
		Inventory.SetOwnerDisplay();
	}
	bUpdatingDisplay = false;
	return;
}

function FinishedInterpolation()
{
	DropToGround();
	return;
}

function JumpOutOfWater(Vector jumpDir)
{
	Falling();
	Velocity = __NFUN_212__(jumpDir, WaterSpeed);
	Acceleration = __NFUN_212__(jumpDir, AccelRate);
	Velocity.Z = __NFUN_245__(380.0000000, JumpZ);
	bUpAndOut = true;
	return;
}

simulated event ModifyVelocity(float DeltaTime, Vector OldVelocity)
{
	return;
}

event FellOutOfWorld(Actor.eKillZType KillType)
{
	// End:0x12
	if(__NFUN_150__(int(Role), int(ROLE_Authority)))
	{
		return;
	}
	// End:0x33
	if(__NFUN_130__(__NFUN_119__(Controller, none), Controller.AvoidCertainDeath()))
	{
		return;
	}
	Health = -1;
	// End:0x62
	if(__NFUN_154__(int(KillType), int(1)))
	{
		Died(none, Class'Engine.FellLava', Location);		
	}
	else
	{
		// End:0x86
		if(__NFUN_154__(int(KillType), int(2)))
		{
			Died(none, Class'Engine.fell', Location);			
		}
		else
		{
			__NFUN_3970__(0);
			Died(none, Class'Engine.fell', Location);
		}
	}
	return;
}

function ShouldCrouch(bool Crouch)
{
	bWantsToCrouch = Crouch;
	return;
}

event EndCrouch(float HeightAdjust)
{
	__NFUN_185__(EyeHeight, HeightAdjust);
	__NFUN_184__(OldZ, HeightAdjust);
	BaseEyeHeight = default.BaseEyeHeight;
	return;
}

event StartCrouch(float HeightAdjust)
{
	__NFUN_184__(EyeHeight, HeightAdjust);
	__NFUN_185__(OldZ, HeightAdjust);
	BaseEyeHeight = __NFUN_244__(__NFUN_171__(0.8000000, CrouchHeight), __NFUN_175__(CrouchHeight, float(10)));
	return;
}

function RestartPlayer()
{
	return;
}

function AddVelocity(Vector NewVelocity)
{
	// End:0x0B
	if(bIgnoreForces)
	{
		return;
	}
	// End:0x5A
	if(__NFUN_132__(__NFUN_154__(int(Physics), int(1)), __NFUN_130__(__NFUN_132__(__NFUN_154__(int(Physics), int(11)), __NFUN_154__(int(Physics), int(9))), __NFUN_177__(NewVelocity.Z, default.JumpZ))))
	{
		__NFUN_3970__(2);
	}
	// End:0x95
	if(__NFUN_130__(__NFUN_177__(Velocity.Z, float(380)), __NFUN_177__(NewVelocity.Z, float(0))))
	{
		__NFUN_182__(NewVelocity.Z, 0.5000000);
	}
	__NFUN_223__(Velocity, NewVelocity);
	return;
}

function KilledBy(Pawn EventInstigator)
{
	local Controller Killer;

	Health = 0;
	// End:0x26
	if(__NFUN_119__(EventInstigator, none))
	{
		Killer = EventInstigator.Controller;
	}
	Died(Killer, Class'Engine.Suicided', Location);
	return;
}

function TakeFallingDamage()
{
	local float Shake, EffectiveSpeed;

	// End:0x154
	if(__NFUN_176__(Velocity.Z, __NFUN_171__(-0.5000000, MaxFallSpeed)))
	{
		// End:0x151
		if(__NFUN_154__(int(Role), int(ROLE_Authority)))
		{
			__NFUN_512__(1.0000000);
			// End:0x151
			if(__NFUN_176__(Velocity.Z, __NFUN_171__(float(-1), MaxFallSpeed)))
			{
				EffectiveSpeed = Velocity.Z;
				// End:0x74
				if(TouchingWaterVolume())
				{
					__NFUN_184__(EffectiveSpeed, float(100));
				}
				// End:0xC8
				if(__NFUN_176__(EffectiveSpeed, __NFUN_171__(float(-1), MaxFallSpeed)))
				{
					TakeDamage(int(__NFUN_172__(__NFUN_171__(float(-100), __NFUN_174__(EffectiveSpeed, MaxFallSpeed)), MaxFallSpeed)), none, Location, vect(0.0000000, 0.0000000, 0.0000000), Class'Engine.fell');
				}
				// End:0x151
				if(__NFUN_119__(Controller, none))
				{
					Shake = __NFUN_244__(1.0000000, __NFUN_172__(__NFUN_171__(-1.0000000, Velocity.Z), MaxFallSpeed));
					Controller.ShakeView(__NFUN_174__(0.1750000, __NFUN_171__(0.1000000, Shake)), __NFUN_171__(850.0000000, Shake), __NFUN_213__(Shake, vect(0.0000000, 0.0000000, 1.5000000)), 120000.0000000, vect(0.0000000, 0.0000000, 10.0000000), 1.0000000);
				}
			}
		}		
	}
	else
	{
		// End:0x177
		if(__NFUN_176__(Velocity.Z, __NFUN_171__(-1.4000000, JumpZ)))
		{
			__NFUN_512__(0.5000000);
		}
	}
	return;
}

function ClientReStart()
{
	Velocity = vect(0.0000000, 0.0000000, 0.0000000);
	Acceleration = vect(0.0000000, 0.0000000, 0.0000000);
	BaseEyeHeight = default.BaseEyeHeight;
	EyeHeight = BaseEyeHeight;
	PlayWaiting();
	return;
}

function ClientSetLocation(Vector NewLocation, Rotator NewRotation)
{
	// End:0x24
	if(__NFUN_119__(Controller, none))
	{
		Controller.ClientSetLocation(NewLocation, NewRotation);
	}
	return;
}

function ClientSetRotation(Rotator NewRotation)
{
	// End:0x1F
	if(__NFUN_119__(Controller, none))
	{
		Controller.ClientSetRotation(NewRotation);
	}
	return;
}

simulated function FaceRotation(Rotator NewRotation, float DeltaTime)
{
	// End:0x24
	if(__NFUN_154__(int(Physics), int(11)))
	{
		__NFUN_299__(OnLadder.WallDir);		
	}
	else
	{
		// End:0x52
		if(__NFUN_132__(__NFUN_154__(int(Physics), int(1)), __NFUN_154__(int(Physics), int(2))))
		{
			NewRotation.Pitch = 0;
		}
		__NFUN_299__(NewRotation);
	}
	return;
}

function ClientDying(Class<DamageType> DamageType, Vector HitLocation)
{
	// End:0x24
	if(__NFUN_119__(Controller, none))
	{
		Controller.ClientDying(DamageType, HitLocation);
	}
	return;
}

function bool InCurrentCombo()
{
	return false;
	return;
}

simulated function bool CanThrowWeapon()
{
	return __NFUN_130__(__NFUN_119__(Weapon, none), __NFUN_132__(__NFUN_114__(Level.Game, none), __NFUN_129__(Level.Game.bWeaponStay)));
	return;
}

function TossWeapon(Vector TossVel)
{
	local Vector X, Y, Z;

	Weapon.Velocity = TossVel;
	__NFUN_229__(Rotation, X, Y, Z);
	Weapon.DropFrom(__NFUN_216__(__NFUN_215__(Location, __NFUN_213__(__NFUN_171__(0.8000000, CollisionRadius), X)), __NFUN_213__(__NFUN_171__(0.5000000, CollisionRadius), Y)));
	return;
}

exec function NextItem()
{
	// End:0x22
	if(__NFUN_114__(SelectedItem, none))
	{
		SelectedItem = Inventory.SelectNext();
		return;
	}
	// End:0x57
	if(__NFUN_119__(SelectedItem.Inventory, none))
	{
		SelectedItem = SelectedItem.Inventory.SelectNext();		
	}
	else
	{
		SelectedItem = Inventory.SelectNext();
	}
	// End:0x8C
	if(__NFUN_114__(SelectedItem, none))
	{
		SelectedItem = Inventory.SelectNext();
	}
	return;
}

function Inventory FindInventoryType(Class DesiredClass)
{
	local Inventory Inv;
	local int Count;

	Inv = Inventory;
	J0x0B:

	// End:0x63 [Loop If]
	if(__NFUN_119__(Inv, none))
	{
		// End:0x34
		if(__NFUN_114__(Inv.Class, DesiredClass))
		{
			return Inv;
		}
		__NFUN_165__(Count);
		// End:0x4C
		if(__NFUN_151__(Count, 1000))
		{
			return none;
		}
		Inv = Inv.Inventory;
		// [Loop Continue]
		goto J0x0B;
	}
	return none;
	return;
}

function bool AddInventory(Inventory NewItem)
{
	local Inventory Inv;
	local Actor Last;

	Last = self;
	// End:0x3A
	if(__NFUN_114__(NewItem, none))
	{
		__NFUN_231__(__NFUN_112__("tried to add none inventory to ", string(self)));
	}
	Inv = Inventory;
	J0x45:

	// End:0x83 [Loop If]
	if(__NFUN_119__(Inv, none))
	{
		// End:0x61
		if(__NFUN_114__(Inv, NewItem))
		{
			return false;
		}
		Last = Inv;
		Inv = Inv.Inventory;
		// [Loop Continue]
		goto J0x45;
	}
	NewItem.__NFUN_272__(self);
	NewItem.Inventory = none;
	Last.Inventory = NewItem;
	// End:0xD3
	if(__NFUN_119__(Controller, none))
	{
		Controller.NotifyAddInventory(NewItem);
	}
	return true;
	return;
}

function DeleteInventory(Inventory item)
{
	local Actor Link;
	local int Count;

	// End:0x16
	if(__NFUN_114__(item, Weapon))
	{
		Weapon = none;
	}
	// End:0x2C
	if(__NFUN_114__(item, SelectedItem))
	{
		SelectedItem = none;
	}
	Link = self;
	J0x33:

	// End:0xCF [Loop If]
	if(__NFUN_119__(Link, none))
	{
		// End:0x86
		if(__NFUN_114__(Link.Inventory, item))
		{
			Link.Inventory = item.Inventory;
			item.Inventory = none;
			// [Explicit Break]
			goto J0xCF;
		}
		// End:0xB8
		if(__NFUN_154__(int(Level.NetMode), int(NM_Client)))
		{
			__NFUN_165__(Count);
			// End:0xB8
			if(__NFUN_151__(Count, 1000))
			{
				// [Explicit Break]
				goto J0xCF;
			}
		}
		Link = Link.Inventory;
		// [Loop Continue]
		goto J0x33;
	}
	J0xCF:

	item.__NFUN_272__(none);
	return;
}

function ChangedWeapon()
{
	local Weapon OldWeapon;

	OldWeapon = Weapon;
	// End:0x76
	if(__NFUN_114__(Weapon, PendingWeapon))
	{
		// End:0x39
		if(__NFUN_114__(Weapon, none))
		{
			Controller.SwitchToBestWeapon();
			return;			
		}
		else
		{
			// End:0x5D
			if(Weapon.__NFUN_281__('DownWeapon'))
			{
				Weapon.__NFUN_113__('Idle');
			}
		}
		PendingWeapon = none;
		ServerChangedWeapon(OldWeapon, Weapon);
		return;
	}
	// End:0x8C
	if(__NFUN_114__(PendingWeapon, none))
	{
		PendingWeapon = Weapon;
	}
	Weapon = PendingWeapon;
	// End:0xCC
	if(__NFUN_130__(__NFUN_119__(Weapon, none), __NFUN_154__(int(Level.NetMode), int(NM_Client))))
	{
		Weapon.BringUp();
	}
	PendingWeapon = none;
	Weapon.Instigator = self;
	ServerChangedWeapon(OldWeapon, Weapon);
	// End:0x10D
	if(__NFUN_119__(Controller, none))
	{
		Controller.ChangedWeapon();
	}
	return;
}

function name GetWeaponBoneFor(Inventory i)
{
	return 'righthand';
	return;
}

function ServerChangedWeapon(Weapon OldWeapon, Weapon W)
{
	// End:0x2A
	if(__NFUN_119__(OldWeapon, none))
	{
		OldWeapon.SetDefaultDisplayProperties();
		OldWeapon.DetachFromPawn(self);
	}
	Weapon = W;
	// End:0x42
	if(__NFUN_114__(Weapon, none))
	{
		return;
	}
	// End:0x5D
	if(__NFUN_119__(Weapon, none))
	{
		Weapon.AttachToPawn(self);
	}
	Weapon.SetRelativeLocation(Weapon.default.RelativeLocation);
	Weapon.SetRelativeRotation(Weapon.default.RelativeRotation);
	// End:0xE2
	if(__NFUN_114__(OldWeapon, Weapon))
	{
		// End:0xC9
		if(Weapon.__NFUN_281__('DownWeapon'))
		{
			Weapon.BringUp();
		}
		Inventory.OwnerEvent('ChangedWeapon');
		return;		
	}
	else
	{
		// End:0x117
		if(__NFUN_119__(Level.Game, none))
		{
			__NFUN_512__(__NFUN_171__(0.1000000, Level.Game.GameDifficulty));
		}
	}
	Inventory.OwnerEvent('ChangedWeapon');
	PlayWeaponSwitch(W);
	Weapon.BringUp();
	return;
}

event bool EncroachingOn(Actor Other)
{
	// End:0x14
	if(Other.bWorldGeometry)
	{
		return true;
	}
	// End:0x54
	if(__NFUN_130__(__NFUN_132__(__NFUN_132__(__NFUN_114__(Controller, none), __NFUN_129__(Controller.bIsPlayer)), bWarping), __NFUN_119__(Pawn(Other), none)))
	{
		return true;
	}
	return false;
	return;
}

event EncroachedBy(Actor Other)
{
	// End:0x1B
	if(__NFUN_119__(Pawn(Other), none))
	{
		gibbedBy(Other);
	}
	return;
}

function gibbedBy(Actor Other)
{
	// End:0x12
	if(__NFUN_150__(int(Role), int(ROLE_Authority)))
	{
		return;
	}
	// End:0x48
	if(__NFUN_119__(Pawn(Other), none))
	{
		Died(Pawn(Other).Controller, Class'Engine.DamTypeTelefragged', Location);		
	}
	else
	{
		Died(none, Class'Engine.Gibbed', Location);
	}
	return;
}

function JumpOffPawn()
{
	__NFUN_223__(Velocity, __NFUN_213__(__NFUN_174__(float(100), CollisionRadius), __NFUN_252__()));
	Velocity.Z = __NFUN_174__(200.0000000, CollisionHeight);
	__NFUN_3970__(2);
	bNoJumpAdjust = true;
	Controller.SetFall();
	return;
}

singular event BaseChange()
{
	local float decorMass;

	// End:0x0B
	if(bInterpolating)
	{
		return;
	}
	// End:0x30
	if(__NFUN_130__(__NFUN_114__(Base, none), __NFUN_154__(int(Physics), int(0))))
	{
		__NFUN_3970__(2);		
	}
	else
	{
		// End:0xBD
		if(__NFUN_119__(Pawn(Base), none))
		{
			// End:0xBA
			if(__NFUN_129__(Pawn(Base).bCanBeBaseForPawns))
			{
				Base.TakeDamage(int(__NFUN_172__(__NFUN_171__(float(int(__NFUN_175__(float(1), __NFUN_172__(Velocity.Z, float(400))))), Mass), Base.Mass)), self, Location, __NFUN_213__(0.5000000, Velocity), Class'Engine.Crushed');
				JumpOffPawn();
			}			
		}
		else
		{
			// End:0x157
			if(__NFUN_130__(__NFUN_119__(Decoration(Base), none), __NFUN_176__(Velocity.Z, float(-400))))
			{
				decorMass = __NFUN_245__(Decoration(Base).Mass, 1.0000000);
				Base.TakeDamage(int(__NFUN_172__(__NFUN_171__(__NFUN_172__(__NFUN_171__(float(-2), Mass), decorMass), Velocity.Z), float(400))), self, Location, __NFUN_213__(0.5000000, Velocity), Class'Engine.Crushed');
			}
		}
	}
	return;
}

event UpdateEyeHeight(float DeltaTime)
{
	local float smooth, MaxEyeHeight, OldEyeHeight;
	local Actor HitActor;
	local Vector HitLocation, HitNormal;

	// End:0x18
	if(__NFUN_114__(Controller, none))
	{
		EyeHeight = 0.0000000;
		return;
	}
	// End:0x36
	if(bTearOff)
	{
		EyeHeight = 0.0000000;
		bUpdateEyeheight = false;
		return;
	}
	HitActor = __NFUN_277__(HitLocation, HitNormal, __NFUN_215__(Location, __NFUN_213__(__NFUN_174__(__NFUN_174__(CollisionHeight, 10.0000000), float(14)), vect(0.0000000, 0.0000000, 1.0000000))), __NFUN_215__(Location, __NFUN_213__(CollisionHeight, vect(0.0000000, 0.0000000, 1.0000000))), true);
	// End:0xAD
	if(__NFUN_114__(HitActor, none))
	{
		MaxEyeHeight = __NFUN_174__(CollisionHeight, 10.0000000);		
	}
	else
	{
		MaxEyeHeight = __NFUN_175__(__NFUN_175__(HitLocation.Z, Location.Z), float(14));
	}
	smooth = __NFUN_244__(1.0000000, __NFUN_172__(__NFUN_171__(10.0000000, DeltaTime), Level.TimeDilation));
	// End:0x163
	if(Controller.WantsSmoothedView())
	{
		OldEyeHeight = EyeHeight;
		EyeHeight = __NFUN_246__(__NFUN_174__(__NFUN_171__(__NFUN_174__(__NFUN_175__(EyeHeight, Location.Z), OldZ), __NFUN_175__(float(1), smooth)), __NFUN_171__(BaseEyeHeight, smooth)), __NFUN_171__(-0.5000000, CollisionHeight), MaxEyeHeight);		
	}
	else
	{
		bJustLanded = false;
		EyeHeight = __NFUN_244__(__NFUN_174__(__NFUN_171__(EyeHeight, __NFUN_175__(float(1), smooth)), __NFUN_171__(BaseEyeHeight, smooth)), MaxEyeHeight);
	}
	Controller.AdjustView(DeltaTime);
	return;
}

function Vector EyePosition()
{
	return __NFUN_215__(__NFUN_213__(EyeHeight, vect(0.0000000, 0.0000000, 1.0000000)), WalkBob);
	return;
}

simulated event Destroyed()
{
	// End:0x17
	if(__NFUN_119__(Shadow, none))
	{
		Shadow.__NFUN_279__();
	}
	// End:0x32
	if(__NFUN_119__(Controller, none))
	{
		Controller.PawnDied(self);
	}
	// End:0x4D
	if(__NFUN_154__(int(Level.NetMode), int(NM_Client)))
	{
		return;
	}
	J0x4D:

	// End:0x67 [Loop If]
	if(__NFUN_119__(Inventory, none))
	{
		Inventory.__NFUN_279__();
		// [Loop Continue]
		goto J0x4D;
	}
	Weapon = none;
	super.Destroyed();
	return;
}

event PreBeginPlay()
{
	super.PreBeginPlay();
	Instigator = self;
	DesiredRotation = Rotation;
	// End:0x23
	if(bDeleteMe)
	{
		return;
	}
	// End:0x42
	if(__NFUN_180__(BaseEyeHeight, float(0)))
	{
		BaseEyeHeight = __NFUN_171__(0.8000000, CollisionHeight);
	}
	EyeHeight = BaseEyeHeight;
	// End:0x6C
	if(__NFUN_122__(MenuName, ""))
	{
		MenuName = GetItemName(string(Class));
	}
	return;
}

event PostBeginPlay()
{
	local AIScript A;

	super.PostBeginPlay();
	SplashTime = 0.0000000;
	SpawnTime = Level.TimeSeconds;
	EyeHeight = BaseEyeHeight;
	OldRotYaw = float(Rotation.Yaw);
	GroundSpeed = GroundMaxSpeed;
	WantedYaw = Rotation.Yaw;
	LastFootRot.Yaw = 0;
	LastFootRot.Pitch = 0;
	LastFootRot.Roll = 0;
	RecentHitWaterInterval = 0.0000000;
	// End:0x164
	if(__NFUN_130__(__NFUN_130__(Level.bStartup, __NFUN_151__(Health, 0)), __NFUN_129__(bDontPossess)))
	{
		// End:0x109
		if(__NFUN_255__(AIScriptTag, 'None'))
		{
			// End:0xE0
			foreach __NFUN_304__(Class'Engine.AIScript', A, AIScriptTag)
			{
				// End:0xE0
				break;				
			}			
			// End:0x109
			if(__NFUN_119__(A, none))
			{
				A.SpawnControllerFor(self);
				// End:0x109
				if(__NFUN_119__(Controller, none))
				{
					return;
				}
			}
		}
		// End:0x12F
		if(__NFUN_130__(__NFUN_119__(ControllerClass, none), __NFUN_114__(Controller, none)))
		{
			Controller = __NFUN_278__(ControllerClass);
		}
		// End:0x164
		if(__NFUN_119__(Controller, none))
		{
			Controller.Possess(self);
			__NFUN_184__(AIController(Controller).Skill, SkillModifier);
		}
	}
	return;
}

simulated event PostNetBeginPlay()
{
	// End:0x40
	if(__NFUN_132__(Level.bDropDetail, __NFUN_154__(int(Level.DetailMode), int(0))))
	{
		MaxLights = byte(__NFUN_249__(4, int(MaxLights)));
	}
	// End:0x52
	if(__NFUN_154__(int(Role), int(ROLE_Authority)))
	{
		return;
	}
	// End:0xB1
	if(__NFUN_119__(Controller, none))
	{
		Controller.Pawn = self;
		// End:0xB1
		if(__NFUN_130__(__NFUN_119__(PlayerController(Controller), none), __NFUN_114__(PlayerController(Controller).ViewTarget, Controller)))
		{
			PlayerController(Controller).SetViewTarget(self);
		}
	}
	// End:0xC9
	if(__NFUN_154__(int(Role), int(ROLE_AutonomousProxy)))
	{
		bUpdateEyeheight = true;
	}
	// End:0xFB
	if(__NFUN_130__(__NFUN_119__(PlayerReplicationInfo, none), __NFUN_114__(PlayerReplicationInfo.Owner, none)))
	{
		PlayerReplicationInfo.__NFUN_272__(Controller);
	}
	PlayWaiting();
	return;
}

simulated function SetMesh()
{
	// End:0x0D
	if(__NFUN_119__(Mesh, none))
	{
		return;
	}
	LinkMesh(default.Mesh);
	return;
}

function Gasp()
{
	return;
}

function SetMovementPhysics()
{
	return;
}

function TakeDamage(int Damage, Pawn instigatedBy, Vector HitLocation, Vector Momentum, Class<DamageType> DamageType)
{
	local int actualDamage;
	local bool bAlreadyDead;
	local Controller Killer;

	// End:0x57
	if(__NFUN_114__(DamageType, none))
	{
		__NFUN_232__(__NFUN_112__(__NFUN_112__(__NFUN_112__("No damagetype for damage by ", string(instigatedBy)), " with weapon "), string(instigatedBy.Weapon)));
	}
	// End:0xA0
	if(__NFUN_150__(int(Role), int(ROLE_Authority)))
	{
		__NFUN_231__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(string(self), " client damage type "), string(DamageType)), " by "), string(instigatedBy)));
		return;
	}
	bAlreadyDead = __NFUN_152__(Health, 0);
	// End:0xC5
	if(__NFUN_154__(int(Physics), int(0)))
	{
		SetMovementPhysics();
	}
	// End:0xFA
	if(__NFUN_154__(int(Physics), int(1)))
	{
		Momentum.Z = __NFUN_245__(Momentum.Z, __NFUN_171__(0.4000000, __NFUN_225__(Momentum)));
	}
	// End:0x163
	if(__NFUN_132__(__NFUN_114__(instigatedBy, self), __NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_119__(Controller, none), __NFUN_119__(instigatedBy, none)), __NFUN_119__(instigatedBy.Controller, none)), instigatedBy.Controller.SameTeamAs(Controller))))
	{
		__NFUN_221__(Momentum, 0.6000000);
	}
	Momentum = __NFUN_214__(Momentum, Mass);
	actualDamage = Level.Game.ReduceDamage(Damage, self, instigatedBy, HitLocation, Momentum, DamageType);
	__NFUN_162__(Health, actualDamage);
	// End:0x1DB
	if(__NFUN_217__(HitLocation, vect(0.0000000, 0.0000000, 0.0000000)))
	{
		HitLocation = Location;
	}
	// End:0x25D
	if(bAlreadyDead)
	{
		__NFUN_232__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(string(self), " took regular damage "), string(DamageType)), " from "), string(instigatedBy)), " while already dead at "), string(Level.TimeSeconds)));
		ChunkUp(Rotation, DamageType);
		return;
	}
	PlayHit(float(actualDamage), instigatedBy, HitLocation, DamageType, Momentum);
	// End:0x2D5
	if(__NFUN_152__(Health, 0))
	{
		// End:0x2A9
		if(__NFUN_119__(instigatedBy, none))
		{
			Killer = instigatedBy.GetKillerController();
		}
		// End:0x2BD
		if(bPhysicsAnimUpdate)
		{
			TearOffMomentum = Momentum;
		}
		Died(Killer, DamageType, HitLocation);		
	}
	else
	{
		// End:0x33E
		if(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_119__(instigatedBy, none), __NFUN_119__(instigatedBy, self)), __NFUN_119__(Controller, none)), __NFUN_119__(instigatedBy.Controller, none)), instigatedBy.Controller.SameTeamAs(Controller)))
		{
			__NFUN_221__(Momentum, 0.5000000);
		}
		AddVelocity(Momentum);
		// End:0x37C
		if(__NFUN_119__(Controller, none))
		{
			Controller.NotifyTakeHit(instigatedBy, HitLocation, actualDamage, DamageType, Momentum);
		}
	}
	__NFUN_512__(1.0000000);
	return;
}

function TeamInfo GetTeam()
{
	// End:0x1A
	if(__NFUN_119__(PlayerReplicationInfo, none))
	{
		return PlayerReplicationInfo.Team;
	}
	return none;
	return;
}

function Controller GetKillerController()
{
	return Controller;
	return;
}

event GetEffTargetLocation(out Vector LocVector)
{
	local Coords C;

	// End:0x23
	if(__NFUN_155__(EffectSpawnBoneIdx, -1))
	{
		C = GetBoneCoordsWithBoneIndex(EffectSpawnBoneIdx);		
	}
	else
	{
		// End:0x45
		if(HasBoneName(SpineBone))
		{
			C = GetBoneCoords(SpineBone);			
		}
		else
		{
			C = GetBoneCoordsWithBoneIndex(0);
		}
	}
	LocVector = C.Origin;
	return;
}

function Died(Controller Killer, Class<DamageType> DamageType, Vector HitLocation)
{
	local Vector TossVel;

	// End:0x0B
	if(bDeleteMe)
	{
		return;
	}
	// End:0x46
	if(Level.Game.PreventDeath(self, Killer, DamageType, HitLocation))
	{
		Health = __NFUN_250__(Health, 1);
		return;
	}
	Health = __NFUN_249__(0, Health);
	// End:0xC8
	if(__NFUN_119__(Weapon, none))
	{
		// End:0x7E
		if(__NFUN_119__(Controller, none))
		{
			Controller.LastPawnWeapon = Weapon;
		}
		TossVel = Vector(GetViewRotation());
		TossVel = __NFUN_215__(__NFUN_212__(TossVel, __NFUN_174__(__NFUN_219__(Velocity, TossVel), float(500))), vect(0.0000000, 0.0000000, 200.0000000));
		TossWeapon(TossVel);
	}
	// End:0x112
	if(__NFUN_119__(Controller, none))
	{
		Controller.WasKilledBy(Killer);
		Level.Game.Killed(Killer, Controller, self, DamageType);		
	}
	else
	{
		Level.Game.Killed(Killer, Controller(Owner), self, DamageType);
	}
	// End:0x167
	if(__NFUN_119__(Killer, none))
	{
		TriggerEvent(Event, self, Killer.Pawn);		
	}
	else
	{
		TriggerEvent(Event, self, none);
	}
	__NFUN_182__(Velocity.Z, 1.3000000);
	// End:0x1A2
	if(IsHumanControlled())
	{
		PlayerController(Controller).ForceDeathUpdate();
	}
	// End:0x1D4
	if(__NFUN_130__(__NFUN_119__(DamageType, none), DamageType.default.bAlwaysGibs))
	{
		ChunkUp(Rotation, DamageType);		
	}
	else
	{
		PlayDying(DamageType, HitLocation);
		// End:0x201
		if(Level.Game.bGameEnded)
		{
			return;
		}
		// End:0x229
		if(__NFUN_130__(__NFUN_129__(bPhysicsAnimUpdate), __NFUN_129__(IsLocallyControlled())))
		{
			ClientDying(DamageType, HitLocation);
		}
	}
	return;
}

function bool Gibbed(Class<DamageType> DamageType)
{
	// End:0x18
	if(__NFUN_180__(DamageType.default.GibModifier, float(0)))
	{
		return false;
	}
	// End:0x31
	if(__NFUN_179__(DamageType.default.GibModifier, float(100)))
	{
		return true;
	}
	// End:0x61
	if(__NFUN_132__(__NFUN_150__(Health, -80), __NFUN_130__(__NFUN_150__(Health, -40), __NFUN_176__(__NFUN_195__(), 0.6000000))))
	{
		return true;
	}
	return false;
	return;
}

event Falling()
{
	// End:0x1A
	if(__NFUN_119__(Controller, none))
	{
		Controller.SetFall();
	}
	return;
}

event HitWall(Vector HitNormal, Actor Wall)
{
	return;
}

event Landed(Vector HitNormal)
{
	LandBob = __NFUN_244__(50.0000000, __NFUN_171__(0.0550000, Velocity.Z));
	// End:0x39
	if(__NFUN_151__(Health, 0))
	{
		PlayLanded(Velocity.Z);
	}
	bJustLanded = true;
	return;
}

event HeadVolumeChange(PhysicsVolume newHeadVolume)
{
	// End:0x28
	if(__NFUN_132__(__NFUN_154__(int(Level.NetMode), int(NM_Client)), __NFUN_114__(Controller, none)))
	{
		return;
	}
	// End:0x35
	if(__NFUN_114__(HeadVolume, none))
	{
		return;
	}
	// End:0x42
	if(__NFUN_114__(newHeadVolume, none))
	{
		return;
	}
	// End:0xAD
	if(HeadVolume.bWaterVolume)
	{
		// End:0xAA
		if(__NFUN_129__(newHeadVolume.bWaterVolume))
		{
			// End:0x9F
			if(__NFUN_130__(__NFUN_130__(Controller.bIsPlayer, __NFUN_177__(BreathTime, float(0))), __NFUN_176__(BreathTime, float(8))))
			{
				Gasp();
			}
			BreathTime = -1.0000000;
		}		
	}
	else
	{
		// End:0xCA
		if(newHeadVolume.bWaterVolume)
		{
			BreathTime = UnderWaterTime;
		}
	}
	return;
}

function bool TouchingWaterVolume()
{
	local PhysicsVolume V;

	// End:0x26
	foreach __NFUN_307__(Class'Engine.PhysicsVolume', V)
	{
		// End:0x25
		if(V.bWaterVolume)
		{			
			return true;
		}		
	}	
	return false;
	return;
}

function bool IsInPain()
{
	local PhysicsVolume V;

	// End:0x58
	foreach __NFUN_307__(Class'Engine.PhysicsVolume', V)
	{
		// End:0x57
		if(__NFUN_130__(__NFUN_130__(V.bPainCausing, __NFUN_119__(V.DamageType, ReducedDamageType)), __NFUN_177__(V.DamagePerSec, float(0))))
		{			
			return true;
		}		
	}	
	return false;
	return;
}

event BreathTimer()
{
	// End:0x28
	if(__NFUN_132__(__NFUN_150__(Health, 0), __NFUN_154__(int(Level.NetMode), int(NM_Client))))
	{
		return;
	}
	TakeDrowningDamage();
	// End:0x44
	if(__NFUN_151__(Health, 0))
	{
		BreathTime = 2.0000000;
	}
	return;
}

function TakeDrowningDamage()
{
	return;
}

function bool CheckWaterJump(out Vector WallNormal)
{
	local Actor HitActor;
	local Vector HitLocation, HitNormal, checkpoint, Start, checkNorm, Extent;

	checkpoint = Vector(Rotation);
	checkpoint.Z = 0.0000000;
	checkNorm = __NFUN_226__(checkpoint);
	checkpoint = __NFUN_215__(Location, __NFUN_213__(CollisionRadius, checkNorm));
	Extent = __NFUN_213__(CollisionRadius, vect(1.0000000, 1.0000000, 0.0000000));
	Extent.Z = CollisionHeight;
	HitActor = __NFUN_277__(HitLocation, HitNormal, checkpoint, Location, true, Extent);
	// End:0x12E
	if(__NFUN_130__(__NFUN_119__(HitActor, none), __NFUN_114__(Pawn(HitActor), none)))
	{
		WallNormal = __NFUN_213__(float(-1), HitNormal);
		Start = Location;
		__NFUN_184__(Start.Z, __NFUN_171__(1.1000000, 10.0000000));
		checkpoint = __NFUN_215__(Start, __NFUN_213__(__NFUN_171__(float(2), CollisionRadius), checkNorm));
		HitActor = __NFUN_277__(HitLocation, HitNormal, checkpoint, Start, true);
		// End:0x12E
		if(__NFUN_114__(HitActor, none))
		{
			return true;
		}
	}
	return false;
	return;
}

function DoDoubleJump(bool bUpdating)
{
	return;
}

function bool CanDoubleJump()
{
	return;
}

function UpdateRocketAcceleration(float DeltaTime, float YawChange, float PitchChange)
{
	return;
}

function bool Dodge(Actor.EDoubleClickDir DoubleClickMove)
{
	return false;
	return;
}

function bool DoJump(bool bUpdating)
{
	// End:0x19A
	if(__NFUN_130__(__NFUN_130__(__NFUN_129__(bIsCrouched), __NFUN_129__(bWantsToCrouch)), __NFUN_132__(__NFUN_132__(__NFUN_154__(int(Physics), int(1)), __NFUN_154__(int(Physics), int(11))), __NFUN_154__(int(Physics), int(9)))))
	{
		// End:0xDF
		if(__NFUN_154__(int(Role), int(ROLE_Authority)))
		{
			// End:0xB5
			if(__NFUN_130__(__NFUN_119__(Level.Game, none), __NFUN_177__(Level.Game.GameDifficulty, float(2))))
			{
				__NFUN_512__(__NFUN_171__(0.1000000, Level.Game.GameDifficulty));
			}
			// End:0xDF
			if(__NFUN_130__(bCountJumps, __NFUN_119__(Inventory, none)))
			{
				Inventory.OwnerEvent('Jumped');
			}
		}
		// End:0x104
		if(__NFUN_154__(int(Physics), int(9)))
		{
			Velocity = __NFUN_213__(JumpZ, Floor);			
		}
		else
		{
			// End:0x127
			if(__NFUN_154__(int(Physics), int(11)))
			{
				Velocity.Z = 0.0000000;				
			}
			else
			{
				// End:0x143
				if(bIsWalking)
				{
					Velocity.Z = default.JumpZ;					
				}
				else
				{
					Velocity.Z = JumpZ;
				}
			}
		}
		// End:0x193
		if(__NFUN_130__(__NFUN_119__(Base, none), __NFUN_129__(Base.bWorldGeometry)))
		{
			__NFUN_184__(Velocity.Z, Base.Velocity.Z);
		}
		__NFUN_3970__(2);
		return true;
	}
	return false;
	return;
}

function PlayMoverHitSound()
{
	return;
}

function PlayDyingSound()
{
	return;
}

function PlayHit(float Damage, Pawn instigatedBy, Vector HitLocation, Class<DamageType> DamageType, Vector Momentum)
{
	local Vector BloodOffset, Mo, HitNormal;
	local Class<Effects> DesiredEffect;
	local Class<Emitter> DesiredEmitter;

	// End:0x32
	if(__NFUN_130__(__NFUN_178__(Damage, float(0)), __NFUN_132__(__NFUN_114__(Controller, none), __NFUN_129__(Controller.bGodMode))))
	{
		return;
	}
	// End:0x1C0
	if(__NFUN_177__(Damage, float(DamageType.default.DamageThreshold)))
	{
		HitNormal = __NFUN_226__(__NFUN_216__(HitLocation, Location));
		// End:0x1C0
		if(EffectIsRelevant(Location, true))
		{
			DesiredEffect = DamageType.static.GetPawnDamageEffect(HitLocation, Damage, Momentum, self, __NFUN_132__(Level.bDropDetail, __NFUN_154__(int(Level.DetailMode), int(0))));
			// End:0x149
			if(__NFUN_119__(DesiredEffect, none))
			{
				BloodOffset = __NFUN_213__(__NFUN_171__(0.2000000, CollisionRadius), HitNormal);
				BloodOffset.Z = __NFUN_171__(BloodOffset.Z, 0.5000000);
				Mo = Momentum;
				// End:0x12C
				if(__NFUN_177__(Mo.Z, float(0)))
				{
					__NFUN_182__(Mo.Z, 0.5000000);
				}
				__NFUN_278__(DesiredEffect, self,, __NFUN_215__(HitLocation, BloodOffset), Rotator(Mo));
			}
			DesiredEmitter = DamageType.static.GetPawnDamageEmitter(HitLocation, Damage, Momentum, self, __NFUN_132__(Level.bDropDetail, __NFUN_154__(int(Level.DetailMode), int(0))));
			// End:0x1C0
			if(__NFUN_119__(DesiredEmitter, none))
			{
				__NFUN_278__(DesiredEmitter,,, __NFUN_215__(HitLocation, HitNormal), Rotator(HitNormal));
			}
		}
	}
	// End:0x206
	if(__NFUN_152__(Health, 0))
	{
		// End:0x204
		if(__NFUN_130__(PhysicsVolume.bDestructive, __NFUN_119__(PhysicsVolume.ExitActor, none)))
		{
			__NFUN_278__(PhysicsVolume.ExitActor);
		}
		return;
	}
	// End:0x250
	if(__NFUN_177__(__NFUN_175__(Level.TimeSeconds, LastPainTime), 0.1000000))
	{
		PlayTakeHit(HitLocation, int(Damage), DamageType);
		LastPainTime = Level.TimeSeconds;
	}
	return;
}

simulated function ChunkUp(Rotator HitRotation, Class<DamageType> D)
{
	// End:0x57
	if(__NFUN_130__(__NFUN_155__(int(Level.NetMode), int(NM_Client)), __NFUN_119__(Controller, none)))
	{
		// End:0x4B
		if(Controller.bIsPlayer)
		{
			Controller.PawnDied(self);			
		}
		else
		{
			Controller.__NFUN_279__();
		}
	}
	__NFUN_279__();
	return;
}

simulated event SetAnimAction(name NewAction)
{
	return;
}

simulated event PlayDying(Class<DamageType> DamageType, Vector HitLoc)
{
	__NFUN_113__('Dying');
	// End:0x31
	if(bPhysicsAnimUpdate)
	{
		bReplicateMovement = false;
		bTearOff = true;
		__NFUN_223__(Velocity, TearOffMomentum);
		__NFUN_3970__(2);
	}
	bPlayedDeath = true;
	return;
}

simulated function PlayFiring(float Rate, name FiringMode)
{
	return;
}

function PlayWeaponSwitch(Weapon newWeapon)
{
	return;
}

simulated event StopPlayFiring()
{
	bSteadyFiring = false;
	return;
}

function PlayTakeHit(Vector HitLoc, int Damage, Class<DamageType> DamageType)
{
	local Sound DesiredSound;

	// End:0x0D
	if(__NFUN_154__(Damage, 0))
	{
		return;
	}
	DesiredSound = DamageType.static.GetPawnDamageSound();
	// End:0x3A
	if(__NFUN_119__(DesiredSound, none))
	{
		PlayOwnedSound(DesiredSound, 1);
	}
	return;
}

simulated event ChangeAnimation()
{
	// End:0x21
	if(__NFUN_130__(__NFUN_119__(Controller, none), Controller.bControlAnimations))
	{
		return;
	}
	PlayWaiting();
	PlayMoving();
	return;
}

simulated event AnimEnd(int Channel)
{
	// End:0x11
	if(__NFUN_154__(Channel, 0))
	{
		PlayWaiting();
	}
	bHideRightHandMesh = false;
	bHideLeftHandMesh = false;
	switch(UpdateMovementAnimType)
	{
		// End:0x2D
		case 1:
		// End:0x45
		case 2:
			UpdateMovementAnimType = 0;
			bUpdateMovementAnim = false;
			// End:0x48
			break;
		// End:0xFFFF
		default:
			break;
	}
	return;
}

function bool CannotJumpNow()
{
	return false;
	return;
}

simulated event PlayJump()
{
	return;
}

simulated event PlayFalling()
{
	return;
}

simulated function PlayMoving()
{
	return;
}

simulated function PlayWaiting()
{
	return;
}

function PlayLanded(float impactVel)
{
	// End:0x16
	if(__NFUN_129__(bPhysicsAnimUpdate))
	{
		PlayLandingAnimation(impactVel);
	}
	return;
}

simulated event PlayLandingAnimation(float impactVel)
{
	return;
}

function PlayVictoryAnimation()
{
	return;
}

function HoldCarriedObject(CarriedObject O, name AttachmentBone)
{
	return;
}

simulated event AttackedNotify(Pawn Attacker, Actor AttackActor, int Type, bool keepframerate, bool showeffect)
{
	return;
}

simulated event AssociateAttackedNotify(Pawn Attacker, Actor AttackActor, int Type, int Damage, bool bMiss, bool bShieldDefense, bool bCritical, bool bSpirit, bool keepframerate, bool showeffect)
{
	return;
}

function NotifyAttack(Actor Attacker)
{
	return;
}

// Export UPawn::execSetRelativeCoords(FFrame&, void* const)
native function bool SetRelativeCoords(Actor Goal);

// Export UPawn::execGetDefenseItemSound(FFrame&, void* const)
native function Sound GetDefenseItemSound();

// Export UPawn::execGetShieldItemSound(FFrame&, void* const)
native function Sound GetShieldItemSound();

// Export UPawn::execGetRefSkeletonNum(FFrame&, void* const)
native function int GetRefSkeletonNum();

simulated event NotifyDie()
{
	return;
}

state Dying
{
	ignores BreathTimer;

	event ChangeAnimation()
	{
		return;
	}

	event StopPlayFiring()
	{
		return;
	}

	function PlayFiring(float Rate, name FiringMode)
	{
		return;
	}

	function PlayWeaponSwitch(Weapon newWeapon)
	{
		return;
	}

	function PlayTakeHit(Vector HitLoc, int Damage, Class<DamageType> DamageType)
	{
		return;
	}

	simulated function PlayNextAnimation()
	{
		return;
	}

	function Died(Controller Killer, Class<DamageType> DamageType, Vector HitLocation)
	{
		return;
	}

	event FellOutOfWorld(Actor.eKillZType KillType)
	{
		// End:0x24
		if(__NFUN_132__(__NFUN_154__(int(KillType), int(1)), __NFUN_154__(int(KillType), int(2))))
		{
			return;
		}
		__NFUN_279__();
		return;
	}

	function Timer()
	{
		// End:0x0E
		if(__NFUN_129__(__NFUN_532__()))
		{
			__NFUN_279__();			
		}
		else
		{
			__NFUN_280__(2.0000000, false);
		}
		return;
	}

	function Landed(Vector HitNormal)
	{
		local Rotator finalRot;

		LandBob = __NFUN_244__(50.0000000, __NFUN_171__(0.0550000, Velocity.Z));
		// End:0x6D
		if(__NFUN_176__(Velocity.Z, float(-500)))
		{
			TakeDamage(int(__NFUN_175__(float(1), __NFUN_172__(Velocity.Z, float(30)))), Instigator, Location, vect(0.0000000, 0.0000000, 0.0000000), Class'Engine.Crushed');
		}
		finalRot = Rotation;
		finalRot.Roll = 0;
		finalRot.Pitch = 0;
		__NFUN_299__(finalRot);
		__NFUN_3970__(0);
		__NFUN_262__(true, false, false);
		// End:0xB2
		if(__NFUN_129__(__NFUN_282__(0)))
		{
			LieStill();
		}
		return;
	}

	function ReduceCylinder()
	{
		__NFUN_262__(false, false, false);
		return;
	}

	function LandThump()
	{
		// End:0x18
		if(__NFUN_154__(int(Physics), int(0)))
		{
			bThumped = true;
		}
		return;
	}

	event AnimEnd(int Channel)
	{
		// End:0x0D
		if(__NFUN_155__(Channel, 0))
		{
			return;
		}
		// End:0x26
		if(__NFUN_154__(int(Physics), int(0)))
		{
			LieStill();			
		}
		else
		{
			// End:0x46
			if(PhysicsVolume.bWaterVolume)
			{
				bThumped = true;
				LieStill();
			}
		}
		return;
	}

	function LieStill()
	{
		// End:0x11
		if(__NFUN_129__(bThumped))
		{
			LandThump();
		}
		__NFUN_262__(false, false, false);
		return;
	}

	singular function BaseChange()
	{
		// End:0x13
		if(__NFUN_114__(Base, none))
		{
			__NFUN_3970__(2);			
		}
		else
		{
			// End:0x33
			if(__NFUN_119__(Pawn(Base), none))
			{
				ChunkUp(Rotation, Class'Engine.fell');
			}
		}
		return;
	}

	function TakeDamage(int Damage, Pawn instigatedBy, Vector HitLocation, Vector Momentum, Class<DamageType> DamageType)
	{
		__NFUN_3970__(2);
		// End:0x3C
		if(__NFUN_130__(__NFUN_154__(int(Physics), int(0)), __NFUN_176__(Momentum.Z, float(0))))
		{
			__NFUN_182__(Momentum.Z, float(-1));
		}
		__NFUN_223__(Velocity, __NFUN_214__(__NFUN_213__(float(3), Momentum), __NFUN_174__(Mass, float(200))));
		// End:0x66
		if(bInvulnerableBody)
		{
			return;
		}
		__NFUN_159__(Damage, DamageType.default.GibModifier);
		__NFUN_162__(Health, Damage);
		// End:0xBE
		if(__NFUN_130__(__NFUN_132__(__NFUN_151__(Damage, 30), __NFUN_129__(__NFUN_282__())), __NFUN_150__(Health, -80)))
		{
			ChunkUp(Rotation, DamageType);
		}
		return;
	}

	function BeginState()
	{
		// End:0x48
		if(__NFUN_130__(__NFUN_119__(LastStartSpot, none), __NFUN_176__(__NFUN_175__(LastStartTime, Level.TimeSeconds), float(6))))
		{
			LastStartSpot.LastSpawnCampTime = Level.TimeSeconds;
		}
		// End:0x7A
		if(__NFUN_130__(bTearOff, __NFUN_154__(int(Level.NetMode), int(NM_DedicatedServer))))
		{
			LifeSpan = 1.0000000;			
		}
		else
		{
			__NFUN_280__(12.0000000, false);
		}
		__NFUN_3970__(2);
		bInvulnerableBody = true;
		// End:0xCC
		if(__NFUN_119__(Controller, none))
		{
			// End:0xC0
			if(Controller.bIsPlayer)
			{
				Controller.PawnDied(self);				
			}
			else
			{
				Controller.__NFUN_279__();
			}
		}
		return;
	}
Begin:

	__NFUN_256__(0.1500000);
	bInvulnerableBody = false;
	PlayDyingSound();
	stop;	
}

defaultproperties
{
	RightHandBone="Weapon_R_Bone"
	LeftHandBone="Weapon_L_Bone"
	RightArmBone="Shield_R_Bone"
	LeftArmBone="Shield_L_Bone"
	SpineBone="bip01_spine2"
	LowbodyBone="bip01_spine"
	CapeBone="Cape_Bone"
	HeadBone="Bip01_head"
	RightFootBone="bip01_r_foot"
	LeftFootBone="bip01_l_foot"
	WingBone="bip01_spine3"
	EffectSpawnBoneIdx=-1
	bEnableFaceRotation=true
	bNpc=true
	AttackSpeedRate=1.0000000
	NonAttackSpeedRate=1.0000000
	SkillSpeedRate=1.0000000
	PhysicalSkillSpeedRate=1.0000000
	GroundMaxSpeed=120.0000000
	GroundMinSpeed=40.0000000
	WaterMaxSpeed=90.0000000
	WaterMinSpeed=30.0000000
	AirMaxSpeed=120.0000000
	AirMinSpeed=40.0000000
	bReadyToWarp=true
	bJumpCapable=true
	bCanJump=true
	bCanWalk=true
	bLOSHearing=true
	bUseCompressedPosition=true
	bWeaponBob=true
	Visibility=128
	DesiredSpeed=1.0000000
	MaxDesiredSpeed=1.0000000
	HearingThreshold=2800.0000000
	SightRadius=5000.0000000
	AvgPhysicsTime=0.1000000
	GroundSpeed=600.0000000
	WaterSpeed=80.0000000
	AirSpeed=600.0000000
	LadderSpeed=200.0000000
	AccelRate=2048.0000000
	JumpZ=420.0000000
	AirControl=0.0500000
	WalkingPct=0.5000000
	CrouchedPct=0.5000000
	MaxFallSpeed=1200.0000000
	BaseEyeHeight=64.0000000
	EyeHeight=54.0000000
	CrouchHeight=40.0000000
	CrouchRadius=34.0000000
	Health=100
	HeadScale=1.0000000
	noise1time=-10.0000000
	noise2time=-10.0000000
	Bob=0.0080000
	SoundDampening=1.0000000
	DamageScaling=1.0000000
	ControllerClass=Class'Engine.AIController'
	LandMovementState="PlayerWalking"
	WaterMovementState="PlayerSwimming"
	BaseMovementRate=525.0000000
	BlendChangeTime=0.2500000
	bNeedTurnAnim=true
	PlayerSlotNum=-1
	BoundRadius=-1.0000000
	bShowGuilty=true
	fAirSpeedAccel=1000.0000000
	CameraWalkingAccelMode=2
	CastingEffectScale=1.0000000
	LastAttackItemClassID=-1
	ColosseumTeam=-1
	bNeedSetPawnResource=true
	DrawType=2
	bNeedCleanup=false
	bStasis=true
	bAcceptsProjectors=false
	bUpdateSimulatedPosition=true
	bCheckChangableLevel=true
	RemoteRole=2
	NetPriority=2.0000000
	Texture=Texture'Engine.S_Pawn'
	bTravel=true
	bShouldBaseAtStartup=true
	bOwnerNoSee=true
	bCanTeleport=true
	bDisturbFluidSurface=true
	CollisionRadius=34.0000000
	CollisionHeight=78.0000000
	bCollideActors=true
	bCollideWorld=true
	bBlockActors=true
	bBlockPlayers=true
	bProjTarget=true
	bRotateToDesired=true
	RotationRate=(Pitch=4096,Yaw=20000,Roll=3072)
	bNoRepMesh=true
	bDirectional=true
}