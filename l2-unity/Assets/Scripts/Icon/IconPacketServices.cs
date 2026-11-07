/// <summary>
/// Icon atlas/cache access with [Inject] wiring and container/live fallback (#131).
/// </summary>
public static class IconPacketServices
{
    private static IconManager _icons;

    public static void Bind(IconManager icons = null)
    {
        if (icons != null)
            _icons = icons;
    }

    public static IconManager Icons =>
        _icons != null
            ? _icons
            : ResolveFromGame<IconManager>() ?? LegacyIconManagerInstance();

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
    static IconManager LegacyIconManagerInstance() => IconManager.Instance;
#pragma warning restore CS0618
}
