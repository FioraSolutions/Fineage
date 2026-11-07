/// <summary>
/// L2 stat unit conversion with [Inject] wiring and singleton fallback (#131 / #142).
/// </summary>
public static class StatsConverterPacketServices
{
    private static StatsConverter _converter;

    public static void Bind(StatsConverter converter = null)
    {
        if (converter != null)
            _converter = converter;
    }

    public static StatsConverter Converter =>
        _converter != null
            ? _converter
            : ResolveFromGame<StatsConverter>() ?? LegacyStatsConverterInstance();

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
    static StatsConverter LegacyStatsConverterInstance() => StatsConverter.Instance;
#pragma warning restore CS0618
}
