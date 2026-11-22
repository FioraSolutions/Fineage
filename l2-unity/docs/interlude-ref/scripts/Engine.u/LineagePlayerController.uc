class LineagePlayerController extends PlayerController
	native
	config(User)
	notplaceable;

struct NViewShakePtr
{
	var int Ptr;
};

struct NViewShakeMgrPtr
{
	var int Ptr;
};

var config int CheatFlyYaw;
var config bool bUseAutoTrackingPawn;
var config bool bUseVolumeCamera;
var config bool bUseHitCheckCamera;
var config float AutoTrackingPawnSpeed;
var config int VolumeCameraRadius;
var config int HitCheckCameraMinDist;
var config int FixedDefaultViewNum;
var config int FixedDefaultCameraYaw[10];
var config int FixedDefaultCameraPitch[10];
var config float FixedDefaultCameraDist[10];
var config float FixedDefaultCameraViewHeight[10];
var config int FixedDefaultCameraHidePlayer[10];
var config int FixedDefaultCameraDisableZoom[10];
var config float CameraViewHeightAdjust;
var bool bDisableCameraManuallyRotating;
var bool bCameraManuallyRotating;
var bool bFixView;
var bool bCameraMovingToDefault;
var bool bUseDefaultCameraYaw;
var bool bUseDefaultCameraPitch;
var bool bUseDefaultCameraDist;
var bool bDisableZoom;
var float OldZoomingDist;
var Vector OldCameraLocation;
var Rotator OldCameraRotation;
var Vector OldViewTargetLocation;
var float ManuallyCameraYaw;
var float ManuallyCameraPitch;
var float CurZoomingDist;
var float DesiredZoomingDist;
var int DesiredPitch;
var int CurVolumeCameraRadius;
var int CurFixedDefaultCameraNo;
var int DefaultCameraYaw;
var int DefaultCameraPitch;
var float DefaultCameraDist;
var bool bCameraSpecialMove;
var bool bCameraMovingToSpecial;
var int SpecialCameraYaw;
var int SpecialCameraPitch;
var float SpecialCameraDist;
var float SpecialCameraDistSpeed;
var int SpecialCameraYawSpeed;
var int SpecialCameraPitchSpeed;
var float SpecialCameraDuration;
var int SpecialCurCameraYaw;
var int SpecialCurCameraPitch;
var float SpecialCurCameraDist;
var int CameraRelYaw;
var int CameraRelPitch;
var int CameraRelYawSpeed;
var int CameraRelPitchSpeed;
var int CameraCurRelYaw;
var int CameraCurRelPitch;
var int SavedViewTargetYaw;
var int SavedViewTargetPitch;
var float ValidateLocationTime;
var float TurningPendingTime;
var bool bKeyboardMoving;
var bool bKeyboardMovingPermanently;
var bool bDesiredKeyboardMoving;
var bool bDesiredKeyboardMovingPermanently;
var int KeyboardMovingDir;
var int KeyboardMovingDirFlg;
var float KeyboardMovingPendingTime;
var bool bJoypadMoving;
var bool bDesiredJoypadMoving;
var bool bDesiredJoypadMovingPermanently;
var int JoypadMovingDir;
var float JoypadMovingPendingTime;
var bool ShouldTurnToMovingDir;
var config float MaxZoomingDist;
var config float MinZoomingDist;
var array<NViewShakePtr> NViewShake;
var array<NViewShakeMgrPtr> NViewShakeMgr;
var bool bObserverModeOn;
var bool bBroadcastObserverModeOn;
var MusicVolume MusicVolume;
var bool bCanPlayMusic;
var float MusicWaitTime;
var float DefaultMusicWaitTime;
var int MusicHandle;
var int VoiceHandle;
var bool bVehicleStart;
var bool bGetServerMusic;
var int nGetServerVoice;
var float PlayMusicDelay;
var float PlayVoiceDelay;
var bool bLockMusic;
var string bServerMusicName;
var string bServerVoiceName;
var bool bCameraWalking;
var Actor CameraModeTarget;
var float ManuallyCameraSpeed;
var(AirVolume) AirEmitter AirEffect;

event PostBeginPlay()
{
	super.PostBeginPlay();
	UnderWaterLoopSound = Sound(DynamicLoadObject("ChrSound.UnderWater_Loop", Class'Engine.Sound'));
	CurVolumeCameraRadius = VolumeCameraRadius;
	return;
}

function name GetMoveSeqName()
{
	return Pawn.RunAnimName[0];
	return;
}

function name GetWaitSeqName()
{
	return Pawn.WaitAnimName[0];
	return;
}

