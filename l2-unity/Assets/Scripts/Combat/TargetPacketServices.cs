using VContainer;

/// <summary>
/// Target selection access with [Inject] wiring and container/live fallback (#131 C2).
/// </summary>
public static class TargetPacketServices
{
    private static TargetManager _targets;

    public static void Bind(TargetManager targets = null)
    {
        if (targets != null)
            _targets = targets;
    }

    public static TargetManager Targets =>
        _targets != null
            ? _targets
            : ResolveFromGame<TargetManager>() ?? LegacyTargetManagerInstance();

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
    static TargetManager LegacyTargetManagerInstance() => TargetManager.Instance;
#pragma warning restore CS0618
}
