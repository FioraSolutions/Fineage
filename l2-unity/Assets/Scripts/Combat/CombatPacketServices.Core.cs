using VContainer;
public static partial class CombatPacketServices
{
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
}
