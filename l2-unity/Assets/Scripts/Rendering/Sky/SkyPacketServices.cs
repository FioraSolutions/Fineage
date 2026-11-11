/// <summary>
/// Sky rendering access with Bind + Obsolete Instance fallback (#131).
/// </summary>

public static class SkyPacketServices
{
    private static L2CloudSimple _clouds;
    private static L2CelestialDiscs _discs;

    public static void Bind(L2CloudSimple clouds = null, L2CelestialDiscs discs = null)
    {
        if (clouds != null)
            _clouds = clouds;
        if (discs != null)
            _discs = discs;
    }

    public static L2CloudSimple Clouds =>
        (_clouds as UnityEngine.Object) != null ? _clouds : LegacyCloudsInstance();

    public static L2CelestialDiscs CelestialDiscs =>
        (_discs as UnityEngine.Object) != null ? _discs : LegacyDiscsInstance();

#pragma warning disable CS0618 // Locator fallback until all call sites are [Inject]
    static L2CloudSimple LegacyCloudsInstance() => L2CloudSimple.Instance;

    static L2CelestialDiscs LegacyDiscsInstance() => L2CelestialDiscs.Instance;
#pragma warning restore CS0618
}
