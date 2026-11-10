using VContainer;

/// <summary>
/// Terrain geodata + importer access with [Inject] wiring and live singleton fallback (#131).
/// </summary>
public static class GeodataPacketServices
{
    private static Geodata _geodata;
    private static GeodataImporterFactory _factory;

    public static void Bind(Geodata geodata = null, GeodataImporterFactory factory = null)
    {
        if (geodata != null)
            _geodata = geodata;
        if (factory != null)
            _factory = factory;
    }

    public static Geodata Geodata =>
        (_geodata as UnityEngine.Object) != null
            ? _geodata
            : ResolveFromGame<Geodata>() ?? LegacyGeodataInstance();

    public static GeodataImporterFactory Factory =>
        (_factory as UnityEngine.Object) != null
            ? _factory
            : ResolveFromGame<GeodataImporterFactory>() ?? LegacyFactoryInstance();

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
    static GeodataImporterFactory LegacyFactoryInstance() => GeodataImporterFactory.Instance;
#pragma warning restore CS0618
}
