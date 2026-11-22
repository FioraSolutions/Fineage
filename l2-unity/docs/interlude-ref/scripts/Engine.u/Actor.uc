class Actor extends Object
	abstract
	native
	nativereplication;

const MAXSTEPHEIGHT = 10.0;
const MINFLOORZ = 0.7;

enum EPhysics
{
	PHYS_None,                      // 0
	PHYS_Walking,                   // 1
	PHYS_Falling,                   // 2
	PHYS_Swimming,                  // 3
	PHYS_Flying,                    // 4
	PHYS_Rotating,                  // 5
	PHYS_Projectile,                // 6
	PHYS_Interpolating,             // 7
	PHYS_MovingBrush,               // 8
	PHYS_Spider,                    // 9
	PHYS_Trailer,                   // 10
	PHYS_Ladder,                    // 11
	PHYS_RootMotion,                // 12
	PHYS_Karma,                     // 13
	PHYS_KarmaRagDoll,              // 14
	PHYS_MovingTrailer,             // 15
	PHYS_EffectTrailer,             // 16
	PHYS_NProjectile,               // 17
	PHYS_NMover,                    // 18
	PHYS_L2Movement                 // 19
};

enum EDrawType
{
	DT_None,                        // 0
	DT_Sprite,                      // 1
	DT_Mesh,                        // 2
	DT_Brush,                       // 3
	DT_RopeSprite,                  // 4
	DT_VerticalSprite,              // 5
	DT_Terraform,                   // 6
	DT_SpriteAnimOnce,              // 7
	DT_StaticMesh,                  // 8
	DT_DrawType,                    // 9
	DT_Particle,                    // 10
	DT_AntiPortal,                  // 11
	DT_FluidSurface,                // 12
	DT_Sun,                         // 13
	DT_MusicVolume,                 // 14
	DT_Custom,                      // 15
	DT_Native                       // 16
};

enum EFilterState
{
	FS_Maybe,                       // 0
	FS_Yes,                         // 1
	FS_No                           // 2
};

enum EDetailMode
{
	DM_Low,                         // 0
	DM_High,                        // 1
	DM_SuperHigh                    // 2
};

enum EL2EventCmd
{
	LEC_None,                       // 0
	LEC_Show,                       // 1
	LEC_Play                        // 2
};

enum EActorViewType
{
	EAVT_None,                      // 0
	EAVT_InSight,                   // 1
	EAVT_InSighting,                // 2
	EAVT_OutSight,                  // 3
	EAVT_OutSighting,               // 4
	EAVT_Spawning,                  // 5
	EAVT_Deleting,                  // 6
	EAVT_Hiding,                    // 7
	EAVT_Showing,                   // 8
	EAVT_Controller,                // 9
	EAVT_HighLight,                 // 10
	EAVT_Despawning                 // 11
};

enum ELodBlendViewType
{
	ELBT_None,                      // 0
	ELBT_Normal,                    // 1
	ELBT_Downing,                   // 2
	ELBT_Upping                     // 3
};

enum EL2ObjectType
{
	EL2_NORMAL,                     // 0
	EL2_DOOR,                       // 1
	EL2_CHAIR                       // 2
};

enum ENetRole
{
	ROLE_None,                      // 0
	ROLE_DumbProxy,                 // 1
	ROLE_SimulatedProxy,            // 2
	ROLE_AutonomousProxy,           // 3
	ROLE_Authority                  // 4
};

enum EAttachType
{
	EATP_Relative,                  // 0
	EATP_Absolute                   // 1
};

enum ETargetSpineStatus
{
	TSR_None,                       // 0
	TSR_START,                      // 1
	TSR_END                         // 2
};

enum ERenderStyle
{
	STY_None,                       // 0
	STY_Normal,                     // 1
	STY_Masked,                     // 2
	STY_Translucent,                // 3
	STY_Modulated,                  // 4
	STY_Alpha,                      // 5
	STY_Additive,                   // 6
	STY_Subtractive,                // 7
	STY_Particle,                   // 8
	STY_AlphaZ                      // 9
};

enum EPawnSubMeshStyle
{
	PMS_Boots,                      // 0
	PMS_LowBody,                    // 1
	PMS_UpBody,                     // 2
	PMS_Glove,                      // 3
	PMS_Hair2,                      // 4
	PMS_Hair1,                      // 5
	PMS_Helm,                       // 6
	PMS_HairAcce1,                  // 7
	PMS_HairAcce2,                  // 8
	PMS_Face,                       // 9
	PMS_RightHand,                  // 10
	PMS_LeftHand,                   // 11
	PMS_RightArm,                   // 12
	PMS_LeftArm,                    // 13
	PMS_Cloak,                      // 14
	PMS_CloakCover                  // 15
};

enum EL2GameAction
{
	LGA_SaintConsensusOpening,      // 0
	LGA_SaintConsensusOpened,       // 1
	LGA_SaintConsensusClosing,      // 2
	LGA_SaintConsensusClosed        // 3
};

enum ESoundSlot
{
	SLOT_None,                      // 0
	SLOT_Misc,                      // 1
	SLOT_Pain,                      // 2
	SLOT_Interact,                  // 3
	SLOT_Ambient,                   // 4
	SLOT_Talk,                      // 5
	SLOT_Interface                  // 6
};

enum EMusicTransition
{
	MTRAN_None,                     // 0
	MTRAN_Instant,                  // 1
	MTRAN_Segue,                    // 2
	MTRAN_Fade,                     // 3
	MTRAN_FastFade,                 // 4
	MTRAN_SlowFade                  // 5
};

enum EForceType
{
	FT_None,                        // 0
	FT_DragAlong                    // 1
};

enum ENMagicAniStatus
{
	NMAS_None,                      // 0
	NMAS_RotationTo,                // 1
	NMAS_CastStart,                 // 2
	NMAS_Cast,                      // 3
	NMAS_CastEnd,                   // 4
	NMAS_ShotStart,                 // 5
	NMAS_Shot,                      // 6
	NMAS_ShotEnd                    // 7
};

enum ENAttackStatus
{
	NAST_None,                      // 0
	NAST_AttackStart,               // 1
	NAST_AttackDelay,               // 2
	NAST_AttackShot,                // 3
	NAST_AttackDemage,              // 4
	NAST_AttackEnd                  // 5
};

enum ESpawnPos
{
	SPN_Normal,                     // 0
	SPN_Top,                        // 1
	SPN_Mid,                        // 2
	SPN_Btm                         // 3
};

enum ETravelType
{
	TRAVEL_Absolute,                // 0
	TRAVEL_Partial,                 // 1
	TRAVEL_Relative                 // 2
};

enum EDoubleClickDir
{
	DCLICK_None,                    // 0
	DCLICK_Left,                    // 1
	DCLICK_Right,                   // 2
	DCLICK_Forward,                 // 3
	DCLICK_Back,                    // 4
	DCLICK_Active,                  // 5
	DCLICK_Done                     // 6
};

enum eKillZType
{
	KILLZ_None,                     // 0
	KILLZ_Lava,                     // 1
	KILLZ_Suicide                   // 2
};

enum EActorEffectType
{
	EAST_None,                      // 0
	EAST_Silhouette,                // 1
	EAST_CubemapSilhouette,         // 2
	EAST_WeaponSilhouette           // 3
};

struct ActorRenderDataPtr
{
	var int Ptr;
};

struct LightRenderDataPtr
{
	var int Ptr;
};

struct L2Event
{
	var() int EventID;
	var() Actor.EL2EventCmd EventCmd;
};

struct NpcPrivate
{
	var() name Name;
	var() name ai;
	var() float Num;
};

struct WhenExtinctionCreate
{
	var() string respawn;
	var() string Name;
};

struct WayPoint
{
	var() Vector point;
	var() string Delay;
};

struct PointRegion
{
	var ZoneInfo Zone;
	var int iLeaf;
	var byte ZoneNumber;
};

struct ProjectorRenderInfoPtr
{
	var int Ptr;
};

struct StaticMeshProjectorRenderInfoPtr
{
	var int Ptr;
};

struct NMoverPtr
{
	var int Ptr;
};

struct KRBVec
{
	var float X;
	var float Y;
	var float Z;
};

