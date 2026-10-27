/// <summary>
/// Game-layer access to shared services with [Inject] wiring and IncomingPacketActions fallback (#142).
/// </summary>
public static class GamePacketServices
{
    private static World _gameWorld;
    private static EventBus _bus;
    private static EffectSkillsmanager _effectSkills;

    public static void Bind(
        World gameWorld = null,
        EventBus bus = null,
        EffectSkillsmanager effectSkills = null)
    {
        if (gameWorld != null)
            _gameWorld = gameWorld;
        if (bus != null)
            _bus = bus;
        if (effectSkills != null)
            _effectSkills = effectSkills;
    }

    public static World GameWorld =>
        _gameWorld != null ? _gameWorld : IncomingPacketActions.GameWorld;

    public static EventBus Bus =>
        _bus != null ? _bus : IncomingPacketActions.Bus;

    public static EffectSkillsmanager EffectSkills =>
        _effectSkills != null ? _effectSkills : IncomingPacketActions.EffectSkills;
}
