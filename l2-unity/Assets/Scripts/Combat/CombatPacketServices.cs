/// <summary>
/// Combat layer access to shared services with [Inject] wiring and peer fallback (#142).
/// </summary>
public static class CombatPacketServices
{
    private static World _gameWorld;
    private static TargetManager _targets;

    public static void Bind(World gameWorld = null, TargetManager targets = null)
    {
        if (gameWorld != null)
            _gameWorld = gameWorld;
        if (targets != null)
            _targets = targets;
    }

    public static World GameWorld =>
        _gameWorld != null ? _gameWorld : NetworkingPacketServices.GameWorld;

    public static TargetManager Targets =>
        _targets != null ? _targets : ControllerPacketServices.Targets;
}
