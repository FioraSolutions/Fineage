/// <summary>
/// Scene loader access to game UI with optional Bind wiring and singleton fallback (#142 sub-slice 45).
/// </summary>
public static class SceneLoaderUiPacketServices
{
    private static L2GameUI _gameUi;

    public static void Bind(L2GameUI gameUi = null)
    {
        if (gameUi != null)
            _gameUi = gameUi;
    }

    public static L2GameUI GameUi =>
        _gameUi != null ? _gameUi : L2GameUI.Instance;
}