exec function HidePlayerPawn()
{
	Pawn.bHidden = true;
	return;
}

exec function ShowPlayerPawn()
{
	Pawn.bHidden = false;
	return;
}

exec function SetFlyYaw(int Value)
{
	CheatFlyYaw = Value;
	return;
}

exec function CameraRotationOn()
{
	// End:0x2C
	if(__NFUN_132__(__NFUN_132__(__NFUN_132__(bCameraMovingToSpecial, bCameraSpecialMove), bDisableCameraManuallyRotating), bCameraMovingToDefault))
	{
		return;
	}
	bCameraManuallyRotating = true;
	return;
}

exec function CameraRotationOff()
{
	bCameraManuallyRotating = false;
	return;
}

exec function UseAutoTrackingPawnOn()
{
	bUseAutoTrackingPawn = true;
	return;
}

exec function UseAutoTrackingPawnOff()
{
	bUseAutoTrackingPawn = false;
	return;
}

exec function UseHitCheckCameraOn()
{
	bUseHitCheckCamera = true;
	return;
}

exec function UseHitCheckCameraOff()
{
	bUseHitCheckCamera = false;
	return;
}

exec function SetHitCheckCameraMinDist(int Delta)
{
	__NFUN_161__(HitCheckCameraMinDist, Delta);
	return;
}

exec function ViewFix()
{
	// End:0x14
	if(bFixView)
	{
		bFixView = false;		
	}
	else
	{
		bFixView = true;
	}
	return;
}

function UpdateRotation(float DeltaTime, float maxPitch)
{
	local Rotator ViewRotation;

	// End:0x0D
	if(__NFUN_129__(bCheatFlying))
	{
		return;
	}
	// End:0x6C
	if(__NFUN_176__(__NFUN_225__(Pawn.Velocity), 1.0000000))
	{
		// End:0x49
		if(__NFUN_154__(__NFUN_156__(Pawn.Rotation.Pitch, 65535), 0))
		{
			return;
		}
		ViewRotation = Pawn.Rotation;
		ViewRotation.Pitch = 0;		
	}
	else
	{
		ViewRotation = Rotation;
	}
	__NFUN_161__(ViewRotation.Yaw, CheatFlyYaw);
	Pawn.DesiredRotation = ViewRotation;
	return;
}

function bool CalcCameraMovingToDefaultYaw(float DeltaTime, int TargetYaw)
{
	local int BackYaw, temp;

	// End:0x2D
	if(__NFUN_180__(__NFUN_173__(float(__NFUN_147__(__NFUN_146__(TargetYaw, DefaultCameraYaw), OldCameraRotation.Yaw)), float(65536)), float(0)))
	{
		return false;
	}
	temp = __NFUN_147__(__NFUN_146__(TargetYaw, DefaultCameraYaw), OldCameraRotation.Yaw);
	J0x4B:

	// End:0x69 [Loop If]
	if(__NFUN_151__(temp, 32768))
	{
		__NFUN_162__(temp, 65536);
		// [Loop Continue]
		goto J0x4B;
	}
	J0x69:

	// End:0x87 [Loop If]
	if(__NFUN_150__(temp, -32767))
	{
		__NFUN_161__(temp, 65536);
		// [Loop Continue]
		goto J0x69;
	}
	// End:0xBA
	if(__NFUN_130__(__NFUN_152__(-600, temp), __NFUN_150__(temp, 0)))
	{
		BackYaw = int(__NFUN_171__(-5000.0000000, DeltaTime));		
	}
	else
	{
		// End:0xED
		if(__NFUN_130__(__NFUN_150__(0, temp), __NFUN_152__(temp, 600)))
		{
			BackYaw = int(__NFUN_171__(5000.0000000, DeltaTime));			
		}
		else
		{
			BackYaw = int(__NFUN_171__(__NFUN_171__(__NFUN_172__(float(temp), 6.0000000), 50.0000000), DeltaTime));
		}
	}
	// End:0x138
	if(__NFUN_130__(__NFUN_151__(temp, 0), __NFUN_151__(BackYaw, temp)))
	{
		BackYaw = temp;
	}
	// End:0x15F
	if(__NFUN_130__(__NFUN_150__(temp, 0), __NFUN_150__(BackYaw, temp)))
	{
		BackYaw = temp;
	}
	__NFUN_161__(OldCameraRotation.Yaw, BackYaw);
	J0x170:

	// End:0x198 [Loop If]
	if(__NFUN_151__(OldCameraRotation.Yaw, 32768))
	{
		__NFUN_162__(OldCameraRotation.Yaw, 65536);
		// [Loop Continue]
		goto J0x170;
	}
	J0x198:

	// End:0x1C0 [Loop If]
	if(__NFUN_150__(OldCameraRotation.Yaw, -32767))
	{
		__NFUN_161__(OldCameraRotation.Yaw, 65536);
		// [Loop Continue]
		goto J0x198;
	}
	return true;
	return;
}

