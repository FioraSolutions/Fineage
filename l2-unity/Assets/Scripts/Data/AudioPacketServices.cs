/// <summary>
/// Audio-layer access to game services with [Inject] wiring and Networking fallback (#142).
/// </summary>
public static class AudioPacketServices
{
    private static World _gameWorld;
    private static PlayerController _player;

    public static void Bind(World gameWorld = null, PlayerController player = null)
    {
        if (gameWorld != null)
            _gameWorld = gameWorld;
        if (player != null)
            _player = player;
    }

    public static World GameWorld =>
        _gameWorld != null ? _gameWorld : NetworkingPacketServices.GameWorld;

    public static PlayerController Player =>
        _player != null ? _player : NetworkingPacketServices.Player;
}
