using VContainer;
public static partial class CombatPacketServices
{
    public static CombatFacingService EnsureFacing()
    {
        CombatFacingService resolved = Facing;
        if (resolved != null)
            return resolved;
#pragma warning disable CS0618 // Locator fallback until all call sites are [Inject]
        return CombatFacingService.Ensure();
#pragma warning restore CS0618
    }

    static CombatFacingService LegacyFacingInstance() => CombatFacingService.Instance;
}
