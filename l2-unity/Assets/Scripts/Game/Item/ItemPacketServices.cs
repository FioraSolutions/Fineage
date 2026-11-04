/// <summary>
/// Game item-layer access with [Inject] wiring and IncomingPacketActions fallback (#142 sub-slice 25).
/// </summary>
public static class ItemPacketServices
{
    private static World _gameWorld;
    private static IAnimationManager _animations;
    private static EffectManager _effects;

    public static void Bind(
        World gameWorld = null,
        IAnimationManager animations = null,
        EffectManager effects = null)
    {
        if (gameWorld != null)
            _gameWorld = gameWorld;
        if (animations != null)
            _animations = animations;
        if (effects != null)
            _effects = effects;
    }

    public static World GameWorld =>
        _gameWorld != null ? _gameWorld : IncomingPacketActions.GameWorld;

    public static IAnimationManager Animations =>
        _animations != null ? _animations : IncomingPacketActions.Animations;

    public static EffectManager Effects =>
        _effects != null ? _effects : IncomingPacketActions.Effects;

    /// <summary>Zone V2 grant (opcode 13); server replies with InventoryUpdate.</summary>
    public static void SendZoneGrantItem(int itemId, int count = 1)
    {
        IZoneTransport game = ZoneCommands.Transport;
        if (game == null)
            return;
        ZoneCommands.Send(new ZoneGrantItemCommand(itemId, count));
    }

    /// <summary>Zone V2 pickup (opcode 14); server replies with InventoryUpdate + DeleteObject fan-out.</summary>
    public static void SendZonePickup(int groundObjectId)
    {
        IZoneTransport game = ZoneCommands.Transport;
        if (game == null || groundObjectId <= 0)
            return;
        ZoneCommands.Send(new ZonePickupRequestCommand(groundObjectId));
    }
}
