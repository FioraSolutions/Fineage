

public partial class HtmlWindow
{
    public void InjectToWindow(string html)
    {
        _normalHtml = html ?? "";

        ApplyNormalWindowMode();
        RenderHtml(_normalHtml, "");
    }

    public void ShowNpcDialog(int npcObjectId, int npcId, string html)
    {
        NetNpcSession.OpenDialog(npcObjectId);
        string titled = html ?? "";
        if (string.IsNullOrEmpty(ExtractTitle(titled)))
        {
            string name = NetNpcSession.NameOf(npcObjectId);
            if (string.IsNullOrEmpty(name))
                name = "NPC";
            titled = "<title>" + name + "</title>" + titled;
        }

        _ = npcId;
        InjectToWindow(titled);
        Show();
    }

    private void RenderHtml(string html, string defaultTitle)
    {
        if (_content == null)
            return;

        _content.Clear();

        string title = ExtractTitle(html);
        SetWindowTitle(!string.IsNullOrEmpty(title) ? title : defaultTitle);

        L2HtmlRenderer renderer = new L2HtmlRenderer(_content, OnHtmlAction);
        renderer.Render(html);
    }

    private string ExtractTitle(string html)
    {
        if (string.IsNullOrEmpty(html))
            return null;

        var match = Regex.Match(
            html,
            @"<title>(.*?)</title>",
            RegexOptions.IgnoreCase | RegexOptions.Singleline
        );

        if (match.Success)
            return match.Groups[1].Value.Trim();

        return null;
    }

    private void OnHtmlAction(string action)
    {
        if (string.IsNullOrEmpty(action))
            return;

        ZoneCommands.Send(new RequestBypassToServerCommand(action));
    }

    public void UseActionCommand(string command)
    {
        OnHtmlAction(command);
    }

    private void SetWindowTitle(string title)
    {
        if (_titleLabel != null)
            _titleLabel.text = title;
    }

}
