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
        _effects != null
            ? _effects
            : ResolveFromGame<EffectManager>() ?? LegacyEffectManagerInstance();

    static T ResolveFromGame<T>() where T : class
    {
        if (App.GameContainer == null)
            return null;

        try
        {
            return App.GameContainer.Resolve<T>();
        }
        catch
        {
            return null;
        }
    }

#pragma warning disable CS0618 // Locator fallback until all call sites are [Inject]
    static EffectManager LegacyEffectManagerInstance() => EffectManager.Instance;
#pragma warning restore CS0618
}
