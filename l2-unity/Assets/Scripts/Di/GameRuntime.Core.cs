using VContainer;

public sealed partial class GameRuntime
{
    [Inject]
    public void Construct(
        EventProcessor events,
        GameClient game,
        LoginClient login,
        GameManager manager,
        IGameEventBus gameEvents,
        World world,
        PlayerController player)
    {
        if (events != null)
            _events = events;
        if (gameEvents != null)
            _gameEvents = gameEvents;
        if (world != null)
            _world = world;
        if (player != null)
            _player = player;
        DiRuntimePacketServices.Bind(game: game, login: login, manager: manager);
    }
}
