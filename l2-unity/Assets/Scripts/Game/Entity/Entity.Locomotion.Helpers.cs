using UnityEngine;

public partial class Entity
{
    protected virtual IAnimationController LocomotionAnimationController =>
        _networkAnimationReceive;

    protected bool UsesNpcLocomotionClipNames =>
        this is MonsterEntity || this is NpcEntity;

    protected string LocomotionWeaponSuffix()
    {
        if (_gear != null && !string.IsNullOrEmpty(_gear.WeaponAnim))
            return _gear.WeaponAnim;
        UserEntity user = this as UserEntity;
        if (user != null && !string.IsNullOrEmpty(user.WeaponAnim))
            return user.WeaponAnim;
        return "hand";
    }

    public float UnityRunSpeedMeters => ResolveUnityRunSpeedMeters();

    public float UnityWalkSpeedMeters => ResolveUnityWalkSpeedMeters();

    float ResolveUnityRunSpeedMeters()
    {
        if (_stats == null)
            return 0f;
        if (_stats.UnitySpeedRun > 0.001f)
            return _stats.UnitySpeedRun;
        if (_stats.RunRealSpeed > 0.001f)
            return StatsConverterPacketServices.Converter.ConvertStat(Stat.SPEED, _stats.RunRealSpeed);
        return 0f;
    }

    float ResolveUnityWalkSpeedMeters()
    {
        if (_stats == null)
            return 0f;
        if (_stats.UnitySpeedWalking > 0.001f)
            return _stats.UnitySpeedWalking;
        if (_stats.WalkRealSpeed > 0.001f)
            return StatsConverterPacketServices.Converter.ConvertStat(Stat.SPEED, _stats.WalkRealSpeed);
        return 0f;
    }

    /// <summary>
    /// Re-apply run_speed / walk_speed from current stats (same planar speeds movement uses).
    /// </summary>
    public void RefreshLocomotionAnimRates()
    {
        IAnimationController animation = LocomotionAnimationController;
        if (animation == null)
            return;
        Animator animator = animation.GetAnimator();
        string suffix = LocomotionWeaponSuffix();
        string runClip = UsesNpcLocomotionClipNames
            ? AnimationNames.MONSTER_RUN.Value
            : AnimationNames.RUN.Concat(suffix);
        string walkClip = UsesNpcLocomotionClipNames
            ? AnimationNames.MONSTER_WALK.Value
            : AnimationNames.WALK.Concat(suffix);
        LocomotionGait.ResolvePlaybackRates(
            animator,
            runClip,
            walkClip,
            UnityRunSpeedMeters,
            UnityWalkSpeedMeters,
            out float runRate,
            out float walkRate);
        animation.SetRunSpeed(runRate);
        animation.SetWalkSpeed(walkRate);
    }
}