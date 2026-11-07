/// <summary>
/// Quantity input dialog access with [Inject] wiring and live fallback (#142).
/// </summary>
public static class QuantityInputPacketServices
{
    private static QuantityInput _window;

    public static void Bind(QuantityInput window = null)
    {
        if (window != null)
            _window = window;
    }

    public static QuantityInput Window =>
        _window != null ? _window : QuantityInput.Instance;
}
