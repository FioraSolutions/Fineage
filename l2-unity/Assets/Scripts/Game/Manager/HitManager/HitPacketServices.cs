using VContainer;

/// <summary>
/// Hit / impact runtime access with [Inject] wiring and container/live fallback (#131).
/// </summary>
public static class HitPacketServices
{
    private static HitManager _hits;

    public static void Bind(HitManager hits = null)
    {
        if (hits != null)
            _hits = hits;
    }

    public static HitManager Hits =>
        _hits != null
            ? _hits
            : ResolveFromGame<HitManager>() ?? LegacyHitManagerInstance();

    static T ResolveFromGame<T>() where T : class
    {
        if (App.GameContainer == null)
            return null;

        try
        {
            return App.GameContainer.Resolve<T>();
        }
        catch
        {
            return null;
        }
    }

#pragma warning disable CS0618 // Locator fallback until all call sites are [Inject]
    static HitManager LegacyHitManagerInstance() => HitManager.Instance;
#pragma warning restore CS0618
}
