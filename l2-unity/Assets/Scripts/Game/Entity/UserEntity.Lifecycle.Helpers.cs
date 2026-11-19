using UnityEngine;
using VContainer;

public partial class UserEntity : NetworkEntity
{
    float ApplyAnimRunSpeed(float serverValue)
    {
        if (_networkAnimationReceive == null)
            return 0f;

        float unity = NumberUtils.ScaleToUnity(serverValue);
        float anim = global::LocomotionGait.Rate(
            _networkAnimationReceive.GetAnimator(),
            AnimationNames.RUN.Concat(WeaponSuffix()),
            unity,
            global::LocomotionGait.MinRunRate,
            global::LocomotionGait.MaxRate);
        _networkAnimationReceive.SetRunSpeed(anim);
        return anim;
    }

    void ApplyAnimWalkSpeed(float serverValue)
    {
        if (_networkAnimationReceive == null)
            return;

        float unity = NumberUtils.ScaleToUnity(serverValue);
        float anim = global::LocomotionGait.Rate(
            _networkAnimationReceive.GetAnimator(),
            AnimationNames.WALK.Concat(WeaponSuffix()),
            unity,
            global::LocomotionGait.MinWalkRate,
            global::LocomotionGait.MaxRate);
        _networkAnimationReceive.SetWalkSpeed(anim);
    }

    string WeaponSuffix()
    {
        if (_gear != null && !string.IsNullOrEmpty(_gear.WeaponAnim))
            return _gear.WeaponAnim;
        if (!string.IsNullOrEmpty(WeaponAnim))
            return WeaponAnim;
        return "hand";
    }

    public void LogSpeed(string where)
    {
        Stats s = Stats;
        float move = 0f;
        if (s != null)
            move = Running ? s.UnitySpeedRun : s.UnitySpeedWalking;
        string msg = LogTag + " speed " + where +
            " id=" + (Identity != null ? Identity.Id : 0) +
            " running=" + Running +
            " BaseRun=" + (s != null ? s.BaseRunSpeed : 0) +
            " BaseWalk=" + (s != null ? s.BaseWalkingSpeed : 0) +
            " RunReal=" + (s != null ? s.RunRealSpeed : 0f) +
            " WalkReal=" + (s != null ? s.WalkRealSpeed : 0f) +
            " unityRun=" + (s != null ? s.UnitySpeedRun : 0f) +
            " unityWalk=" + (s != null ? s.UnitySpeedWalking : 0f) +
            " moveSpeed=" + move;
        if (move <= 0.001f)
            Debug.LogWarning(msg);
        // else
            // Debug.Log(msg);
    }

    public void BindCharInfoAnimEvents(UserBowArrowEvents bowArrowEvents)
    {
        _bowArrowEvents?.Exit();
        if (Identity == null)
            return;
        _bowArrowEvents = bowArrowEvents;
        _bowArrowEvents?.Enter();
    }

    void OnDestroy()
    {
        _bowArrowEvents?.Exit();
        _bowArrowEvents = null;
    }
}