function bool CalcCameraMovingToDefaultPitch(float DeltaTime, int TargetPitch)
{
	local int BackPitch, temp;

	// End:0x2D
	if(__NFUN_180__(__NFUN_173__(float(__NFUN_147__(__NFUN_146__(TargetPitch, DefaultCameraPitch), OldCameraRotation.Pitch)), float(65536)), float(0)))
	{
		return false;
	}
	temp = __NFUN_147__(__NFUN_146__(TargetPitch, DefaultCameraPitch), OldCameraRotation.Pitch);
	J0x4B:

	// End:0x69 [Loop If]
	if(__NFUN_151__(temp, 32768))
	{
		__NFUN_162__(temp, 65536);
		// [Loop Continue]
		goto J0x4B;
	}
	J0x69:

	// End:0x87 [Loop If]
	if(__NFUN_150__(temp, -32767))
	{
		__NFUN_161__(temp, 65536);
		// [Loop Continue]
		goto J0x69;
	}
	// End:0xBA
	if(__NFUN_130__(__NFUN_152__(-600, temp), __NFUN_150__(temp, 0)))
	{
		BackPitch = int(__NFUN_171__(-5000.0000000, DeltaTime));		
	}
	else
	{
		// End:0xED
		if(__NFUN_130__(__NFUN_150__(0, temp), __NFUN_152__(temp, 600)))
		{
			BackPitch = int(__NFUN_171__(5000.0000000, DeltaTime));			
		}
		else
		{
			BackPitch = int(__NFUN_171__(__NFUN_171__(__NFUN_172__(float(temp), 6.0000000), 50.0000000), DeltaTime));
		}
	}
	// End:0x138
	if(__NFUN_130__(__NFUN_151__(temp, 0), __NFUN_151__(BackPitch, temp)))
	{
		BackPitch = temp;
	}
	// End:0x15F
	if(__NFUN_130__(__NFUN_150__(temp, 0), __NFUN_150__(BackPitch, temp)))
	{
		BackPitch = temp;
	}
	__NFUN_161__(OldCameraRotation.Pitch, BackPitch);
	J0x170:

	// End:0x198 [Loop If]
	if(__NFUN_151__(OldCameraRotation.Pitch, 32768))
	{
		__NFUN_162__(OldCameraRotation.Pitch, 65536);
		// [Loop Continue]
		goto J0x170;
	}
	J0x198:

	// End:0x1C0 [Loop If]
	if(__NFUN_150__(OldCameraRotation.Pitch, -32767))
	{
		__NFUN_161__(OldCameraRotation.Pitch, 65536);
		// [Loop Continue]
		goto J0x198;
	}
	DesiredPitch = OldCameraRotation.Pitch;
	return true;
	return;
}

function bool CalcCameraMovingToDefaultDistance(float DeltaTime)
{
	local float temp, BackDist;

	// End:0x18
	if(__NFUN_180__(__NFUN_174__(CurZoomingDist, 250.0000000), DefaultCameraDist))
	{
		return false;
	}
	temp = __NFUN_175__(DefaultCameraDist, __NFUN_174__(CurZoomingDist, 250.0000000));
	// End:0x66
	if(__NFUN_130__(__NFUN_178__(-6.0000000, temp), __NFUN_176__(temp, 0.0000000)))
	{
		BackDist = __NFUN_171__(-50.0000000, DeltaTime);		
	}
	else
	{
		// End:0x9B
		if(__NFUN_130__(__NFUN_176__(0.0000000, temp), __NFUN_178__(temp, 6.0000000)))
		{
			BackDist = __NFUN_171__(50.0000000, DeltaTime);			
		}
		else
		{
			BackDist = __NFUN_171__(__NFUN_171__(__NFUN_172__(temp, 6.0000000), 50.0000000), DeltaTime);
		}
	}
	// End:0xE6
	if(__NFUN_130__(__NFUN_177__(temp, 0.0000000), __NFUN_177__(BackDist, temp)))
	{
		BackDist = temp;
	}
	// End:0x111
	if(__NFUN_130__(__NFUN_176__(temp, 0.0000000), __NFUN_176__(BackDist, temp)))
	{
		BackDist = temp;
	}
	__NFUN_184__(CurZoomingDist, BackDist);
	DesiredZoomingDist = CurZoomingDist;
	return true;
	return;
}

