public partial class HtmlWindow
{
    private void RegisterCommunityButtons()
    {
        Button btnHome = _windowEle.Q<Button>("CommunityBtnHome");
        Button btnRegion = _windowEle.Q<Button>("CommunityBtnRegion");
        Button btnClan = _windowEle.Q<Button>("CommunityBtnClan");
        Button btnMemo = _windowEle.Q<Button>("CommunityBtnMemo");
        Button btnMail = _windowEle.Q<Button>("CommunityBtnMail");

        if (btnHome != null)
            btnHome.clicked += () => UseActionCommand("_bbshome");

        if (btnRegion != null)
            btnRegion.clicked += () => UseActionCommand("_bbsregion");

        if (btnClan != null)
            btnClan.clicked += () => UseActionCommand("_bbsclan");

        if (btnMemo != null)
            btnMemo.clicked += () => UseActionCommand("_bbsmemo");

        if (btnMail != null)
            btnMail.clicked += () => UseActionCommand("_bbsmail");
    }

    public void InjectToCommunityWindow(string html)
    {
        _communityHtml = html ?? "";

        ApplyCommunityBoardMode();
        RenderHtml(_communityHtml, "Community Board");
    }
}
