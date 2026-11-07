/// <summary>
/// Service-layer access with [Inject] wiring and peer PacketServices fallback (#142).
/// </summary>
public static class ServicePacketServices
{
    private static PlayerController _player;
    private static IAnimationManager _animations;
    private static EffectManager _effects;
    private static StatsConverter _statsConverter;

    public static void Bind(
        PlayerController player = null,
        IAnimationManager animations = null,
        EffectManager effects = null,
        StatsConverter statsConverter = null)
    {
        if (player != null)
            _player = player;
        if (animations != null)
            _animations = animations;
        if (effects != null)
            _effects = effects;
        if (statsConverter != null)
            _statsConverter = statsConverter;
    }

    public static PlayerController Player =>
        _player != null ? _player : NetworkingPacketServices.Player;

    public static IAnimationManager Animations =>
        _animations != null ? _animations : GamePacketServices.Animations;

    public static EffectManager Effects =>
        _effects != null ? _effects : ControllerPacketServices.Effects;

    public static StatsConverter StatsConverter =>
        _statsConverter != null ? _statsConverter : StatsConverterPacketServices.Converter;
}
