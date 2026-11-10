using VContainer;

/// <summary>
/// Terrain geodata access with [Inject] wiring and container/live fallback (#131).
/// </summary>
public static class GeodataPacketServices
{
    private static Geodata _geodata;

    public static void Bind(Geodata geodata = null)
    {
        if (geodata != null)
            _geodata = geodata;
    }

    public static Geodata Geodata =>
        (_geodata as UnityEngine.Object) != null
            ? _geodata
            : ResolveFromGame<Geodata>() ?? LegacyGeodataInstance();

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
    static Geodata LegacyGeodataInstance() => Geodata.Instance;
#pragma warning restore CS0618
}
