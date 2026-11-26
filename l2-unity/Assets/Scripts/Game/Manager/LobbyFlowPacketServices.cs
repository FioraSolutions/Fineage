/// <summary>
/// Lobby flow access to player state and music (#142 sub-slice 43).
/// Scene loads go through injected <see cref="ISceneLoader"/>, not this type.
/// </summary>
public static class LobbyFlowPacketServices
{
    public static void Bind(
        PlayerStateMachine playerState = null,
        MusicManager music = null)
    {
        if (playerState != null)
            PlayerStatePacketServices.Bind(playerState: playerState);
        if (music != null)
            MusicPacketServices.Bind(music: music);
    }

    public static PlayerStateMachine PlayerState => PlayerStatePacketServices.PlayerState;

    public static MusicManager Music => MusicPacketServices.Music;
}
