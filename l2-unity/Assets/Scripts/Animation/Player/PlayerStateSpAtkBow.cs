
using UnityEngine;

public partial class PlayerStateSpAtkBow : StateMachineBehaviour
{

    static IAnimationManager Animations => AnimationPacketServices.Animations;
    private float _startTime;
    private float _endTime;
    private float _linearSpeed;
    private bool _isSwitchIdle;

    public string parameterName;
    public string motionName;
    public string eventShootName;




}