struct KRigidBodyState
{
	var KRBVec Position;
	var Quat Quaternion;
	var KRBVec LinVel;
	var KRBVec AngVel;
};

struct AnimRep
{
	var name AnimSequence;
	var bool bAnimLoop;
	var byte AnimRate;
	var byte AnimFrame;
	var byte TweenRate;
};

struct NMagicInfo
{
	var SkillVisualEffect Agent;
	var int DummyPtr;
	var int MagicID;
	var int LevelID;
	var int AniIndex;
	var int FlexibleAniIndex;
	var int StageShot;
	var int StagePreshot;
	var float SkillHitTime;
	var float ShotTime;
	var float TweenTime;
	var float ActiveTime;
	var Actor TargetPawn;
	var float MagicSpeed;
	var int MagicAniStatus;
	var int MagicType;
	var bool bTargetExcepted;
	var int nSlice;
	var Rotator NormalRotationRate;
	var Rotator SkillRotationRate;
	var transient array<Actor> AssociatedActor;
	var transient array<Actor> EffectActor;
	var transient array<int> EffectID;
	var transient array<Vector> LocLIst;
	var name Anis[3];
	var float AniDues[3];
	var name LastShotName;
	var int PendingNotify;
	var int PendingPreshotNotify;
	var int PendingChannelingNotify;
};

struct TextureModifyinfo
{
	var bool bUseModify;
	var bool bTwoSide;
	var bool bAlphaBlend;
	var bool bDummy;
	var Color Color;
	var int AlphaOp;
	var int ColorOp;
};

var(Movement) const Actor.EPhysics Physics;
var(Display) const Actor.EDrawType DrawType;
var(Display) const StaticMesh StaticMesh;
var const Actor Owner;
var const Actor Base;
var native const ActorRenderDataPtr ActorRenderData;
var native const LightRenderDataPtr LightRenderData;
var native const int RenderRevision;
var native const Actor.EFilterState StaticFilterState;
var bool bDynamicActorFilterState;
var(Display) const name ForcedVisibilityZoneTag;
var(Lighting) bool bSpecialLit;
var(Lighting) bool bActorShadows;
var(Lighting) bool bCorona;
var(Lighting) bool bLightingVisibility;
var(Display) bool bUseDynamicLights;
var bool bLightChanged;
var bool bUpdateShadow;
var bool bHideShadow;
var bool bHideRightHandMesh;
var bool bHideLeftHandMesh;
var bool bNeedCleanup;
var bool bShadowOnly;
var int CreatureID;
var bool NoCheatCollision;
var bool CanIngnoreCollision;
var bool bDeleteNow;
var bool bAlwaysVisible;
var(Advanced) const bool bStatic;
var(Advanced) bool bHidden;
var(Advanced) const bool bNoDelete;
var const bool bDeleteMe;
var const transient bool bTicked;
var(Lighting) bool bDynamicLight;
var bool bTimerLoop;
var bool bOnlyOwnerSee;
var(Advanced) bool bHighDetail;
var(Advanced) bool bSuperHighDetail;
var bool bOnlyDrawIfAttached;
var(Advanced) bool bStasis;
var bool bTrailerAllowRotation;
var bool bTrailerSameRotation;
var bool bTrailerPrePivot;
var bool bTrailerNoOwnerDestroy;
var bool bRelativeTrail;
var Vector RelativeTrailOffset;
var(Rotation) bool bSelfRotation;
var bool bWorldGeometry;
var(Display) bool bAcceptsProjectors;
var bool bOrientOnSlope;
var const bool bOnlyAffectPawns;
var(Display) bool bDisableSorting;
var(Movement) bool bIgnoreEncroachers;
var bool bShowOctreeNodes;
var bool bWasSNFiltered;
var const bool bNetTemporary;
var bool bOnlyRelevantToOwner;
var const transient bool bNetDirty;
var bool bAlwaysRelevant;
var bool bReplicateInstigator;
var bool bReplicateMovement;
var bool bSkipActorPropertyReplication;
var bool bUpdateSimulatedPosition;
var bool bTearOff;
var bool bOnlyDirtyReplication;
var bool bReplicateAnimations;
var const bool bNetInitialRotation;
var bool bCompressedPosition;
var bool bAlwaysZeroBoneOffset;
var Vector RelativeLocInVehicle;
var int VehicleID;
var bool bVehicleTargetMove;
var bool bVehicleCompensativeMove;
var bool bHasActorTarget;
var bool bL2DesiredRotated;
var Rotator L2DesriedRotator;
var bool L2NeedTick;
var bool bCheckChangableLevel;
var bool bImmediatelyStop;
var bool bNeedPostSpawnProcess;
var Actor.EActorViewType L2ActorViewtype;
var float L2ActorViewDuration;
var float L2ActorViewElapsedTime;
var Actor.ELodBlendViewType L2LodViewType;
var float L2LodViewDuration;
var float L2LodViewElapsedTime;
var int L2CurrentLod;
var int L2ServerObjectRealID;
var(L2ServerObject) int L2ServerObjectID;
var(L2ServerObject) const Actor.EL2ObjectType L2ServerObjectType;
var Actor.ENetRole RemoteRole;
var Actor.ENetRole Role;
var const transient int NetTag;
var float NetUpdateTime;
var float NetUpdateFrequency;
var float NetPriority;
var Pawn Instigator;
var const name AttachmentBone;
var Actor.EAttachType AttachType;
var const LevelInfo Level;
var const transient Level XLevel;
var(Advanced) float LifeSpan;
var(Advanced) const PointRegion Region;
var(Advanced) const name ForcedRegionTag;
var(Advanced) const int ForcedRegion;
var float TimerRate;
var(Display) Mesh Mesh;
var transient float LastRenderTime;
var(Events) name Tag;
var transient array<int> Leaves;
var(Events) name Event;
var(Events) array<L2Event> L2GameEvent;
var Inventory Inventory;
var const float TimerCounter;
var transient MeshInstance MeshInstance;
var(Events) name L2MoveEvent;
var(ZoneLight) bool bSunAffect;
var Actor.ETargetSpineStatus TargetSpineStatus;
var(Display) float LODBias;
var(Object) name InitialState;
var(Object) name Group;
var const array<Actor> Child;
var const array<Actor> Touching;
var const transient array<int> OctreeNodes;
var const transient Box OctreeBox;
var const transient Vector OctreeBoxCenter;
var const transient Vector OctreeBoxRadii;
var const Actor Deleted;
var const float LatentFloat;
var native const int CollisionTag;
var const transient int JoinedTag;
var const PhysicsVolume PhysicsVolume;
var(Movement) const Vector Location;
var(Movement) const Rotator Rotation;
var(Movement) Vector Velocity;
var Vector Acceleration;
var(Movement) name AttachTag;
var const array<Actor> Attached;
var const Vector RelativeLocation;
var const Rotator RelativeRotation;
var(Movement) const bool bHardAttach;
var const Matrix HardRelMatrix;
var native const array<ProjectorRenderInfoPtr> Projectors;
var native const array<StaticMeshProjectorRenderInfoPtr> StaticMeshProjectors;
var(Display) Material Texture;
var StaticMeshInstance StaticMeshInstance;
var const export Model Brush;
var(Display) const float DrawScale;
var(Display) const Vector DrawScale3D;
var(Display) Vector PrePivot;
var(Display) array<Material> Skins;
var Material OverlayMaterial;
var float OverlayTimer;
var Color OverlayColor;
var Material RepSkin;
var(Display) byte AmbientGlow;
var(Display) byte MaxLights;
var(Display) ConvexVolume AntiPortal;
var(Display) float CullDistance;
var(Display) float ScaleGlow;
var NMoverPtr NMoverActor;
var L2NMover L2NMover;
var const transient Level SWXLevel;
var Rotator SwayRotationOrig;
var(Advanced) bool bDontBatch;
var(Display) Actor.ERenderStyle Style;
var(Display) bool bUnlit;
var(Display) bool bShadowCast;
var(Display) bool bStaticLighting;
var(Display) bool bUseLightingFromBase;
var(Display) bool bIgnoredRange;
var(Display) bool bUnlitCheck;
var bool bCulledSunlight;
var bool bHurtEntry;
var(Advanced) bool bGameRelevant;
var(Advanced) bool bCollideWhenPlacing;
var bool bTravel;
var(Advanced) bool bMovable;
var bool bDestroyInPainVolume;
var(Advanced) bool bShouldBaseAtStartup;
var bool bPendingDelete;
var bool bAnimByOwner;
var bool bOwnerNoSee;
var(Advanced) bool bCanTeleport;
var bool bClientAnim;
var bool bDisturbFluidSurface;
var const bool bAlwaysTick;
var(Sound) float TransientSoundVolume;
var(Sound) float TransientSoundRadius;
var(Collision) const float CollisionRadius;
var(Collision) const float CollisionHeight;
var(Collision) const bool bCollideActors;
var(Collision) bool bCollideWorld;
var(Collision) bool bBlockActors;
var(Collision) bool bBlockPlayers;
var(Collision) bool bProjTarget;
var(Collision) bool bBlockZeroExtentTraces;
var(Collision) bool bBlockNonZeroExtentTraces;
var(Collision) bool bAutoAlignToTerrain;
var(Collision) bool bUseCylinderCollision;
var(Collision) const bool bBlockKarma;
var bool bNetNotify;
var bool bIgnoreOutOfWorld;
var(Movement) bool bBounce;
var(Movement) bool bFixedRotationDir;
var(Movement) bool bRotateToDesired;
var bool bInterpolating;
var const bool bJustTeleported;
var(Movement) float Mass;
var(Movement) float Buoyancy;
var(Movement) Rotator RotationRate;
var Rotator KayboardRotationRate;
var int KeyboardRotationYawFromServer;
var(Movement) Rotator RotationLimit;
var(Movement) Rotator DesiredRotation;
var Actor PendingTouch;
var const Vector ColLocation;
var(Karma) export editinline KarmaParamsCollision KParams;
var native const int KStepTag;
var transient AnimRep SimAnim;
var(Force) Actor.EForceType ForceType;
var(Force) float ForceRadius;
var(Force) float ForceScale;
var const bool bNetInitial;
var const bool bNetOwner;
var const bool bNetRelevant;
var const bool bDemoRecording;
var const bool bClientDemoRecording;
var const bool bClientDemoNetFunc;
var bool bNoRepMesh;
var(Advanced) bool bHiddenEd;
var(Advanced) bool bHiddenEdGroup;
var(Advanced) bool bDirectional;
var const bool bSelected;
var(Advanced) bool bEdShouldSnap;
var transient bool bEdSnap;
var const transient bool bTempEditor;
var bool bObsolete;
var(Collision) bool bPathColliding;
var transient bool bPathTemp;
var bool bScriptInitialized;
var(Advanced) bool bLockLocation;
var(Advanced) bool bLockUndelete;
var Class<LocalMessage> MessageClass;
var transient array<NSkillProjectile> NSkillProjectileActor;
var transient array<Actor> SpelledNEffectActor;
var transient array<Actor> NProjectileActor;
var Actor.ENAttackStatus NAttackStatus;
var Actor EffectOwner;
var(Movement) Actor.ESpawnPos SpawnPos;
var TextureModifyinfo TexModifyInfo;
var Actor.EActorEffectType L2ActorEffecttype;
var transient bool bTangentLoad;
var transient int nUseNormalmap;

