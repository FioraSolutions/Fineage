using UnityEngine;

public partial class PlayerOverriddenMagicAtk : StateMachineBehaviour
{

    static IAnimationManager Animations => AnimationPacketServices.Animations;
    private const string TriggerCastMid = "CastMid";
    private const string TriggerCastEnd = "CastEnd";
    private const string TriggerMagicShot = "MagicShot";
    private const float ShotBlendCompensationSeconds = 0.06f;
    /// <summary>
    /// Принудительный MagicShot по wall-clock во время CastEnd (включено только при EnableForceSync = true и HitTime пороге ниже).
    /// Сейчас выключено — только SkillAnimationRunner.
    /// </summary>
    private const bool EnableForceSync = false;

    /// <summary>
    /// ForceSync только когда server HitTime >= этого порога (сек). Короче 2 с — только runner, без принудительного MagicShot.
    /// </summary>
    private const float ForceSyncMinServerHitTimeSeconds = 2f;
    private const string ShotSourceForceSync = "ForceSync";

    private const string MagicExitLogTag = "[MAGIC_SMB_EXIT]";
    /// <summary>
    /// End the phase with the clip. 0.95 cut CastMid/CastEnd short of the 30 fps length
    /// that <see cref="MagicCastData"/> schedules. wallDone still fires at clip/speed.
    /// </summary>
    private const float PhaseDoneNormalizedTime = 1f;

    private MagicCastData _castData;
    private int _objectId;
    private bool _isSwitchIdle;
    private bool _phaseFinished;
    private float _stateEnterTime;
    private bool _forcedShotTriggered;
    private float _lastExitDiagLogTime;
    private float _clipLengthAtEnter;

    [Header("Settings")]
    public string parameterName; 
    public bool isFinalShotState;
    public int stateIndex;
    private bool _eventLogged = false;
    float _eventTimeInClip;





}
