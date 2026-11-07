using AtmosphericHeightFog;

/// <summary>
/// Rendering-layer access to game services with [Inject] wiring and Game fallback (#142).
/// </summary>
public static class RenderingPacketServices
{
    private static IAnimationManager _animations;
    private static HeightFogGlobal _heightFog;

    public static void Bind(IAnimationManager animations = null, HeightFogGlobal heightFog = null)
    {
        if (animations != null)
            _animations = animations;
        if (heightFog != null)
            _heightFog = heightFog;
    }

    public static IAnimationManager Animations =>
        _animations != null ? _animations : GamePacketServices.Animations;

    public static HeightFogGlobal HeightFog =>
        _heightFog != null ? _heightFog : LegacyHeightFogGlobalInstance();

#pragma warning disable CS0618 // Locator fallback until plugin bind is wired (#131)
    static HeightFogGlobal LegacyHeightFogGlobalInstance() => HeightFogGlobal.Instance;
#pragma warning restore CS0618
}