// Export UActor::execConsoleCommand(FFrame&, void* const)
native function string ConsoleCommand(string Command);

// Export UActor::execGetViewport(FFrame&, void* const)
native function Viewport GetViewport();

// Export UActor::execError(FFrame&, void* const)
native(233) final function Error(coerce string S);

// Export UActor::execSleep(FFrame&, void* const)
native(256) final latent function Sleep(float Seconds);

// Export UActor::execSetCollision(FFrame&, void* const)
native(262) final function SetCollision(optional bool NewColActors, optional bool NewBlockActors, optional bool NewBlockPlayers);

// Export UActor::execSetCollisionSize(FFrame&, void* const)
native(283) final function bool SetCollisionSize(float NewRadius, float NewHeight);

// Export UActor::execSetDrawScale(FFrame&, void* const)
native final function SetDrawScale(float NewScale);

// Export UActor::execSetDrawScale3D(FFrame&, void* const)
native final function SetDrawScale3D(Vector NewScale3D);

// Export UActor::execSetStaticMesh(FFrame&, void* const)
native final function SetStaticMesh(StaticMesh NewStaticMesh);

// Export UActor::execSetDrawType(FFrame&, void* const)
native final function SetDrawType(Actor.EDrawType NewDrawType);

// Export UActor::execMove(FFrame&, void* const)
native(266) final function bool Move(Vector Delta);

// Export UActor::execSetLocation(FFrame&, void* const)
native(267) final function bool SetLocation(Vector NewLocation);

// Export UActor::execSetRotation(FFrame&, void* const)
native(299) final function bool SetRotation(Rotator NewRotation);

// Export UActor::execSetRelativeRotation(FFrame&, void* const)
native final function bool SetRelativeRotation(Rotator NewRotation);

// Export UActor::execSetRelativeLocation(FFrame&, void* const)
native final function bool SetRelativeLocation(Vector NewLocation);

// Export UActor::execMoveSmooth(FFrame&, void* const)
native(3969) final function bool MoveSmooth(Vector Delta);

// Export UActor::execAutonomousPhysics(FFrame&, void* const)
native(3971) final function AutonomousPhysics(float DeltaSeconds);

// Export UActor::execSetBase(FFrame&, void* const)
native(298) final function SetBase(Actor NewBase, optional Vector NewFloor);

// Export UActor::execSetOwner(FFrame&, void* const)
native(272) final function SetOwner(Actor NewOwner);

// Export UActor::execGetMeshName(FFrame&, void* const)
native final function string GetMeshName();

// Export UActor::execPlayAnim(FFrame&, void* const)
native(259) final function PlayAnim(name Sequence, optional float Rate, optional float TweenTime, optional int Channel);

// Export UActor::execLoopAnim(FFrame&, void* const)
native(260) final function LoopAnim(name Sequence, optional float Rate, optional float TweenTime, optional int Channel);

// Export UActor::execTweenAnim(FFrame&, void* const)
native(294) final function TweenAnim(name Sequence, float Time, optional int Channel);

// Export UActor::execIsAnimating(FFrame&, void* const)
native(282) final function bool IsAnimating(optional int Channel);

// Export UActor::execFinishAnim(FFrame&, void* const)
native(261) final latent function FinishAnim(optional int Channel);

// Export UActor::execHasAnim(FFrame&, void* const)
native(263) final function bool HasAnim(name Sequence);

// Export UActor::execStopAnimating(FFrame&, void* const)
native final function StopAnimating(optional bool ClearAllButBase);

// Export UActor::execHasBoneName(FFrame&, void* const)
native final function bool HasBoneName(name bonename);

// Export UActor::execFreezeAnimAt(FFrame&, void* const)
native final function FreezeAnimAt(float Time, optional int Channel);

// Export UActor::execSetAnimFrame(FFrame&, void* const)
native final function SetAnimFrame(float Time, optional int Channel, optional int UnitFlag);

// Export UActor::execIsTweening(FFrame&, void* const)
native final function bool IsTweening(int Channel);

event L2GameActionNotify(Actor.EL2GameAction GameAction)
{
	return;
}

event ClearL2Game()
{
	return;
}

event AnimEnd(int Channel)
{
	return;
}

// Export UActor::execEnableChannelNotify(FFrame&, void* const)
native final function EnableChannelNotify(int Channel, int Switch);

// Export UActor::execGetNotifyChannel(FFrame&, void* const)
native final function int GetNotifyChannel();

// Export UActor::execLinkSkelAnim(FFrame&, void* const)
native final simulated function LinkSkelAnim(MeshAnimation Anim, optional Mesh NewMesh);

// Export UActor::execLinkMesh(FFrame&, void* const)
native final simulated function LinkMesh(Mesh NewMesh, optional bool bKeepAnim);

// Export UActor::execBoneRefresh(FFrame&, void* const)
native final function BoneRefresh();

// Export UActor::execAnimBlendParams(FFrame&, void* const)
native final function AnimBlendParams(int Stage, optional float BlendAlpha, optional float InTime, optional float OutTime, optional name bonename, optional bool bGlobalPose);

