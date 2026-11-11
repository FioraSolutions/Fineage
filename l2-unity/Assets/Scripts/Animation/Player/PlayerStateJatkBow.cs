using UnityEngine;

public partial class PlayerStateJatkBow : StateMachineBehaviour
{

    static IAnimationManager Animations => AnimationPacketServices.Animations;
    private float _startTime;
    private float _endTime;
    private float _releaseAt;
    private float _tweenSeconds;
    private float _motionSpeed = 1f;
    private bool _isSwitchIdle;

    public string parameterName;
    public string motionName;
    public string eventShootName;





}
