/// <summary>
/// Service-layer access with [Inject] wiring and IncomingPacketActions fallback (#142 sub-slice 31).
/// </summary>
public static class ServicePacketServices
{
    private static PlayerController _player;
    private static IAnimationManager _animations;
    private static EffectManager _effects;

    public static void Bind(
        PlayerController player = null,
        IAnimationManager animations = null,
        EffectManager effects = null)
    {
        if (player != null)
            _player = player;
        if (animations != null)
            _animations = animations;
        if (effects != null)
            _effects = effects;
    }

    public static PlayerController Player =>
        _player != null ? _player : IncomingPacketActions.Player;

    public static IAnimationManager Animations =>
        _animations != null ? _animations : IncomingPacketActions.Animations;

    public static EffectManager Effects =>
        _effects != null ? _effects : IncomingPacketActions.Effects;
}
