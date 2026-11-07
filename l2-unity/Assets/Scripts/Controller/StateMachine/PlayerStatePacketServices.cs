/// <summary>
/// Player state-machine access with live bind and singleton fallback (#131).
/// </summary>
public static class PlayerStatePacketServices
{
    private static PlayerStateMachine _playerState;

    public static void Bind(PlayerStateMachine playerState = null)
    {
        if (playerState != null)
            _playerState = playerState;
    }

    public static PlayerStateMachine PlayerState =>
        _playerState != null
            ? _playerState
            : ResolveFromGame<PlayerStateMachine>() ?? LegacyPlayerStateInstance();

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
    static PlayerStateMachine LegacyPlayerStateInstance() => PlayerStateMachine.Instance;
#pragma warning restore CS0618
}
