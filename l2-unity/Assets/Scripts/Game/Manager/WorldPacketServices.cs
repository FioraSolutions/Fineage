/// <summary>
/// World runtime access to shared services with [Inject] wiring and Networking fallback (#142).
/// </summary>
public static class WorldPacketServices
{
    private static IAnimationManager _animations;
    private static GameManager _manager;
    private static WorldPacketApply _worldApply;

    public static void Bind(
        IAnimationManager animations = null,
        GameManager manager = null,
        WorldPacketApply worldApply = null)
    {
        if (animations != null)
            _animations = animations;
        if (manager != null)
            _manager = manager;
        if (worldApply != null)
            _worldApply = worldApply;
    }

    public static IAnimationManager Animations =>
        _animations != null ? _animations : GamePacketServices.Animations;

    public static GameManager Manager =>
        _manager != null ? _manager : NetworkingPacketServices.Manager;

    public static WorldPacketApply WorldApply =>
        _worldApply != null ? _worldApply : NetworkingPacketServices.WorldApply;

    public static Geodata Geodata => GeodataPacketServices.Geodata;
}
