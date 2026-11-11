using VContainer;

public static partial class GamePacketServices
{
    public static void Bind(
        World gameWorld = null,
        EventBus bus = null,
        GameManager manager = null,
        IAnimationManager animations = null)
    {
        if (gameWorld != null)
            _gameWorld = gameWorld;
        if (bus != null)
            _bus = bus;
        if (manager != null)
            _manager = manager;
        if (animations != null)
            _animations = animations;
    }
}
