/// <summary>
/// NPC gravity access with [Inject] wiring and container/live fallback (#131).
/// </summary>
public static class GravityNpcPacketServices
{
    private static IGravity _gravity;

    public static void Bind(IGravity gravity = null)
    {
        if (gravity != null)
            _gravity = gravity;
    }

    public static IGravity Gravity =>
        (_gravity as UnityEngine.Object) != null
            ? _gravity
            : ResolveFromGame<GravityNpc>() ?? LegacyGravityInstance();

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
    static IGravity LegacyGravityInstance() => GravityNpc.Instance;
#pragma warning restore CS0618
}