function bool IsBlockRotation(Rotator Rotation, Vector TargetLocation, float dist, int VolumeRadius)
{
	local Vector HitLocation, HitNormal;

	__NFUN_161__(Rotation.Yaw, VolumeRadius);
	// End:0x41
	if(__NFUN_119__(__NFUN_277__(HitLocation, HitNormal, __NFUN_216__(TargetLocation, __NFUN_213__(dist, Vector(Rotation))), TargetLocation, false), none))
	{
		return true;
	}
	__NFUN_162__(Rotation.Yaw, __NFUN_144__(VolumeRadius, 2));
	// End:0x86
	if(__NFUN_119__(__NFUN_277__(HitLocation, HitNormal, __NFUN_216__(TargetLocation, __NFUN_213__(dist, Vector(Rotation))), TargetLocation, false), none))
	{
		return true;
	}
	__NFUN_161__(Rotation.Yaw, VolumeRadius);
	__NFUN_161__(Rotation.Pitch, VolumeRadius);
	// End:0xD8
	if(__NFUN_119__(__NFUN_277__(HitLocation, HitNormal, __NFUN_216__(TargetLocation, __NFUN_213__(dist, Vector(Rotation))), TargetLocation, false), none))
	{
		return true;
	}
	__NFUN_162__(Rotation.Pitch, __NFUN_144__(VolumeRadius, 2));
	// End:0x11D
	if(__NFUN_119__(__NFUN_277__(HitLocation, HitNormal, __NFUN_216__(TargetLocation, __NFUN_213__(dist, Vector(Rotation))), TargetLocation, false), none))
	{
		return true;
	}
	return false;
	return;
}

function CalcVolumeZoomingDistance(float DeltaTime)
{
	local Vector HitLocation, HitNormal;
	local float temp, TempDist;
	local Vector TempVec;

	// End:0x11
	if(__NFUN_180__(CurZoomingDist, DesiredZoomingDist))
	{
		return;
	}
	temp = __NFUN_175__(DesiredZoomingDist, CurZoomingDist);
	// End:0x58
	if(__NFUN_130__(__NFUN_178__(-6.0000000, temp), __NFUN_176__(temp, 0.0000000)))
	{
		TempDist = __NFUN_171__(-50.0000000, DeltaTime);		
	}
	else
	{
		// End:0x8D
		if(__NFUN_130__(__NFUN_176__(0.0000000, temp), __NFUN_178__(temp, 6.0000000)))
		{
			TempDist = __NFUN_171__(50.0000000, DeltaTime);			
		}
		else
		{
			TempDist = __NFUN_171__(__NFUN_171__(__NFUN_172__(temp, 6.0000000), 50.0000000), DeltaTime);
		}
	}
	// End:0xD8
	if(__NFUN_130__(__NFUN_177__(temp, 0.0000000), __NFUN_177__(TempDist, temp)))
	{
		TempDist = temp;
	}
	// End:0x103
	if(__NFUN_130__(__NFUN_176__(temp, 0.0000000), __NFUN_176__(TempDist, temp)))
	{
		TempDist = temp;
	}
	__NFUN_184__(CurZoomingDist, TempDist);
	// End:0x149
	if(IsBlockRotation(OldCameraRotation, ViewTarget.Location, __NFUN_174__(CurZoomingDist, float(250)), VolumeCameraRadius))
	{
		__NFUN_185__(CurZoomingDist, TempDist);
		return;
	}
	CurVolumeCameraRadius = VolumeCameraRadius;
	TempVec = ViewTarget.Location;
	__NFUN_184__(TempVec.Z, CameraViewHeightAdjust);
	// End:0x1CC
	if(__NFUN_119__(__NFUN_277__(HitLocation, HitNormal, __NFUN_216__(TempVec, __NFUN_213__(__NFUN_174__(CurZoomingDist, float(250)), Vector(Rotation))), TempVec, false), none))
	{
		CurZoomingDist = __NFUN_225__(__NFUN_216__(HitLocation, TempVec));
		__NFUN_185__(CurZoomingDist, float(250));
	}
	return;
}

