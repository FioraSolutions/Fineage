/// <summary>
/// Shared fade tuning for player CrossFadeInFixedTime (locomotion + basic atk).
/// Defaults live on <see cref="LocomotionCrossFadeDefaults"/>; per-state exit
/// overrides live on <see cref="Animation.ExitCrossFadeDuration"/>.
/// </summary>
public static class LocomotionCrossFadeSettings
{
    public const float DefaultFixedDuration = LocomotionCrossFadeDefaults.DefaultFixedDuration;

    public static float FixedDuration
    {
        get => LocomotionCrossFadeDefaults.FixedDuration;
        set => LocomotionCrossFadeDefaults.FixedDuration = value;
    }

    /// <summary>
    /// Filled AnimTween wait (0.20). Empty table rows keep <see cref="FixedDuration"/>.
    /// Dual jatk exit (0.25) stays longer via <see cref="ResolveDuration"/>.
    /// </summary>
    public const float RemoteStopDuration = LocomotionCrossFadeDefaults.RemoteStopDuration;

    public static float ResolveExitDuration(IAnimationController controller) =>
        AnimationExitCrossFade.Resolve(controller);

    /// <summary>
    /// Table tween if the clip family is filled; otherwise Unity default.
    /// Authored clip-exit longer than both (dual 0.25) wins.
    /// </summary>
    public static float ResolveDuration(string animName, IAnimationController controller = null)
    {
        float duration = TablePacketServices.AnimTweens.TryGetTween(animName, out float tween)
            ? tween
            : FixedDuration;

        if (controller == null)
            return duration;

        float exit = ResolveExitDuration(controller);
        if (exit > FixedDuration && exit > duration)
            return exit;
        return duration;
    }

    public static float ResolveWaitStopDuration(IAnimationController controller) =>
        ResolveDuration("wait", controller);
}
