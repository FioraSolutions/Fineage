/// <summary>
/// Combat layer access to shared services with [Inject] wiring and IncomingPacketActions fallback (#142 sub-slice 32).
/// </summary>
public static class CombatPacketServices
{
    private static GameClient _game;
    private static World _gameWorld;

    public static void Bind(GameClient game = null, World gameWorld = null)
    {
        if (game != null)
            _game = game;
        if (gameWorld != null)
            _gameWorld = gameWorld;
    }

    public static GameClient Game =>
        _game != null ? _game : IncomingPacketActions.Game;

    public static World GameWorld =>
        _gameWorld != null ? _gameWorld : IncomingPacketActions.GameWorld;
}