event PlayerTick(float DeltaTime)
{
	// End:0x94
	if(bCameraMovingToDefault)
	{
		// End:0x29
		if(bUseDefaultCameraYaw)
		{
			bUseDefaultCameraYaw = CalcCameraMovingToDefaultYaw(DeltaTime, SavedViewTargetYaw);
		}
		// End:0x49
		if(bUseDefaultCameraPitch)
		{
			bUseDefaultCameraPitch = CalcCameraMovingToDefaultPitch(DeltaTime, SavedViewTargetPitch);
		}
		// End:0x64
		if(bUseDefaultCameraDist)
		{
			bUseDefaultCameraDist = CalcCameraMovingToDefaultDistance(DeltaTime);
		}
		// End:0x91
		if(__NFUN_130__(__NFUN_130__(__NFUN_129__(bUseDefaultCameraYaw), __NFUN_129__(bUseDefaultCameraPitch)), __NFUN_129__(bUseDefaultCameraDist)))
		{
			bCameraMovingToDefault = false;
		}		
	}
	else
	{
		// End:0xAB
		if(bUseVolumeCamera)
		{
			CalcVolumeZoomingDistance(DeltaTime);			
		}
		else
		{
			// End:0xBF
			if(bCameraWalking)
			{
				CalcCameraWalkingMode(DeltaTime);
			}
		}
	}
	super.PlayerTick(DeltaTime);
	return;
}

function CalcCameraWalkingMode(float DeltaTime)
{
	local int temp, TempYaw, TempPitch;
	local Rotator ViewRotation;
	local Vector ViewVector;

	// End:0x0D
	if(__NFUN_129__(bCameraWalking))
	{
		return;
	}
	// End:0x1A
	if(__NFUN_114__(CameraModeTarget, none))
	{
		return;
	}
	ViewVector = __NFUN_216__(CameraModeTarget.Location, Pawn.Location);
	ViewRotation = Vector2Rotator(ViewVector);
	// End:0x209
	if(__NFUN_181__(__NFUN_173__(float(__NFUN_147__(ViewRotation.Yaw, OldCameraRotation.Yaw)), float(65536)), float(0)))
	{
		temp = __NFUN_147__(ViewRotation.Yaw, OldCameraRotation.Yaw);
		J0x94:

		// End:0xB2 [Loop If]
		if(__NFUN_151__(temp, 32768))
		{
			__NFUN_162__(temp, 65536);
			// [Loop Continue]
			goto J0x94;
		}
		J0xB2:

		// End:0xD0 [Loop If]
		if(__NFUN_150__(temp, -32767))
		{
			__NFUN_161__(temp, 65536);
			// [Loop Continue]
			goto J0xB2;
		}
		// End:0x103
		if(__NFUN_130__(__NFUN_152__(-600, temp), __NFUN_150__(temp, 0)))
		{
			TempYaw = int(__NFUN_171__(-5000.0000000, DeltaTime));			
		}
		else
		{
			// End:0x136
			if(__NFUN_130__(__NFUN_150__(0, temp), __NFUN_152__(temp, 600)))
			{
				TempYaw = int(__NFUN_171__(5000.0000000, DeltaTime));				
			}
			else
			{
				TempYaw = int(__NFUN_171__(__NFUN_171__(__NFUN_172__(float(temp), 6.0000000), 50.0000000), DeltaTime));
			}
		}
		// End:0x181
		if(__NFUN_130__(__NFUN_151__(temp, 0), __NFUN_151__(TempYaw, temp)))
		{
			TempYaw = temp;
		}
		// End:0x1A8
		if(__NFUN_130__(__NFUN_150__(temp, 0), __NFUN_150__(TempYaw, temp)))
		{
			TempYaw = temp;
		}
		__NFUN_161__(OldCameraRotation.Yaw, TempYaw);
		J0x1B9:

		// End:0x1E1 [Loop If]
		if(__NFUN_151__(OldCameraRotation.Yaw, 32768))
		{
			__NFUN_162__(OldCameraRotation.Yaw, 65536);
			// [Loop Continue]
			goto J0x1B9;
		}
		J0x1E1:

		// End:0x209 [Loop If]
		if(__NFUN_150__(OldCameraRotation.Yaw, -32767))
		{
			__NFUN_161__(OldCameraRotation.Yaw, 65536);
			// [Loop Continue]
			goto J0x1E1;
		}
	}
	// End:0x3D3
	if(__NFUN_181__(__NFUN_173__(float(__NFUN_147__(ViewRotation.Pitch, OldCameraRotation.Pitch)), float(65536)), float(0)))
	{
		temp = __NFUN_147__(ViewRotation.Pitch, OldCameraRotation.Pitch);
		J0x24E:

		// End:0x26C [Loop If]
		if(__NFUN_151__(temp, 32768))
		{
			__NFUN_162__(temp, 65536);
			// [Loop Continue]
			goto J0x24E;
		}
		J0x26C:

		// End:0x28A [Loop If]
		if(__NFUN_150__(temp, -32767))
		{
			__NFUN_161__(temp, 65536);
			// [Loop Continue]
			goto J0x26C;
		}
		// End:0x2BD
		if(__NFUN_130__(__NFUN_152__(-600, temp), __NFUN_150__(temp, 0)))
		{
			TempPitch = int(__NFUN_171__(-5000.0000000, DeltaTime));			
		}
		else
		{
			// End:0x2F0
			if(__NFUN_130__(__NFUN_150__(0, temp), __NFUN_152__(temp, 600)))
			{
				TempPitch = int(__NFUN_171__(5000.0000000, DeltaTime));				
			}
			else
			{
				TempPitch = int(__NFUN_171__(__NFUN_171__(__NFUN_172__(float(temp), 6.0000000), 50.0000000), DeltaTime));
			}
		}
		// End:0x33B
		if(__NFUN_130__(__NFUN_151__(temp, 0), __NFUN_151__(TempPitch, temp)))
		{
			TempPitch = temp;
		}
		// End:0x362
		if(__NFUN_130__(__NFUN_150__(temp, 0), __NFUN_150__(TempPitch, temp)))
		{
			TempPitch = temp;
		}
		__NFUN_161__(OldCameraRotation.Pitch, TempPitch);
		J0x373:

		// End:0x39B [Loop If]
		if(__NFUN_151__(OldCameraRotation.Pitch, 32768))
		{
			__NFUN_162__(OldCameraRotation.Pitch, 65536);
			// [Loop Continue]
			goto J0x373;
		}
		J0x39B:

		// End:0x3C3 [Loop If]
		if(__NFUN_150__(OldCameraRotation.Pitch, -32767))
		{
			__NFUN_161__(OldCameraRotation.Pitch, 65536);
			// [Loop Continue]
			goto J0x39B;
		}
		DesiredPitch = OldCameraRotation.Pitch;
	}
	return;
}

