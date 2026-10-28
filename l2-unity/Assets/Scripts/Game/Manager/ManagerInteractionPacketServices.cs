using VContainer;

/// <summary>
/// Manager interaction access (cursor, dead fade, drop pick) with [Inject] wiring and container fallback (#142 sub-slice 37).
/// </summary>
public static class ManagerInteractionPacketServices
{
    private static ItemDropPicker _dropPicker;
    private static L2ActorFade _actorFade;

    public static void Bind(ItemDropPicker dropPicker = null, L2ActorFade actorFade = null)
    {
        if (dropPicker != null)
            _dropPicker = dropPicker;
        if (actorFade != null)
            _actorFade = actorFade;
    }

    public static ItemDropPicker DropPicker =>
        _dropPicker != null ? _dropPicker : ResolveFromGameContainer<ItemDropPicker>();

    public static L2ActorFade ActorFade =>
        _actorFade != null ? _actorFade : ResolveFromGameContainer<L2ActorFade>();

    static T ResolveFromGameContainer<T>() where T : class
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
}
