using VContainer;

/// <summary>
/// Drag ghost UI access with [Inject] wiring and container/live fallback (#131).
/// </summary>
public static class DragIconPacketServices
{
    private static DragIcon _icon;

    public static void Bind(DragIcon icon = null)
    {
        if (icon != null)
            _icon = icon;
    }

    public static DragIcon Icon =>
        (_icon as UnityEngine.Object) != null
            ? _icon
            : ResolveFromGame<DragIcon>() ?? LegacyDragIconInstance();

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
    static DragIcon LegacyDragIconInstance() => DragIcon.Instance;
#pragma warning restore CS0618
}
