/// <summary>
/// Lobby loading UI access with wiring and IncomingPacketActions fallback (#142 sub-slice 39).
/// </summary>
public static class LobbyUiPacketServices
{
    private static L2GameUI _gameUi;
    private static L2LoginUI _loginUi;

    public static void Bind(L2GameUI gameUi = null, L2LoginUI loginUi = null)
    {
        if (gameUi != null)
            _gameUi = gameUi;
        if (loginUi != null)
            _loginUi = loginUi;
    }

    public static L2GameUI GameUi =>
        _gameUi != null ? _gameUi : IncomingPacketActions.Ui;

    public static L2LoginUI LoginUi =>
        _loginUi != null ? _loginUi : IncomingPacketActions.LoginUi;
}
