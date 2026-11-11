using VContainer;
public static partial class CombatPacketServices
{
    private static World _gameWorld;
    private static TargetManager _targets;
    private static CombatFacingService _facing;
    private static StatsConverter _statsConverter;


    public static World GameWorld =>
        _gameWorld != null ? _gameWorld : NetworkingPacketServices.GameWorld;

    public static TargetManager Targets =>
        _targets != null ? _targets : ControllerPacketServices.Targets;

    public static CombatFacingService Facing =>
        _facing != null
            ? _facing
            : ResolveFromGame<CombatFacingService>() ?? LegacyFacingInstance();

    /// <summary>Non-null facing service; may spawn a host object when none is registered yet.</summary>

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
#pragma warning restore CS0618
}
