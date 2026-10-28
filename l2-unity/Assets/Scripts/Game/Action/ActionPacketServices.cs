/// <summary>
/// Game action layer access to shared services with [Inject] wiring and IncomingPacketActions fallback (#142 sub-slice 26).
/// </summary>
public static class ActionPacketServices
{
    private static GameClient _game;
    private static InputManager _input;
    private static TargetManager _targets;

    public static void Bind(
        GameClient game = null,
        InputManager input = null,
        TargetManager targets = null)
    {
        if (game != null)
            _game = game;
        if (input != null)
            _input = input;
        if (targets != null)
            _targets = targets;
    }

    public static GameClient Game =>
        _game != null ? _game : IncomingPacketActions.Game;

    public static InputManager Input =>
        _input != null ? _input : IncomingPacketActions.Input;

    public static TargetManager Targets =>
        _targets != null ? _targets : IncomingPacketActions.Targets;
}
