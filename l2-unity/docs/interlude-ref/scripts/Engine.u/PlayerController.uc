class PlayerController extends Controller
	native
	nativereplication
	config(User)
	notplaceable;

var const Player Player;
var globalconfig bool bLookUpStairs;
var globalconfig bool bSnapToLevel;
var globalconfig bool bAlwaysMouseLook;
var globalconfig bool bKeyboardLook;
var bool bCenterView;
var bool bBehindView;
var bool bFrozen;
var bool bPressedJump;
var bool bDoubleJump;
var bool bUpdatePosition;
var bool bIsTyping;
var bool bFixedCamera;
var bool bJumpStatus;
var bool bUpdating;
var globalconfig bool bNeverSwitchOnPickup;
var bool bZooming;
var globalconfig bool bAlwaysLevel;
var bool bSetTurnRot;
var bool bCheatFlying;
var bool bFreeCamera;
var bool bZeroRoll;
var bool bCameraPositionLocked;
var bool bViewBot;
var bool UseFixedVisibility;
var bool bBlockCloseCamera;
var bool bValidBehindCamera;
var bool bForcePrecache;
var globalconfig bool bNoVoiceMessages;
var globalconfig bool bNoVoiceTaunts;
var globalconfig bool bNoAutoTaunts;
var globalconfig bool bAutoTaunt;
var globalconfig bool bNoMatureLanguage;
var globalconfig byte AnnouncerLevel;
var globalconfig byte AnnouncerVolume;
var globalconfig float AimingHelp;
var globalconfig float MaxResponseTime;
var float WaitDelay;
var input float aBaseX;
var input float aBaseY;
var input float aBaseZ;
var input float aMouseX;
var input float aMouseY;
var input float aForward;
var input float aTurn;
var input float aStrafe;
var input float aUp;
var input float aLookUp;
var input byte bStrafe;
var input byte bSnapLevel;
var input byte bLook;
var input byte bFreeLook;
var input byte bTurn180;
var input byte bTurnToNearest;
var input byte bXAxis;
var input byte bYAxis;
var Actor.EDoubleClickDir DoubleClickDir;
var int ShowFlags;
var int Misc1;
var int Misc2;
var int RendMap;
var float OrthoZoom;
var const Actor ViewTarget;
var const Controller RealViewTarget;
var float CameraDist;
var Vector OldCameraLoc;
var Rotator OldCameraRot;
var transient array<CameraEffect> CameraEffects;
var bool bRenderWide;
var globalconfig float DesiredFOV;
var globalconfig float DefaultFOV;
var float ZoomLevel;
var Vector FixedLocation;
var Rotator FixedRotation;
var Matrix RenderWorldToCamera;
var Vector FlashScale;
var Vector FlashFog;
var float ConstantGlowScale;
var Vector ConstantGlowFog;
var Color LastDistanceFogColor;
var float LastDistanceFogStart;
var float LastDistanceFogEnd;
var float CurrentDistanceFogEnd;
var float TimeSinceLastFogChange;
var int LastZone;
var Rotator TargetViewRotation;
var float TargetEyeHeight;
var Vector TargetWeaponViewOffset;
var HUD myHUD;
var float LastPlaySound;
var float LastPlaySpeech;
var string Song;
var Actor.EMusicTransition Transition;
var SavedMove SavedMoves;
var SavedMove FreeMoves;
var SavedMove PendingMove;
var float CurrentTimeStamp;
var float LastUpdateTime;
var float ServerTimeStamp;
var float TimeMargin;
var float ClientUpdateTime;
var globalconfig float MaxTimeMargin;
var Weapon OldClientWeapon;
var int WeaponUpdate;
var string ProgressMessage[4];
var Color ProgressColor[4];
var float ProgressTimeOut;
var localized string QuickSaveString;
var localized string NoPauseMessage;
var localized string ViewingFrom;
var localized string OwnCamera;
var GameReplicationInfo GameReplicationInfo;
var globalconfig string StatsUsername;
var globalconfig string StatsPassword;
var Class<LocalMessage> LocalMessageClass;
var float MaxShakeRoll;
var Vector MaxShakeOffset;
var float ShakeRollRate;
var Vector ShakeOffsetRate;
var Vector ShakeOffset;
var float ShakeRollTime;
var Vector ShakeOffsetTime;
var Pawn TurnTarget;
var config int EnemyTurnSpeed;
var int GroundPitch;
var Rotator TurnRot180;
var Vector OldFloor;
var private transient CheatManager CheatManager;
var Class<CheatManager> CheatClass;
var private transient PlayerInput PlayerInput;
var config Class<PlayerInput> InputClass;
var const Vector FailedPathStart;
var int DemoViewPitch;
var int DemoViewYaw;
var Security PlayerSecurity;
var float ForcePrecacheTime;
var float LastPingUpdate;
var PhysicsVolume LastPhysicsVolume;
var float CurrentDistanceFogStart;
var Color CurrentDistanceColor;
var float VolumeFogBlendRatio;
var Sound UnderWaterLoopSound;
var bool bUseHoldCamera;
var float CompensateYaw;
var float CopmensatePitch;
var Emitter FloatingSolid;
var Emitter SunBeam;
var Vector LastSamplingLocation;

replication
{
	// Pos:0x0B3
	unreliable if(__NFUN_130__(__NFUN_154__(int(Role), int(ROLE_Authority)), __NFUN_129__(bDemoRecording)))
		ClientPlaySound, PlayAnnouncement;

	// Pos:0x0E7
	unreliable if(__NFUN_154__(int(Role), int(ROLE_Authority)))
		ClientAdjustPosition, ClientFlash, 
		ClientShake, LongClientAdjustPosition, 
		SetFOVAngle, ShortClientAdjustPosition, 
		VeryShortClientAdjustPosition;

	// Pos:0x0F4
	unreliable if(__NFUN_130__(__NFUN_132__(__NFUN_129__(bDemoRecording), __NFUN_130__(bClientDemoRecording, bClientDemoNetFunc)), __NFUN_154__(int(Role), int(ROLE_Authority))))
		ClientHearSound;

	// Pos:0x124
	unreliable if(__NFUN_150__(int(Role), int(ROLE_Authority)))
		RocketServerMove, Say, 
		ServerDrive, ServerMove, 
		ServerSetHandedness, ServerUpdatePing, 
		ServerUse, ServerViewNextPlayer, 
		ServerViewSelf, ShortServerMove, 
		TeamSay;

	// Pos:0x000
	reliable if(__NFUN_130__(__NFUN_130__(bNetDirty, bNetOwner), __NFUN_154__(int(Role), int(ROLE_Authority))))
		GameReplicationInfo;

	// Pos:0x023
	reliable if(__NFUN_130__(__NFUN_130__(__NFUN_130__(bNetOwner, __NFUN_154__(int(Role), int(ROLE_Authority))), __NFUN_119__(ViewTarget, Pawn)), __NFUN_119__(Pawn(ViewTarget), none)))
		TargetEyeHeight, TargetViewRotation, 
		TargetWeaponViewOffset;

	// Pos:0x05E
	reliable if(__NFUN_130__(bDemoRecording, __NFUN_154__(int(Role), int(ROLE_Authority))))
		DemoViewPitch, DemoViewYaw;

	// Pos:0x076
	reliable if(__NFUN_154__(int(Role), int(ROLE_Authority)))
		ClearProgressMessages, ClientAdjustGlow, 
		ClientChangeVoiceChatter, ClientCloseMenu, 
		ClientGotoState, ClientLeaveVoiceChat, 
		ClientOpenMenu, ClientReStart, 
		ClientReliablePlaySound, ClientSetBehindView, 
		ClientSetFixedCamera, ClientSetHUD, 
		ClientSetMusic, ClientSetViewTarget, 
		ClientValidate, EndZoom, 
		FOV, GivePawn, 
		SetProgressMessage, SetProgressTime, 
		StartZoom, StopZoom, 
		ToggleZoom;

	// Pos:0x083
	reliable if(__NFUN_130__(__NFUN_154__(int(Role), int(ROLE_Authority)), __NFUN_132__(__NFUN_129__(bDemoRecording), __NFUN_130__(bClientDemoRecording, bClientDemoNetFunc))))
		ClientMessage, ReceiveLocalizedMessage, 
		TeamMessage;

	// Pos:0x0CD
	reliable if(__NFUN_130__(__NFUN_154__(int(Role), int(ROLE_Authority)), __NFUN_129__(bDemoRecording)))
		ClientTravel;

	// Pos:0x131
	reliable if(__NFUN_150__(int(Role), int(ROLE_Authority)))
		ActivateItem, AskForPawn, 
		BehindView, ChangeName, 
		ChangeTeam, Mutate, 
		Pause, PrevItem, 
		ServerChangeVoiceChatter, ServerGetVoiceChatters, 
		ServerRestartGame, ServerThrowWeapon, 
		ServerValidationResponse, ServerVerifyViewTarget, 
		SetPause, Speech, 
		Suicide, Typing;
}

// Export UPlayerController::execGetPlayerNetworkAddress(FFrame&, void* const)
native final function string GetPlayerNetworkAddress();

// Export UPlayerController::execGetServerNetworkAddress(FFrame&, void* const)
native final function string GetServerNetworkAddress();

// Export UPlayerController::execConsoleCommand(FFrame&, void* const)
native function string ConsoleCommand(string Command);

// Export UPlayerController::execGetEntryLevel(FFrame&, void* const)
native final function LevelInfo GetEntryLevel();

// Export UPlayerController::execResetKeyboard(FFrame&, void* const)
native(544) final function ResetKeyboard();

// Export UPlayerController::execSetViewTarget(FFrame&, void* const)
native final function SetViewTarget(Actor NewViewTarget);

// Export UPlayerController::execClientTravel(FFrame&, void* const)
native event ClientTravel(string URL, Actor.ETravelType TravelType, bool bItems);

// Export UPlayerController::execUpdateURL(FFrame&, void* const)
native(546) final function UpdateURL(string NewOption, string NewValue, bool bSaveDefault);

// Export UPlayerController::execGetDefaultURL(FFrame&, void* const)
native final function string GetDefaultURL(string Option);

// Export UPlayerController::execCopyToClipboard(FFrame&, void* const)
native function CopyToClipboard(string Text);

// Export UPlayerController::execPasteFromClipboard(FFrame&, void* const)
native function string PasteFromClipboard();

// Export UPlayerController::execClientValidate(FFrame&, void* const)
private native event ClientValidate(string C);

// Export UPlayerController::execServerValidationResponse(FFrame&, void* const)
private native event ServerValidationResponse(string R);

// Export UPlayerController::execFindStairRotation(FFrame&, void* const)
native(524) final function int FindStairRotation(float DeltaTime);

// Export UPlayerController::execClientHearSound(FFrame&, void* const)
native event ClientHearSound(Actor Actor, int Id, Sound S, Vector SoundLocation, Vector Parameters, bool Attenuate);

event PostBeginPlay()
{
	super.PostBeginPlay();
	// End:0x2F
	if(__NFUN_123__(Level.LevelEnterText, ""))
	{
		ClientMessage(Level.LevelEnterText);
	}
	DesiredFOV = DefaultFOV;
	SetViewTarget(self);
	bForcePrecache = __NFUN_150__(int(Role), int(ROLE_Authority));
	ForcePrecacheTime = __NFUN_174__(Level.TimeSeconds, float(2));
	return;
}

function ServerGivePawn()
{
	GivePawn(Pawn);
	return;
}

function PendingStasis()
{
	bStasis = true;
	Pawn = none;
	__NFUN_113__('Scripting');
	return;
}

function AddCheats()
{
	// End:0x35
	if(__NFUN_130__(__NFUN_114__(CheatManager, none), __NFUN_154__(int(Level.NetMode), int(NM_Standalone))))
	{
		CheatManager = new CheatClass;
	}
	return;
}

event AddCheatManager()
{
	// End:0x1A
	if(__NFUN_114__(CheatManager, none))
	{
		CheatManager = new CheatClass;
	}
	return;
}

event ClientSetViewTarget(Actor A)
{
	// End:0x11
	if(__NFUN_114__(A, none))
	{
		ServerVerifyViewTarget();
	}
	SetViewTarget(A);
	return;
}

function ServerVerifyViewTarget()
{
	// End:0x0D
	if(__NFUN_114__(ViewTarget, self))
	{
		return;
	}
	ClientSetViewTarget(ViewTarget);
	return;
}

function SpawnDefaultHUD()
{
	myHUD = __NFUN_278__(Class'Engine.HUD', self);
	return;
}

function Reset()
{
	PawnDied(Pawn);
	super.Reset();
	SetViewTarget(self);
	bBehindView = false;
	WaitDelay = __NFUN_174__(Level.TimeSeconds, float(2));
	FixFOV();
	// End:0x5B
	if(__NFUN_129__(PlayerReplicationInfo.bOnlySpectator))
	{
		__NFUN_113__('PlayerWaiting');
	}
	return;
}

function CleanOutSavedMoves()
{
	local SavedMove Next;

	J0x00:
	// End:0x39 [Loop If]
	if(__NFUN_119__(SavedMoves, none))
	{
		Next = SavedMoves.NextMove;
		SavedMoves.__NFUN_279__();
		SavedMoves = Next;
		// [Loop Continue]
		goto J0x00;
	}
	// End:0x57
	if(__NFUN_119__(PendingMove, none))
	{
		PendingMove.__NFUN_279__();
		PendingMove = none;
	}
	return;
}

event InitInputSystem()
{
	PlayerInput = new InputClass;
	return;
}

function ClientGotoState(name NewState, name NewLabel)
{
	__NFUN_113__(NewState, NewLabel);
	return;
}

function AskForPawn()
{
	// End:0x1E
	if(__NFUN_281__('GameEnded'))
	{
		ClientGotoState('GameEnded', 'Begin');		
	}
	else
	{
		// End:0x37
		if(__NFUN_119__(Pawn, none))
		{
			GivePawn(Pawn);			
		}
		else
		{
			bFrozen = false;
			ServerReStartPlayer();
		}
	}
	return;
}

function GivePawn(Pawn NewPawn)
{
	// End:0x0D
	if(__NFUN_114__(NewPawn, none))
	{
		return;
	}
	Pawn = NewPawn;
	NewPawn.Controller = self;
	ClientReStart();
	return;
}

function int GetFacingDirection()
{
	local Vector X, Y, Z, Dir;

	__NFUN_229__(Pawn.Rotation, X, Y, Z);
	Dir = __NFUN_226__(Pawn.Acceleration);
	// End:0x6D
	if(__NFUN_177__(__NFUN_219__(Y, Dir), float(0)))
	{
		return int(__NFUN_174__(float(49152), __NFUN_171__(float(16384), __NFUN_219__(X, Dir))));		
	}
	else
	{
		return int(__NFUN_175__(float(16384), __NFUN_171__(float(16384), __NFUN_219__(X, Dir))));
	}
	return;
}

function Possess(Pawn aPawn)
{
	// End:0x21
	if(__NFUN_130__(__NFUN_119__(none, PlayerReplicationInfo), PlayerReplicationInfo.bOnlySpectator))
	{
		return;
	}
	__NFUN_299__(aPawn.Rotation);
	aPawn.PossessedBy(self);
	Pawn = aPawn;
	Pawn.bStasis = false;
	CleanOutSavedMoves();
	// End:0x8E
	if(__NFUN_119__(none, PlayerReplicationInfo))
	{
		PlayerReplicationInfo.bIsFemale = Pawn.bIsFemale;
	}
	ServerSetHandedness(Handedness);
	Restart();
	return;
}

function UnPossess()
{
	// End:0x58
	if(__NFUN_119__(Pawn, none))
	{
		__NFUN_267__(Pawn.Location);
		Pawn.RemoteRole = ROLE_SimulatedProxy;
		Pawn.UnPossessed();
		CleanOutSavedMoves();
		// End:0x58
		if(__NFUN_114__(ViewTarget, Pawn))
		{
			SetViewTarget(self);
		}
	}
	Pawn = none;
	__NFUN_113__('Spectating');
	return;
}

function ViewNextBot()
{
	// End:0x1A
	if(__NFUN_119__(CheatManager, none))
	{
		CheatManager.ViewBot();
	}
	return;
}

function PawnDied(Pawn P)
{
	// End:0x11
	if(__NFUN_119__(P, Pawn))
	{
		return;
	}
	EndZoom();
	// End:0x33
	if(__NFUN_119__(Pawn, none))
	{
		Pawn.RemoteRole = ROLE_SimulatedProxy;
	}
	// End:0x4A
	if(__NFUN_114__(ViewTarget, Pawn))
	{
		bBehindView = true;
	}
	super.PawnDied(P);
	return;
}

function ClientSetHUD(Class<HUD> newHUDType, Class<ScoreBoard> newScoringType)
{
	local HUD NewHUD;

	// End:0x6E
	if(__NFUN_132__(__NFUN_114__(myHUD, none), __NFUN_130__(__NFUN_119__(newHUDType, none), __NFUN_119__(newHUDType, myHUD.Class))))
	{
		NewHUD = __NFUN_278__(newHUDType, self);
		// End:0x6E
		if(__NFUN_119__(NewHUD, none))
		{
			// End:0x63
			if(__NFUN_119__(myHUD, none))
			{
				myHUD.__NFUN_279__();
			}
			myHUD = NewHUD;
		}
	}
	// End:0x9A
	if(__NFUN_130__(__NFUN_119__(myHUD, none), __NFUN_119__(newScoringType, none)))
	{
		myHUD.SpawnScoreBoard(newScoringType);
	}
	return;
}

function HandlePickup(Pickup pick)
{
	ReceiveLocalizedMessage(pick.MessageClass,,,, pick.Class);
	return;
}

final function float UpdateFlashComponent(float Current, float Step, float Goal)
{
	// End:0x26
	if(__NFUN_177__(Goal, Current))
	{
		return __NFUN_244__(__NFUN_174__(Current, Step), Goal);		
	}
	else
	{
		return __NFUN_245__(__NFUN_175__(Current, Step), Goal);
	}
	return;
}

function ViewFlash(float DeltaTime)
{
	local Vector goalFog;
	local float goalscale, Delta, Step;

	Delta = __NFUN_244__(0.1000000, DeltaTime);
	goalscale = 1.0000000;
	goalFog = vect(0.0000000, 0.0000000, 0.0000000);
	// End:0x7C
	if(__NFUN_119__(Pawn, none))
	{
		__NFUN_184__(goalscale, Pawn.HeadVolume.ViewFlash.X);
		__NFUN_223__(goalFog, Pawn.HeadVolume.ViewFog);
	}
	Step = __NFUN_171__(0.6000000, Delta);
	FlashScale.X = UpdateFlashComponent(FlashScale.X, Step, goalscale);
	FlashScale = __NFUN_213__(FlashScale.X, vect(1.0000000, 1.0000000, 1.0000000));
	FlashFog.X = UpdateFlashComponent(FlashFog.X, Step, goalFog.X);
	FlashFog.Y = UpdateFlashComponent(FlashFog.Y, Step, goalFog.Y);
	FlashFog.Z = UpdateFlashComponent(FlashFog.Z, Step, goalFog.Z);
	return;
}

event ReceiveLocalizedMessage(Class<LocalMessage> Message, optional int Switch, optional PlayerReplicationInfo RelatedPRI_1, optional PlayerReplicationInfo RelatedPRI_2, optional Object OptionalObject)
{
	Message.static.ClientReceive(self, Switch, RelatedPRI_1, RelatedPRI_2, OptionalObject);
	// End:0x99
	if(__NFUN_130__(__NFUN_130__(Message.default.bIsConsoleMessage, __NFUN_119__(Player, none)), __NFUN_119__(Player.Console, none)))
	{
		Player.Console.Message(Message.static.GetString(Switch, RelatedPRI_1, RelatedPRI_2, OptionalObject), 0.0000000);
	}
	return;
}

