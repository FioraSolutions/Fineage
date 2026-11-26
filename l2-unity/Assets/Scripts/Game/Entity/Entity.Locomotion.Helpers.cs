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

    /// <summary>
    /// Planar speed from the server: UnitySpeed* when a status update already converted it,
    /// otherwise CharInfo/UserInfo/NpcInfo RunRealSpeed (class-template base × move multiplier).
    /// 0 means the packet has not supplied a speed yet — playback does not invent one.
    /// </summary>
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
    /// Pawn GroundSpeed in meters: the planar speed at which PlayAnim rate is 1.
    /// BaseRunSpeed / BaseWalkingSpeed from the server template. 0 is missing, not a default.
    /// </summary>
    float AuthoredSpeedMeters(bool walk)
    {
        if (_stats == null)
            return 0f;
        int raw = walk ? _stats.BaseWalkingSpeed : _stats.BaseRunSpeed;
        if (raw <= 0)
            return 0f;
        return StatsConverterPacketServices.Converter.ConvertStat(Stat.SPEED, raw);
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
        PlayerLocomotionPlayback.ResolvePlaybackRates(
            this,
            animator,
            runClip,
            walkClip,
            UnityRunSpeedMeters,
            UnityWalkSpeedMeters,
            AuthoredSpeedMeters(walk: false),
            AuthoredSpeedMeters(walk: true),
            out float runRate,
            out float walkRate);
        if (animation is BaseAnimationController baseAnimation)
        {
            baseAnimation.SetRunSpeed(runRate);
            baseAnimation.SetWalkSpeed(walkRate);
        }
    }
}