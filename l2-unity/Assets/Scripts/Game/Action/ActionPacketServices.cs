/// <summary>
/// Game action layer access with [Inject] wiring and Controller fallback (#142).
/// </summary>
public static class ActionPacketServices
{
    private static InputManager _input;
    private static TargetManager _targets;

    public static void Bind(
        InputManager input = null,
        TargetManager targets = null)
    {
        if (input != null)
            _input = input;
        if (targets != null)
            _targets = targets;
    }

    public static InputManager Input =>
        _input != null ? _input : ControllerPacketServices.Input;

    public static TargetManager Targets =>
        _targets != null ? _targets : ControllerPacketServices.Targets;
}
