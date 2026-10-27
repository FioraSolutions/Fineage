/// <summary>
/// Game event listeners access with [Inject] wiring and singleton / IncomingPacketActions fallback (#142 sub-slice 42).
/// </summary>
public static class GameEventPacketServices
{
    private static PlayerEntity _localPlayer;
    private static TargetManager _targets;

    public static void Bind(PlayerEntity localPlayer = null, TargetManager targets = null)
    {
        if (localPlayer != null)
            _localPlayer = localPlayer;
        if (targets != null)
            _targets = targets;
    }

    public static PlayerEntity LocalPlayer =>
        _localPlayer != null ? _localPlayer : PlayerEntity.Instance;

    public static TargetManager Targets =>
        _targets != null ? _targets : IncomingPacketActions.Targets;
}
