using VContainer;

/// <summary>
/// Gameplay camera access with [Inject] wiring and container/live fallback (#131).
/// </summary>
public static class CameraPacketServices
{
    private static CameraController _camera;

    public static void Bind(CameraController camera = null)
    {
        if (camera != null)
            _camera = camera;
    }

    public static CameraController Controller =>
        _camera != null
            ? _camera
            : ResolveFromGame<CameraController>() ?? LegacyCameraInstance();

    static T ResolveFromGame<T>() where T : class
    {
        if (App.GameContainer == null)
            return null;

        try
        {
            return App.GameContainer.Resolve<T>();
        }
        catch
        {
            return null;
        }
    }

#pragma warning disable CS0618 // Locator fallback until all call sites are [Inject]
    static CameraController LegacyCameraInstance() => CameraController.Instance;
#pragma warning restore CS0618
}
