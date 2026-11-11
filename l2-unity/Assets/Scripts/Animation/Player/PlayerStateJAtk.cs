using UnityEngine;

public partial class PlayerStateJAtk : StateMachineBehaviour
{

    static IAnimationManager Animations => AnimationPacketServices.Animations;
    private const string JATK_TIMING_LOG = "[JATK_TIMING]";
    private const string JATK_EARLY_LOG = "[JATK_EARLY]";

    private float _startTime = -1f;
    private float _endTime = -1f;
    private float _linearPatkSpd = 1f;
    private float _lastAnimNormalized = 0f;
    private float _fullCycleMs = 0f;
    private float _clipLengthSec = 0f;
    private int _milestoneMask = 0;
    private int _attackEpoch = 0;
    private bool _isSwordHitLogged = false;
    private bool _isColliderSubscribed = false;
    private bool _isSwitchIdle = false;
    private bool _dieTargetLatched = false;
    private bool _exitViaSwitchToIdle = false;
    private string _lastSwitchDenyReason = "";
    private bool _rateFromSwingClip;

    public string parameterName;



}
