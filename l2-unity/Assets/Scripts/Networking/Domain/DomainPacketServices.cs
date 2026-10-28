/// <summary>
/// Domain sink access to local player with optional Bind wiring and singleton fallback (#142 sub-slice 45).
/// </summary>
public static class DomainPacketServices
{
    private static PlayerEntity _localPlayer;

    public static void Bind(PlayerEntity localPlayer = null)
    {
        if (localPlayer != null)
            _localPlayer = localPlayer;
    }

    public static PlayerEntity LocalPlayer =>
        _localPlayer != null ? _localPlayer : PlayerEntity.Instance;
}
