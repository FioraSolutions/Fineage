using VContainer;

/// <summary>
/// Melee sword collision / attack timing access with [Inject] wiring and container/live fallback (#131).
/// </summary>
public static class SwordCollisionPacketServices
{
    private static SwordCollisionService _collision;

    public static void Bind(SwordCollisionService collision = null)
    {
        if (collision != null)
            _collision = collision;
    }

    public static SwordCollisionService Collision =>
        _collision != null
            ? _collision
            : ResolveFromGame<SwordCollisionService>() ?? LegacySwordCollisionInstance();

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
    static SwordCollisionService LegacySwordCollisionInstance() => SwordCollisionService.Instance;
#pragma warning restore CS0618
}