event ClientMessage(coerce string S, optional name Type)
{
	// End:0x1A
	if(__NFUN_254__(Type, 'None'))
	{
		Type = 'Event';
	}
	TeamMessage(PlayerReplicationInfo, S, Type);
	return;
}

event TeamMessage(PlayerReplicationInfo PRI, coerce string S, name Type)
{
	// End:0x29
	if(__NFUN_119__(myHUD, none))
	{
		myHUD.Message(PRI, S, Type);
	}
	// End:0x77
	if(__NFUN_130__(__NFUN_132__(__NFUN_254__(Type, 'Say'), __NFUN_254__(Type, 'TeamSay')), __NFUN_119__(PRI, none)))
	{
		S = __NFUN_112__(__NFUN_112__(PRI.PlayerName, ": "), S);
	}
	Player.Console.Message(S, 6.0000000);
	return;
}

simulated function PlayBeepSound()
{
	return;
}

simulated function PlayAnnouncement(Sound ASound, byte AnnouncementLevel, optional bool bForce)
{
	local float Atten;

	// End:0x15
	if(__NFUN_151__(int(AnnouncementLevel), int(AnnouncerLevel)))
	{
		return;
	}
	// End:0x41
	if(__NFUN_130__(__NFUN_129__(bForce), __NFUN_176__(__NFUN_175__(Level.TimeSeconds, LastPlaySound), float(1))))
	{
		return;
	}
	LastPlaySound = Level.TimeSeconds;
	LastPlaySpeech = Level.TimeSeconds;
	Atten = __NFUN_246__(__NFUN_174__(0.1000000, __NFUN_171__(float(AnnouncerVolume), 0.2250000)), 0.2000000, 1.0000000);
	ClientPlaySound(ASound, true, Atten, 5);
	return;
}

function bool AllowVoiceMessage(name messagetype)
{
	// End:0x1B
	if(__NFUN_154__(int(Level.NetMode), int(NM_Standalone)))
	{
		return true;
	}
	// End:0x69
	if(__NFUN_176__(__NFUN_175__(Level.TimeSeconds, OldMessageTime), float(3)))
	{
		// End:0x4A
		if(__NFUN_255__(messagetype, 'ORDER'))
		{
			return false;
		}
		// End:0x69
		if(__NFUN_176__(__NFUN_175__(Level.TimeSeconds, OldMessageTime), float(1)))
		{
			return false;
		}
	}
	// End:0xA4
	if(__NFUN_176__(__NFUN_175__(Level.TimeSeconds, OldMessageTime), float(10)))
	{
		OldMessageTime = __NFUN_174__(Level.TimeSeconds, float(5));		
	}
	else
	{
		OldMessageTime = Level.TimeSeconds;
	}
	return true;
	return;
}

simulated function ClientPlaySound(Sound ASound, optional bool bVolumeControl, optional float inAtten, optional Actor.ESoundSlot Slot)
{
	local float Atten;

	Atten = 0.9000000;
	// End:0x2B
	if(bVolumeControl)
	{
		Atten = __NFUN_246__(inAtten, 0.0000000, 1.0000000);
	}
	// End:0x55
	if(__NFUN_119__(ViewTarget, none))
	{
		ViewTarget.__NFUN_264__(ASound, Slot, Atten,,,, false);
	}
	return;
}

simulated function ClientReliablePlaySound(Sound ASound, optional bool bVolumeControl)
{
	ClientPlaySound(ASound, bVolumeControl);
	return;
}

simulated event Destroyed()
{
	local SavedMove Next;
	local KVehicle DrivenVehicle;
	local Pawn Driver;

	// End:0x9B
	if(__NFUN_119__(Pawn, none))
	{
		DrivenVehicle = KVehicle(Pawn);
		// End:0x59
		if(__NFUN_119__(DrivenVehicle, none))
		{
			Driver = DrivenVehicle.Driver;
			DrivenVehicle.KDriverLeave(true);
			Driver.__NFUN_279__();			
		}
		else
		{
			Pawn.Health = 0;
			Pawn.UnPossessed();
			Pawn.Died(self, Class'Engine.Suicided', Pawn.Location);
		}
	}
	// End:0xB2
	if(__NFUN_119__(myHUD, none))
	{
		myHUD.__NFUN_279__();
	}
	J0xB2:

	// End:0xEB [Loop If]
	if(__NFUN_119__(FreeMoves, none))
	{
		Next = FreeMoves.NextMove;
		FreeMoves.__NFUN_279__();
		FreeMoves = Next;
		// [Loop Continue]
		goto J0xB2;
	}
	J0xEB:

	// End:0x124 [Loop If]
	if(__NFUN_119__(SavedMoves, none))
	{
		Next = SavedMoves.NextMove;
		SavedMoves.__NFUN_279__();
		SavedMoves = Next;
		// [Loop Continue]
		goto J0xEB;
	}
	// End:0x142
	if(__NFUN_119__(PlayerSecurity, none))
	{
		PlayerSecurity.__NFUN_279__();
		PlayerSecurity = none;
	}
	super.Destroyed();
	return;
}

function ClientSetMusic(string NewSong, Actor.EMusicTransition NewTransition)
{
	StopAllMusic(0.0000000);
	PlayMusic(NewSong, 3.0000000);
	Song = NewSong;
	Transition = NewTransition;
	return;
}

function ToggleZoom()
{
	// End:0x18
	if(__NFUN_181__(DefaultFOV, DesiredFOV))
	{
		EndZoom();		
	}
	else
	{
		StartZoom();
	}
	return;
}

function StartZoom()
{
	ZoomLevel = 0.0000000;
	bZooming = true;
	return;
}

function StopZoom()
{
	bZooming = false;
	return;
}

function EndZoom()
{
	bZooming = false;
	DesiredFOV = DefaultFOV;
	return;
}

function FixFOV()
{
	FovAngle = default.DefaultFOV;
	DesiredFOV = default.DefaultFOV;
	DefaultFOV = default.DefaultFOV;
	return;
}

function SetFOV(float NewFOV)
{
	DesiredFOV = NewFOV;
	FovAngle = NewFOV;
	return;
}

function ResetFOV()
{
	DesiredFOV = DefaultFOV;
	FovAngle = DefaultFOV;
	return;
}

exec function FOV(float f)
{
	// End:0x4F
	if(__NFUN_132__(__NFUN_179__(f, 80.0000000), __NFUN_154__(int(Level.NetMode), int(NM_Standalone))))
	{
		DefaultFOV = __NFUN_246__(f, 1.0000000, 170.0000000);
		DesiredFOV = DefaultFOV;
		__NFUN_536__();
	}
	return;
}

exec function Mutate(string MutateString)
{
	// End:0x1B
	if(__NFUN_154__(int(Level.NetMode), int(NM_Client)))
	{
		return;
	}
	Level.Game.BaseMutator.Mutate(MutateString, self);
	return;
}

exec function SetSensitivity(float f)
{
	PlayerInput.UpdateSensitivity(f);
	return;
}

exec function SetMouseSmoothing(int Mode)
{
	PlayerInput.UpdateSmoothing(Mode);
	return;
}

exec function SetMouseAccel(float f)
{
	PlayerInput.UpdateAccel(f);
	return;
}

exec function ForceReload()
{
	// End:0x39
	if(__NFUN_130__(__NFUN_119__(Pawn, none), __NFUN_119__(Pawn.Weapon, none)))
	{
		Pawn.Weapon.ForceReload();
	}
	return;
}

exec function Say(string Msg)
{
	// End:0x4F
	if(__NFUN_130__(PlayerReplicationInfo.bAdmin, __NFUN_122__(__NFUN_128__(Msg, 1), "#")))
	{
		Level.Game.AdminSay(__NFUN_234__(Msg, __NFUN_147__(__NFUN_125__(Msg), 1)));
		return;
	}
	Level.Game.Broadcast(self, Msg, 'Say');
	return;
}

exec function TeamSay(string Msg)
{
	// End:0x21
	if(__NFUN_129__(GameReplicationInfo.bTeamGame))
	{
		Say(Msg);
		return;
	}
	Level.Game.BroadcastTeam(self, Level.Game.ParseMessageString(Level.Game.BaseMutator, self, Msg), 'TeamSay');
	return;
}

function ServerSetHandedness(float hand)
{
	Handedness = hand;
	// End:0x49
	if(__NFUN_130__(__NFUN_119__(Pawn, none), __NFUN_119__(Pawn.Weapon, none)))
	{
		Pawn.Weapon.SetHand(Handedness);
	}
	return;
}

function SetHand(int IntValue)
{
	Class'Engine.PlayerController'.default.Handedness = float(IntValue);
	Class'Engine.PlayerController'.static.StaticSaveConfig();
	Handedness = float(IntValue);
	// End:0x70
	if(__NFUN_130__(__NFUN_119__(Pawn, none), __NFUN_119__(Pawn.Weapon, none)))
	{
		Pawn.Weapon.SetHand(Handedness);
	}
	ServerSetHandedness(Handedness);
	return;
}

exec function SetWeaponHand(string S)
{
	// End:0x1E
	if(__NFUN_124__(S, "Left"))
	{
		Handedness = -1.0000000;		
	}
	else
	{
		// End:0x3D
		if(__NFUN_124__(S, "Right"))
		{
			Handedness = 1.0000000;			
		}
		else
		{
			// End:0x5D
			if(__NFUN_124__(S, "Center"))
			{
				Handedness = 0.0000000;				
			}
			else
			{
				// End:0x7A
				if(__NFUN_124__(S, "Hidden"))
				{
					Handedness = 2.0000000;
				}
			}
		}
	}
	SetHand(int(Handedness));
	return;
}

function bool IsDead()
{
	return false;
	return;
}

exec function ShowGun()
{
	// End:0x1C
	if(__NFUN_180__(Handedness, float(2)))
	{
		Handedness = 1.0000000;		
	}
	else
	{
		Handedness = 2.0000000;
	}
	SetHand(int(Handedness));
	return;
}

event PreClientTravel()
{
	return;
}

function ClientSetFixedCamera(bool B)
{
	bFixedCamera = B;
	return;
}

function ClientSetBehindView(bool B)
{
	bBehindView = B;
	return;
}

function ClientVoiceMessage(PlayerReplicationInfo Sender, PlayerReplicationInfo Recipient, name messagetype, byte MessageID)
{
	local VoicePack V;

	// End:0x39
	if(__NFUN_132__(__NFUN_132__(__NFUN_114__(Sender, none), __NFUN_114__(Sender.VoiceType, none)), __NFUN_114__(Player.Console, none)))
	{
		return;
	}
	V = __NFUN_278__(Sender.VoiceType, self);
	// End:0x7F
	if(__NFUN_119__(V, none))
	{
		V.ClientInitialize(Sender, Recipient, messagetype, MessageID);
	}
	return;
}

function ForceDeathUpdate()
{
	LastUpdateTime = __NFUN_175__(Level.TimeSeconds, float(10));
	return;
}

function RocketServerMove(float TimeStamp, Vector InAccel, Vector ClientLoc, byte ClientRoll, int View)
{
	ServerMove(TimeStamp, InAccel, ClientLoc, false, false, false, false, 0, ClientRoll, View);
	return;
}

function ShortServerMove(float TimeStamp, Vector ClientLoc, bool NewbRun, bool NewbDuck, bool NewbJumpStatus, byte ClientRoll, int View)
{
	ServerMove(TimeStamp, vect(0.0000000, 0.0000000, 0.0000000), ClientLoc, NewbRun, NewbDuck, NewbJumpStatus, false, 0, ClientRoll, View);
	return;
}

function ServerMove(float TimeStamp, Vector InAccel, Vector ClientLoc, bool NewbRun, bool NewbDuck, bool NewbJumpStatus, bool NewbDoubleJump, Actor.EDoubleClickDir DoubleClickMove, byte ClientRoll, int View, optional byte OldTimeDelta, optional int OldAccel)
{
	local float DeltaTime, clientErr, OldTimeStamp;
	local Rotator DeltaRot, Rot, ViewRot;
	local Vector Accel, LocDiff, ClientVel, ClientFloor;
	local int maxPitch, ViewPitch, ViewYaw;
	local bool NewbPressedJump, OldbRun, OldbDoubleJump;
	local Actor.EDoubleClickDir OldDoubleClickMove;
	local Actor ClientBase;
	local Actor.EPhysics ClientPhysics;

	// End:0x11
	if(__NFUN_179__(CurrentTimeStamp, TimeStamp))
	{
		return;
	}
	// End:0x268
	if(__NFUN_155__(int(OldTimeDelta), 0))
	{
		OldTimeStamp = __NFUN_175__(__NFUN_175__(TimeStamp, __NFUN_172__(float(OldTimeDelta), float(500))), 0.0010000);
		// End:0x268
		if(__NFUN_176__(CurrentTimeStamp, __NFUN_175__(OldTimeStamp, 0.0010000)))
		{
			Accel.X = float(__NFUN_196__(OldAccel, 23));
			// End:0xA3
			if(__NFUN_177__(Accel.X, float(127)))
			{
				Accel.X = __NFUN_171__(-1.0000000, __NFUN_175__(Accel.X, float(128)));
			}
			Accel.Y = float(__NFUN_156__(int(float(__NFUN_196__(OldAccel, 15))), 255));
			// End:0xF6
			if(__NFUN_177__(Accel.Y, float(127)))
			{
				Accel.Y = __NFUN_171__(-1.0000000, __NFUN_175__(Accel.Y, float(128)));
			}
			Accel.Z = float(__NFUN_156__(int(float(__NFUN_196__(OldAccel, 7))), 255));
			// End:0x149
			if(__NFUN_177__(Accel.Z, float(127)))
			{
				Accel.Z = __NFUN_171__(-1.0000000, __NFUN_175__(Accel.Z, float(128)));
			}
			__NFUN_221__(Accel, float(20));
			OldbRun = __NFUN_155__(__NFUN_156__(OldAccel, 64), 0);
			OldbDoubleJump = __NFUN_155__(__NFUN_156__(OldAccel, 32), 0);
			NewbPressedJump = __NFUN_155__(__NFUN_156__(OldAccel, 16), 0);
			// End:0x1A3
			if(NewbPressedJump)
			{
				bJumpStatus = NewbJumpStatus;
			}
			switch(__NFUN_156__(OldAccel, 7))
			{
				// End:0x1BD
				case 0:
					OldDoubleClickMove = 0;
					// End:0x1FF
					break;
				// End:0x1CC
				case 1:
					OldDoubleClickMove = 1;
					// End:0x1FF
					break;
				// End:0x1DC
				case 2:
					OldDoubleClickMove = 2;
					// End:0x1FF
					break;
				// End:0x1EC
				case 3:
					OldDoubleClickMove = 3;
					// End:0x1FF
					break;
				// End:0x1FC
				case 4:
					OldDoubleClickMove = 4;
					// End:0x1FF
					break;
				// End:0xFFFF
				default:
					break;
			}
			OldTimeStamp = __NFUN_244__(OldTimeStamp, __NFUN_174__(CurrentTimeStamp, MaxResponseTime));
			MoveAutonomous(__NFUN_175__(OldTimeStamp, CurrentTimeStamp), OldbRun, __NFUN_154__(int(bDuck), 1), NewbPressedJump, OldbDoubleJump, OldDoubleClickMove, Accel, rot(0, 0, 0));
			CurrentTimeStamp = OldTimeStamp;
		}
	}
	ViewPitch = __NFUN_145__(View, 32768);
	ViewYaw = __NFUN_144__(2, __NFUN_147__(View, __NFUN_144__(32768, ViewPitch)));
	__NFUN_159__(ViewPitch, float(2));
	Accel = __NFUN_214__(InAccel, float(10));
	NewbPressedJump = __NFUN_243__(bJumpStatus, NewbJumpStatus);
	bJumpStatus = NewbJumpStatus;
	DeltaTime = __NFUN_244__(MaxResponseTime, __NFUN_175__(TimeStamp, CurrentTimeStamp));
	// End:0x380
	if(__NFUN_177__(ServerTimeStamp, float(0)))
	{
		TimeMargin = __NFUN_245__(0.0000000, __NFUN_175__(__NFUN_174__(TimeMargin, DeltaTime), __NFUN_171__(1.0100000, __NFUN_175__(Level.TimeSeconds, ServerTimeStamp))));
		// End:0x380
		if(__NFUN_177__(TimeMargin, MaxTimeMargin))
		{
			__NFUN_185__(TimeMargin, DeltaTime);
			// End:0x36A
			if(__NFUN_176__(TimeMargin, 0.5000000))
			{
				MaxTimeMargin = default.MaxTimeMargin;				
			}
			else
			{
				MaxTimeMargin = 0.5000000;
			}
			DeltaTime = 0.0000000;
		}
	}
	CurrentTimeStamp = TimeStamp;
	ServerTimeStamp = Level.TimeSeconds;
	ViewRot.Pitch = ViewPitch;
	ViewRot.Yaw = ViewYaw;
	ViewRot.Roll = 0;
	__NFUN_299__(ViewRot);
	// End:0x514
	if(__NFUN_119__(Pawn, none))
	{
		Rot.Roll = __NFUN_144__(256, int(ClientRoll));
		Rot.Yaw = ViewYaw;
		// End:0x446
		if(__NFUN_132__(__NFUN_154__(int(Pawn.Physics), int(3)), __NFUN_154__(int(Pawn.Physics), int(4))))
		{
			maxPitch = 2;			
		}
		else
		{
			maxPitch = 0;
		}
		// End:0x4E0
		if(__NFUN_130__(__NFUN_151__(ViewPitch, __NFUN_144__(maxPitch, RotationRate.Pitch)), __NFUN_150__(ViewPitch, __NFUN_147__(65536, __NFUN_144__(maxPitch, RotationRate.Pitch)))))
		{
			// End:0x4BA
			if(__NFUN_150__(ViewPitch, 32768))
			{
				Rot.Pitch = __NFUN_144__(maxPitch, RotationRate.Pitch);				
			}
			else
			{
				Rot.Pitch = __NFUN_147__(65536, __NFUN_144__(maxPitch, RotationRate.Pitch));
			}			
		}
		else
		{
			Rot.Pitch = ViewPitch;
		}
		DeltaRot = __NFUN_317__(Rotation, Rot);
		Pawn.__NFUN_299__(Rot);
	}
	// End:0x569
	if(__NFUN_130__(__NFUN_114__(Level.Pauser, none), __NFUN_177__(DeltaTime, float(0))))
	{
		MoveAutonomous(DeltaTime, NewbRun, NewbDuck, NewbPressedJump, NewbDoubleJump, DoubleClickMove, Accel, DeltaRot);
	}
	// End:0x596
	if(__NFUN_177__(__NFUN_175__(Level.TimeSeconds, LastUpdateTime), 0.3000000))
	{
		clientErr = 10000.0000000;		
	}
	else
	{
		// End:0x614
		if(__NFUN_177__(__NFUN_175__(Level.TimeSeconds, LastUpdateTime), __NFUN_172__(180.0000000, float(Player.CurrentNetSpeed))))
		{
			// End:0x5E7
			if(__NFUN_114__(Pawn, none))
			{
				LocDiff = __NFUN_216__(Location, ClientLoc);				
			}
			else
			{
				LocDiff = __NFUN_216__(Pawn.Location, ClientLoc);
			}
			clientErr = __NFUN_219__(LocDiff, LocDiff);
		}
	}
	// End:0x851
	if(__NFUN_177__(clientErr, float(3)))
	{
		// End:0x651
		if(__NFUN_114__(Pawn, none))
		{
			ClientPhysics = Physics;
			ClientLoc = Location;
			ClientVel = Velocity;			
		}
		else
		{
			ClientPhysics = Pawn.Physics;
			ClientVel = Pawn.Velocity;
			ClientBase = Pawn.Base;
			// End:0x6D6
			if(__NFUN_119__(Mover(Pawn.Base), none))
			{
				ClientLoc = __NFUN_216__(Pawn.Location, Pawn.Base.Location);				
			}
			else
			{
				ClientLoc = Pawn.Location;
			}
			ClientFloor = Pawn.Floor;
		}
		LastUpdateTime = Level.TimeSeconds;
		// End:0x7DF
		if(__NFUN_132__(__NFUN_114__(Pawn, none), __NFUN_155__(int(Pawn.Physics), int(9))))
		{
			// End:0x788
			if(__NFUN_217__(ClientVel, vect(0.0000000, 0.0000000, 0.0000000)))
			{
				ShortClientAdjustPosition(TimeStamp, __NFUN_284__(), ClientPhysics, ClientLoc.X, ClientLoc.Y, ClientLoc.Z, ClientBase);				
			}
			else
			{
				ClientAdjustPosition(TimeStamp, __NFUN_284__(), ClientPhysics, ClientLoc.X, ClientLoc.Y, ClientLoc.Z, ClientVel.X, ClientVel.Y, ClientVel.Z, ClientBase);
			}			
		}
		else
		{
			LongClientAdjustPosition(TimeStamp, __NFUN_284__(), ClientPhysics, ClientLoc.X, ClientLoc.Y, ClientLoc.Z, ClientVel.X, ClientVel.Y, ClientVel.Z, ClientBase, ClientFloor.X, ClientFloor.Y, ClientFloor.Z);
		}
	}
	return;
}

