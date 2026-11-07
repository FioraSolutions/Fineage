/// <summary>
/// Simple item/skill tooltip window access with [Inject] wiring and live fallback (#142).
/// </summary>
public static class ToolTipSimplePacketServices
{
    private static ToolTipSimple _window;

    public static void Bind(ToolTipSimple window = null)
    {
        if (window != null)
            _window = window;
    }

    public static ToolTipSimple Window =>
        _window != null ? _window : ToolTipSimple.Instance;
}
