/// <summary>
/// Global effect reactions access with [Inject] wiring and IncomingPacketActions fallback (#142 sub-slice 44).
/// </summary>
public static class GlobalEffectsPacketServices
{
    private static EventBus _bus;
    private static EffectManager _effects;

    public static void Bind(EventBus bus = null, EffectManager effects = null)
    {
        if (bus != null)
            _bus = bus;
        if (effects != null)
            _effects = effects;
    }

    public static EventBus Bus =>
        _bus != null ? _bus : GamePacketServices.Bus;

    public static EffectManager Effects =>
        _effects != null ? _effects : IncomingPacketActions.Effects;
}
