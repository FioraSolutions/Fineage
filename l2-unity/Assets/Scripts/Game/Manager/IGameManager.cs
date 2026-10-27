using System.Collections.Generic;

/// <summary>
/// Networking-facing contract of <see cref="GameManager"/>.
/// Packet handlers and transport adapters should depend on this interface
/// rather than the concrete MonoBehaviour so that the lobby-flow surface
/// is explicit and testable.
/// </summary>
public interface IGameManager
{
    int ProtocolVersion { get; }
    bool IsSwitchingServer { get; set; }
    bool WorldSpawnReady { get; }

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
}