function CalcVolumeCamera(out Vector CameraLocation, out Rotator CameraRotation, float ViewDist)
{
	local Vector View, HitLocation, HitNormal, TempCameraLocation;
	local Rotator TempRotation;

	View = __NFUN_276__(vect(1.0000000, 0.0000000, 0.0000000), CameraRotation);
	TempCameraLocation = __NFUN_216__(CameraLocation, __NFUN_213__(__NFUN_174__(ViewDist, float(30)), Vector(CameraRotation)));
	// End:0x78
	if(__NFUN_114__(__NFUN_277__(HitLocation, HitNormal, TempCameraLocation, CameraLocation, false), none))
	{
		__NFUN_224__(CameraLocation, __NFUN_213__(__NFUN_175__(ViewDist, float(30)), View));
		return;		
	}
	else
	{
		ViewDist = __NFUN_244__(__NFUN_219__(__NFUN_216__(CameraLocation, HitLocation), View), ViewDist);
		__NFUN_224__(CameraLocation, __NFUN_213__(__NFUN_175__(ViewDist, float(30)), View));
		return;
	}
	TempRotation = CameraRotation;
	__NFUN_185__(CurZoomingDist, float(5));
	__NFUN_185__(ViewDist, float(5));
	View = __NFUN_276__(vect(1.0000000, 0.0000000, 0.0000000), OldCameraRotation);
	ViewDist = __NFUN_244__(__NFUN_219__(__NFUN_216__(CameraLocation, HitLocation), View), ViewDist);
	__NFUN_224__(CameraLocation, __NFUN_213__(__NFUN_175__(ViewDist, float(30)), View));
	CameraRotation = OldCameraRotation;
	return;
}

