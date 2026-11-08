/// <summary>
/// Root-level game UI window access with [Inject] wiring and live fallback (#142).
/// </summary>
public static class GameWindowPacketServices
{
    private static HtmlWindow _html;
    private static ShowListWindow _showList;
    private static DeadWindow _dead;
    private static StatusWindow _status;

    public static void Bind(
        HtmlWindow html = null,
        ShowListWindow showList = null,
        DeadWindow dead = null,
        StatusWindow status = null)
    {
        if (html != null)
            _html = html;
        if (showList != null)
            _showList = showList;
        if (dead != null)
            _dead = dead;
        if (status != null)
            _status = status;
    }

    public static HtmlWindow Html =>
        _html != null ? _html : HtmlWindow.Instance;

    public static ShowListWindow ShowList =>
        _showList != null ? _showList : ShowListWindow.Instance;

    public static DeadWindow Dead =>
        _dead != null ? _dead : DeadWindow.Instance;

    public static StatusWindow Status =>
        _status != null ? _status : StatusWindow.Instance;
}