function ServerDrive(float InForward, float InStrafe, bool InJump)
{
	ProcessDrive(InForward, InStrafe, InJump);
	return;
}

function ProcessDrive(float InForward, float InStrafe, bool InJump)
{
	__NFUN_231__("ProcessDrive Not Valid Outside State PlayerDriving");
	return;
}

function ProcessMove(float DeltaTime, Vector NewAccel, Actor.EDoubleClickDir DoubleClickMove, Rotator DeltaRot)
{
	// End:0x1F
	if(__NFUN_119__(Pawn, none))
	{
		Pawn.Acceleration = NewAccel;
	}
	return;
}

final function MoveAutonomous(float DeltaTime, bool NewbRun, bool NewbDuck, bool NewbPressedJump, bool NewbDoubleJump, Actor.EDoubleClickDir DoubleClickMove, Vector NewAccel, Rotator DeltaRot)
{
	// End:0x14
	if(NewbRun)
	{
		bRun = 1;		
	}
	else
	{
		bRun = 0;
	}
	// End:0x30
	if(NewbDuck)
	{
		bDuck = 1;		
	}
	else
	{
		bDuck = 0;
	}
	bPressedJump = NewbPressedJump;
	bDoubleJump = NewbDoubleJump;
	HandleWalking();
	ProcessMove(DeltaTime, NewAccel, DoubleClickMove, DeltaRot);
	// End:0x91
	if(__NFUN_119__(Pawn, none))
	{
		Pawn.__NFUN_3971__(DeltaTime);		
	}
	else
	{
		__NFUN_3971__(DeltaTime);
	}
	bDoubleJump = false;
	return;
}

function VeryShortClientAdjustPosition(float TimeStamp, float NewLocX, float NewLocY, float NewLocZ, Actor NewBase)
{
	local Vector Floor;

	// End:0x1F
	if(__NFUN_119__(Pawn, none))
	{
		Floor = Pawn.Floor;
	}
	LongClientAdjustPosition(TimeStamp, 'PlayerWalking', 1, NewLocX, NewLocY, NewLocZ, 0.0000000, 0.0000000, 0.0000000, NewBase, Floor.X, Floor.Y, Floor.Z);
	return;
}

function ShortClientAdjustPosition(float TimeStamp, name NewState, Actor.EPhysics newPhysics, float NewLocX, float NewLocY, float NewLocZ, Actor NewBase)
{
	local Vector Floor;

	// End:0x1F
	if(__NFUN_119__(Pawn, none))
	{
		Floor = Pawn.Floor;
	}
	LongClientAdjustPosition(TimeStamp, NewState, newPhysics, NewLocX, NewLocY, NewLocZ, 0.0000000, 0.0000000, 0.0000000, NewBase, Floor.X, Floor.Y, Floor.Z);
	return;
}

function ClientAdjustPosition(float TimeStamp, name NewState, Actor.EPhysics newPhysics, float NewLocX, float NewLocY, float NewLocZ, float NewVelX, float NewVelY, float NewVelZ, Actor NewBase)
{
	local Vector Floor;

	// End:0x1F
	if(__NFUN_119__(Pawn, none))
	{
		Floor = Pawn.Floor;
	}
	LongClientAdjustPosition(TimeStamp, NewState, newPhysics, NewLocX, NewLocY, NewLocZ, NewVelX, NewVelY, NewVelZ, NewBase, Floor.X, Floor.Y, Floor.Z);
	return;
}

function LongClientAdjustPosition(float TimeStamp, name NewState, Actor.EPhysics newPhysics, float NewLocX, float NewLocY, float NewLocZ, float NewVelX, float NewVelY, float NewVelZ, Actor NewBase, float NewFloorX, float NewFloorY, float NewFloorZ)
{
	local Vector NewLocation, NewVelocity, NewFloor;
	local Actor MoveActor;
	local SavedMove CurrentMove;
	local float NewPing;

	// End:0xDF
	if(__NFUN_119__(PlayerReplicationInfo, none))
	{
		// End:0x45
		if(__NFUN_150__(PlayerReplicationInfo.Ping, 6))
		{
			NewPing = __NFUN_171__(1000.0000000, __NFUN_175__(Level.TimeSeconds, TimeStamp));			
		}
		else
		{
			NewPing = float(PlayerReplicationInfo.Ping);
			NewPing = __NFUN_174__(__NFUN_171__(0.9900000, NewPing), __NFUN_171__(float(10), __NFUN_175__(Level.TimeSeconds, TimeStamp)));
		}
		PlayerReplicationInfo.Ping = int(NewPing);
		// End:0xDF
		if(__NFUN_177__(__NFUN_175__(Level.TimeSeconds, LastPingUpdate), float(4)))
		{
			LastPingUpdate = Level.TimeSeconds;
			ServerUpdatePing(int(NewPing));
		}
	}
	// End:0x192
	if(__NFUN_119__(Pawn, none))
	{
		// End:0x128
		if(Pawn.bTearOff)
		{
			Pawn = none;
			// End:0x126
			if(__NFUN_130__(__NFUN_129__(__NFUN_281__('GameEnded')), __NFUN_129__(__NFUN_281__('Dead'))))
			{
				__NFUN_113__('Dead');
			}
			return;
		}
		MoveActor = Pawn;
		// End:0x18F
		if(__NFUN_130__(__NFUN_119__(ViewTarget, Pawn), __NFUN_132__(__NFUN_114__(ViewTarget, self), __NFUN_130__(__NFUN_119__(Pawn(ViewTarget), none), __NFUN_152__(Pawn(ViewTarget).Health, 0)))))
		{
			bBehindView = false;
			SetViewTarget(Pawn);
		}		
	}
	else
	{
		MoveActor = self;
		// End:0x20C
		if(__NFUN_255__(__NFUN_284__(), NewState))
		{
			// End:0x1BF
			if(__NFUN_254__(NewState, 'GameEnded'))
			{
				__NFUN_113__(NewState);				
			}
			else
			{
				// End:0x1F6
				if(__NFUN_281__('Dead'))
				{
					// End:0x1F1
					if(__NFUN_130__(__NFUN_255__(NewState, 'PlayerWalking'), __NFUN_255__(NewState, 'PlayerSwimming')))
					{
						__NFUN_113__(NewState);
					}
					return;					
				}
				else
				{
					// End:0x20C
					if(__NFUN_254__(NewState, 'Dead'))
					{
						__NFUN_113__(NewState);
					}
				}
			}
		}
	}
	// End:0x21D
	if(__NFUN_177__(CurrentTimeStamp, TimeStamp))
	{
		return;
	}
	CurrentTimeStamp = TimeStamp;
	NewLocation.X = NewLocX;
	NewLocation.Y = NewLocY;
	NewLocation.Z = NewLocZ;
	NewVelocity.X = NewVelX;
	NewVelocity.Y = NewVelY;
	NewVelocity.Z = NewVelZ;
	CurrentMove = SavedMoves;
	J0x293:

	// End:0x3BE [Loop If]
	if(__NFUN_119__(CurrentMove, none))
	{
		// End:0x3B4
		if(__NFUN_178__(CurrentMove.TimeStamp, CurrentTimeStamp))
		{
			SavedMoves = CurrentMove.NextMove;
			CurrentMove.NextMove = FreeMoves;
			FreeMoves = CurrentMove;
			// End:0x397
			if(__NFUN_180__(CurrentMove.TimeStamp, CurrentTimeStamp))
			{
				FreeMoves.Clear();
				// End:0x38D
				if(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_176__(__NFUN_225__(__NFUN_216__(CurrentMove.SavedLocation, NewLocation)), float(3)), __NFUN_176__(__NFUN_225__(__NFUN_216__(CurrentMove.SavedVelocity, NewVelocity)), float(3))), __NFUN_254__(__NFUN_284__(), NewState)), __NFUN_132__(__NFUN_155__(int(MoveActor.Physics), int(4)), __NFUN_129__(__NFUN_281__('PlayerWalking')))))
				{
					return;
				}
				CurrentMove = none;				
			}
			else
			{
				FreeMoves.Clear();
				CurrentMove = SavedMoves;
			}			
		}
		else
		{
			CurrentMove = none;
		}
		// [Loop Continue]
		goto J0x293;
	}
	NewFloor.X = NewFloorX;
	NewFloor.Y = NewFloorY;
	NewFloor.Z = NewFloorZ;
	MoveActor.__NFUN_298__(NewBase, NewFloor);
	// End:0x429
	if(__NFUN_119__(Mover(NewBase), none))
	{
		__NFUN_223__(NewLocation, NewBase.Location);
	}
	MoveActor.bCanTeleport = false;
	// End:0x523
	if(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_129__(MoveActor.__NFUN_267__(NewLocation)), __NFUN_119__(Pawn(MoveActor), none)), __NFUN_177__(MoveActor.CollisionHeight, Pawn(MoveActor).CrouchHeight)), __NFUN_129__(Pawn(MoveActor).bIsCrouched)), __NFUN_154__(int(newPhysics), int(1))), __NFUN_155__(int(MoveActor.Physics), int(13))), __NFUN_155__(int(MoveActor.Physics), int(14))))
	{
		MoveActor.__NFUN_3970__(newPhysics);
		Pawn(MoveActor).ForceCrouch();
		MoveActor.__NFUN_267__(NewLocation);
	}
	MoveActor.bCanTeleport = true;
	// End:0x59D
	if(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_155__(int(MoveActor.Physics), int(13)), __NFUN_155__(int(MoveActor.Physics), int(14))), __NFUN_155__(int(newPhysics), int(13))), __NFUN_155__(int(newPhysics), int(14))))
	{
		MoveActor.__NFUN_3970__(newPhysics);
	}
	MoveActor.Velocity = NewVelocity;
	// End:0x5C5
	if(__NFUN_255__(__NFUN_284__(), NewState))
	{
		__NFUN_113__(NewState);
	}
	bUpdatePosition = true;
	return;
}

function ServerUpdatePing(int NewPing)
{
	PlayerReplicationInfo.Ping = NewPing;
	PlayerReplicationInfo.bReceivedPing = true;
	return;
}

function ClientUpdatePosition()
{
	local SavedMove CurrentMove;
	local int realbRun, realbDuck;
	local bool bRealJump;

	// End:0x43
	if(__NFUN_130__(__NFUN_119__(Pawn, none), __NFUN_132__(__NFUN_154__(int(Pawn.Physics), int(13)), __NFUN_154__(int(Pawn.Physics), int(14)))))
	{
		return;
	}
	bUpdatePosition = false;
	realbRun = int(bRun);
	realbDuck = int(bDuck);
	bRealJump = bPressedJump;
	CurrentMove = SavedMoves;
	bUpdating = true;
	J0x85:

	// End:0x240 [Loop If]
	if(__NFUN_119__(CurrentMove, none))
	{
		// End:0xF8
		if(__NFUN_178__(CurrentMove.TimeStamp, CurrentTimeStamp))
		{
			SavedMoves = CurrentMove.NextMove;
			CurrentMove.NextMove = FreeMoves;
			FreeMoves = CurrentMove;
			FreeMoves.Clear();
			CurrentMove = SavedMoves;			
		}
		else
		{
			// End:0x1B0
			if(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_119__(Pawn, none), __NFUN_155__(int(Pawn.Physics), int(CurrentMove.SavedPhysics))), __NFUN_155__(int(CurrentMove.SavedPhysics), int(0))), __NFUN_155__(int(CurrentMove.SavedPhysics), int(13))), __NFUN_155__(int(Pawn.Physics), int(13))), __NFUN_155__(int(Pawn.Physics), int(14))))
			{
				Pawn.__NFUN_3970__(CurrentMove.SavedPhysics);
			}
			MoveAutonomous(CurrentMove.Delta, CurrentMove.bRun, CurrentMove.bDuck, CurrentMove.bPressedJump, CurrentMove.bDoubleJump, CurrentMove.DoubleClickMove, CurrentMove.Acceleration, rot(0, 0, 0));
			CurrentMove = CurrentMove.NextMove;
		}
		// [Loop Continue]
		goto J0x85;
	}
	// End:0x37C
	if(__NFUN_119__(PendingMove, none))
	{
		// End:0x303
		if(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_119__(Pawn, none), __NFUN_155__(int(Pawn.Physics), int(PendingMove.SavedPhysics))), __NFUN_155__(int(PendingMove.SavedPhysics), int(0))), __NFUN_155__(int(PendingMove.SavedPhysics), int(13))), __NFUN_155__(int(Pawn.Physics), int(13))), __NFUN_155__(int(Pawn.Physics), int(14))))
		{
			Pawn.__NFUN_3970__(PendingMove.SavedPhysics);
		}
		MoveAutonomous(PendingMove.Delta, PendingMove.bRun, PendingMove.bDuck, PendingMove.bPressedJump, PendingMove.bDoubleJump, PendingMove.DoubleClickMove, PendingMove.Acceleration, rot(0, 0, 0));
	}
	bUpdating = false;
	bDuck = byte(realbDuck);
	bRun = byte(realbRun);
	bPressedJump = bRealJump;
	return;
}

final function SavedMove GetFreeMove()
{
	local SavedMove S, first;
	local int i;

	// End:0xF6
	if(__NFUN_114__(FreeMoves, none))
	{
		S = SavedMoves;
		J0x16:

		// End:0xEA [Loop If]
		if(__NFUN_119__(S, none))
		{
			__NFUN_165__(i);
			// End:0xD3
			if(__NFUN_151__(i, 100))
			{
				first = SavedMoves;
				SavedMoves = SavedMoves.NextMove;
				first.Clear();
				first.NextMove = none;
				J0x72:

				// End:0xCD [Loop If]
				if(__NFUN_119__(SavedMoves, none))
				{
					S = SavedMoves;
					SavedMoves = SavedMoves.NextMove;
					S.Clear();
					S.NextMove = FreeMoves;
					FreeMoves = S;
					// [Loop Continue]
					goto J0x72;
				}
				return first;
			}
			S = S.NextMove;
			// [Loop Continue]
			goto J0x16;
		}
		return __NFUN_278__(Class'Engine.SavedMove');		
	}
	else
	{
		S = FreeMoves;
		FreeMoves = FreeMoves.NextMove;
		S.NextMove = none;
		return S;
	}
	return;
}

function int CompressAccel(int C)
{
	// End:0x1D
	if(__NFUN_153__(C, 0))
	{
		C = __NFUN_249__(C, 127);		
	}
	else
	{
		C = __NFUN_146__(__NFUN_249__(int(__NFUN_186__(float(C))), 127), 128);
	}
	return C;
	return;
}

