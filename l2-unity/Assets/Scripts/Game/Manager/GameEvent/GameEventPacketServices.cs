/// <summary>
/// Game event listeners access with [Inject] wiring and Controller/live fallback (#142).
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
        _localPlayer != null ? _localPlayer : DomainPacketServices.LocalPlayer;

    public static TargetManager Targets =>
        _targets != null ? _targets : ControllerPacketServices.Targets;
}
