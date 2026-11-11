public partial class MonsterStateAtk : MonsterStateBase
{

    static IAnimationManager Animations => AnimationPacketServices.Animations;
    private float _startTime = -1;
    private float _endTime = -1;

    public string parameterName;
    private AnimationCurve _animationCurve;
    private bool _isSwitchIdle = false;
    private bool _loggedSkillHold;
    private bool _loggedSkillStuck;
    private int _swingId;
    private float _lastNormalizedTime;










}
