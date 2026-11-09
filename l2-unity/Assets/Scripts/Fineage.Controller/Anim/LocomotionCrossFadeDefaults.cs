/// <summary>
/// Cycle-safe default CrossFade durations. Table-backed resolve stays on
/// <c>LocomotionCrossFadeSettings</c> in Assembly-CSharp (AnimTween ties).
/// </summary>
public static class LocomotionCrossFadeDefaults
{
    public const float DefaultFixedDuration = 0.15f;
    public static float FixedDuration = DefaultFixedDuration;

    /// <summary>
    /// Filled AnimTween wait (0.20). Empty table rows keep <see cref="FixedDuration"/>.
    /// </summary>
    public const float RemoteStopDuration = 0.20f;
}
