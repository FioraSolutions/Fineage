/// <summary>
/// Lobby session control for App/Login scopes (#131).
/// WireLogin/WireGame stay on ACS <c>LobbyFlowController</c> (UI/world concretes).
/// </summary>
public interface ILobbySession
{
    void LogIn();
    void LogOut();
}
