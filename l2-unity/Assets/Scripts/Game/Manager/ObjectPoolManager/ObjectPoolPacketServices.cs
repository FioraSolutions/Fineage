using VContainer;

/// <summary>
/// Object pool runtime access with [Inject] wiring and singleton fallback (#131).
/// </summary>
public static class ObjectPoolPacketServices
{
    private static IPoolManager _pool;

    public static void Bind(ObjectPoolManager pool = null)
    {
        if (pool != null)
            _pool = pool;
    }

    public static ModelTable Models => TablePacketServices.Models;

    public static IPoolManager Pool =>
        (_pool as UnityEngine.Object) != null
            ? _pool
            : ResolveFromGame<ObjectPoolManager>() ?? LegacyPoolInstance();

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
    static IPoolManager LegacyPoolInstance() => ObjectPoolManager.Instance;
#pragma warning restore CS0618
}
