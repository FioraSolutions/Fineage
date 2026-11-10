using VContainer;

/// <summary>
/// World clock access with [Inject] wiring and singleton fallback (#131 C2 / #142).
/// </summary>
public static class WorldClockPacketServices
{
    private static WorldClock _clock;

    public static void Bind(WorldClock clock = null)
    {
        if (clock != null)
            _clock = clock;
    }

    public static WorldClock Clock =>
        _clock != null
            ? _clock
            : ResolveFromGame<WorldClock>() ?? LegacyWorldClockInstance();

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
    static WorldClock LegacyWorldClockInstance() => WorldClock.Instance;
#pragma warning restore CS0618
}