function ReplicateMove(float DeltaTime, Vector NewAccel, Actor.EDoubleClickDir DoubleClickMove, Rotator DeltaRot)
{
	local SavedMove NewMove, OldMove, LastMove;
	local byte ClientRoll;
	local float OldTimeDelta, NetMoveDelta;
	local int OldAccel;
	local Vector BuildAccel, AccelNorm, MoveLoc;

	// End:0x192
	if(__NFUN_119__(SavedMoves, none))
	{
		LastMove = SavedMoves;
		AccelNorm = __NFUN_226__(NewAccel);
		J0x23:

		// End:0xF0 [Loop If]
		if(__NFUN_119__(LastMove.NextMove, none))
		{
			// End:0xD9
			if(__NFUN_132__(__NFUN_132__(__NFUN_132__(LastMove.bPressedJump, LastMove.bDoubleJump), __NFUN_130__(__NFUN_155__(int(LastMove.DoubleClickMove), int(0)), __NFUN_150__(int(LastMove.DoubleClickMove), 5))), __NFUN_130__(__NFUN_218__(LastMove.Acceleration, NewAccel), __NFUN_176__(__NFUN_219__(__NFUN_226__(LastMove.Acceleration), AccelNorm), 0.9500000))))
			{
				OldMove = LastMove;
			}
			LastMove = LastMove.NextMove;
			// [Loop Continue]
			goto J0x23;
		}
		// End:0x192
		if(__NFUN_132__(__NFUN_132__(__NFUN_132__(LastMove.bPressedJump, LastMove.bDoubleJump), __NFUN_130__(__NFUN_155__(int(LastMove.DoubleClickMove), int(0)), __NFUN_150__(int(LastMove.DoubleClickMove), 5))), __NFUN_130__(__NFUN_218__(LastMove.Acceleration, NewAccel), __NFUN_176__(__NFUN_219__(__NFUN_226__(LastMove.Acceleration), AccelNorm), 0.9500000))))
		{
			OldMove = LastMove;
		}
	}
	// End:0x1BC
	if(__NFUN_119__(PendingMove, none))
	{
		PendingMove.SetMoveFor(self, DeltaTime, NewAccel, DoubleClickMove);
	}
	NewMove = GetFreeMove();
	// End:0x1D5
	if(__NFUN_114__(NewMove, none))
	{
		return;
	}
	NewMove.SetMoveFor(self, DeltaTime, NewAccel, DoubleClickMove);
	bDoubleJump = false;
	ProcessMove(NewMove.Delta, NewMove.Acceleration, NewMove.DoubleClickMove, DeltaRot);
	// End:0x259
	if(__NFUN_119__(Pawn, none))
	{
		Pawn.__NFUN_3971__(NewMove.Delta);		
	}
	else
	{
		__NFUN_3971__(DeltaTime);
	}
	// End:0x27A
	if(__NFUN_114__(PendingMove, none))
	{
		PendingMove = NewMove;		
	}
	else
	{
		NewMove.NextMove = FreeMoves;
		FreeMoves = NewMove;
		FreeMoves.Clear();
		NewMove = PendingMove;
	}
	NewMove.PostUpdate(self);
	NetMoveDelta = __NFUN_245__(__NFUN_172__(80.0000000, float(Player.CurrentNetSpeed)), 0.0150000);
	// End:0x3C2
	if(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_129__(Level.bCapFramerate), __NFUN_129__(PendingMove.bPressedJump)), __NFUN_129__(PendingMove.bDoubleJump)), __NFUN_132__(__NFUN_154__(int(PendingMove.DoubleClickMove), int(0)), __NFUN_154__(int(PendingMove.DoubleClickMove), int(5)))), __NFUN_132__(__NFUN_217__(PendingMove.Acceleration, NewAccel), __NFUN_177__(__NFUN_219__(__NFUN_226__(NewAccel), __NFUN_226__(PendingMove.Acceleration)), 0.9500000))), __NFUN_176__(PendingMove.Delta, __NFUN_175__(NetMoveDelta, ClientUpdateTime))))
	{
		return;		
	}
	else
	{
		ClientUpdateTime = __NFUN_175__(PendingMove.Delta, NetMoveDelta);
		// End:0x3F6
		if(__NFUN_114__(SavedMoves, none))
		{
			SavedMoves = PendingMove;			
		}
		else
		{
			LastMove.NextMove = PendingMove;
		}
		PendingMove = none;
	}
	// End:0x52E
	if(__NFUN_119__(OldMove, none))
	{
		OldTimeDelta = __NFUN_244__(255.0000000, __NFUN_171__(__NFUN_175__(Level.TimeSeconds, OldMove.TimeStamp), float(500)));
		BuildAccel = __NFUN_215__(__NFUN_213__(0.0500000, OldMove.Acceleration), vect(0.5000000, 0.5000000, 0.5000000));
		OldAccel = __NFUN_146__(__NFUN_146__(__NFUN_148__(CompressAccel(int(BuildAccel.X)), 23), __NFUN_148__(CompressAccel(int(BuildAccel.Y)), 15)), __NFUN_148__(CompressAccel(int(BuildAccel.Z)), 7));
		// End:0x4E1
		if(OldMove.bRun)
		{
			__NFUN_161__(OldAccel, 64);
		}
		// End:0x4FC
		if(OldMove.bDoubleJump)
		{
			__NFUN_161__(OldAccel, 32);
		}
		// End:0x517
		if(OldMove.bPressedJump)
		{
			__NFUN_161__(OldAccel, 16);
		}
		__NFUN_161__(OldAccel, int(OldMove.DoubleClickMove));
	}
	ClientRoll = byte(__NFUN_156__(int(byte(__NFUN_149__(Rotation.Roll, 8))), 255));
	// End:0x56D
	if(NewMove.bPressedJump)
	{
		bJumpStatus = __NFUN_129__(bJumpStatus);
	}
	// End:0x586
	if(__NFUN_114__(Pawn, none))
	{
		MoveLoc = Location;		
	}
	else
	{
		MoveLoc = Pawn.Location;
	}
	// End:0x60D
	if(__NFUN_281__('PlayerRocketing'))
	{
		RocketServerMove(NewMove.TimeStamp, __NFUN_212__(NewMove.Acceleration, float(10)), MoveLoc, ClientRoll, __NFUN_146__(__NFUN_144__(__NFUN_156__(32767, __NFUN_145__(Rotation.Pitch, 2)), 32768), __NFUN_156__(32767, __NFUN_145__(Rotation.Yaw, 2))));		
	}
	else
	{
		// End:0x6D6
		if(__NFUN_130__(__NFUN_130__(__NFUN_217__(NewMove.Acceleration, vect(0.0000000, 0.0000000, 0.0000000)), __NFUN_154__(int(NewMove.DoubleClickMove), int(0))), __NFUN_129__(NewMove.bDoubleJump)))
		{
			ShortServerMove(NewMove.TimeStamp, MoveLoc, NewMove.bRun, NewMove.bDuck, bJumpStatus, ClientRoll, __NFUN_146__(__NFUN_144__(__NFUN_156__(32767, __NFUN_145__(Rotation.Pitch, 2)), 32768), __NFUN_156__(32767, __NFUN_145__(Rotation.Yaw, 2))));			
		}
		else
		{
			ServerMove(NewMove.TimeStamp, __NFUN_212__(NewMove.Acceleration, float(10)), MoveLoc, NewMove.bRun, NewMove.bDuck, bJumpStatus, NewMove.bDoubleJump, NewMove.DoubleClickMove, ClientRoll, __NFUN_146__(__NFUN_144__(__NFUN_156__(32767, __NFUN_145__(Rotation.Pitch, 2)), 32768), __NFUN_156__(32767, __NFUN_145__(Rotation.Yaw, 2))), byte(OldTimeDelta), OldAccel);
		}
	}
	return;
}

function HandleWalking()
{
	// End:0x41
	if(__NFUN_119__(Pawn, none))
	{
		Pawn.SetWalking(__NFUN_130__(__NFUN_155__(int(bRun), 0), __NFUN_129__(Region.Zone.__NFUN_303__('WarpZoneInfo'))));
	}
	return;
}

function ServerRestartGame()
{
	return;
}

function SetFOVAngle(float NewFOV)
{
	FovAngle = NewFOV;
	return;
}

function ClientFlash(float Scale, Vector fog)
{
	FlashScale = __NFUN_213__(Scale, vect(1.0000000, 1.0000000, 1.0000000));
	FlashFog = __NFUN_213__(0.0010000, fog);
	return;
}

function ClientAdjustGlow(float Scale, Vector fog)
{
	__NFUN_184__(ConstantGlowScale, Scale);
	__NFUN_223__(ConstantGlowFog, __NFUN_213__(0.0010000, fog));
	return;
}

private function ClientShake(Vector ShakeRoll, Vector OffsetMag, Vector ShakeRate, float OffsetTime)
{
	// End:0x6F
	if(__NFUN_132__(__NFUN_176__(MaxShakeRoll, ShakeRoll.X), __NFUN_176__(ShakeRollTime, __NFUN_171__(0.0100000, ShakeRoll.Y))))
	{
		MaxShakeRoll = ShakeRoll.X;
		ShakeRollTime = __NFUN_171__(0.0100000, ShakeRoll.Y);
		ShakeRollRate = __NFUN_171__(0.0100000, ShakeRoll.Z);
	}
	// End:0xB2
	if(__NFUN_177__(__NFUN_225__(OffsetMag), __NFUN_225__(MaxShakeOffset)))
	{
		ShakeOffsetTime = __NFUN_213__(OffsetTime, vect(1.0000000, 1.0000000, 1.0000000));
		MaxShakeOffset = OffsetMag;
		ShakeOffsetRate = ShakeRate;
	}
	return;
}

function ShakeView(float ShakeTime, float RollMag, Vector OffsetMag, float RollRate, Vector OffsetRate, float OffsetTime)
{
	local Vector ShakeRoll;

	ShakeRoll.X = RollMag;
	ShakeRoll.Y = __NFUN_171__(100.0000000, ShakeTime);
	ShakeRoll.Z = __NFUN_171__(100.0000000, RollRate);
	ClientShake(ShakeRoll, OffsetMag, OffsetRate, OffsetTime);
	return;
}

function damageAttitudeTo(Pawn Other, float Damage)
{
	// End:0x36
	if(__NFUN_130__(__NFUN_130__(__NFUN_119__(Other, none), __NFUN_119__(Other, Pawn)), __NFUN_177__(Damage, float(0))))
	{
		Enemy = Other;
	}
	return;
}

function Typing(bool bTyping)
{
	bIsTyping = bTyping;
	Pawn.bIsTyping = bIsTyping;
	// End:0x5E
	if(__NFUN_130__(__NFUN_130__(bTyping, __NFUN_119__(Pawn, none)), __NFUN_129__(Pawn.bTearOff)))
	{
		Pawn.ChangeAnimation();
	}
	return;
}

exec function Jump(optional float f)
{
	// End:0x22
	if(__NFUN_114__(Level.Pauser, PlayerReplicationInfo))
	{
		SetPause(false);		
	}
	else
	{
		bPressedJump = true;
	}
	return;
}

exec function Speech(name Type, int Index, string Callsign)
{
	// End:0x3C
	if(__NFUN_119__(PlayerReplicationInfo.VoiceType, none))
	{
		PlayerReplicationInfo.VoiceType.static.PlayerSpeech(Type, Index, Callsign, self);
	}
	return;
}

exec function RestartLevel()
{
	// End:0x2C
	if(__NFUN_154__(int(Level.NetMode), int(NM_Standalone)))
	{
		ClientTravel("?restart", NM_ListenServer, false);
	}
	return;
}

exec function LocalTravel(string URL)
{
	// End:0x27
	if(__NFUN_154__(int(Level.NetMode), int(NM_Standalone)))
	{
		ClientTravel(URL, 2, true);
	}
	return;
}

exec function QuickSave()
{
	// End:0x4D
	if(__NFUN_130__(__NFUN_151__(Pawn.Health, 0), __NFUN_154__(int(Level.NetMode), int(NM_Standalone))))
	{
		ClientMessage(QuickSaveString);
		ConsoleCommand("SaveGame 9");
	}
	return;
}

exec function QuickLoad()
{
	// End:0x2B
	if(__NFUN_154__(int(Level.NetMode), int(NM_Standalone)))
	{
		ClientTravel("?load=9", NM_Standalone, false);
	}
	return;
}

function bool SetPause(bool bPause)
{
	bFire = 0;
	bAltFire = 0;
	return Level.Game.SetPause(bPause, self);
	return;
}

exec function Pause()
{
	// End:0x1E
	if(__NFUN_114__(Level.Pauser, none))
	{
		SetPause(true);		
	}
	else
	{
		SetPause(false);
	}
	return;
}

exec function ShowMenu()
{
	// End:0x1B
	if(__NFUN_114__(Level.Pauser, none))
	{
		SetPause(true);
	}
	return;
}

exec function ActivateInventoryItem(Class InvItem)
{
	local Powerups Inv;

	Inv = Powerups(Pawn.FindInventoryType(InvItem));
	// End:0x39
	if(__NFUN_119__(Inv, none))
	{
		Inv.Activate();
	}
	return;
}

exec function ThrowWeapon()
{
	// End:0x23
	if(__NFUN_132__(__NFUN_114__(Pawn, none), __NFUN_114__(Pawn.Weapon, none)))
	{
		return;
	}
	ServerThrowWeapon();
	return;
}

function ServerThrowWeapon()
{
	local Vector TossVel;

	// End:0x74
	if(Pawn.CanThrowWeapon())
	{
		TossVel = Vector(GetViewRotation());
		TossVel = __NFUN_215__(__NFUN_212__(TossVel, __NFUN_174__(__NFUN_219__(Pawn.Velocity, TossVel), float(500))), vect(0.0000000, 0.0000000, 200.0000000));
		Pawn.TossWeapon(TossVel);
		ClientSwitchToBestWeapon();
	}
	return;
}

exec function PrevWeapon()
{
	// End:0x16
	if(__NFUN_119__(Level.Pauser, none))
	{
		return;
	}
	// End:0x32
	if(__NFUN_114__(Pawn.Weapon, none))
	{
		SwitchToBestWeapon();
		return;
	}
	// End:0x7F
	if(__NFUN_119__(Pawn.PendingWeapon, none))
	{
		Pawn.PendingWeapon = Pawn.Inventory.PrevWeapon(none, Pawn.PendingWeapon);		
	}
	else
	{
		Pawn.PendingWeapon = Pawn.Inventory.PrevWeapon(none, Pawn.Weapon);
	}
	// End:0xE1
	if(__NFUN_119__(Pawn.PendingWeapon, none))
	{
		Pawn.Weapon.PutDown();
	}
	return;
}

exec function NextWeapon()
{
	// End:0x16
	if(__NFUN_119__(Level.Pauser, none))
	{
		return;
	}
	// End:0x32
	if(__NFUN_114__(Pawn.Weapon, none))
	{
		SwitchToBestWeapon();
		return;
	}
	// End:0x7F
	if(__NFUN_119__(Pawn.PendingWeapon, none))
	{
		Pawn.PendingWeapon = Pawn.Inventory.NextWeapon(none, Pawn.PendingWeapon);		
	}
	else
	{
		Pawn.PendingWeapon = Pawn.Inventory.NextWeapon(none, Pawn.Weapon);
	}
	// End:0xE1
	if(__NFUN_119__(Pawn.PendingWeapon, none))
	{
		Pawn.Weapon.PutDown();
	}
	return;
}

exec function SwitchWeapon(byte f)
{
	local Weapon newWeapon;

	// End:0x39
	if(__NFUN_132__(__NFUN_132__(__NFUN_119__(Level.Pauser, none), __NFUN_114__(Pawn, none)), __NFUN_114__(Pawn.Inventory, none)))
	{
		return;
	}
	// End:0x9B
	if(__NFUN_130__(__NFUN_119__(Pawn.Weapon, none), __NFUN_119__(Pawn.Weapon.Inventory, none)))
	{
		newWeapon = Pawn.Weapon.Inventory.WeaponChange(f);		
	}
	else
	{
		newWeapon = none;
	}
	// End:0xD0
	if(__NFUN_114__(newWeapon, none))
	{
		newWeapon = Pawn.Inventory.WeaponChange(f);
	}
	// End:0xDD
	if(__NFUN_114__(newWeapon, none))
	{
		return;
	}
	// End:0x117
	if(__NFUN_114__(Pawn.Weapon, none))
	{
		Pawn.PendingWeapon = newWeapon;
		Pawn.ChangedWeapon();		
	}
	else
	{
		// End:0x170
		if(__NFUN_119__(Pawn.Weapon, newWeapon))
		{
			Pawn.PendingWeapon = newWeapon;
			// End:0x170
			if(__NFUN_129__(Pawn.Weapon.PutDown()))
			{
				Pawn.PendingWeapon = none;
			}
		}
	}
	return;
}

exec function GetWeapon(Class<Weapon> NewWeaponClass)
{
	local Inventory Inv;
	local int Count;

	// End:0x5C
	if(__NFUN_132__(__NFUN_132__(__NFUN_114__(Pawn.Inventory, none), __NFUN_114__(NewWeaponClass, none)), __NFUN_130__(__NFUN_119__(Pawn.Weapon, none), __NFUN_114__(Pawn.Weapon.Class, NewWeaponClass))))
	{
		return;
	}
	Inv = Pawn.Inventory;
	J0x70:

	// End:0x15A [Loop If]
	if(__NFUN_119__(Inv, none))
	{
		// End:0x12B
		if(__NFUN_114__(Inv.Class, NewWeaponClass))
		{
			Pawn.PendingWeapon = Weapon(Inv);
			// End:0x111
			if(__NFUN_129__(Pawn.PendingWeapon.HasAmmo()))
			{
				ClientMessage(__NFUN_112__(Pawn.PendingWeapon.ItemName, Pawn.PendingWeapon.MessageNoAmmo));
				Pawn.PendingWeapon = none;
				return;
			}
			Pawn.Weapon.PutDown();
			return;
		}
		__NFUN_165__(Count);
		// End:0x143
		if(__NFUN_151__(Count, 1000))
		{
			return;
		}
		Inv = Inv.Inventory;
		// [Loop Continue]
		goto J0x70;
	}
	return;
}

exec function PrevItem()
{
	local Inventory Inv;
	local Powerups LastItem;

	// End:0x23
	if(__NFUN_132__(__NFUN_119__(Level.Pauser, none), __NFUN_114__(Pawn, none)))
	{
		return;
	}
	// End:0x60
	if(__NFUN_114__(Pawn.SelectedItem, none))
	{
		Pawn.SelectedItem = Pawn.Inventory.SelectNext();
		return;
	}
	// End:0x107
	if(__NFUN_119__(Pawn.SelectedItem.Inventory, none))
	{
		Inv = Pawn.SelectedItem.Inventory;
		J0x9A:

		// End:0x107 [Loop If]
		if(__NFUN_119__(Inv, none))
		{
			// End:0xB3
			if(__NFUN_114__(Inv, none))
			{
				// [Explicit Break]
				goto J0x107;
			}
			// End:0xF0
			if(__NFUN_130__(Inv.__NFUN_303__('Powerups'), Powerups(Inv).bActivatable))
			{
				LastItem = Powerups(Inv);
			}
			Inv = Inv.Inventory;
			// [Loop Continue]
			goto J0x9A;
		}
	}
	J0x107:

	Inv = Pawn.Inventory;
	J0x11B:

	// End:0x195 [Loop If]
	if(__NFUN_119__(Inv, Pawn.SelectedItem))
	{
		// End:0x141
		if(__NFUN_114__(Inv, none))
		{
			// [Explicit Break]
			goto J0x195;
		}
		// End:0x17E
		if(__NFUN_130__(Inv.__NFUN_303__('Powerups'), Powerups(Inv).bActivatable))
		{
			LastItem = Powerups(Inv);
		}
		Inv = Inv.Inventory;
		// [Loop Continue]
		goto J0x11B;
	}
	J0x195:

	// End:0x1B4
	if(__NFUN_119__(LastItem, none))
	{
		Pawn.SelectedItem = LastItem;
	}
	return;
}

exec function ActivateItem()
{
	// End:0x16
	if(__NFUN_119__(Level.Pauser, none))
	{
		return;
	}
	// End:0x4F
	if(__NFUN_130__(__NFUN_119__(Pawn, none), __NFUN_119__(Pawn.SelectedItem, none)))
	{
		Pawn.SelectedItem.Activate();
	}
	return;
}

exec function Fire(optional float f)
{
	// End:0x21
	if(__NFUN_114__(Level.Pauser, PlayerReplicationInfo))
	{
		SetPause(false);
		return;
	}
	Pawn.Fire(f);
	return;
}

exec function AltFire(optional float f)
{
	// End:0x21
	if(__NFUN_114__(Level.Pauser, PlayerReplicationInfo))
	{
		SetPause(false);
		return;
	}
	// End:0x52
	if(__NFUN_119__(Pawn.Weapon, none))
	{
		Pawn.Weapon.AltFire(f);
	}
	return;
}

exec function Use()
{
	ServerUse();
	return;
}

function ServerUse()
{
	local Actor A;

	// End:0x21
	if(__NFUN_114__(Level.Pauser, PlayerReplicationInfo))
	{
		SetPause(false);
		return;
	}
	// End:0x2E
	if(__NFUN_114__(Pawn, none))
	{
		return;
	}
	// End:0x5C
	foreach Pawn.__NFUN_307__(Class'Engine.Actor', A)
	{
		A.UsedBy(Pawn);		
	}	
	return;
}

exec function Suicide()
{
	// End:0x47
	if(__NFUN_130__(__NFUN_119__(Pawn, none), __NFUN_177__(__NFUN_175__(Level.TimeSeconds, Pawn.LastStartTime), float(1))))
	{
		Pawn.KilledBy(Pawn);
	}
	return;
}

exec function Name(coerce string S)
{
	SetName(S);
	return;
}

exec function SetName(coerce string S)
{
	ChangeName(S);
	__NFUN_546__("Name", S, true);
	__NFUN_536__();
	return;
}

function ChangeName(coerce string S)
{
	// End:0x1D
	if(__NFUN_151__(__NFUN_125__(S), 20))
	{
		S = __NFUN_128__(S, 20);
	}
	ReplaceText(S, " ", "_");
	Level.Game.ChangeName(self, S, true);
	return;
}

exec function SwitchTeam()
{
	// End:0x3D
	if(__NFUN_132__(__NFUN_114__(PlayerReplicationInfo.Team, none), __NFUN_154__(PlayerReplicationInfo.Team.TeamIndex, 1)))
	{
		ChangeTeam(0);		
	}
	else
	{
		ChangeTeam(1);
	}
	return;
}

exec function ChangeTeam(int N)
{
	local TeamInfo OldTeam;

	OldTeam = PlayerReplicationInfo.Team;
	Level.Game.ChangeTeam(self, N, true);
	// End:0x96
	if(__NFUN_130__(Level.Game.bTeamGame, __NFUN_119__(PlayerReplicationInfo.Team, OldTeam)))
	{
		// End:0x96
		if(__NFUN_119__(Pawn, none))
		{
			Pawn.Died(none, Class'Engine.DamageType', Pawn.Location);
		}
	}
	return;
}