function CalcBehindView(out Vector CameraLocation, out Rotator CameraRotation, float dist)
{
	local Vector View, HitLocation, HitNormal;
	local float ViewDist;

	__NFUN_184__(CameraLocation.Z, CameraViewHeightAdjust);
	// End:0x56
	if(__NFUN_130__(__NFUN_130__(__NFUN_180__(OldCameraLocation.X, float(0)), __NFUN_180__(OldCameraLocation.Y, float(0))), __NFUN_180__(OldCameraLocation.Z, float(0))))
	{
		OldCameraRotation = Rotation;
	}
	// End:0xBA
	if(__NFUN_130__(__NFUN_130__(__NFUN_130__(__NFUN_217__(OldViewTargetLocation, CameraLocation), __NFUN_180__(ManuallyCameraPitch, float(0))), __NFUN_180__(ManuallyCameraYaw, float(0))), __NFUN_129__(bCameraMovingToDefault)))
	{
		CameraRotation = OldCameraRotation;
		// End:0xB7
		if(__NFUN_180__(CurZoomingDist, OldZoomingDist))
		{
			CameraLocation = OldCameraLocation;
			return;
		}		
	}
	else
	{
		// End:0xE7
		if(__NFUN_181__(ManuallyCameraYaw, float(0)))
		{
			CameraRotation.Yaw = OldCameraRotation.Yaw;
			bCameraMovingToDefault = false;			
		}
		else
		{
			// End:0xFE
			if(bCameraMovingToDefault)
			{
				CameraRotation = OldCameraRotation;				
			}
			else
			{
				// End:0x146
				if(__NFUN_130__(__NFUN_130__(__NFUN_180__(OldCameraLocation.X, float(0)), __NFUN_180__(OldCameraLocation.Y, float(0))), __NFUN_180__(OldCameraLocation.Z, float(0))))
				{
					CameraRotation = Rotation;					
				}
				else
				{
					// End:0x185
					if(bUseAutoTrackingPawn)
					{
						View = __NFUN_296__(vect(1.0000000, 1.0000000, 0.0000000), __NFUN_216__(CameraLocation, OldCameraLocation));
						CameraRotation = Vector2Rotator(View);						
					}
					else
					{
						CameraRotation = OldCameraRotation;
					}
				}
			}
		}
	}
	OldViewTargetLocation = CameraLocation;
	// End:0x2FA
	if(bFixView)
	{
		CameraRotation = OldCameraRotation;
		__NFUN_161__(CameraRotation.Yaw, int(ManuallyCameraYaw));
		__NFUN_161__(CameraRotation.Pitch, int(ManuallyCameraPitch));
		CameraLocation = OldCameraLocation;
		J0x1E0:

		// End:0x213 [Loop If]
		if(__NFUN_151__(CameraRotation.Yaw, 32768))
		{
			CameraRotation.Yaw = __NFUN_147__(CameraRotation.Yaw, 65536);
			// [Loop Continue]
			goto J0x1E0;
		}
		J0x213:

		// End:0x246 [Loop If]
		if(__NFUN_150__(CameraRotation.Yaw, -32767))
		{
			CameraRotation.Yaw = __NFUN_146__(CameraRotation.Yaw, 65536);
			// [Loop Continue]
			goto J0x213;
		}
		J0x246:

		// End:0x279 [Loop If]
		if(__NFUN_151__(CameraRotation.Pitch, 32768))
		{
			CameraRotation.Pitch = __NFUN_147__(CameraRotation.Pitch, 65536);
			// [Loop Continue]
			goto J0x246;
		}
		J0x279:

		// End:0x2AC [Loop If]
		if(__NFUN_150__(CameraRotation.Pitch, -32767))
		{
			CameraRotation.Pitch = __NFUN_146__(CameraRotation.Pitch, 65536);
			// [Loop Continue]
			goto J0x279;
		}
		// End:0x2D3
		if(__NFUN_150__(CameraRotation.Pitch, -15000))
		{
			CameraRotation.Pitch = -15000;			
		}
		else
		{
			// End:0x2F7
			if(__NFUN_151__(CameraRotation.Pitch, 15000))
			{
				CameraRotation.Pitch = 15000;
			}
		}		
	}
	else
	{
		CameraRotation.Pitch = OldCameraRotation.Pitch;
		__NFUN_161__(CameraRotation.Yaw, int(ManuallyCameraYaw));
		__NFUN_161__(CameraRotation.Pitch, int(ManuallyCameraPitch));
		J0x335:

		// End:0x368 [Loop If]
		if(__NFUN_151__(CameraRotation.Yaw, 32768))
		{
			CameraRotation.Yaw = __NFUN_147__(CameraRotation.Yaw, 65536);
			// [Loop Continue]
			goto J0x335;
		}
		J0x368:

		// End:0x39B [Loop If]
		if(__NFUN_150__(CameraRotation.Yaw, -32767))
		{
			CameraRotation.Yaw = __NFUN_146__(CameraRotation.Yaw, 65536);
			// [Loop Continue]
			goto J0x368;
		}
		J0x39B:

		// End:0x3CE [Loop If]
		if(__NFUN_151__(CameraRotation.Pitch, 32768))
		{
			CameraRotation.Pitch = __NFUN_147__(CameraRotation.Pitch, 65536);
			// [Loop Continue]
			goto J0x39B;
		}
		J0x3CE:

		// End:0x401 [Loop If]
		if(__NFUN_150__(CameraRotation.Pitch, -32767))
		{
			CameraRotation.Pitch = __NFUN_146__(CameraRotation.Pitch, 65536);
			// [Loop Continue]
			goto J0x3CE;
		}
		// End:0x428
		if(__NFUN_150__(CameraRotation.Pitch, -15000))
		{
			CameraRotation.Pitch = -15000;			
		}
		else
		{
			// End:0x44C
			if(__NFUN_151__(CameraRotation.Pitch, 15000))
			{
				CameraRotation.Pitch = 15000;
			}
		}
		ViewDist = __NFUN_174__(dist, CurZoomingDist);
		// End:0x47F
		if(bUseVolumeCamera)
		{
			CalcVolumeCamera(CameraLocation, CameraRotation, ViewDist);			
		}
		else
		{
			View = __NFUN_276__(vect(1.0000000, 0.0000000, 0.0000000), CameraRotation);
			// End:0x4EE
			if(__NFUN_119__(__NFUN_277__(HitLocation, HitNormal, __NFUN_216__(CameraLocation, __NFUN_213__(__NFUN_174__(ViewDist, float(30)), Vector(CameraRotation))), CameraLocation, false), none))
			{
				ViewDist = __NFUN_244__(__NFUN_219__(__NFUN_216__(CameraLocation, HitLocation), View), ViewDist);
			}
			__NFUN_224__(CameraLocation, __NFUN_213__(__NFUN_175__(ViewDist, float(30)), View));
		}
	}
	OldCameraLocation = CameraLocation;
	OldCameraRotation = CameraRotation;
	ManuallyCameraPitch = 0.0000000;
	ManuallyCameraYaw = 0.0000000;
	OldZoomingDist = CurZoomingDist;
	return;
}

