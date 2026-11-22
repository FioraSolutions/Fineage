class SceneManager extends Info
	native
	config
	placeable
	hidecategories(Movement,Collision,Lighting,LightColor,Karma,Force);

enum EAffect
{
	AFFECT_ViewportCamera,          // 0
	AFFECT_Actor                    // 1
};

struct Orientation
{
	var() Object.ECamOrientation CamOrientation;
	var() Actor LookAt;
	var() float EaseIntime;
	var() int bReversePitch;
	var() int bReverseYaw;
	var() int bReverseRoll;
	var int MA;
	var float PctInStart;
	var float PctInEnd;
	var float PctInDuration;
	var Rotator StartingRotation;
};

struct Interpolator
{
	var() int bDone;
	var() float _value;
	var() float _remainingTime;
	var() float _totalTime;
	var() float _speed;
	var() float _acceleration;
};

var() export editinline array<export editinline MatAction> Actions;
var() config SceneManager.EAffect Affect;
var() Actor AffectedActor;
var() bool bLooping;
var() bool bCinematicView;
var() name PlayerScriptTag;
var() name NextSceneTag;
var bool bReverseView;
var int nSceneManagerID;
var transient float PctSceneComplete;
var transient MatAction CurrentAction;
var transient float SceneSpeed;
var transient float TotalSceneTime;
var transient Actor Viewer;
var transient Pawn OldPawn;
var transient bool bIsRunning;
var transient bool bIsSceneStarted;
var transient float CurrentTime;
var transient array<Vector> SampleLocations;
var transient array<MatSubAction> SubActions;
var transient Orientation CamOrientation;
var transient Orientation PrevOrientation;
var transient Interpolator RotInterpolator;
var transient Vector CameraShake;
var transient Vector DollyOffset;

// Export USceneManager::execGetTotalSceneTime(FFrame&, void* const)
native function float GetTotalSceneTime();

simulated function BeginPlay()
{
	super(Actor).BeginPlay();
	// End:0x4D
	if(__NFUN_130__(__NFUN_154__(int(Affect), int(1)), __NFUN_114__(AffectedActor, none)))
	{
		__NFUN_231__("SceneManager : Affected actor is NULL!");
	}
	TotalSceneTime = GetTotalSceneTime();
	bIsRunning = false;
	bIsSceneStarted = false;
	return;
}

function Trigger(Actor Other, Pawn EventInstigator)
{
	bIsRunning = true;
	bIsSceneStarted = false;
	// End:0x23
	if(__NFUN_119__(Other, none))
	{
		bReverseView = true;
	}
	return;
}

event SceneStarted()
{
	local Controller P;
	local AIScript S;

	Viewer = none;
	// End:0x25
	if(__NFUN_154__(int(Affect), int(1)))
	{
		Viewer = AffectedActor;		
	}
	else
	{
		P = Level.ControllerList;
		J0x39:

		// End:0x149 [Loop If]
		if(__NFUN_119__(P, none))
		{
			// End:0x132
			if(P.__NFUN_303__('LineagePlayerController'))
			{
				Viewer = P;
				OldPawn = PlayerController(Viewer).Pawn;
				// End:0x11B
				if(__NFUN_119__(OldPawn, none))
				{
					OldPawn.Velocity = vect(0.0000000, 0.0000000, 0.0000000);
					OldPawn.Acceleration = vect(0.0000000, 0.0000000, 0.0000000);
					PlayerController(Viewer).UnPossess();
					// End:0x11B
					if(__NFUN_255__(PlayerScriptTag, 'None'))
					{
						// End:0xFB
						foreach __NFUN_313__(Class'Engine.AIScript', S, PlayerScriptTag)
						{
							// End:0xFB
							break;							
						}						
						// End:0x11B
						if(__NFUN_119__(S, none))
						{
							S.TakeOver(OldPawn);
						}
					}
				}
				PlayerController(Viewer).StartInterpolation();
				// [Explicit Break]
				goto J0x149;
			}
			P = P.nextController;
			// [Loop Continue]
			goto J0x39;
		}
	}
	J0x149:

	Viewer.StartInterpolation();
	return;
}

event SceneEnded()
{
	bIsSceneStarted = false;
	// End:0x4C
	if(__NFUN_154__(int(Affect), int(0)))
	{
		// End:0x4C
		if(__NFUN_119__(PlayerController(Viewer), none))
		{
			// End:0x4C
			if(__NFUN_119__(OldPawn, none))
			{
				PlayerController(Viewer).Possess(OldPawn);
			}
		}
	}
	Viewer.FinishedInterpolation();
	bReverseView = false;
	__NFUN_117__('Trigger');
	return;
}

defaultproperties
{
	Texture=Texture'Engine.S_SceneManager'
}