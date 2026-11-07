/// <summary>
/// Entity-layer access with [Inject] wiring and Networking fallback (#142).
/// </summary>
public static class EntityPacketServices
{
    private static PlayerController _player;

    public static void Bind(PlayerController player = null)
    {
        if (player != null)
            _player = player;
    }

    public static PlayerController Player =>
        _player != null ? _player : NetworkingPacketServices.Player;
}