event PlayerCalcView(out Actor ViewActor, out Vector CameraLocation, out Rotator CameraRotation)
{
	// End:0x55
	if(__NFUN_132__(__NFUN_114__(ViewTarget, none), ViewTarget.bDeleteMe))
	{
		// End:0x4E
		if(__NFUN_130__(__NFUN_119__(Pawn, none), __NFUN_129__(Pawn.bDeleteMe)))
		{
			SetViewTarget(Pawn);			
		}
		else
		{
			SetViewTarget(self);
		}
	}
	// End:0x6B
	if(__NFUN_129__(bUseVolumeCamera))
	{
		CurZoomingDist = DesiredZoomingDist;
	}
	ViewActor = ViewTarget;
	CameraLocation = ViewTarget.Location;
	// End:0xCC
	if(__NFUN_114__(ViewTarget, Pawn))
	{
		// End:0xBA
		if(bBehindView)
		{
			CalcBehindView(CameraLocation, CameraRotation, 250.0000000);			
		}
		else
		{
			CalcFirstPersonView(CameraLocation, CameraRotation);
		}
		return;
	}
	// End:0xE4
	if(__NFUN_114__(ViewTarget, self))
	{
		CameraRotation = Rotation;
		return;
	}
	return;
}

function ViewPlayer()
{
	SetViewTarget(Pawn);
	bBehindView = true;
	// End:0x2B
	if(bBehindView)
	{
		ViewTarget.BecomeViewTarget();
	}
	return;
}

function ClientReStart()
{
	// End:0x14
	if(__NFUN_114__(Pawn, none))
	{
		__NFUN_113__('WaitingForPawn');
		return;
	}
	Pawn.ClientReStart();
	SetViewTarget(Pawn);
	ViewPlayer();
	EnterStartState();
	return;
}

simulated event LostChild(Actor Other)
{
	// End:0x16
	if(__NFUN_114__(AirEffect, Other))
	{
		AirEffect = none;
	}
	super.LostChild(Other);
	return;
}

simulated event ClearL2Game()
{
	// End:0x21
	if(__NFUN_119__(AirEffect, none))
	{
		AirEffect.NDestroy();
		AirEffect = none;
	}
	super.ClearL2Game();
	return;
}

state PlayerSpidering
{	stop;
}

state Spectating
{
	exec function AltFire(optional float f)
	{
		ClientMessage(OwnCamera, 'Event');
		return;
	}
	stop;
}

defaultproperties
{
	MaxZoomingDist=1000.0000000
	MinZoomingDist=-200.0000000
	bCanPlayMusic=true
	MusicHandle=-1
	VoiceHandle=-1
	ManuallyCameraSpeed=1.0000000
	DesiredFOV=60.0000000
	DefaultFOV=60.0000000
	bMyController=true
}