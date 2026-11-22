/// <summary>
/// Lobby flow access to scene and audio with DI wiring and singleton fallback (#142 sub-slice 43).
/// </summary>
public static class LobbyFlowPacketServices
{
    private static ISceneLoader _scenes;

    public static void Bind(
        ISceneLoader scenes = null,
        PlayerStateMachine playerState = null,
        MusicManager music = null)
    {
        if (scenes != null)
            _scenes = scenes;
        if (playerState != null)
            PlayerStatePacketServices.Bind(playerState: playerState);
        if (music != null)
            MusicPacketServices.Bind(music: music);
    }

    public static ISceneLoader Scenes =>
        _scenes != null ? _scenes : SceneLoader.Instance;

    public static PlayerStateMachine PlayerState => PlayerStatePacketServices.PlayerState;

    public static MusicManager Music => MusicPacketServices.Music;
}
