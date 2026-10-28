/// <summary>
/// Game item-layer access with [Inject] wiring and IncomingPacketActions fallback (#142 sub-slice 25).
/// </summary>
public static class ItemPacketServices
{
    private static GameClient _game;
    private static World _gameWorld;
    private static IAnimationManager _animations;
    private static EffectManager _effects;

    public static void Bind(
        GameClient game = null,
        World gameWorld = null,
        IAnimationManager animations = null,
        EffectManager effects = null)
    {
        if (game != null)
            _game = game;
        if (gameWorld != null)
            _gameWorld = gameWorld;
        if (animations != null)
            _animations = animations;
        if (effects != null)
            _effects = effects;
    }

    public static GameClient Game =>
        _game != null ? _game : IncomingPacketActions.Game;

    public static World GameWorld =>
        _gameWorld != null ? _gameWorld : IncomingPacketActions.GameWorld;

    public static IAnimationManager Animations =>
        _animations != null ? _animations : IncomingPacketActions.Animations;

    public static EffectManager Effects =>
        _effects != null ? _effects : IncomingPacketActions.Effects;

    /// <summary>Zone V2 grant (opcode 13); server replies with InventoryUpdate.</summary>
    public static void SendZoneGrantItem(int itemId, int count = 1)
    {
        GameClient game = Game;
        if (game == null)
            return;
        game.Send(new ZoneGrantItemCommand(itemId, count));
    }
}