// Export UActor::execAnimBlendToAlpha(FFrame&, void* const)
native final function AnimBlendToAlpha(int Stage, float TargetAlpha, float TimeInterval);

// Export UActor::execGetBoneCoords(FFrame&, void* const)
native final function Coords GetBoneCoords(name bonename);

// Export UActor::execGetBoneRotation(FFrame&, void* const)
native final function Rotator GetBoneRotation(name bonename, optional int Space);

// Export UActor::execGetBoneCoordsWithBoneIndex(FFrame&, void* const)
native final function Coords GetBoneCoordsWithBoneIndex(int BoneIndex);

// Export UActor::execGetRootLocation(FFrame&, void* const)
native final function Vector GetRootLocation();

// Export UActor::execGetRootRotation(FFrame&, void* const)
native final function Rotator GetRootRotation();

// Export UActor::execGetRootLocationDelta(FFrame&, void* const)
native final function Vector GetRootLocationDelta();

// Export UActor::execGetRootRotationDelta(FFrame&, void* const)
native final function Rotator GetRootRotationDelta();

// Export UActor::execAttachToBone(FFrame&, void* const)
native final function bool AttachToBone(Actor Attachment, name bonename);

// Export UActor::execDetachFromBone(FFrame&, void* const)
native final function bool DetachFromBone(Actor Attachment);

// Export UActor::execAttachToBoneWithIndex(FFrame&, void* const)
native final function bool AttachToBoneWithIndex(Actor Attachment, int BoneIndex);

// Export UActor::execLockRootMotion(FFrame&, void* const)
native final function LockRootMotion(int Lock);

// Export UActor::execSetBoneScale(FFrame&, void* const)
native final function SetBoneScale(int Slot, optional float BoneScale, optional name bonename);

// Export UActor::execSetBoneDirection(FFrame&, void* const)
native final function SetBoneDirection(name bonename, Rotator BoneTurn, optional Vector BoneTrans, optional float Alpha, optional int Space);

// Export UActor::execSetBoneLocation(FFrame&, void* const)
native final function SetBoneLocation(name bonename, optional Vector BoneTrans, optional float Alpha);

// Export UActor::execSetBoneRotation(FFrame&, void* const)
native final function SetBoneRotation(name bonename, optional Rotator BoneTurn, optional int Space, optional float Alpha);

// Export UActor::execGetAnimParams(FFrame&, void* const)
native final function GetAnimParams(int Channel, out name OutSeqName, out float OutAnimFrame, out float OutAnimRate);

// Export UActor::execAnimIsInGroup(FFrame&, void* const)
native final function bool AnimIsInGroup(int Channel, name GroupName);

// Export UActor::execGetRenderBoundingSphere(FFrame&, void* const)
native final function Plane GetRenderBoundingSphere();

// Export UActor::execDrawDebugLine(FFrame&, void* const)
native final function DrawDebugLine(Vector LineStart, Vector LineEnd, byte R, byte G, byte B);

// Export UActor::execDebugClock(FFrame&, void* const)
native final function DebugClock();

// Export UActor::execDebugUnclock(FFrame&, void* const)
native final function DebugUnclock();

// Export UActor::execFinishInterpolation(FFrame&, void* const)
native(301) final latent function FinishInterpolation();

// Export UActor::execSetPhysics(FFrame&, void* const)
native(3970) final function SetPhysics(Actor.EPhysics newPhysics);

// Export UActor::execOnlyAffectPawns(FFrame&, void* const)
native final function OnlyAffectPawns(bool B);

// Export UActor::execKGetRBQuaternion(FFrame&, void* const)
native final function Quat KGetRBQuaternion();

// Export UActor::execKGetRigidBodyState(FFrame&, void* const)
native final function KGetRigidBodyState(out KRigidBodyState RBstate);

// Export UActor::execKDrawRigidBodyState(FFrame&, void* const)
native final function KDrawRigidBodyState(KRigidBodyState RBstate, bool AltColour);

// Export UActor::execKRBVecToVector(FFrame&, void* const)
native final function Vector KRBVecToVector(KRBVec RBvec);

// Export UActor::execKRBVecFromVector(FFrame&, void* const)
native final function KRBVec KRBVecFromVector(Vector V);

// Export UActor::execKSetMass(FFrame&, void* const)
native final function KSetMass(float Mass);

// Export UActor::execKGetMass(FFrame&, void* const)
native final function float KGetMass();

// Export UActor::execKSetInertiaTensor(FFrame&, void* const)
native final function KSetInertiaTensor(Vector it1, Vector it2);

// Export UActor::execKGetInertiaTensor(FFrame&, void* const)
native final function KGetInertiaTensor(out Vector it1, out Vector it2);

// Export UActor::execKSetDampingProps(FFrame&, void* const)
native final function KSetDampingProps(float lindamp, float angdamp);

// Export UActor::execKGetDampingProps(FFrame&, void* const)
native final function KGetDampingProps(out float lindamp, out float angdamp);

// Export UActor::execKSetFriction(FFrame&, void* const)
native final function KSetFriction(float friction);

// Export UActor::execKGetFriction(FFrame&, void* const)
native final function float KGetFriction();

// Export UActor::execKSetRestitution(FFrame&, void* const)
native final function KSetRestitution(float rest);

// Export UActor::execKGetRestitution(FFrame&, void* const)
native final function float KGetRestitution();

// Export UActor::execKSetCOMOffset(FFrame&, void* const)
native final function KSetCOMOffset(Vector offset);

// Export UActor::execKGetCOMOffset(FFrame&, void* const)
native final function KGetCOMOffset(out Vector offset);

// Export UActor::execKGetCOMPosition(FFrame&, void* const)
native final function KGetCOMPosition(out Vector pos);

// Export UActor::execKSetImpactThreshold(FFrame&, void* const)
native final function KSetImpactThreshold(float thresh);

// Export UActor::execKGetImpactThreshold(FFrame&, void* const)
native final function float KGetImpactThreshold();

// Export UActor::execKWake(FFrame&, void* const)
native final function KWake();

// Export UActor::execKIsAwake(FFrame&, void* const)
native final function bool KIsAwake();

// Export UActor::execKAddImpulse(FFrame&, void* const)
native final function KAddImpulse(Vector Impulse, Vector Position, optional name bonename);

// Export UActor::execKSetStayUpright(FFrame&, void* const)
native final function KSetStayUpright(bool stayUpright, bool allowRotate);

// Export UActor::execKSetBlockKarma(FFrame&, void* const)
native final function KSetBlockKarma(bool newBlock);

// Export UActor::execKSetActorGravScale(FFrame&, void* const)
native final function KSetActorGravScale(float ActorGravScale);

// Export UActor::execKGetActorGravScale(FFrame&, void* const)
native final function float KGetActorGravScale();

// Export UActor::execKDisableCollision(FFrame&, void* const)
native final function KDisableCollision(Actor Other);

// Export UActor::execKEnableCollision(FFrame&, void* const)
native final function KEnableCollision(Actor Other);

// Export UActor::execKSetSkelVel(FFrame&, void* const)
native final function KSetSkelVel(Vector Velocity, optional Vector AngVelocity, optional bool AddToCurrent);

// Export UActor::execKGetSkelMass(FFrame&, void* const)
native final function float KGetSkelMass();

// Export UActor::execKFreezeRagdoll(FFrame&, void* const)
native final function KFreezeRagdoll();

// Export UActor::execKAddBoneLifter(FFrame&, void* const)
native final function KAddBoneLifter(name bonename, InterpCurve LiftVel, float LateralFriction, InterpCurve Softness);

// Export UActor::execKRemoveLifterFromBone(FFrame&, void* const)
native final function KRemoveLifterFromBone(name bonename);

// Export UActor::execKRemoveAllBoneLifters(FFrame&, void* const)
native final function KRemoveAllBoneLifters();

// Export UActor::execKMakeRagdollAvailable(FFrame&, void* const)
native final function KMakeRagdollAvailable();

// Export UActor::execKIsRagdollAvailable(FFrame&, void* const)
native final function bool KIsRagdollAvailable();

