/// <summary>
/// Combat layer access to shared services with [Inject] wiring and IncomingPacketActions fallback (#142 sub-slice 32).
/// </summary>
public static class CombatPacketServices
{
    private static GameClient _game;
    private static World _gameWorld;
    private static TargetManager _targets;

    public static void Bind(GameClient game = null, World gameWorld = null, TargetManager targets = null)
    {
        if (game != null)
            _game = game;
        if (gameWorld != null)
            _gameWorld = gameWorld;
        if (targets != null)
            _targets = targets;
    }

    public static GameClient Game =>
        _game != null ? _game : IncomingPacketActions.Game;

    public static World GameWorld =>
        _gameWorld != null ? _gameWorld : IncomingPacketActions.GameWorld;

    public static TargetManager Targets =>
        _targets != null ? _targets : IncomingPacketActions.Targets;
}
