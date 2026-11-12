using System.Collections.Generic;

/// <summary>
/// Networking/lobby contract of <see cref="GameManager"/>.
/// Packet handlers, transport adapters, and Di scopes should depend on this
/// rather than the concrete MonoBehaviour (#131).
/// </summary>
public interface IGameManager
{
    int ProtocolVersion { get; }
    bool IsSwitchingServer { get; set; }
    bool WorldSpawnReady { get; }
    GameState GameState { get; }

    void OnLoginServerConnected();
    void OnLoginServerAuthAllowed();
    void OnLoginServerPlayOk();
    void OnConnectingToGameServer();
    void OnReceivedServerList(byte lastServer, List<ServerData> serverData, Dictionary<int, int> charsOnServers);
    void OnAuthAllowed();
    void OnCharacterSelect();
    void OnCreateUser(List<PlayerTemplates> playerTemplates);
    void OnCreateUserFail(string text);
    void OnWorldSceneLoaded();
    void OnWorldLoading();
    void OnRelogin();
    void OnDisconnect();
    void QuitGame();
}