event KImpact(Actor Other, Vector pos, Vector impactVel, Vector impactNorm)
{
	return;
}

event KVelDropBelow()
{
	return;
}

event KSkelConvulse()
{
	return;
}

event KApplyForce(out Vector Force, out Vector Torque)
{
	return;
}

event bool KUpdateState(out KRigidBodyState NewState)
{
	return;
}

// Export UActor::execClock(FFrame&, void* const)
native final function Clock(out float Time);

// Export UActor::execUnClock(FFrame&, void* const)
native final function UnClock(out float Time);

// Export UActor::execPlayMusic(FFrame&, void* const)
native final function int PlayMusic(string Song, float FadeInTime);

// Export UActor::execStopMusic(FFrame&, void* const)
native final function StopMusic(int SongHandle, float FadeOutTime);

// Export UActor::execStopAllMusic(FFrame&, void* const)
native final function StopAllMusic(float FadeOutTime);

// Export UActor::execNDestroy(FFrame&, void* const)
native final function bool NDestroy();

event Destroyed()
{
	return;
}

event GainedChild(Actor Other)
{
	return;
}

event LostChild(Actor Other)
{
	return;
}

event Tick(float DeltaTime)
{
	return;
}

event PostNetReceive()
{
	return;
}

event Trigger(Actor Other, Pawn EventInstigator)
{
	return;
}

event UnTrigger(Actor Other, Pawn EventInstigator)
{
	return;
}

event BeginEvent()
{
	return;
}

event EndEvent()
{
	return;
}

event Timer()
{
	return;
}

event HitWall(Vector HitNormal, Actor HitWall)
{
	return;
}

event Falling()
{
	return;
}

event Landed(Vector HitNormal)
{
	return;
}

event ZoneChange(ZoneInfo NewZone)
{
	return;
}

event PhysicsVolumeChange(PhysicsVolume NewVolume)
{
	return;
}

event Touch(Actor Other)
{
	return;
}

event PostTouch(Actor Other)
{
	return;
}

event UnTouch(Actor Other)
{
	return;
}

event Bump(Actor Other)
{
	return;
}

event BaseChange()
{
	return;
}

event Attach(Actor Other)
{
	return;
}

event Detach(Actor Other)
{
	return;
}

event Actor SpecialHandling(Pawn Other)
{
	return;
}

event bool EncroachingOn(Actor Other)
{
	return;
}

event EncroachedBy(Actor Other)
{
	return;
}

event FinishedInterpolation()
{
	bInterpolating = false;
	return;
}

event EndedRotation()
{
	return;
}

event UsedBy(Pawn User)
{
	return;
}

simulated event FellOutOfWorld(Actor.eKillZType KillType)
{
	__NFUN_3970__(0);
	__NFUN_279__();
	return;
}

event KilledBy(Pawn EventInstigator)
{
	return;
}

event TakeDamage(int Damage, Pawn EventInstigator, Vector HitLocation, Vector Momentum, Class<DamageType> DamageType)
{
	return;
}

// Export UActor::execTrace(FFrame&, void* const)
native(277) final function Actor Trace(out Vector HitLocation, out Vector HitNormal, Vector TraceEnd, optional Vector TraceStart, optional bool bTraceActors, optional Vector Extent, optional out Material Material);

// Export UActor::execFastTrace(FFrame&, void* const)
native(548) final function bool FastTrace(Vector TraceEnd, optional Vector TraceStart);

// Export UActor::execSpawn(FFrame&, void* const)
native(278) final function Actor Spawn(Class<Actor> SpawnClass, optional Actor SpawnOwner, optional name SpawnTag, optional Vector SpawnLocation, optional Rotator SpawnRotation, optional bool bNoCollisionCheck);

// Export UActor::execDestroy(FFrame&, void* const)
native(279) final function bool Destroy();

event TornOff()
{
	return;
}

// Export UActor::execSetTimer(FFrame&, void* const)
native(280) final function SetTimer(float NewTimerRate, bool bLoop);

// Export UActor::execPlaySound(FFrame&, void* const)
native(264) final function PlaySound(Sound Sound, optional Actor.ESoundSlot Slot, optional float Volume, optional bool bNoOverride, optional float Radius, optional float Pitch, optional bool Attenuate);

// Export UActor::execPlayOwnedSound(FFrame&, void* const)
native final simulated function PlayOwnedSound(Sound Sound, optional Actor.ESoundSlot Slot, optional float Volume, optional bool bNoOverride, optional float Radius, optional float Pitch, optional bool Attenuate);

// Export UActor::execDemoPlaySound(FFrame&, void* const)
native simulated event DemoPlaySound(Sound Sound, optional Actor.ESoundSlot Slot, optional float Volume, optional bool bNoOverride, optional float Radius, optional float Pitch, optional bool Attenuate);

// Export UActor::execPlaySoundOnVehicle(FFrame&, void* const)
native function PlaySoundOnVehicle(Sound Sound, optional Actor.ESoundSlot Slot, optional float Volume, optional bool bNoOverride, optional float Radius, optional float Pitch, optional bool Attenuate);

// Export UActor::execGetSoundDuration(FFrame&, void* const)
native final function float GetSoundDuration(Sound Sound);

// Export UActor::execMakeNoise(FFrame&, void* const)
native(512) final function MakeNoise(float Loudness);

// Export UActor::execPlayerCanSeeMe(FFrame&, void* const)
native(532) final function bool PlayerCanSeeMe();

// Export UActor::execSuggestFallVelocity(FFrame&, void* const)
native final function Vector SuggestFallVelocity(Vector Destination, Vector Start, float MaxZ, float MaxXYSpeed);

event bool PreTeleport(Teleporter InTeleporter)
{
	return;
}

event PostTeleport(Teleporter OutTeleporter)
{
	return;
}

event BeginPlay()
{
	return;
}

// Export UActor::execGetMapName(FFrame&, void* const)
native(539) final function string GetMapName(string NameEnding, string MapName, int Dir);

// Export UActor::execGetNextSkin(FFrame&, void* const)
native(545) final function GetNextSkin(string Prefix, string CurrentSkin, int Dir, out string SkinName, out string SkinDesc);

// Export UActor::execGetURLMap(FFrame&, void* const)
native(547) final function string GetURLMap();

// Export UActor::execGetNextInt(FFrame&, void* const)
native final function string GetNextInt(string ClassName, int Num);

// Export UActor::execGetNextIntDesc(FFrame&, void* const)
native final function GetNextIntDesc(string ClassName, int Num, out string Entry, out string Description);

// Export UActor::execGetCacheEntry(FFrame&, void* const)
native final function bool GetCacheEntry(int Num, out string Guid, out string Filename);

// Export UActor::execMoveCacheEntry(FFrame&, void* const)
native final function bool MoveCacheEntry(string Guid, optional string NewFilename);

// Export UActor::execAllActors(FFrame&, void* const)
native(304) final iterator function AllActors(Class<Actor> BaseClass, out Actor Actor, optional name MatchTag);

// Export UActor::execDynamicActors(FFrame&, void* const)
native(313) final iterator function DynamicActors(Class<Actor> BaseClass, out Actor Actor, optional name MatchTag);

// Export UActor::execChildActors(FFrame&, void* const)
native(305) final iterator function ChildActors(Class<Actor> BaseClass, out Actor Actor);

// Export UActor::execBasedActors(FFrame&, void* const)
native(306) final iterator function BasedActors(Class<Actor> BaseClass, out Actor Actor);

// Export UActor::execTouchingActors(FFrame&, void* const)
native(307) final iterator function TouchingActors(Class<Actor> BaseClass, out Actor Actor);

// Export UActor::execTraceActors(FFrame&, void* const)
native(309) final iterator function TraceActors(Class<Actor> BaseClass, out Actor Actor, out Vector HitLoc, out Vector HitNorm, Vector End, optional Vector Start, optional Vector Extent);

// Export UActor::execRadiusActors(FFrame&, void* const)
native(310) final iterator function RadiusActors(Class<Actor> BaseClass, out Actor Actor, float Radius, optional Vector Loc);

// Export UActor::execVisibleActors(FFrame&, void* const)
native(311) final iterator function VisibleActors(Class<Actor> BaseClass, out Actor Actor, optional float Radius, optional Vector Loc);

