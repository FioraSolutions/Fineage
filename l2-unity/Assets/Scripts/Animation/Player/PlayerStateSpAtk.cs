
using UnityEngine;

public partial class PlayerStateSpAtk : StateMachineBehaviour
{

    static IAnimationManager Animations => AnimationPacketServices.Animations;
    public const float HitTimeCompensationSeconds = 0.08f;
    public const float MinServerHitSeconds = 0.1f;
    public const float SwitchToIdleNormalizedTime = 0.95f;

    private float _startTime;
    private float _eventHitTimeInClip;
    private float _serverHitTime;
    private float _linearPatkSpd = 1f;
    private bool _isSwitchIdle;
    private bool _hitExecuted;
    private bool _applyingEnterPose;

    public string motionName;
    public string eventStartHitName; // "hit_start"
    public string eventEndHitName;   // "hit_end"

    [Header("Legacy (unused in linear mode)")]
    public float postHitSpeed = 2.0f;

    /// <summary>
    /// Wall-clock duration until SwitchToIdle (norm >= 0.95).
    /// Linear mode: one speed for the whole clip (hit synced to server HitTime).
    /// </summary>





}