exec function SwitchLevel(string URL)
{
	// End:0x49
	if(__NFUN_132__(__NFUN_154__(int(Level.NetMode), int(NM_Standalone)), __NFUN_154__(int(Level.NetMode), int(NM_ListenServer))))
	{
		Level.ServerTravel(URL, false);
	}
	return;
}

exec function ClearProgressMessages()
{
	local int i;

	i = 0;
	J0x07:

	// End:0x52 [Loop If]
	if(__NFUN_150__(i, 4))
	{
		ProgressMessage[i] = "";
		ProgressColor[i] = Class'Engine.Canvas'.static.MakeColor(byte(255), byte(255), byte(255));
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x07;
	}
	return;
}

exec event SetProgressMessage(int Index, string S, Color C)
{
	// End:0x2E
	if(__NFUN_150__(Index, 4))
	{
		ProgressMessage[Index] = S;
		ProgressColor[Index] = C;
	}
	return;
}

exec event SetProgressTime(float t)
{
	ProgressTimeOut = __NFUN_174__(t, Level.TimeSeconds);
	return;
}

function Restart()
{
	super.Restart();
	ServerTimeStamp = 0.0000000;
	TimeMargin = 0.0000000;
	EnterStartState();
	SetViewTarget(Pawn);
	bBehindView = Pawn.PointOfView();
	ClientReStart();
	return;
}

function EnterStartState()
{
	local name NewState;

	// End:0x6A
	if(Pawn.PhysicsVolume.bWaterVolume)
	{
		// End:0x53
		if(Pawn.HeadVolume.bWaterVolume)
		{
			Pawn.BreathTime = Pawn.UnderWaterTime;
		}
		NewState = Pawn.WaterMovementState;		
	}
	else
	{
		NewState = Pawn.LandMovementState;
	}
	// End:0x92
	if(__NFUN_281__(NewState))
	{
		BeginState();		
	}
	else
	{
		__NFUN_113__(NewState);
	}
	return;
}

function ClientReStart()
{
	// End:0x36
	if(__NFUN_130__(__NFUN_119__(Pawn, none), Pawn.bTearOff))
	{
		Pawn.Controller = none;
		Pawn = none;
	}
	// End:0x4A
	if(__NFUN_114__(Pawn, none))
	{
		__NFUN_113__('WaitingForPawn');
		return;
	}
	Pawn.ClientReStart();
	SetViewTarget(Pawn);
	bBehindView = Pawn.PointOfView();
	CleanOutSavedMoves();
	EnterStartState();
	return;
}

exec function BehindView(bool B)
{
	// End:0x32
	if(__NFUN_154__(int(Level.NetMode), int(NM_Standalone)))
	{
		bBehindView = B;
		ClientSetBehindView(bBehindView);
	}
	return;
}

function ChangedWeapon()
{
	// End:0x31
	if(__NFUN_119__(Pawn.PendingWeapon, none))
	{
		Pawn.PendingWeapon.SetHand(Handedness);
	}
	return;
}

event TravelPostAccept()
{
	// End:0x31
	if(__NFUN_152__(Pawn.Health, 0))
	{
		Pawn.Health = Pawn.default.Health;
	}
	return;
}

event PlayerTick(float DeltaTime)
{
	// End:0x49
	if(__NFUN_130__(bForcePrecache, __NFUN_177__(Level.TimeSeconds, ForcePrecacheTime)))
	{
		bForcePrecache = false;
		Level.FillPrecacheMaterialsArray();
		Level.FillPrecacheStaticMeshesArray();
	}
	PlayerInput.PlayerInput(DeltaTime);
	// End:0x6C
	if(bUpdatePosition)
	{
		ClientUpdatePosition();
	}
	return;
}

function PlayerMove(float DeltaTime)
{
	return;
}

function Rotator AdjustAim(Ammunition FiredAmmunition, Vector projStart, int aimerror)
{
	local Vector FireDir, AimSpot, HitNormal, HitLocation, OldAim, AimOffset;

	local Actor BestTarget;
	local float bestAim, bestDist, projSpeed;
	local Actor HitActor;
	local bool bNoZAdjust, bLeading;
	local Rotator AimRot;

	FireDir = Vector(Rotation);
	// End:0x50
	if(FiredAmmunition.bInstantHit)
	{
		HitActor = __NFUN_277__(HitLocation, HitNormal, __NFUN_215__(projStart, __NFUN_213__(float(10000), FireDir)), projStart, true);		
	}
	else
	{
		HitActor = __NFUN_277__(HitLocation, HitNormal, __NFUN_215__(projStart, __NFUN_213__(float(4000), FireDir)), projStart, true);
	}
	// End:0x102
	if(__NFUN_130__(__NFUN_119__(HitActor, none), HitActor.bProjTarget))
	{
		FiredAmmunition.WarnTarget(Target, Pawn, FireDir);
		BestTarget = HitActor;
		bNoZAdjust = true;
		OldAim = HitLocation;
		bestDist = __NFUN_225__(__NFUN_216__(BestTarget.Location, Pawn.Location));		
	}
	else
	{
		bestAim = 0.9500000;
		// End:0x165
		if(__NFUN_180__(AimingHelp, float(1)))
		{
			bestAim = 0.9300000;
			// End:0x142
			if(FiredAmmunition.bInstantHit)
			{
				bestAim = 0.9700000;
			}
			// End:0x162
			if(__NFUN_176__(FovAngle, __NFUN_175__(DefaultFOV, float(8))))
			{
				bestAim = 0.9900000;
			}			
		}
		else
		{
			// End:0x182
			if(FiredAmmunition.bInstantHit)
			{
				bestAim = 0.9800000;
			}
			// End:0x19C
			if(__NFUN_181__(FovAngle, DefaultFOV))
			{
				bestAim = 0.9950000;
			}
		}
		BestTarget = __NFUN_531__(bestAim, bestDist, FireDir, projStart, FiredAmmunition.MaxAutoAimRange);
		// End:0x1F3
		if(__NFUN_114__(BestTarget, none))
		{
			// End:0x1ED
			if(bBehindView)
			{
				return Pawn.Rotation;				
			}
			else
			{
				return Rotation;
			}
		}
		FiredAmmunition.WarnTarget(Target, Pawn, FireDir);
		OldAim = __NFUN_215__(projStart, __NFUN_212__(FireDir, bestDist));
	}
	// End:0x273
	if(__NFUN_132__(__NFUN_180__(AimingHelp, float(0)), __NFUN_155__(int(Level.NetMode), int(NM_Standalone))))
	{
		// End:0x26D
		if(bBehindView)
		{
			return Pawn.Rotation;			
		}
		else
		{
			return Rotation;
		}
	}
	// End:0x428
	if(__NFUN_129__(FiredAmmunition.bInstantHit))
	{
		projSpeed = FiredAmmunition.ProjectileClass.default.Speed;
		bestDist = __NFUN_225__(__NFUN_216__(__NFUN_215__(BestTarget.Location, __NFUN_212__(BestTarget.Velocity, __NFUN_244__(2.0000000, __NFUN_174__(0.0200000, __NFUN_172__(bestDist, projSpeed))))), projStart));
		bLeading = true;
		FireDir = __NFUN_216__(__NFUN_215__(BestTarget.Location, __NFUN_212__(BestTarget.Velocity, __NFUN_244__(2.0000000, __NFUN_174__(0.0200000, __NFUN_172__(bestDist, projSpeed))))), projStart);
		AimSpot = __NFUN_215__(projStart, __NFUN_213__(bestDist, __NFUN_226__(FireDir)));
		// End:0x425
		if(__NFUN_130__(FiredAmmunition.bTrySplash, __NFUN_132__(__NFUN_218__(BestTarget.Velocity, vect(0.0000000, 0.0000000, 0.0000000)), __NFUN_177__(bestDist, float(1500)))))
		{
			HitActor = __NFUN_277__(HitLocation, HitNormal, __NFUN_216__(AimSpot, __NFUN_213__(BestTarget.CollisionHeight, vect(0.0000000, 0.0000000, 2.0000000))), AimSpot, false);
			// End:0x425
			if(__NFUN_130__(__NFUN_119__(HitActor, none), __NFUN_548__(__NFUN_215__(HitLocation, vect(0.0000000, 0.0000000, 4.0000000)), projStart)))
			{
				return Rotator(__NFUN_216__(__NFUN_215__(HitLocation, vect(0.0000000, 0.0000000, 6.0000000)), projStart));
			}
		}		
	}
	else
	{
		FireDir = __NFUN_216__(BestTarget.Location, projStart);
		AimSpot = __NFUN_215__(projStart, __NFUN_213__(bestDist, __NFUN_226__(FireDir)));
	}
	AimOffset = __NFUN_216__(AimSpot, OldAim);
	// End:0x4BD
	if(__NFUN_132__(bNoZAdjust, __NFUN_130__(bLeading, __NFUN_176__(__NFUN_186__(AimOffset.Z), BestTarget.CollisionHeight))))
	{
		AimSpot.Z = OldAim.Z;		
	}
	else
	{
		// End:0x507
		if(__NFUN_176__(AimOffset.Z, float(0)))
		{
			AimSpot.Z = __NFUN_174__(BestTarget.Location.Z, __NFUN_171__(0.4000000, BestTarget.CollisionHeight));			
		}
		else
		{
			AimSpot.Z = __NFUN_175__(BestTarget.Location.Z, __NFUN_171__(0.7000000, BestTarget.CollisionHeight));
		}
	}
	// End:0x5CE
	if(__NFUN_129__(bLeading))
	{
		// End:0x5CB
		if(__NFUN_129__(bNoZAdjust))
		{
			AimRot = Rotator(__NFUN_216__(AimSpot, projStart));
			// End:0x5A0
			if(__NFUN_176__(FovAngle, __NFUN_175__(DefaultFOV, float(8))))
			{
				AimRot.Yaw = __NFUN_147__(__NFUN_146__(AimRot.Yaw, 200), __NFUN_167__(400));				
			}
			else
			{
				AimRot.Yaw = __NFUN_147__(__NFUN_146__(AimRot.Yaw, 375), __NFUN_167__(750));
			}
			return AimRot;
		}		
	}
	else
	{
		// End:0x62D
		if(__NFUN_129__(__NFUN_548__(__NFUN_215__(projStart, __NFUN_213__(__NFUN_171__(0.9000000, bestDist), __NFUN_226__(FireDir))), projStart)))
		{
			FireDir = __NFUN_216__(BestTarget.Location, projStart);
			AimSpot = __NFUN_215__(projStart, __NFUN_213__(bestDist, __NFUN_226__(FireDir)));
		}
	}
	return Rotator(__NFUN_216__(AimSpot, projStart));
	return;
}

function bool NotifyLanded(Vector HitNormal)
{
	return bUpdating;
	return;
}

function AdjustView(float DeltaTime)
{
	// End:0x9F
	if(__NFUN_181__(FovAngle, DesiredFOV))
	{
		// End:0x4F
		if(__NFUN_177__(FovAngle, DesiredFOV))
		{
			FovAngle = __NFUN_175__(FovAngle, __NFUN_245__(7.0000000, __NFUN_171__(__NFUN_171__(0.9000000, DeltaTime), __NFUN_175__(FovAngle, DesiredFOV))));			
		}
		else
		{
			FovAngle = __NFUN_175__(FovAngle, __NFUN_244__(-7.0000000, __NFUN_171__(__NFUN_171__(0.9000000, DeltaTime), __NFUN_175__(FovAngle, DesiredFOV))));
		}
		// End:0x9F
		if(__NFUN_178__(__NFUN_186__(__NFUN_175__(FovAngle, DesiredFOV)), float(10)))
		{
			FovAngle = DesiredFOV;
		}
	}
	// End:0xFA
	if(bZooming)
	{
		__NFUN_184__(ZoomLevel, __NFUN_171__(DeltaTime, 1.0000000));
		// End:0xD5
		if(__NFUN_177__(ZoomLevel, 0.9000000))
		{
			ZoomLevel = 0.9000000;
		}
		DesiredFOV = __NFUN_246__(__NFUN_175__(90.0000000, __NFUN_171__(ZoomLevel, 88.0000000)), 1.0000000, 170.0000000);
	}
	return;
}

function CalcBehindView(out Vector CameraLocation, out Rotator CameraRotation, float dist)
{
	local Vector View, HitLocation, HitNormal;
	local float ViewDist, RealDist;

	CameraRotation = Rotation;
	// End:0x24
	if(bBlockCloseCamera)
	{
		__NFUN_184__(CameraLocation.Z, float(12));
	}
	View = __NFUN_276__(vect(1.0000000, 0.0000000, 0.0000000), CameraRotation);
	RealDist = dist;
	// End:0xA8
	if(__NFUN_119__(__NFUN_277__(HitLocation, HitNormal, __NFUN_216__(CameraLocation, __NFUN_213__(dist, Vector(CameraRotation))), CameraLocation, false, vect(10.0000000, 10.0000000, 10.0000000)), none))
	{
		ViewDist = __NFUN_244__(__NFUN_219__(__NFUN_216__(CameraLocation, HitLocation), View), dist);		
	}
	else
	{
		ViewDist = dist;
	}
	// End:0x12A
	if(__NFUN_132__(__NFUN_132__(__NFUN_129__(bBlockCloseCamera), __NFUN_129__(bValidBehindCamera)), __NFUN_177__(ViewDist, __NFUN_174__(float(10), __NFUN_245__(ViewTarget.CollisionRadius, ViewTarget.CollisionHeight)))))
	{
		bValidBehindCamera = true;
		OldCameraLoc = __NFUN_216__(CameraLocation, __NFUN_213__(ViewDist, View));
		OldCameraRot = CameraRotation;		
	}
	else
	{
		__NFUN_299__(OldCameraRot);
	}
	CameraLocation = OldCameraLoc;
	CameraRotation = OldCameraRot;
	return;
}

function CalcFirstPersonView(out Vector CameraLocation, out Rotator CameraRotation)
{
	CameraRotation = Rotation;
	CameraLocation = __NFUN_215__(__NFUN_215__(CameraLocation, Pawn.EyePosition()), ShakeOffset);
	return;
}

event AddCameraEffect(CameraEffect NewEffect, optional bool RemoveExisting)
{
	// End:0x14
	if(RemoveExisting)
	{
		RemoveCameraEffect(NewEffect);
	}
	CameraEffects.Length = __NFUN_146__(CameraEffects.Length, 1);
	CameraEffects[__NFUN_147__(CameraEffects.Length, 1)] = NewEffect;
	return;
}

event bool RemoveCameraEffect(CameraEffect ExEffect)
{
	local int EffectIndex;

	EffectIndex = 0;
	J0x07:

	// End:0x44 [Loop If]
	if(__NFUN_150__(EffectIndex, CameraEffects.Length))
	{
		// End:0x3A
		if(__NFUN_114__(CameraEffects[EffectIndex], ExEffect))
		{
			CameraEffects.Remove(EffectIndex, 1);
			return true;
		}
		__NFUN_165__(EffectIndex);
		// [Loop Continue]
		goto J0x07;
	}
	return false;
	return;
}

exec function CreateCameraEffect(Class<CameraEffect> EffectClass)
{
	AddCameraEffect(new EffectClass);
	return;
}

function Rotator GetViewRotation()
{
	// End:0x25
	if(__NFUN_130__(bBehindView, __NFUN_119__(Pawn, none)))
	{
		return Pawn.Rotation;
	}
	return Rotation;
	return;
}

event PlayerCalcView(out Actor ViewActor, out Vector CameraLocation, out Rotator CameraRotation)
{
	local Pawn PTarget;

	// End:0x42
	if(__NFUN_130__(__NFUN_119__(Pawn, none), Pawn.bSpecialCalcView))
	{
		// End:0x42
		if(Pawn.SpecialCalcView(ViewActor, CameraLocation, CameraRotation))
		{
			return;
		}
	}
	// End:0xD8
	if(__NFUN_132__(__NFUN_114__(ViewTarget, none), ViewTarget.bDeleteMe))
	{
		// End:0x89
		if(__NFUN_130__(bViewBot, __NFUN_119__(CheatManager, none)))
		{
			CheatManager.ViewBot();			
		}
		else
		{
			// End:0xB8
			if(__NFUN_130__(__NFUN_119__(Pawn, none), __NFUN_129__(Pawn.bDeleteMe)))
			{
				SetViewTarget(Pawn);				
			}
			else
			{
				// End:0xD1
				if(__NFUN_119__(RealViewTarget, none))
				{
					SetViewTarget(RealViewTarget);					
				}
				else
				{
					SetViewTarget(self);
				}
			}
		}
	}
	ViewActor = ViewTarget;
	CameraLocation = ViewTarget.Location;
	// End:0x149
	if(__NFUN_114__(ViewTarget, Pawn))
	{
		// End:0x137
		if(bBehindView)
		{
			CalcBehindView(CameraLocation, CameraRotation, __NFUN_171__(CameraDist, Pawn.default.CollisionRadius));			
		}
		else
		{
			CalcFirstPersonView(CameraLocation, CameraRotation);
		}
		return;
	}
	// End:0x181
	if(__NFUN_114__(ViewTarget, self))
	{
		// End:0x174
		if(bCameraPositionLocked)
		{
			CameraRotation = CheatManager.LockedRotation;			
		}
		else
		{
			CameraRotation = Rotation;
		}
		return;
	}
	// End:0x1D3
	if(__NFUN_130__(ViewTarget.__NFUN_303__('Projectile'), __NFUN_129__(bBehindView)))
	{
		__NFUN_223__(CameraLocation, __NFUN_213__(ViewTarget.CollisionHeight, vect(0.0000000, 0.0000000, 1.0000000)));
		CameraRotation = Rotation;
		return;
	}
	CameraRotation = ViewTarget.Rotation;
	PTarget = Pawn(ViewTarget);
	// End:0x2DC
	if(__NFUN_119__(PTarget, none))
	{
		// End:0x294
		if(__NFUN_154__(int(Level.NetMode), int(NM_Client)))
		{
			// End:0x24C
			if(PTarget.IsPlayerPawn())
			{
				PTarget.SetViewRotation(TargetViewRotation);
				CameraRotation = TargetViewRotation;
			}
			PTarget.EyeHeight = TargetEyeHeight;
			// End:0x291
			if(__NFUN_119__(PTarget.Weapon, none))
			{
				PTarget.Weapon.PlayerViewOffset = TargetWeaponViewOffset;
			}			
		}
		else
		{
			// End:0x2BB
			if(PTarget.IsPlayerPawn())
			{
				CameraRotation = PTarget.GetViewRotation();
			}
		}
		// End:0x2DC
		if(__NFUN_129__(bBehindView))
		{
			__NFUN_223__(CameraLocation, PTarget.EyePosition());
		}
	}
	// End:0x344
	if(bBehindView)
	{
		CameraLocation = __NFUN_215__(CameraLocation, __NFUN_213__(__NFUN_175__(ViewTarget.default.CollisionHeight, ViewTarget.CollisionHeight), vect(0.0000000, 0.0000000, 1.0000000)));
		CalcBehindView(CameraLocation, CameraRotation, __NFUN_171__(CameraDist, ViewTarget.default.CollisionRadius));
	}
	return;
}

