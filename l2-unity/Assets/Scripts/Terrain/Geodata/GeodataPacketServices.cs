using VContainer;

/// <summary>
/// Geodata importer access with [Inject] wiring and live singleton fallback (#131).
/// </summary>
public static class GeodataPacketServices
{
    private static GeodataImporterFactory _factory;

    public static void Bind(GeodataImporterFactory factory = null)
    {
        if (factory != null)
            _factory = factory;
    }

    public static GeodataImporterFactory Factory =>
        (_factory as UnityEngine.Object) != null
            ? _factory
            : ResolveFromScene() ?? LegacyFactoryInstance();

    static GeodataImporterFactory ResolveFromScene()
    {
        if (App.GameContainer == null)
            return null;

        try
        {
            return App.GameContainer.Resolve<GeodataImporterFactory>();
        }
        catch
        {
            return null;
        }
    }

#pragma warning disable CS0618 // Locator fallback until all call sites are [Inject]
    static GeodataImporterFactory LegacyFactoryInstance() => GeodataImporterFactory.Instance;
#pragma warning restore CS0618
}
