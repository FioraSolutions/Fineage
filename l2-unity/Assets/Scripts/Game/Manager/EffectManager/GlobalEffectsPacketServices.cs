/// <summary>
/// Global effect reactions access with [Inject] wiring and Controller/Game fallback (#142).
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
        _effects != null ? _effects : ControllerPacketServices.Effects;
}
