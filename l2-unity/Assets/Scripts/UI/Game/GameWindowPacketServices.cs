/// <summary>
/// Root-level game UI window access with [Inject] wiring and live fallback (#142).
/// </summary>
public static class GameWindowPacketServices
{
    private static HtmlWindow _html;
    private static ShowListWindow _showList;

    public static void Bind(
        HtmlWindow html = null,
        ShowListWindow showList = null)
    {
        if (html != null)
            _html = html;
        if (showList != null)
            _showList = showList;
    }

    public static HtmlWindow Html =>
        _html != null ? _html : HtmlWindow.Instance;

    public static ShowListWindow ShowList =>
        _showList != null ? _showList : ShowListWindow.Instance;
}
