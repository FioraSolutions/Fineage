using VContainer;

/// <summary>
/// Projectile flight/hit access with [Inject] wiring and container/live fallback (#131).
/// </summary>
public static class ProjectilePacketServices
{
    private static IProjectileManager _projectiles;

    public static void Bind(IProjectileManager projectiles = null)
    {
        if (projectiles != null)
            _projectiles = projectiles;
    }

    public static IProjectileManager Projectiles =>
        _projectiles != null
            ? _projectiles
            : ResolveFromGame<IProjectileManager>() ?? LegacyProjectileManagerInstance();

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
    static IProjectileManager LegacyProjectileManagerInstance() => ProjectileManager.Instance;
#pragma warning restore CS0618
}
