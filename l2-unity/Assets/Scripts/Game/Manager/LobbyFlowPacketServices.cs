/// <summary>
/// Lobby flow access to scene, player state, and audio with DI wiring and singleton fallback (#142 sub-slice 43).
/// </summary>
public static class LobbyFlowPacketServices
{
    private static SceneLoader _scenes;
    private static PlayerStateMachine _playerState;
    private static MusicManager _music;

    public static void Bind(
        SceneLoader scenes = null,
        PlayerStateMachine playerState = null,
        MusicManager music = null)
    {
        if (scenes != null)
            _scenes = scenes;
        if (playerState != null)
            _playerState = playerState;
        if (music != null)
            _music = music;
    }

    public static SceneLoader Scenes =>
        _scenes != null ? _scenes : SceneLoader.Instance;

    public static PlayerStateMachine PlayerState =>
        _playerState != null ? _playerState : PlayerStateMachine.Instance;

    public static MusicManager Music =>
        _music != null ? _music : MusicManager.Instance;
}
