/// <summary>
/// Spine procedural pose access with [Inject] wiring and singleton fallback (#131 C2).
/// </summary>
public static class SpineProceduralPacketServices
{
    private static SpineProceduralController _controller;

    public static void Bind(SpineProceduralController controller = null)
    {
        if (controller != null)
            _controller = controller;
    }

#pragma warning disable CS0618 // Locator fallback until all call sites are [Inject]
    public static SpineProceduralController Controller =>
        _controller != null ? _controller : SpineProceduralController.Instance;
#pragma warning restore CS0618
}
