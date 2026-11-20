/// <summary>
/// Retail zone music exits use FMOD allow-fadeout, not an immediate stop (#Interlude parity).
/// </summary>
public static class MusicAreaExitFade
{
    /// <summary>FMOD <c>STOP_MODE.ALLOWFADEOUT</c> — mirrored in <see cref="MusicManager.StopMusic"/>.</summary>
    public const bool UseAllowFadeoutOnAreaExit = true;

    public static bool ShouldResetPriorityOnExit(bool useFadeout) => useFadeout;
}