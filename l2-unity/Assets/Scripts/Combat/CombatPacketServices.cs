/// <summary>
/// Combat layer access to shared services with [Inject] wiring and peer fallback (#142).
/// </summary>
public static class CombatPacketServices
{
    private static World _gameWorld;
    private static TargetManager _targets;
    private static CombatFacingService _facing;
    private static StatsConverter _statsConverter;

    public static void Bind(
        World gameWorld = null,
        TargetManager targets = null,
        CombatFacingService facing = null,
        StatsConverter statsConverter = null)
    {
        if (gameWorld != null)
            _gameWorld = gameWorld;
        if (targets != null)
            _targets = targets;
        if (facing != null)
            _facing = facing;
        if (statsConverter != null)
            _statsConverter = statsConverter;
    }

    public static World GameWorld =>
        _gameWorld != null ? _gameWorld : NetworkingPacketServices.GameWorld;

    public static TargetManager Targets =>
        _targets != null ? _targets : ControllerPacketServices.Targets;

    public static CombatFacingService Facing =>
        _facing != null
            ? _facing
            : ResolveFromGame<CombatFacingService>() ?? LegacyFacingInstance();

    /// <summary>Non-null facing service; may spawn a host object when none is registered yet.</summary>
    public static CombatFacingService EnsureFacing()
    {
        CombatFacingService resolved = Facing;
        if (resolved != null)
            return resolved;
#pragma warning disable CS0618 // Locator fallback until all call sites are [Inject]
        return CombatFacingService.Ensure();
#pragma warning restore CS0618
    }

    public static HitManager Hits => HitPacketServices.Hits;

    public static SwordCollisionService SwordCollision => SwordCollisionPacketServices.Collision;

    public static StatsConverter StatsConverter =>
        _statsConverter != null ? _statsConverter : StatsConverterPacketServices.Converter;

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
    static CombatFacingService LegacyFacingInstance() => CombatFacingService.Instance;
#pragma warning restore CS0618
}