// Export UActor::execVisibleCollidingActors(FFrame&, void* const)
native(312) final iterator function VisibleCollidingActors(Class<Actor> BaseClass, out Actor Actor, float Radius, optional Vector Loc, optional bool bIgnoreHidden);

// Export UActor::execCollidingActors(FFrame&, void* const)
native(321) final iterator function CollidingActors(Class<Actor> BaseClass, out Actor Actor, float Radius, optional Vector Loc);

// Export UActor::execSubtract_ColorColor(FFrame&, void* const)
native(549) static final operator(20) Color -(Color A, Color B);

// Export UActor::execMultiply_FloatColor(FFrame&, void* const)
native(550) static final operator(16) Color *(float A, Color B);

// Export UActor::execAdd_ColorColor(FFrame&, void* const)
native(551) static final operator(20) Color +(Color A, Color B);

// Export UActor::execMultiply_ColorFloat(FFrame&, void* const)
native(552) static final operator(16) Color *(Color A, float B);

function RenderOverlays(Canvas Canvas)
{
	return;
}

event RenderTexture(ScriptedTexture Tex)
{
	return;
}

event PreBeginPlay()
{
	// End:0x87
	if(__NFUN_130__(__NFUN_130__(__NFUN_129__(bGameRelevant), __NFUN_155__(int(Level.NetMode), int(NM_Client))), __NFUN_130__(__NFUN_130__(__NFUN_119__(none, Level.Game), __NFUN_119__(none, Level.Game.BaseMutator)), __NFUN_129__(Level.Game.BaseMutator.CheckRelevance(self)))))
	{
		__NFUN_279__();
	}
	return;
}

event BroadcastLocalizedMessage(Class<LocalMessage> MessageClass, optional int Switch, optional PlayerReplicationInfo RelatedPRI_1, optional PlayerReplicationInfo RelatedPRI_2, optional Object OptionalObject)
{
	Level.Game.BroadcastLocalized(self, MessageClass, Switch, RelatedPRI_1, RelatedPRI_2, OptionalObject);
	return;
}

event PostBeginPlay()
{
	return;
}

simulated event SetInitialState()
{
	bScriptInitialized = true;
	// End:0x21
	if(__NFUN_255__(InitialState, 'None'))
	{
		__NFUN_113__(InitialState);		
	}
	else
	{
		__NFUN_113__('Auto');
	}
	return;
}

event PostNetBeginPlay()
{
	return;
}

event PostSetPawnResource()
{
	return;
}

simulated function UpdatePrecacheMaterials()
{
	local int i;

	i = 0;
	J0x07:

	// End:0x3B [Loop If]
	if(__NFUN_150__(i, Skins.Length))
	{
		Level.AddPrecacheMaterial(Skins[i]);
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x07;
	}
	return;
}

simulated function UpdatePrecacheStaticMeshes()
{
	// End:0x3E
	if(__NFUN_130__(__NFUN_130__(__NFUN_154__(int(DrawType), int(8)), __NFUN_129__(bStatic)), __NFUN_129__(bNoDelete)))
	{
		Level.AddPrecacheStaticMesh(StaticMesh);
	}
	return;
}

final simulated function HurtRadius(float DamageAmount, float DamageRadius, Class<DamageType> DamageType, float Momentum, Vector HitLocation)
{
	local Actor Victims;
	local float damageScale, dist;
	local Vector Dir;

	// End:0x0B
	if(bHurtEntry)
	{
		return;
	}
	bHurtEntry = true;
	// End:0x153
	foreach __NFUN_312__(Class'Engine.Actor', Victims, DamageRadius, HitLocation)
	{
		// End:0x152
		if(__NFUN_130__(__NFUN_130__(__NFUN_119__(Victims, self), __NFUN_154__(int(Victims.Role), int(ROLE_Authority))), __NFUN_129__(Victims.__NFUN_303__('FluidSurfaceInfo'))))
		{
			Dir = __NFUN_216__(Victims.Location, HitLocation);
			dist = __NFUN_245__(1.0000000, __NFUN_225__(Dir));
			Dir = __NFUN_214__(Dir, dist);
			damageScale = __NFUN_175__(1.0000000, __NFUN_245__(0.0000000, __NFUN_172__(__NFUN_175__(dist, Victims.CollisionRadius), DamageRadius)));
			Victims.TakeDamage(int(__NFUN_171__(damageScale, DamageAmount)), Instigator, __NFUN_216__(Victims.Location, __NFUN_213__(__NFUN_171__(0.5000000, __NFUN_174__(Victims.CollisionHeight, Victims.CollisionRadius)), Dir)), __NFUN_213__(__NFUN_171__(damageScale, Momentum), Dir), DamageType);
		}		
	}	
	bHurtEntry = false;
	return;
}

event TravelPreAccept()
{
	return;
}

event TravelPostAccept()
{
	return;
}

function BecomeViewTarget()
{
	return;
}

function string GetItemName(string FullName)
{
	local int pos;

	pos = __NFUN_126__(FullName, ".");
	J0x10:

	// End:0x50 [Loop If]
	if(__NFUN_155__(pos, -1))
	{
		FullName = __NFUN_234__(FullName, __NFUN_147__(__NFUN_147__(__NFUN_125__(FullName), pos), 1));
		pos = __NFUN_126__(FullName, ".");
		// [Loop Continue]
		goto J0x10;
	}
	return FullName;
	return;
}

simulated function string GetHumanReadableName()
{
	return GetItemName(string(Class));
	return;
}

final function ReplaceText(out string Text, string Replace, string With)
{
	local int i;
	local string Input;

	Input = Text;
	Text = "";
	i = __NFUN_126__(Input, Replace);
	J0x25:

	// End:0x84 [Loop If]
	if(__NFUN_155__(i, -1))
	{
		Text = __NFUN_112__(__NFUN_112__(Text, __NFUN_128__(Input, i)), With);
		Input = __NFUN_127__(Input, __NFUN_146__(i, __NFUN_125__(Replace)));
		i = __NFUN_126__(Input, Replace);
		// [Loop Continue]
		goto J0x25;
	}
	Text = __NFUN_112__(Text, Input);
	return;
}

function SetDisplayProperties(Actor.ERenderStyle NewStyle, Material NewTexture, bool bLighting)
{
	Style = NewStyle;
	Texture = NewTexture;
	bUnlit = bLighting;
	return;
}

function SetDefaultDisplayProperties()
{
	Style = default.Style;
	Texture = default.Texture;
	bUnlit = default.bUnlit;
	return;
}

static function string GetLocalString(optional int Switch, optional PlayerReplicationInfo RelatedPRI_1, optional PlayerReplicationInfo RelatedPRI_2)
{
	return "";
	return;
}

function MatchStarting()
{
	return;
}

function SetGRI(GameReplicationInfo GRI)
{
	return;
}

function string GetDebugName()
{
	return GetItemName(string(self));
	return;
}

