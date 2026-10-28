/// <summary>
/// Rendering-layer access to game services with [Inject] wiring and IncomingPacketActions fallback (#142 sub-slice 33).
/// </summary>
public static class RenderingPacketServices
{
    private static IAnimationManager _animations;

    public static void Bind(IAnimationManager animations = null)
    {
        if (animations != null)
            _animations = animations;
    }

    public static IAnimationManager Animations =>
        _animations != null ? _animations : IncomingPacketActions.Animations;
}
