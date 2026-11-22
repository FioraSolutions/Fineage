using System.Collections.Generic;

/// <summary>
/// Lobby session control for App/Login scopes (#131).
/// The callbacks are the ones <c>GameManager</c> forwards. WireLogin/WireGame stay on the concrete controller.
/// </summary>
public interface ILobbySession
{
    void Attach(IGameManager game);
    void LogIn();
    void LogOut();

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
    void OnGameserverSelected();
    void OnStartingGame();
    void OnGameLaunched();
}