simulated function DisplayDebug(Canvas Canvas, out float YL, out float YPos)
{
	local string t;
	local float XL;
	local int i;
	local Actor A;
	local name Anim;
	local float frame, Rate;

	Canvas.Style = 1;
	Canvas.__NFUN_464__("TEST", XL, YL);
	YPos = __NFUN_174__(YPos, YL);
	Canvas.SetPos(4.0000000, YPos);
	Canvas.SetDrawColor(byte(255), 0, 0);
	t = GetDebugName();
	// End:0xAF
	if(bDeleteMe)
	{
		t = __NFUN_112__(t, " DELETED (bDeleteMe == true)");
	}
	Canvas.__NFUN_465__(t, false);
	__NFUN_184__(YPos, YL);
	Canvas.SetPos(4.0000000, YPos);
	Canvas.SetDrawColor(byte(255), byte(255), byte(255));
	// End:0x2FB
	if(__NFUN_155__(int(Level.NetMode), int(NM_Standalone)))
	{
		t = "ROLE ";
		switch(Role)
		{
			// End:0x149
			case ROLE_None:
				t = __NFUN_112__(t, "None");
				// End:0x1D7
				break;
			// End:0x169
			case 1:
				t = __NFUN_112__(t, "DumbProxy");
				// End:0x1D7
				break;
			// End:0x18E
			case 2:
				t = __NFUN_112__(t, "SimulatedProxy");
				// End:0x1D7
				break;
			// End:0x1B4
			case 3:
				t = __NFUN_112__(t, "AutonomousProxy");
				// End:0x1D7
				break;
			// End:0x1D4
			case 4:
				t = __NFUN_112__(t, "Authority");
				// End:0x1D7
				break;
			// End:0xFFFF
			default:
				break;
		}
		t = __NFUN_112__(t, " REMOTE ROLE ");
		switch(RemoteRole)
		{
			// End:0x215
			case ROLE_None:
				t = __NFUN_112__(t, "None");
				// End:0x2A3
				break;
			// End:0x235
			case 1:
				t = __NFUN_112__(t, "DumbProxy");
				// End:0x2A3
				break;
			// End:0x25A
			case 2:
				t = __NFUN_112__(t, "SimulatedProxy");
				// End:0x2A3
				break;
			// End:0x280
			case 3:
				t = __NFUN_112__(t, "AutonomousProxy");
				// End:0x2A3
				break;
			// End:0x2A0
			case 4:
				t = __NFUN_112__(t, "Authority");
				// End:0x2A3
				break;
			// End:0xFFFF
			default:
				break;
		}
		// End:0x2C4
		if(bTearOff)
		{
			t = __NFUN_112__(t, " Tear Off");
		}
		Canvas.__NFUN_465__(t, false);
		__NFUN_184__(YPos, YL);
		Canvas.SetPos(4.0000000, YPos);
	}
	t = "Physics ";
	switch(Physics)
	{
		// End:0x32D
		case 0:
			t = __NFUN_112__(t, "None");
			// End:0x486
			break;
		// End:0x34B
		case 1:
			t = __NFUN_112__(t, "Walking");
			// End:0x486
			break;
		// End:0x369
		case 2:
			t = __NFUN_112__(t, "Falling");
			// End:0x486
			break;
		// End:0x388
		case 3:
			t = __NFUN_112__(t, "Swimming");
			// End:0x486
			break;
		// End:0x3A5
		case 4:
			t = __NFUN_112__(t, "Flying");
			// End:0x486
			break;
		// End:0x3C4
		case 5:
			t = __NFUN_112__(t, "Rotating");
			// End:0x486
			break;
		// End:0x3E5
		case 6:
			t = __NFUN_112__(t, "Projectile");
			// End:0x486
			break;
		// End:0x409
		case 7:
			t = __NFUN_112__(t, "Interpolating");
			// End:0x486
			break;
		// End:0x42B
		case 8:
			t = __NFUN_112__(t, "MovingBrush");
			// End:0x486
			break;
		// End:0x448
		case 9:
			t = __NFUN_112__(t, "Spider");
			// End:0x486
			break;
		// End:0x466
		case 10:
			t = __NFUN_112__(t, "Trailer");
			// End:0x486
			break;
		// End:0x483
		case 11:
			t = __NFUN_112__(t, "Ladder");
			// End:0x486
			break;
		// End:0xFFFF
		default:
			break;
	}
	t = __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(t, " in physicsvolume "), GetItemName(string(PhysicsVolume))), " on base "), GetItemName(string(Base)));
	// End:0x4F8
	if(bBounce)
	{
		t = __NFUN_112__(t, " - will bounce");
	}
	Canvas.__NFUN_465__(t, false);
	__NFUN_184__(YPos, YL);
	Canvas.SetPos(4.0000000, YPos);
	Canvas.__NFUN_465__(__NFUN_112__(__NFUN_112__(__NFUN_112__("Location: ", string(Location)), " Rotation "), string(Rotation)), false);
	__NFUN_184__(YPos, YL);
	Canvas.SetPos(4.0000000, YPos);
	Canvas.__NFUN_465__(__NFUN_112__(__NFUN_112__(__NFUN_112__("Velocity: ", string(Velocity)), " Speed "), string(__NFUN_225__(Velocity))), false);
	__NFUN_184__(YPos, YL);
	Canvas.SetPos(4.0000000, YPos);
	Canvas.__NFUN_465__(__NFUN_112__("Acceleration: ", string(Acceleration)), false);
	__NFUN_184__(YPos, YL);
	Canvas.SetPos(4.0000000, YPos);
	Canvas.DrawColor.B = 0;
	Canvas.__NFUN_465__(__NFUN_112__(__NFUN_112__(__NFUN_112__("Collision Radius ", string(CollisionRadius)), " Height "), string(CollisionHeight)));
	__NFUN_184__(YPos, YL);
	Canvas.SetPos(4.0000000, YPos);
	Canvas.__NFUN_465__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("Collides with Actors ", string(bCollideActors)), ", world "), string(bCollideWorld)), ", proj. target "), string(bProjTarget)));
	__NFUN_184__(YPos, YL);
	Canvas.SetPos(4.0000000, YPos);
	Canvas.__NFUN_465__(__NFUN_112__(__NFUN_112__(__NFUN_112__("Blocks Actors ", string(bBlockActors)), ", players "), string(bBlockPlayers)));
	__NFUN_184__(YPos, YL);
	Canvas.SetPos(4.0000000, YPos);
	t = "Touching ";
	// End:0x7D6
	foreach __NFUN_307__(Class'Engine.Actor', A)
	{
		t = __NFUN_112__(__NFUN_112__(t, GetItemName(string(A))), " ");		
	}	
	// End:0x804
	if(__NFUN_122__(t, "Touching "))
	{
		t = "Touching nothing";
	}
	Canvas.__NFUN_465__(t, false);
	__NFUN_184__(YPos, YL);
	Canvas.SetPos(4.0000000, YPos);
	Canvas.DrawColor.R = 0;
	t = "Rendered: ";
	switch(Style)
	{
		// End:0x87D
		case 0:
			t = t;
			// End:0x918
			break;
		// End:0x89A
		case 1:
			t = __NFUN_112__(t, "Normal");
			// End:0x918
			break;
		// End:0x8B7
		case 2:
			t = __NFUN_112__(t, "Masked");
			// End:0x918
			break;
		// End:0x8D9
		case 3:
			t = __NFUN_112__(t, "Translucent");
			// End:0x918
			break;
		// End:0x8F9
		case 4:
			t = __NFUN_112__(t, "Modulated");
			// End:0x918
			break;
		// End:0x915
		case 5:
			t = __NFUN_112__(t, "Alpha");
			// End:0x918
			break;
		// End:0xFFFF
		default:
			break;
	}
	switch(DrawType)
	{
		// End:0x93B
		case 0:
			t = __NFUN_112__(t, " None");
			// End:0xA4E
			break;
		// End:0x95A
		case 1:
			t = __NFUN_112__(t, " Sprite ");
			// End:0xA4E
			break;
		// End:0x977
		case 2:
			t = __NFUN_112__(t, " Mesh ");
			// End:0xA4E
			break;
		// End:0x995
		case 3:
			t = __NFUN_112__(t, " Brush ");
			// End:0xA4E
			break;
		// End:0x9B8
		case 4:
			t = __NFUN_112__(t, " RopeSprite ");
			// End:0xA4E
			break;
		// End:0x9DF
		case 5:
			t = __NFUN_112__(t, " VerticalSprite ");
			// End:0xA4E
			break;
		// End:0xA01
		case 6:
			t = __NFUN_112__(t, " Terraform ");
			// End:0xA4E
			break;
		// End:0xA28
		case 7:
			t = __NFUN_112__(t, " SpriteAnimOnce ");
			// End:0xA4E
			break;
		// End:0xA4B
		case 8:
			t = __NFUN_112__(t, " StaticMesh ");
			// End:0xA4E
			break;
		// End:0xFFFF
		default:
			break;
	}
	// End:0xBB4
	if(__NFUN_154__(int(DrawType), int(2)))
	{
		t = __NFUN_112__(t, GetItemName(string(Mesh)));
		// End:0xAF9
		if(__NFUN_151__(Skins.Length, 0))
		{
			t = __NFUN_112__(t, " skins: ");
			i = 0;
			J0xAA2:

			// End:0xAF9 [Loop If]
			if(__NFUN_150__(i, Skins.Length))
			{
				// End:0xAC9
				if(__NFUN_114__(Skins[i], none))
				{
					// [Explicit Break]
					goto J0xAF9;
					// [Explicit Continue]
					goto J0xAEF;
				}
				t = __NFUN_112__(__NFUN_112__(t, GetItemName(string(Skins[i]))), ", ");
				J0xAEF:

				__NFUN_165__(i);
				// [Loop Continue]
				goto J0xAA2;
			}
		}
		J0xAF9:

		Canvas.__NFUN_465__(t, false);
		__NFUN_184__(YPos, YL);
		Canvas.SetPos(4.0000000, YPos);
		GetAnimParams(0, Anim, frame, Rate);
		t = __NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("AnimSequence ", string(Anim)), " Frame "), string(frame)), " Rate "), string(Rate));
		// End:0xBB1
		if(bAnimByOwner)
		{
			t = __NFUN_112__(t, " Anim by Owner");
		}		
	}
	else
	{
		// End:0xBED
		if(__NFUN_132__(__NFUN_154__(int(DrawType), int(1)), __NFUN_154__(int(DrawType), int(7))))
		{
			t = __NFUN_112__(t, string(Texture));			
		}
		else
		{
			// End:0xC11
			if(__NFUN_154__(int(DrawType), int(3)))
			{
				t = __NFUN_112__(t, string(Brush));
			}
		}
	}
	Canvas.__NFUN_465__(t, false);
	__NFUN_184__(YPos, YL);
	Canvas.SetPos(4.0000000, YPos);
	Canvas.DrawColor.B = byte(255);
	Canvas.__NFUN_465__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__(__NFUN_112__("Tag: ", string(Tag)), " Event: "), string(Event)), " STATE: "), string(__NFUN_284__())), false);
	__NFUN_184__(YPos, YL);
	Canvas.SetPos(4.0000000, YPos);
	Canvas.__NFUN_465__(__NFUN_112__(__NFUN_112__(__NFUN_112__("Instigator ", GetItemName(string(Instigator))), " Owner "), GetItemName(string(Owner))));
	__NFUN_184__(YPos, YL);
	Canvas.SetPos(4.0000000, YPos);
	return;
}

