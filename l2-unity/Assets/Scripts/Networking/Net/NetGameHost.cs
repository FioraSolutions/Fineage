/// <summary>
/// Composition-bound <see cref="IGameManager"/> for Fineage Net apply/lobby paths (GPT A09).
/// Prefer this over <c>NetworkingPacketServices.Manager</c> when the App scope has bound a host.
/// </summary>
public static class NetGameHost
{
    static IGameManager _manager;

    public static void Bind(IGameManager manager)
    {
        if (manager != null)
            _manager = manager;
    }

    public static void Unbind(IGameManager manager)
    {
        if (manager != null && ReferenceEquals(_manager, manager))
            _manager = null;
    }

    public static IGameManager TryGet() => _manager;
}
