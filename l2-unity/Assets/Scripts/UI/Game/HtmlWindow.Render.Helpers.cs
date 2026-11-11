public partial class HtmlWindow
{
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
