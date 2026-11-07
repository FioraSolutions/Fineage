/// <summary>
/// Confirm/dialog SystemMessageWindow access with [Inject] wiring and live fallback (#142).
/// Distinct from Interlude <c>SystemMsgWnd</c> (<see cref="SystemMsgWindow"/>).
/// </summary>
public static class SystemMessagePacketServices
{
    private static SystemMessageWindow _window;

    public static void Bind(SystemMessageWindow window = null)
    {
        if (window != null)
            _window = window;
    }

    public static SystemMessageWindow Window =>
        _window != null ? _window : SystemMessageWindow.Instance;
}