function CheckShake(out float MaxOffset, out float offset, out float Rate, out float Time)
{
	// End:0x15
	if(__NFUN_176__(__NFUN_186__(offset), __NFUN_186__(MaxOffset)))
	{
		return;
	}
	offset = MaxOffset;
	// End:0x92
	if(__NFUN_177__(Time, float(1)))
	{
		// End:0x69
		if(__NFUN_178__(__NFUN_171__(Time, __NFUN_186__(__NFUN_172__(MaxOffset, Rate))), float(1)))
		{
			MaxOffset = __NFUN_171__(MaxOffset, __NFUN_175__(__NFUN_172__(float(1), Time), float(1)));			
		}
		else
		{
			__NFUN_182__(MaxOffset, float(-1));
		}
		__NFUN_185__(Time, float(1));
		__NFUN_182__(Rate, float(-1));		
	}
	else
	{
		MaxOffset = 0.0000000;
		offset = 0.0000000;
		Rate = 0.0000000;
	}
	return;
}

function ViewShake(float DeltaTime)
{
	local Rotator ViewRotation;
	local float FRoll;

	// End:0xF8
	if(__NFUN_218__(ShakeOffsetRate, vect(0.0000000, 0.0000000, 0.0000000)))
	{
		__NFUN_184__(ShakeOffset.X, __NFUN_171__(DeltaTime, ShakeOffsetRate.X));
		CheckShake(MaxShakeOffset.X, ShakeOffset.X, ShakeOffsetRate.X, ShakeOffsetTime.X);
		__NFUN_184__(ShakeOffset.Y, __NFUN_171__(DeltaTime, ShakeOffsetRate.Y));
		CheckShake(MaxShakeOffset.Y, ShakeOffset.Y, ShakeOffsetRate.Y, ShakeOffsetTime.Y);
		__NFUN_184__(ShakeOffset.Z, __NFUN_171__(DeltaTime, ShakeOffsetRate.Z));
		CheckShake(MaxShakeOffset.Z, ShakeOffset.Z, ShakeOffsetRate.Z, ShakeOffsetTime.Z);
	}
	ViewRotation = Rotation;
	// End:0x1AB
	if(__NFUN_181__(ShakeRollRate, float(0)))
	{
		ViewRotation.Roll = __NFUN_156__(int(__NFUN_174__(float(__NFUN_156__(ViewRotation.Roll, 65535)), __NFUN_171__(ShakeRollRate, DeltaTime))), 65535);
		// End:0x16A
		if(__NFUN_151__(ViewRotation.Roll, 32768))
		{
			__NFUN_162__(ViewRotation.Roll, 65536);
		}
		FRoll = float(ViewRotation.Roll);
		CheckShake(MaxShakeRoll, FRoll, ShakeRollRate, ShakeRollTime);
		ViewRotation.Roll = int(FRoll);		
	}
	else
	{
		// End:0x1C0
		if(bZeroRoll)
		{
			ViewRotation.Roll = 0;
		}
	}
	__NFUN_299__(ViewRotation);
	return;
}

function bool TurnTowardNearestEnemy()
{
	return;
}

function TurnAround()
{
	// End:0x2F
	if(__NFUN_129__(bSetTurnRot))
	{
		TurnRot180 = Rotation;
		__NFUN_161__(TurnRot180.Yaw, 32768);
		bSetTurnRot = true;
	}
	DesiredRotation = TurnRot180;
	bRotateToDesired = __NFUN_155__(DesiredRotation.Yaw, Rotation.Yaw);
	return;
}

function UpdateRotation(float DeltaTime, float maxPitch)
{
	local Rotator NewRotation, ViewRotation;

	// End:0x37
	if(__NFUN_132__(bInterpolating, __NFUN_130__(__NFUN_119__(Pawn, none), Pawn.bInterpolating)))
	{
		ViewShake(DeltaTime);
		return;
	}
	ViewRotation = Rotation;
	DesiredRotation = ViewRotation;
	// End:0x63
	if(__NFUN_155__(int(bTurnToNearest), 0))
	{
		TurnTowardNearestEnemy();		
	}
	else
	{
		// End:0x79
		if(__NFUN_155__(int(bTurn180), 0))
		{
			TurnAround();			
		}
		else
		{
			TurnTarget = none;
			bRotateToDesired = false;
			bSetTurnRot = false;
			__NFUN_161__(ViewRotation.Yaw, int(__NFUN_171__(__NFUN_171__(32.0000000, DeltaTime), aTurn)));
			__NFUN_161__(ViewRotation.Pitch, int(__NFUN_171__(__NFUN_171__(32.0000000, DeltaTime), aLookUp)));
		}
	}
	ViewRotation.Pitch = __NFUN_156__(ViewRotation.Pitch, 65535);
	// End:0x148
	if(__NFUN_130__(__NFUN_151__(ViewRotation.Pitch, 18000), __NFUN_150__(ViewRotation.Pitch, 49152)))
	{
		// End:0x138
		if(__NFUN_177__(aLookUp, float(0)))
		{
			ViewRotation.Pitch = 18000;			
		}
		else
		{
			ViewRotation.Pitch = 49152;
		}
	}
	__NFUN_299__(ViewRotation);
	ViewShake(DeltaTime);
	ViewFlash(DeltaTime);
	NewRotation = ViewRotation;
	NewRotation.Roll = Rotation.Roll;
	// End:0x1D1
	if(__NFUN_130__(__NFUN_130__(__NFUN_129__(bRotateToDesired), __NFUN_119__(Pawn, none)), __NFUN_132__(__NFUN_129__(bFreeCamera), __NFUN_129__(bBehindView))))
	{
		Pawn.FaceRotation(NewRotation, DeltaTime);
	}
	return;
}

function ClearDoubleClick()
{
	// End:0x1F
	if(__NFUN_119__(PlayerInput, none))
	{
		PlayerInput.DoubleClickTimer = 0.0000000;
	}
	return;
}

function bool IsSpectating()
{
	return false;
	return;
}

function ServerViewNextPlayer()
{
	local Controller C, pick;
	local bool bFound, bRealSpec;

	bRealSpec = PlayerReplicationInfo.bOnlySpectator;
	PlayerReplicationInfo.bOnlySpectator = true;
	C = Level.ControllerList;
	J0x3B:

	// End:0xC2 [Loop If]
	if(__NFUN_119__(C, none))
	{
		// End:0xAB
		if(Level.Game.CanSpectate(self, true, C))
		{
			// End:0x7E
			if(__NFUN_114__(pick, none))
			{
				pick = C;
			}
			// End:0x98
			if(bFound)
			{
				pick = C;
				// [Explicit Break]
				goto J0xC2;				
			}
			else
			{
				bFound = __NFUN_114__(ViewTarget, C);
			}
		}
		C = C.nextController;
		// [Loop Continue]
		goto J0x3B;
	}
	J0xC2:

	SetViewTarget(pick);
	ClientSetViewTarget(pick);
	// End:0xEE
	if(__NFUN_114__(ViewTarget, self))
	{
		bBehindView = false;		
	}
	else
	{
		bBehindView = true;
	}
	PlayerReplicationInfo.bOnlySpectator = bRealSpec;
	return;
}

function ServerViewSelf()
{
	bBehindView = false;
	SetViewTarget(self);
	ClientSetViewTarget(self);
	ClientMessage(OwnCamera, 'Event');
	return;
}

function LoadPlayers()
{
	local int i;

	// End:0x0D
	if(__NFUN_114__(GameReplicationInfo, none))
	{
		return;
	}
	i = 0;
	J0x14:

	// End:0x55 [Loop If]
	if(__NFUN_150__(i, GameReplicationInfo.PRIArray.Length))
	{
		GameReplicationInfo.PRIArray[i].UpdatePrecacheMaterials();
		__NFUN_165__(i);
		// [Loop Continue]
		goto J0x14;
	}
	return;
}

function ChangeStairLook(bool B)
{
	bLookUpStairs = B;
	// End:0x1E
	if(bLookUpStairs)
	{
		bAlwaysMouseLook = false;
	}
	return;
}

function ChangeAlwaysMouseLook(bool B)
{
	bAlwaysMouseLook = B;
	// End:0x1E
	if(bAlwaysMouseLook)
	{
		bLookUpStairs = false;
	}
	return;
}

event ClientOpenMenu(string Menu, optional bool bDisconnect, optional string Msg1, optional string Msg2)
{
	Player.GUIController.OpenMenu(Menu, Msg1, Msg2);
	// End:0x43
	if(bDisconnect)
	{
		ConsoleCommand("Disconnect");
	}
	return;
}

event ClientCloseMenu(optional bool bCloseAll, optional bool bCancel)
{
	// End:0x2A
	if(bCloseAll)
	{
		Player.GUIController.CloseAll(bCancel);		
	}
	else
	{
		Player.GUIController.CloseMenu(bCancel);
	}
	return;
}

function bool CanRestartPlayer()
{
	return __NFUN_129__(PlayerReplicationInfo.bOnlySpectator);
	return;
}

event ServerChangeVoiceChatter(PlayerController Player, int IpAddr, int Handle, bool Add)
{
	// End:0x61
	if(__NFUN_132__(__NFUN_154__(int(Level.NetMode), int(NM_DedicatedServer)), __NFUN_154__(int(Level.NetMode), int(NM_ListenServer))))
	{
		Level.Game.ChangeVoiceChatter(Player, IpAddr, Handle, Add);
	}
	return;
}

event ServerGetVoiceChatters(PlayerController Player)
{
	local int i;

	// End:0xE7
	if(__NFUN_132__(__NFUN_154__(int(Level.NetMode), int(NM_DedicatedServer)), __NFUN_154__(int(Level.NetMode), int(NM_ListenServer))))
	{
		i = 0;
		J0x3B:

		// End:0xE7 [Loop If]
		if(__NFUN_150__(i, Level.Game.VoiceChatters.Length))
		{
			// End:0xDD
			if(__NFUN_119__(Player, Level.Game.VoiceChatters[i].Controller))
			{
				Player.ClientChangeVoiceChatter(Level.Game.VoiceChatters[i].IpAddr, Level.Game.VoiceChatters[i].Handle, true);
			}
			__NFUN_165__(i);
			// [Loop Continue]
			goto J0x3B;
		}
	}
	return;
}

simulated function ClientChangeVoiceChatter(int IpAddr, int Handle, bool Add)
{
	ChangeVoiceChatter(IpAddr, Handle, Add);
	return;
}

simulated function ClientLeaveVoiceChat()
{
	LeaveVoiceChat();
	return;
}

// Export UPlayerController::execLeaveVoiceChat(FFrame&, void* const)
native final function LeaveVoiceChat();

// Export UPlayerController::execChangeVoiceChatter(FFrame&, void* const)
native final function ChangeVoiceChatter(int IpAddr, int Handle, bool Add);

simulated event LostChild(Actor Other)
{
	// End:0x19
	if(__NFUN_114__(SunBeam, Other))
	{
		SunBeam = none;		
	}
	else
	{
		// End:0x2F
		if(__NFUN_114__(FloatingSolid, Other))
		{
			FloatingSolid = none;
		}
	}
	return;
}

simulated event ClearL2Game()
{
	// End:0x21
	if(__NFUN_119__(SunBeam, none))
	{
		SunBeam.NDestroy();
		SunBeam = none;
	}
	// End:0x42
	if(__NFUN_119__(FloatingSolid, none))
	{
		FloatingSolid.NDestroy();
		FloatingSolid = none;
	}
	return;
}

state PlayerWalking
{
	function bool NotifyPhysicsVolumeChange(PhysicsVolume NewVolume)
	{
		// End:0x22
		if(NewVolume.bWaterVolume)
		{
			__NFUN_113__(Pawn.WaterMovementState);
		}
		return false;
		return;
	}

	function ProcessMove(float DeltaTime, Vector NewAccel, Actor.EDoubleClickDir DoubleClickMove, Rotator DeltaRot)
	{
		local Vector OldAccel;
		local bool OldCrouch;

		OldAccel = Pawn.Acceleration;
		Pawn.Acceleration = NewAccel;
		// End:0x68
		if(__NFUN_130__(bDoubleJump, __NFUN_132__(bUpdating, Pawn.CanDoubleJump())))
		{
			Pawn.DoDoubleJump(bUpdating);			
		}
		else
		{
			// End:0x86
			if(bPressedJump)
			{
				Pawn.DoJump(bUpdating);
			}
		}
		// End:0xF7
		if(__NFUN_155__(int(Pawn.Physics), int(2)))
		{
			OldCrouch = Pawn.bWantsToCrouch;
			// End:0xD5
			if(__NFUN_154__(int(bDuck), 0))
			{
				Pawn.ShouldCrouch(false);				
			}
			else
			{
				// End:0xF7
				if(Pawn.bCanCrouch)
				{
					Pawn.ShouldCrouch(true);
				}
			}
		}
		return;
	}

	function PlayerMove(float DeltaTime)
	{
		local Vector X, Y, Z, NewAccel;
		local Actor.EDoubleClickDir DoubleClickMove;
		local Rotator OldRotation, ViewRotation;
		local bool bSaveJump;

		__NFUN_229__(Pawn.Rotation, X, Y, Z);
		NewAccel = __NFUN_215__(__NFUN_213__(aForward, X), __NFUN_213__(aStrafe, Y));
		NewAccel.Z = 0.0000000;
		// End:0x73
		if(__NFUN_176__(__NFUN_225__(NewAccel), 1.0000000))
		{
			NewAccel = vect(0.0000000, 0.0000000, 0.0000000);
		}
		DoubleClickMove = PlayerInput.CheckForDoubleClickMove(DeltaTime);
		GroundPitch = 0;
		ViewRotation = Rotation;
		// End:0x1E4
		if(__NFUN_154__(int(Pawn.Physics), int(1)))
		{
			// End:0x1E1
			if(__NFUN_130__(__NFUN_154__(int(bLook), 0), __NFUN_132__(__NFUN_130__(__NFUN_130__(__NFUN_218__(Pawn.Acceleration, vect(0.0000000, 0.0000000, 0.0000000)), bAlwaysLevel), bSnapToLevel), __NFUN_129__(bKeyboardLook))))
			{
				// End:0x13F
				if(__NFUN_132__(bLookUpStairs, bSnapToLevel))
				{
					GroundPitch = __NFUN_524__(DeltaTime);
					ViewRotation.Pitch = GroundPitch;					
				}
				else
				{
					// End:0x1E1
					if(bCenterView)
					{
						ViewRotation.Pitch = __NFUN_156__(ViewRotation.Pitch, 65535);
						// End:0x189
						if(__NFUN_151__(ViewRotation.Pitch, 32768))
						{
							__NFUN_162__(ViewRotation.Pitch, 65536);
						}
						ViewRotation.Pitch = int(__NFUN_171__(float(ViewRotation.Pitch), __NFUN_175__(float(1), __NFUN_171__(float(12), __NFUN_244__(0.0833000, DeltaTime)))));
						// End:0x1E1
						if(__NFUN_176__(__NFUN_186__(float(ViewRotation.Pitch)), float(1000)))
						{
							ViewRotation.Pitch = 0;
						}
					}
				}
			}			
		}
		else
		{
			// End:0x2A2
			if(__NFUN_130__(__NFUN_130__(__NFUN_129__(bKeyboardLook), __NFUN_154__(int(bLook), 0)), bCenterView))
			{
				ViewRotation.Pitch = __NFUN_156__(ViewRotation.Pitch, 65535);
				// End:0x24A
				if(__NFUN_151__(ViewRotation.Pitch, 32768))
				{
					__NFUN_162__(ViewRotation.Pitch, 65536);
				}
				ViewRotation.Pitch = int(__NFUN_171__(float(ViewRotation.Pitch), __NFUN_175__(float(1), __NFUN_171__(float(12), __NFUN_244__(0.0833000, DeltaTime)))));
				// End:0x2A2
				if(__NFUN_176__(__NFUN_186__(float(ViewRotation.Pitch)), float(1000)))
				{
					ViewRotation.Pitch = 0;
				}
			}
		}
		Pawn.CheckBob(DeltaTime, Y);
		__NFUN_299__(ViewRotation);
		OldRotation = Rotation;
		UpdateRotation(DeltaTime, 1.0000000);
		bDoubleJump = false;
		// End:0x316
		if(__NFUN_130__(bPressedJump, Pawn.CannotJumpNow()))
		{
			bSaveJump = true;
			bPressedJump = false;			
		}
		else
		{
			bSaveJump = false;
		}
		// End:0x353
		if(__NFUN_150__(int(Role), int(ROLE_Authority)))
		{
			ReplicateMove(DeltaTime, NewAccel, DoubleClickMove, __NFUN_317__(OldRotation, Rotation));			
		}
		else
		{
			ProcessMove(DeltaTime, NewAccel, DoubleClickMove, __NFUN_317__(OldRotation, Rotation));
		}
		bPressedJump = bSaveJump;
		return;
	}

	function BeginState()
	{
		DoubleClickDir = 0;
		bPressedJump = false;
		GroundPitch = 0;
		// End:0x97
		if(__NFUN_119__(Pawn, none))
		{
			// End:0x45
			if(__NFUN_114__(Pawn.Mesh, none))
			{
				Pawn.SetMesh();
			}
			Pawn.ShouldCrouch(false);
			// End:0x97
			if(__NFUN_130__(__NFUN_155__(int(Pawn.Physics), int(2)), __NFUN_155__(int(Pawn.Physics), int(13))))
			{
				Pawn.__NFUN_3970__(1);
			}
		}
		return;
	}

	function EndState()
	{
		GroundPitch = 0;
		// End:0x31
		if(__NFUN_130__(__NFUN_119__(Pawn, none), __NFUN_154__(int(bDuck), 0)))
		{
			Pawn.ShouldCrouch(false);
		}
		return;
	}
	stop;
}

state PlayerClimbing
{
	function bool NotifyPhysicsVolumeChange(PhysicsVolume NewVolume)
	{
		// End:0x25
		if(NewVolume.bWaterVolume)
		{
			__NFUN_113__(Pawn.WaterMovementState);			
		}
		else
		{
			__NFUN_113__(Pawn.LandMovementState);
		}
		return false;
		return;
	}

	function ProcessMove(float DeltaTime, Vector NewAccel, Actor.EDoubleClickDir DoubleClickMove, Rotator DeltaRot)
	{
		local Vector OldAccel;

		OldAccel = Pawn.Acceleration;
		Pawn.Acceleration = NewAccel;
		// End:0x66
		if(bPressedJump)
		{
			Pawn.DoJump(bUpdating);
			// End:0x66
			if(__NFUN_154__(int(Pawn.Physics), int(2)))
			{
				__NFUN_113__('PlayerWalking');
			}
		}
		return;
	}

	function PlayerMove(float DeltaTime)
	{
		local Vector X, Y, Z, NewAccel;
		local Actor.EDoubleClickDir DoubleClickMove;
		local Rotator OldRotation, ViewRotation;
		local bool bSaveJump;

		__NFUN_229__(Rotation, X, Y, Z);
		// End:0x51
		if(__NFUN_119__(Pawn.OnLadder, none))
		{
			NewAccel = __NFUN_213__(aForward, Pawn.OnLadder.ClimbDir);			
		}
		else
		{
			NewAccel = __NFUN_215__(__NFUN_213__(aForward, X), __NFUN_213__(aStrafe, Y));
		}
		// End:0x95
		if(__NFUN_176__(__NFUN_225__(NewAccel), 1.0000000))
		{
			NewAccel = vect(0.0000000, 0.0000000, 0.0000000);
		}
		ViewRotation = Rotation;
		__NFUN_299__(ViewRotation);
		OldRotation = Rotation;
		UpdateRotation(DeltaTime, 1.0000000);
		// End:0xF8
		if(__NFUN_150__(int(Role), int(ROLE_Authority)))
		{
			ReplicateMove(DeltaTime, NewAccel, DoubleClickMove, __NFUN_317__(OldRotation, Rotation));			
		}
		else
		{
			ProcessMove(DeltaTime, NewAccel, DoubleClickMove, __NFUN_317__(OldRotation, Rotation));
		}
		bPressedJump = bSaveJump;
		return;
	}

	function BeginState()
	{
		Pawn.ShouldCrouch(false);
		bPressedJump = false;
		return;
	}

	function EndState()
	{
		// End:0x1B
		if(__NFUN_119__(Pawn, none))
		{
			Pawn.ShouldCrouch(false);
		}
		return;
	}
	stop;
}