final simulated function bool NearSpot(Vector Spot)
{
	local Vector Dir;

	Dir = __NFUN_216__(Location, Spot);
	// End:0x2A
	if(__NFUN_177__(__NFUN_186__(Dir.Z), CollisionHeight))
	{
		return false;
	}
	Dir.Z = 0.0000000;
	return __NFUN_178__(__NFUN_225__(Dir), CollisionRadius);
	return;
}

final simulated function bool TouchingActor(Actor A)
{
	local Vector Dir;

	Dir = __NFUN_216__(Location, A.Location);
	// End:0x43
	if(__NFUN_177__(__NFUN_186__(Dir.Z), __NFUN_174__(CollisionHeight, A.CollisionHeight)))
	{
		return false;
	}
	Dir.Z = 0.0000000;
	return __NFUN_178__(__NFUN_225__(Dir), __NFUN_174__(CollisionRadius, A.CollisionRadius));
	return;
}

simulated function StartInterpolation()
{
	__NFUN_113__('None');
	bCollideWorld = false;
	bInterpolating = true;
	__NFUN_3970__(0);
	return;
}

function Reset()
{
	return;
}

event TriggerEvent(name EventName, Actor Other, Pawn EventInstigator)
{
	local Actor A;

	// End:0x11
	if(__NFUN_254__(EventName, 'None'))
	{
		return;
	}
	// End:0x40
	foreach __NFUN_313__(Class'Engine.Actor', A, EventName)
	{
		A.Trigger(Other, EventInstigator);		
	}	
	return;
}

function UntriggerEvent(name EventName, Actor Other, Pawn EventInstigator)
{
	local Actor A;

	// End:0x11
	if(__NFUN_254__(EventName, 'None'))
	{
		return;
	}
	// End:0x40
	foreach __NFUN_313__(Class'Engine.Actor', A, EventName)
	{
		A.UnTrigger(Other, EventInstigator);		
	}	
	return;
}

function bool IsInVolume(Volume aVolume)
{
	local Volume V;

	// End:0x23
	foreach __NFUN_307__(Class'Engine.Volume', V)
	{
		// End:0x22
		if(__NFUN_114__(V, aVolume))
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

	// End:0x3E
	foreach __NFUN_307__(Class'Engine.PhysicsVolume', V)
	{
		// End:0x3D
		if(__NFUN_130__(V.bPainCausing, __NFUN_177__(V.DamagePerSec, float(0))))
		{			
			return true;
		}		
	}	
	return false;
	return;
}

function PlayTeleportEffect(bool bOut, bool bSound)
{
	return;
}

function bool CanSplash()
{
	return false;
	return;
}

function Vector GetCollisionExtent()
{
	local Vector Extent;

	Extent = __NFUN_213__(CollisionRadius, vect(1.0000000, 1.0000000, 0.0000000));
	Extent.Z = CollisionHeight;
	return Extent;
	return;
}

simulated function bool EffectIsRelevant(Vector SpawnLocation, bool bForceDedicated)
{
	local PlayerController P;
	local bool bResult;

	// End:0x29
	if(__NFUN_154__(int(Level.NetMode), int(NM_DedicatedServer)))
	{
		bResult = bForceDedicated;		
	}
	else
	{
		// End:0x4D
		if(__NFUN_154__(int(Level.NetMode), int(NM_Client)))
		{
			bResult = true;			
		}
		else
		{
			// End:0x77
			if(__NFUN_130__(__NFUN_119__(Instigator, none), Instigator.IsHumanControlled()))
			{
				bResult = true;				
			}
			else
			{
				// End:0xAB
				if(__NFUN_217__(SpawnLocation, Location))
				{
					bResult = __NFUN_176__(__NFUN_175__(Level.TimeSeconds, LastRenderTime), float(3));					
				}
				else
				{
					// End:0xEA
					if(__NFUN_130__(__NFUN_119__(Instigator, none), __NFUN_176__(__NFUN_175__(Level.TimeSeconds, Instigator.LastRenderTime), float(3))))
					{
						bResult = true;						
					}
					else
					{
						P = Level.GetLocalPlayerController();
						// End:0x115
						if(__NFUN_114__(P, none))
						{
							bResult = false;							
						}
						else
						{
							bResult = __NFUN_177__(__NFUN_219__(Vector(P.Rotation), __NFUN_216__(SpawnLocation, P.ViewTarget.Location)), 0.0000000);
						}
					}
				}
			}
		}
	}
	return bResult;
	return;
}

function SetOverlayColor(byte R, byte G, byte B, byte A)
{
	OverlayColor.R = R;
	OverlayColor.G = G;
	OverlayColor.B = B;
	OverlayColor.A = A;
	return;
}

event GetEffTargetLocation(out Vector LocVector)
{
	LocVector = Location;
	return;
}

defaultproperties
{
	DrawType=1
	bLightingVisibility=true
	bUseDynamicLights=true
	bUpdateShadow=true
	bNeedCleanup=true
	CreatureID=-1
	bAcceptsProjectors=true
	bReplicateMovement=true
	L2NeedTick=true
	bNeedPostSpawnProcess=true
	L2LodViewDuration=1.5000000
	L2CurrentLod=-1
	L2ServerObjectRealID=-1
	L2ServerObjectID=-1
	RemoteRole=1
	Role=4
	NetUpdateFrequency=100.0000000
	NetPriority=1.0000000
	LODBias=1.0000000
	Texture=Texture'Engine.S_Actor'
	DrawScale=1.0000000
	DrawScale3D=(X=1.0000000,Y=1.0000000,Z=1.0000000)
	MaxLights=4
	ScaleGlow=1.0000000
	Style=1
	bMovable=true
	TransientSoundVolume=0.3000000
	TransientSoundRadius=300.0000000
	CollisionRadius=22.0000000
	CollisionHeight=22.0000000
	bBlockZeroExtentTraces=true
	bBlockNonZeroExtentTraces=true
	bJustTeleported=true
	Mass=100.0000000
	RotationLimit=(Pitch=65535,Yaw=65535,Roll=65535)
	MessageClass=Class'Engine.LocalMessage'
}