state PlayerDriving
{
	function ProcessMove(float DeltaTime, Vector NewAccel, Actor.EDoubleClickDir DoubleClickMove, Rotator DeltaRot)
	{
		return;
	}

	exec function Fire(optional float f)
	{
		local KVehicle DrivenVehicle;

		DrivenVehicle = KVehicle(Pawn);
		// End:0x3C
		if(__NFUN_119__(DrivenVehicle, none))
		{
			DrivenVehicle.VehicleFire(false);
			DrivenVehicle.bVehicleIsFiring = true;
		}
		return;
	}

	exec function AltFire(optional float f)
	{
		local KVehicle DrivenVehicle;

		DrivenVehicle = KVehicle(Pawn);
		// End:0x3C
		if(__NFUN_119__(DrivenVehicle, none))
		{
			DrivenVehicle.VehicleFire(true);
			DrivenVehicle.bVehicleIsAltFiring = true;
		}
		return;
	}

	function ProcessDrive(float InForward, float InStrafe, bool InJump)
	{
		local KVehicle DrivenVehicle;

		DrivenVehicle = KVehicle(Pawn);
		// End:0x45
		if(__NFUN_114__(DrivenVehicle, none))
		{
			__NFUN_231__("PlayerDriving.PlayerMove: No Vehicle");
			return;
		}
		// End:0x73
		if(__NFUN_130__(InJump, __NFUN_154__(int(Role), int(ROLE_Authority))))
		{
			DrivenVehicle.bGetOut = true;
			return;
		}
		// End:0x97
		if(__NFUN_177__(InForward, float(1)))
		{
			DrivenVehicle.Throttle = 1.0000000;			
		}
		else
		{
			// End:0xBF
			if(__NFUN_176__(InForward, float(-1)))
			{
				DrivenVehicle.Throttle = -1.0000000;				
			}
			else
			{
				DrivenVehicle.Throttle = 0.0000000;
			}
		}
		// End:0xFB
		if(__NFUN_176__(InStrafe, float(-1)))
		{
			DrivenVehicle.Steering = 1.0000000;			
		}
		else
		{
			// End:0x11F
			if(__NFUN_177__(InStrafe, float(1)))
			{
				DrivenVehicle.Steering = -1.0000000;				
			}
			else
			{
				DrivenVehicle.Steering = 0.0000000;
			}
		}
		return;
	}

	function PlayerMove(float DeltaTime)
	{
		local KVehicle DrivenVehicle;

		// End:0x29
		if(__NFUN_150__(int(Role), int(ROLE_Authority)))
		{
			ServerDrive(aForward, aStrafe, bPressedJump);			
		}
		else
		{
			ProcessDrive(aForward, aStrafe, bPressedJump);
		}
		DrivenVehicle = KVehicle(Pawn);
		// End:0xDE
		if(__NFUN_119__(DrivenVehicle, none))
		{
			// End:0x9C
			if(__NFUN_130__(__NFUN_154__(int(bFire), 0), DrivenVehicle.bVehicleIsFiring))
			{
				DrivenVehicle.VehicleCeaseFire(false);
				DrivenVehicle.bVehicleIsFiring = false;
			}
			// End:0xDE
			if(__NFUN_130__(__NFUN_154__(int(bAltFire), 0), DrivenVehicle.bVehicleIsAltFiring))
			{
				DrivenVehicle.VehicleCeaseFire(true);
				DrivenVehicle.bVehicleIsAltFiring = false;
			}
		}
		UpdateRotation(DeltaTime, 2.0000000);
		return;
	}

	function BeginState()
	{
		CleanOutSavedMoves();
		return;
	}

	function EndState()
	{
		CleanOutSavedMoves();
		return;
	}
	stop;
}

state PlayerSpidering
{
	event bool NotifyHitWall(Vector HitNormal, Actor HitActor)
	{
		Pawn.__NFUN_3970__(9);
		Pawn.__NFUN_298__(HitActor, HitNormal);
		return true;
		return;
	}

	function UpdateRotation(float DeltaTime, float maxPitch)
	{
		local Rotator ViewRotation;
		local Vector MyFloor, CrossDir, FwdDir, OldFwdDir, OldX, RealFloor;

		// End:0x2A
		if(__NFUN_132__(bInterpolating, Pawn.bInterpolating))
		{
			ViewShake(DeltaTime);
			return;
		}
		TurnTarget = none;
		bRotateToDesired = false;
		bSetTurnRot = false;
		// End:0x8D
		if(__NFUN_132__(__NFUN_114__(Pawn.Base, none), __NFUN_217__(Pawn.Floor, vect(0.0000000, 0.0000000, 0.0000000))))
		{
			MyFloor = vect(0.0000000, 0.0000000, 1.0000000);			
		}
		else
		{
			MyFloor = Pawn.Floor;
		}
		// End:0x206
		if(__NFUN_218__(MyFloor, OldFloor))
		{
			RealFloor = MyFloor;
			MyFloor = __NFUN_226__(__NFUN_215__(__NFUN_213__(__NFUN_171__(float(6), DeltaTime), MyFloor), __NFUN_213__(__NFUN_175__(float(1), __NFUN_171__(float(6), DeltaTime)), OldFloor)));
			// End:0x10F
			if(__NFUN_177__(__NFUN_219__(RealFloor, MyFloor), 0.9990000))
			{
				MyFloor = RealFloor;
			}
			CrossDir = __NFUN_226__(__NFUN_220__(RealFloor, OldFloor));
			FwdDir = __NFUN_220__(CrossDir, MyFloor);
			OldFwdDir = __NFUN_220__(CrossDir, OldFloor);
			ViewX = __NFUN_215__(__NFUN_215__(__NFUN_212__(MyFloor, __NFUN_219__(OldFloor, ViewX)), __NFUN_212__(CrossDir, __NFUN_219__(CrossDir, ViewX))), __NFUN_212__(FwdDir, __NFUN_219__(OldFwdDir, ViewX)));
			ViewX = __NFUN_226__(ViewX);
			ViewZ = __NFUN_215__(__NFUN_215__(__NFUN_212__(MyFloor, __NFUN_219__(OldFloor, ViewZ)), __NFUN_212__(CrossDir, __NFUN_219__(CrossDir, ViewZ))), __NFUN_212__(FwdDir, __NFUN_219__(OldFwdDir, ViewZ)));
			ViewZ = __NFUN_226__(ViewZ);
			OldFloor = MyFloor;
			ViewY = __NFUN_226__(__NFUN_220__(MyFloor, ViewX));
		}
		// End:0x35C
		if(__NFUN_132__(__NFUN_181__(aTurn, float(0)), __NFUN_181__(aLookUp, float(0))))
		{
			// End:0x260
			if(__NFUN_181__(aTurn, float(0)))
			{
				ViewX = __NFUN_226__(__NFUN_215__(ViewX, __NFUN_212__(__NFUN_213__(float(2), ViewY), __NFUN_187__(__NFUN_171__(__NFUN_171__(0.0005000, DeltaTime), aTurn)))));
			}
			// End:0x348
			if(__NFUN_181__(aLookUp, float(0)))
			{
				OldX = ViewX;
				ViewX = __NFUN_226__(__NFUN_215__(ViewX, __NFUN_212__(__NFUN_213__(float(2), ViewZ), __NFUN_187__(__NFUN_171__(__NFUN_171__(0.0005000, DeltaTime), aLookUp)))));
				ViewZ = __NFUN_226__(__NFUN_220__(ViewX, ViewY));
				// End:0x348
				if(__NFUN_176__(__NFUN_219__(ViewZ, MyFloor), 0.7070000))
				{
					OldX = __NFUN_226__(__NFUN_216__(OldX, __NFUN_212__(MyFloor, __NFUN_219__(MyFloor, OldX))));
					// End:0x320
					if(__NFUN_177__(__NFUN_219__(ViewX, MyFloor), float(0)))
					{
						ViewX = __NFUN_226__(__NFUN_215__(OldX, MyFloor));						
					}
					else
					{
						ViewX = __NFUN_226__(__NFUN_216__(OldX, MyFloor));
					}
					ViewZ = __NFUN_226__(__NFUN_220__(ViewX, ViewY));
				}
			}
			ViewY = __NFUN_226__(__NFUN_220__(MyFloor, ViewX));
		}
		ViewRotation = OrthoRotation(ViewX, ViewY, ViewZ);
		__NFUN_299__(ViewRotation);
		ViewShake(DeltaTime);
		ViewFlash(DeltaTime);
		Pawn.FaceRotation(ViewRotation, DeltaTime);
		return;
	}

	function bool NotifyLanded(Vector HitNormal)
	{
		Pawn.__NFUN_3970__(9);
		return bUpdating;
		return;
	}

	function bool NotifyPhysicsVolumeChange(PhysicsVolume NewVolume)
	{
		// End:0x22
		if(NewVolume.bWaterVolume)
		{
			__NFUN_113__(Pawn.WaterMovementState);
		}
		return false;
		return;
	}

	function ProcessMove(float DeltaTime, Vector NewAccel, Actor.EDoubleClickDir DoubleClickMove, Rotator DeltaRot)
	{
		local Vector OldAccel;

		OldAccel = Pawn.Acceleration;
		Pawn.Acceleration = NewAccel;
		// End:0x46
		if(bPressedJump)
		{
			Pawn.DoJump(bUpdating);
		}
		return;
	}

	function PlayerMove(float DeltaTime)
	{
		local Vector NewAccel;
		local Actor.EDoubleClickDir DoubleClickMove;
		local Rotator OldRotation, ViewRotation;
		local bool bSaveJump;

		GroundPitch = 0;
		ViewRotation = Rotation;
		// End:0x37
		if(__NFUN_130__(__NFUN_130__(__NFUN_129__(bKeyboardLook), __NFUN_154__(int(bLook), 0)), bCenterView))
		{
		}
		Pawn.CheckBob(DeltaTime, vect(0.0000000, 0.0000000, 0.0000000));
		__NFUN_299__(ViewRotation);
		OldRotation = Rotation;
		UpdateRotation(DeltaTime, 1.0000000);
		NewAccel = __NFUN_215__(__NFUN_213__(aForward, __NFUN_226__(__NFUN_216__(ViewX, __NFUN_212__(OldFloor, __NFUN_219__(OldFloor, ViewX))))), __NFUN_213__(aStrafe, ViewY));
		// End:0xD6
		if(__NFUN_176__(__NFUN_225__(NewAccel), 1.0000000))
		{
			NewAccel = vect(0.0000000, 0.0000000, 0.0000000);
		}
		// End:0x106
		if(__NFUN_130__(bPressedJump, Pawn.CannotJumpNow()))
		{
			bSaveJump = true;
			bPressedJump = false;			
		}
		else
		{
			bSaveJump = false;
		}
		// End:0x143
		if(__NFUN_150__(int(Role), int(ROLE_Authority)))
		{
			ReplicateMove(DeltaTime, NewAccel, DoubleClickMove, __NFUN_317__(OldRotation, Rotation));			
		}
		else
		{
			ProcessMove(DeltaTime, NewAccel, DoubleClickMove, __NFUN_317__(OldRotation, Rotation));
		}
		bPressedJump = bSaveJump;
		return;
	}

	function BeginState()
	{
		// End:0x23
		if(__NFUN_114__(Pawn.Mesh, none))
		{
			Pawn.SetMesh();
		}
		OldFloor = vect(0.0000000, 0.0000000, 1.0000000);
		__NFUN_229__(Rotation, ViewX, ViewY, ViewZ);
		DoubleClickDir = 0;
		Pawn.ShouldCrouch(false);
		bPressedJump = false;
		// End:0x93
		if(__NFUN_155__(int(Pawn.Physics), int(2)))
		{
			Pawn.__NFUN_3970__(9);
		}
		GroundPitch = 0;
		Pawn.bCrawler = true;
		Pawn.__NFUN_283__(Pawn.default.CollisionHeight, Pawn.default.CollisionHeight);
		return;
	}

	function EndState()
	{
		GroundPitch = 0;
		// End:0x69
		if(__NFUN_119__(Pawn, none))
		{
			Pawn.__NFUN_283__(Pawn.default.CollisionRadius, Pawn.default.CollisionHeight);
			Pawn.ShouldCrouch(false);
			Pawn.bCrawler = Pawn.default.bCrawler;
		}
		return;
	}
	stop;
}

state PlayerSwimming
{
	function bool WantsSmoothedView()
	{
		return __NFUN_129__(Pawn.bJustLanded);
		return;
	}

	function bool NotifyLanded(Vector HitNormal)
	{
		// End:0x2C
		if(Pawn.PhysicsVolume.bWaterVolume)
		{
			Pawn.__NFUN_3970__(3);			
		}
		else
		{
			__NFUN_113__(Pawn.LandMovementState);
		}
		return bUpdating;
		return;
	}

	function bool NotifyPhysicsVolumeChange(PhysicsVolume NewVolume)
	{
		local Actor HitActor;
		local Vector HitLocation, HitNormal, checkpoint;

		// End:0x18C
		if(__NFUN_129__(NewVolume.bWaterVolume))
		{
			Pawn.__NFUN_3970__(2);
			// End:0x189
			if(__NFUN_177__(Pawn.Velocity.Z, float(0)))
			{
				// End:0xBA
				if(__NFUN_130__(Pawn.bUpAndOut, Pawn.CheckWaterJump(HitNormal)))
				{
					Pawn.Velocity.Z = __NFUN_174__(__NFUN_245__(Pawn.JumpZ, 420.0000000), __NFUN_171__(float(2), Pawn.CollisionRadius));
					__NFUN_113__(Pawn.LandMovementState);					
				}
				else
				{
					// End:0xFF
					if(__NFUN_132__(__NFUN_177__(Pawn.Velocity.Z, float(160)), __NFUN_129__(Pawn.TouchingWaterVolume())))
					{
						__NFUN_113__(Pawn.LandMovementState);						
					}
					else
					{
						checkpoint = Pawn.Location;
						__NFUN_185__(checkpoint.Z, __NFUN_174__(Pawn.CollisionHeight, 6.0000000));
						HitActor = __NFUN_277__(HitLocation, HitNormal, checkpoint, Pawn.Location, false);
						// End:0x179
						if(__NFUN_119__(HitActor, none))
						{
							__NFUN_113__(Pawn.LandMovementState);							
						}
						else
						{
							__NFUN_117__('Timer');
							__NFUN_280__(0.7000000, false);
						}
					}
				}
			}			
		}
		else
		{
			__NFUN_118__('Timer');
			Pawn.__NFUN_3970__(3);
		}
		return false;
		return;
	}

	function ProcessMove(float DeltaTime, Vector NewAccel, Actor.EDoubleClickDir DoubleClickMove, Rotator DeltaRot)
	{
		local Vector X, Y, Z, OldAccel;

		__NFUN_229__(Rotation, X, Y, Z);
		OldAccel = Pawn.Acceleration;
		Pawn.Acceleration = NewAccel;
		Pawn.bUpAndOut = __NFUN_130__(__NFUN_177__(__NFUN_219__(X, Pawn.Acceleration), float(0)), __NFUN_132__(__NFUN_177__(Pawn.Acceleration.Z, float(0)), __NFUN_151__(Rotation.Pitch, 2048)));
		// End:0xCC
		if(__NFUN_129__(Pawn.PhysicsVolume.bWaterVolume))
		{
			NotifyPhysicsVolumeChange(Pawn.PhysicsVolume);
		}
		return;
	}

	function PlayerMove(float DeltaTime)
	{
		local Rotator OldRotation;
		local Vector X, Y, Z, NewAccel;

		__NFUN_229__(Rotation, X, Y, Z);
		NewAccel = __NFUN_215__(__NFUN_215__(__NFUN_213__(aForward, X), __NFUN_213__(aStrafe, Y)), __NFUN_213__(aUp, vect(0.0000000, 0.0000000, 1.0000000)));
		// End:0x70
		if(__NFUN_176__(__NFUN_225__(NewAccel), 1.0000000))
		{
			NewAccel = vect(0.0000000, 0.0000000, 0.0000000);
		}
		Pawn.CheckBob(DeltaTime, Y);
		OldRotation = Rotation;
		UpdateRotation(DeltaTime, 2.0000000);
		// End:0xD6
		if(__NFUN_150__(int(Role), int(ROLE_Authority)))
		{
			ReplicateMove(DeltaTime, NewAccel, 0, __NFUN_317__(OldRotation, Rotation));			
		}
		else
		{
			ProcessMove(DeltaTime, NewAccel, 0, __NFUN_317__(OldRotation, Rotation));
		}
		bPressedJump = false;
		return;
	}

	function Timer()
	{
		// End:0x3F
		if(__NFUN_130__(__NFUN_129__(Pawn.PhysicsVolume.bWaterVolume), __NFUN_154__(int(Role), int(ROLE_Authority))))
		{
			__NFUN_113__(Pawn.LandMovementState);
		}
		__NFUN_118__('Timer');
		return;
	}

	function BeginState()
	{
		__NFUN_118__('Timer');
		Pawn.__NFUN_3970__(3);
		return;
	}
	stop;
}

state PlayerFlying
{
	function PlayerMove(float DeltaTime)
	{
		local Vector X, Y, Z;

		__NFUN_229__(Rotation, X, Y, Z);
		Pawn.Acceleration = __NFUN_215__(__NFUN_213__(aForward, X), __NFUN_213__(aStrafe, Y));
		// End:0x75
		if(__NFUN_176__(__NFUN_225__(Pawn.Acceleration), 1.0000000))
		{
			Pawn.Acceleration = vect(0.0000000, 0.0000000, 0.0000000);
		}
		// End:0xBC
		if(__NFUN_130__(bCheatFlying, __NFUN_217__(Pawn.Acceleration, vect(0.0000000, 0.0000000, 0.0000000))))
		{
			Pawn.Velocity = vect(0.0000000, 0.0000000, 0.0000000);
		}
		UpdateRotation(DeltaTime, 2.0000000);
		// End:0x107
		if(__NFUN_150__(int(Role), int(ROLE_Authority)))
		{
			ReplicateMove(DeltaTime, Pawn.Acceleration, 0, rot(0, 0, 0));			
		}
		else
		{
			ProcessMove(DeltaTime, Pawn.Acceleration, 0, rot(0, 0, 0));
		}
		return;
	}

	function BeginState()
	{
		Pawn.__NFUN_3970__(4);
		return;
	}
	stop;
}

state PlayerRocketing
{
	function ServerMove(float TimeStamp, Vector InAccel, Vector ClientLoc, bool NewbRun, bool NewbDuck, bool NewbJumpStatus, bool NewbDoubleJump, Actor.EDoubleClickDir DoubleClickMove, byte ClientRoll, int View, optional byte OldTimeDelta, optional int OldAccel)
	{
		// End:0x27
		if(__NFUN_176__(CurrentTimeStamp, TimeStamp))
		{
			Pawn.__NFUN_3971__(__NFUN_175__(TimeStamp, CurrentTimeStamp));
		}
		CurrentTimeStamp = TimeStamp;
		ServerTimeStamp = Level.TimeSeconds;
		return;
	}

	function RocketServerMove(float TimeStamp, Vector InAccel, Vector ClientLoc, byte ClientRoll, int View)
	{
		global.ServerMove(TimeStamp, InAccel, ClientLoc, false, false, false, false, 0, ClientRoll, View);
		return;
	}

	function PlayerMove(float DeltaTime)
	{
		Pawn.UpdateRocketAcceleration(DeltaTime, aTurn, aLookUp);
		__NFUN_299__(Pawn.Rotation);
		// End:0x6A
		if(__NFUN_150__(int(Role), int(ROLE_Authority)))
		{
			ReplicateMove(DeltaTime, Pawn.Acceleration, 0, rot(0, 0, 0));			
		}
		else
		{
			ProcessMove(DeltaTime, Pawn.Acceleration, 0, rot(0, 0, 0));
		}
		return;
	}

	function BeginState()
	{
		Pawn.__NFUN_3970__(4);
		return;
	}
	stop;
}

state BaseSpectating
{
	function bool IsSpectating()
	{
		return true;
		return;
	}

	function ProcessMove(float DeltaTime, Vector NewAccel, Actor.EDoubleClickDir DoubleClickMove, Rotator DeltaRot)
	{
		Acceleration = NewAccel;
		__NFUN_3969__(__NFUN_212__(__NFUN_213__(__NFUN_171__(float(2), Class'Engine.Pawn'.default.AirSpeed), __NFUN_226__(Acceleration)), DeltaTime));
		return;
	}

	function PlayerMove(float DeltaTime)
	{
		local Vector X, Y, Z;

		__NFUN_229__(Rotation, X, Y, Z);
		Acceleration = __NFUN_213__(0.0200000, __NFUN_215__(__NFUN_215__(__NFUN_213__(aForward, X), __NFUN_213__(aStrafe, Y)), __NFUN_213__(aUp, vect(0.0000000, 0.0000000, 1.0000000))));
		UpdateRotation(DeltaTime, 1.0000000);
		// End:0x95
		if(__NFUN_150__(int(Role), int(ROLE_Authority)))
		{
			ReplicateMove(DeltaTime, Acceleration, 0, rot(0, 0, 0));			
		}
		else
		{
			ProcessMove(DeltaTime, Acceleration, 0, rot(0, 0, 0));
		}
		return;
	}
	stop;
}

state Scripting
{
	exec function Fire(optional float f)
	{
		return;
	}

	exec function AltFire(optional float f)
	{
		Fire(f);
		return;
	}
	stop;
}

state Spectating extends BaseSpectating
{
	ignores ThrowWeapon, Suicide, ClientReStart, RestartLevel, SwitchWeapon;

	exec function Fire(optional float f)
	{
		bBehindView = true;
		ServerViewNextPlayer();
		return;
	}

	exec function AltFire(optional float f)
	{
		bBehindView = false;
		ServerViewSelf();
		return;
	}

	function BeginState()
	{
		// End:0x22
		if(__NFUN_119__(Pawn, none))
		{
			__NFUN_267__(Pawn.Location);
			UnPossess();
		}
		bCollideWorld = true;
		return;
	}

	function EndState()
	{
		// End:0x1C
		if(__NFUN_119__(none, PlayerReplicationInfo))
		{
			PlayerReplicationInfo.bIsSpectator = false;
		}
		bCollideWorld = false;
		return;
	}
	stop;
}

auto state PlayerWaiting extends BaseSpectating
{
	ignores SwitchToBestWeapon, PrevWeapon, NextWeapon, TakeDamage;

	exec function Jump(optional float f)
	{
		return;
	}

	exec function Suicide()
	{
		return;
	}

	function ChangeTeam(int N)
	{
		Level.Game.ChangeTeam(self, N, true);
		return;
	}

	function ServerReStartPlayer()
	{
		// End:0x1A
		if(__NFUN_176__(Level.TimeSeconds, WaitDelay))
		{
			return;
		}
		// End:0x35
		if(__NFUN_154__(int(Level.NetMode), int(NM_Client)))
		{
			return;
		}
		// End:0x64
		if(Level.Game.bWaitingToStartMatch)
		{
			PlayerReplicationInfo.bReadyToPlay = true;			
		}
		else
		{
			Level.Game.RestartPlayer(self);
		}
		return;
	}

	exec function Fire(optional float f)
	{
		LoadPlayers();
		ServerReStartPlayer();
		return;
	}

	exec function AltFire(optional float f)
	{
		Fire(f);
		return;
	}

	function EndState()
	{
		// End:0x1A
		if(__NFUN_119__(Pawn, none))
		{
			Pawn.SetMesh();
		}
		// End:0x35
		if(__NFUN_119__(PlayerReplicationInfo, none))
		{
			PlayerReplicationInfo.SetWaitingPlayer(false);
		}
		bCollideWorld = false;
		return;
	}

	function BeginState()
	{
		// End:0x1B
		if(__NFUN_119__(PlayerReplicationInfo, none))
		{
			PlayerReplicationInfo.SetWaitingPlayer(true);
		}
		bCollideWorld = true;
		return;
	}
	stop;
}

state WaitingForPawn extends BaseSpectating
{
	ignores SwitchWeapon, KilledBy;

	exec function Fire(optional float f)
	{
		AskForPawn();
		return;
	}

	exec function AltFire(optional float f)
	{
		return;
	}

	function LongClientAdjustPosition(float TimeStamp, name NewState, Actor.EPhysics newPhysics, float NewLocX, float NewLocY, float NewLocZ, float NewVelX, float NewVelY, float NewVelZ, Actor NewBase, float NewFloorX, float NewFloorY, float NewFloorZ)
	{
		// End:0x16
		if(__NFUN_254__(NewState, 'GameEnded'))
		{
			__NFUN_113__(NewState);
		}
		return;
	}

	function PlayerTick(float DeltaTime)
	{
		global.PlayerTick(DeltaTime);
		// End:0x40
		if(__NFUN_119__(Pawn, none))
		{
			Pawn.Controller = self;
			Pawn.bUpdateEyeheight = true;
			ClientReStart();			
		}
		else
		{
			// End:0x6F
			if(__NFUN_132__(__NFUN_178__(TimerRate, 0.0000000), __NFUN_177__(TimerRate, 1.0000000)))
			{
				__NFUN_280__(0.2000000, true);
				AskForPawn();
			}
		}
		return;
	}

	function Timer()
	{
		AskForPawn();
		return;
	}

	function BeginState()
	{
		__NFUN_280__(0.2000000, true);
		AskForPawn();
		return;
	}

	function EndState()
	{
		bBehindView = false;
		__NFUN_280__(0.0000000, false);
		return;
	}
	stop;
}

state GameEnded
{
	ignores Suicide, TakeDamage, KilledBy;

	function ServerReStartPlayer()
	{
		return;
	}

	function bool IsSpectating()
	{
		return true;
		return;
	}

	exec function ThrowWeapon()
	{
		return;
	}

	function ServerRestartGame()
	{
		Level.Game.RestartGame();
		return;
	}

	exec function Fire(optional float f)
	{
		// End:0x12
		if(__NFUN_150__(int(Role), int(ROLE_Authority)))
		{
			return;
		}
		// End:0x26
		if(__NFUN_129__(bFrozen))
		{
			ServerRestartGame();			
		}
		else
		{
			// End:0x3C
			if(__NFUN_178__(TimerRate, float(0)))
			{
				__NFUN_280__(1.5000000, false);
			}
		}
		return;
	}

	exec function AltFire(optional float f)
	{
		Fire(f);
		return;
	}

	function PlayerMove(float DeltaTime)
	{
		local Vector X, Y, Z;
		local Rotator ViewRotation;

		__NFUN_229__(Rotation, X, Y, Z);
		// End:0xEF
		if(__NFUN_129__(bFixedCamera))
		{
			ViewRotation = Rotation;
			__NFUN_161__(ViewRotation.Yaw, int(__NFUN_171__(__NFUN_171__(32.0000000, DeltaTime), aTurn)));
			__NFUN_161__(ViewRotation.Pitch, int(__NFUN_171__(__NFUN_171__(32.0000000, DeltaTime), aLookUp)));
			ViewRotation.Pitch = __NFUN_156__(ViewRotation.Pitch, 65535);
			// End:0xE4
			if(__NFUN_130__(__NFUN_151__(ViewRotation.Pitch, 18000), __NFUN_150__(ViewRotation.Pitch, 49152)))
			{
				// End:0xD4
				if(__NFUN_177__(aLookUp, float(0)))
				{
					ViewRotation.Pitch = 18000;					
				}
				else
				{
					ViewRotation.Pitch = 49152;
				}
			}
			__NFUN_299__(ViewRotation);			
		}
		else
		{
			// End:0x10B
			if(__NFUN_119__(ViewTarget, none))
			{
				__NFUN_299__(ViewTarget.Rotation);
			}
		}
		ViewShake(DeltaTime);
		ViewFlash(DeltaTime);
		// End:0x15B
		if(__NFUN_150__(int(Role), int(ROLE_Authority)))
		{
			ReplicateMove(DeltaTime, vect(0.0000000, 0.0000000, 0.0000000), 0, rot(0, 0, 0));			
		}
		else
		{
			ProcessMove(DeltaTime, vect(0.0000000, 0.0000000, 0.0000000), 0, rot(0, 0, 0));
		}
		bPressedJump = false;
		return;
	}

	function ServerMove(float TimeStamp, Vector InAccel, Vector ClientLoc, bool NewbRun, bool NewbDuck, bool NewbJumpStatus, bool NewbDoubleJump, Actor.EDoubleClickDir DoubleClickMove, byte ClientRoll, int View, optional byte OldTimeDelta, optional int OldAccel)
	{
		global.ServerMove(TimeStamp, InAccel, ClientLoc, NewbRun, NewbDuck, NewbJumpStatus, NewbDoubleJump, DoubleClickMove, ClientRoll, __NFUN_146__(__NFUN_144__(__NFUN_156__(32767, __NFUN_145__(Rotation.Pitch, 2)), 32768), __NFUN_156__(32767, __NFUN_145__(Rotation.Yaw, 2))));
		return;
	}

	function FindGoodView()
	{
		local Vector cameraLoc;
		local Rotator cameraRot, ViewRotation;
		local int tries, besttry;
		local float bestDist, newdist;
		local int startYaw;
		local Actor ViewActor;

		ViewRotation = Rotation;
		ViewRotation.Pitch = 56000;
		tries = 0;
		besttry = 0;
		bestDist = 0.0000000;
		startYaw = ViewRotation.Yaw;
		tries = 0;
		J0x4B:

		// End:0xE5 [Loop If]
		if(__NFUN_150__(tries, 16))
		{
			cameraLoc = ViewTarget.Location;
			__NFUN_299__(ViewRotation);
			PlayerCalcView(ViewActor, cameraLoc, cameraRot);
			newdist = __NFUN_225__(__NFUN_216__(cameraLoc, ViewTarget.Location));
			// End:0xCA
			if(__NFUN_177__(newdist, bestDist))
			{
				bestDist = newdist;
				besttry = tries;
			}
			__NFUN_161__(ViewRotation.Yaw, 4096);
			__NFUN_165__(tries);
			// [Loop Continue]
			goto J0x4B;
		}
		ViewRotation.Yaw = __NFUN_146__(startYaw, __NFUN_144__(besttry, 4096));
		__NFUN_299__(ViewRotation);
		return;
	}

	function Timer()
	{
		bFrozen = false;
		return;
	}

	function LongClientAdjustPosition(float TimeStamp, name NewState, Actor.EPhysics newPhysics, float NewLocX, float NewLocY, float NewLocZ, float NewVelX, float NewVelY, float NewVelZ, Actor NewBase, float NewFloorX, float NewFloorY, float NewFloorZ)
	{
		return;
	}

	function BeginState()
	{
		local Pawn P;

		EndZoom();
		FovAngle = DesiredFOV;
		bFire = 0;
		bAltFire = 0;
		// End:0xD4
		if(__NFUN_119__(Pawn, none))
		{
			Pawn.Velocity = vect(0.0000000, 0.0000000, 0.0000000);
			Pawn.__NFUN_3970__(0);
			Pawn.bSpecialHUD = false;
			Pawn.bNoWeaponFiring = true;
			Pawn.SimAnim.AnimRate = 0;
			Pawn.bPhysicsAnimUpdate = false;
			Pawn.StopAnimating();
			Pawn.__NFUN_262__(true, false, false);
			StopFiring();
			Pawn.bIgnoreForces = true;
		}
		myHUD.bShowScores = true;
		bFrozen = true;
		// End:0x106
		if(__NFUN_129__(bFixedCamera))
		{
			FindGoodView();
			bBehindView = true;
		}
		__NFUN_280__(5.0000000, false);
		// End:0x1C5
		foreach __NFUN_313__(Class'Engine.Pawn', P)
		{
			// End:0x149
			if(__NFUN_154__(int(P.Role), int(ROLE_Authority)))
			{
				P.RemoteRole = ROLE_DumbProxy;
			}
			P.__NFUN_262__(true, false, false);
			P.bNoWeaponFiring = true;
			P.Velocity = vect(0.0000000, 0.0000000, 0.0000000);
			P.__NFUN_3970__(0);
			P.bPhysicsAnimUpdate = false;
			P.StopAnimating();
			P.bIgnoreForces = true;			
		}		
		return;
	}
Begin:

	stop;			
}

state Dead
{
	ignores PrevWeapon, NextWeapon, SwitchWeapon, KilledBy;

	function bool IsDead()
	{
		return true;
		return;
	}

	function ServerReStartPlayer()
	{
		super.ServerReStartPlayer();
		return;
	}

	exec function Fire(optional float f)
	{
		// End:0x33
		if(bFrozen)
		{
			// End:0x31
			if(__NFUN_132__(__NFUN_178__(TimerRate, 0.0000000), __NFUN_177__(TimerRate, 1.0000000)))
			{
				bFrozen = false;
			}
			return;
		}
		LoadPlayers();
		ServerReStartPlayer();
		return;
	}

	exec function AltFire(optional float f)
	{
		Fire(f);
		return;
	}

	function ServerMove(float TimeStamp, Vector Accel, Vector ClientLoc, bool NewbRun, bool NewbDuck, bool NewbJumpStatus, bool NewbDoubleJump, Actor.EDoubleClickDir DoubleClickMove, byte ClientRoll, int View, optional byte OldTimeDelta, optional int OldAccel)
	{
		global.ServerMove(TimeStamp, Accel, ClientLoc, false, false, false, false, DoubleClickMove, ClientRoll, View);
		return;
	}

	function PlayerMove(float DeltaTime)
	{
		local Vector X, Y, Z;
		local Rotator ViewRotation;

		// End:0x142
		if(__NFUN_129__(bFrozen))
		{
			// End:0x27
			if(bPressedJump)
			{
				Fire(0.0000000);
				bPressedJump = false;
			}
			__NFUN_229__(Rotation, X, Y, Z);
			ViewRotation = Rotation;
			__NFUN_161__(ViewRotation.Yaw, int(__NFUN_171__(__NFUN_171__(32.0000000, DeltaTime), aTurn)));
			__NFUN_161__(ViewRotation.Pitch, int(__NFUN_171__(__NFUN_171__(32.0000000, DeltaTime), aLookUp)));
			ViewRotation.Pitch = __NFUN_156__(ViewRotation.Pitch, 65535);
			// End:0x100
			if(__NFUN_130__(__NFUN_151__(ViewRotation.Pitch, 18000), __NFUN_150__(ViewRotation.Pitch, 49152)))
			{
				// End:0xF0
				if(__NFUN_177__(aLookUp, float(0)))
				{
					ViewRotation.Pitch = 18000;					
				}
				else
				{
					ViewRotation.Pitch = 49152;
				}
			}
			__NFUN_299__(ViewRotation);
			// End:0x13F
			if(__NFUN_150__(int(Role), int(ROLE_Authority)))
			{
				ReplicateMove(DeltaTime, vect(0.0000000, 0.0000000, 0.0000000), 0, rot(0, 0, 0));
			}			
		}
		else
		{
			// End:0x16A
			if(__NFUN_132__(__NFUN_178__(TimerRate, 0.0000000), __NFUN_177__(TimerRate, 1.0000000)))
			{
				bFrozen = false;
			}
		}
		ViewShake(DeltaTime);
		ViewFlash(DeltaTime);
		return;
	}

	function FindGoodView()
	{
		local Vector cameraLoc;
		local Rotator cameraRot, ViewRotation;
		local int tries, besttry;
		local float bestDist, newdist;
		local int startYaw;
		local Actor ViewActor;

		ViewRotation = Rotation;
		ViewRotation.Pitch = 56000;
		tries = 0;
		besttry = 0;
		bestDist = 0.0000000;
		startYaw = ViewRotation.Yaw;
		tries = 0;
		J0x4B:

		// End:0xE5 [Loop If]
		if(__NFUN_150__(tries, 16))
		{
			cameraLoc = ViewTarget.Location;
			__NFUN_299__(ViewRotation);
			PlayerCalcView(ViewActor, cameraLoc, cameraRot);
			newdist = __NFUN_225__(__NFUN_216__(cameraLoc, ViewTarget.Location));
			// End:0xCA
			if(__NFUN_177__(newdist, bestDist))
			{
				bestDist = newdist;
				besttry = tries;
			}
			__NFUN_161__(ViewRotation.Yaw, 4096);
			__NFUN_165__(tries);
			// [Loop Continue]
			goto J0x4B;
		}
		ViewRotation.Yaw = __NFUN_146__(startYaw, __NFUN_144__(besttry, 4096));
		__NFUN_299__(ViewRotation);
		return;
	}

	function Timer()
	{
		// End:0x0D
		if(__NFUN_129__(bFrozen))
		{
			return;
		}
		bFrozen = false;
		bPressedJump = false;
		return;
	}

	function BeginState()
	{
		// End:0x31
		if(__NFUN_130__(__NFUN_119__(Pawn, none), __NFUN_114__(Pawn.Controller, self)))
		{
			Pawn.Controller = none;
		}
		EndZoom();
		FovAngle = DesiredFOV;
		Pawn = none;
		Enemy = none;
		bBehindView = true;
		bFrozen = true;
		bJumpStatus = false;
		bPressedJump = false;
		bBlockCloseCamera = true;
		bValidBehindCamera = false;
		FindGoodView();
		__NFUN_280__(1.0000000, false);
		CleanOutSavedMoves();
		return;
	}

	function EndState()
	{
		bBlockCloseCamera = false;
		CleanOutSavedMoves();
		Velocity = vect(0.0000000, 0.0000000, 0.0000000);
		Acceleration = vect(0.0000000, 0.0000000, 0.0000000);
		bBehindView = false;
		bPressedJump = false;
		myHUD.bShowScores = false;
		return;
	}
Begin:

	__NFUN_256__(3.0000000);
	myHUD.bShowScores = true;
	stop;		
}

defaultproperties
{
	bAlwaysMouseLook=true
	bZeroRoll=true
	AnnouncerVolume=4
	MaxResponseTime=0.7000000
	OrthoZoom=40000.0000000
	CameraDist=9.0000000
	DesiredFOV=50.0000000
	DefaultFOV=50.0000000
	FlashScale=(X=1.0000000,Y=1.0000000,Z=1.0000000)
	MaxTimeMargin=0.3500000
	ProgressTimeOut=8.0000000
	QuickSaveString="Quick Saving"
	NoPauseMessage="Game is not pauseable"
	ViewingFrom="Now viewing from"
	OwnCamera="Now viewing from own camera"
	LocalMessageClass=Class'Engine.LocalMessage'
	EnemyTurnSpeed=45000
	CheatClass=Class'Engine.CheatManager'
	InputClass=Class'Engine.PlayerInput'
	FovAngle=50.0000000
	Handedness=1.0000000
	bCanOpenDoors=true
	bCanDoSpecial=true
	NetPriority=3.0000000
	bTravel=true
}