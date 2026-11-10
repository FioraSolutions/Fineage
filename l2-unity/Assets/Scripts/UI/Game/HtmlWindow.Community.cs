

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

    public void ToggleCommunityBoard(bool close)
    {
        if (_windowEle == null)
        {
            // Debug.LogError("HtmlWindow: _windowEle is null.");
            return;
        }

        if (close && _isCommunityBoardOpen && _windowEle.style.display == DisplayStyle.Flex)
        {
            _windowEle.style.display = DisplayStyle.None;
            _isCommunityBoardOpen = false;

            if (_mouseOverDetection != null)
                _mouseOverDetection.Disable();

            // Debug.Log("Community board closed with B");
            return;
        }

        ApplyCommunityBoardMode();
        RenderHtml(_communityHtml, "Community Board");

        _windowEle.style.display = DisplayStyle.Flex;

        if (_mouseOverDetection != null)
            _mouseOverDetection.Enable();

        BringToFront();

        // Debug.Log("Community board opened with B");
    }

    private void ApplyNormalWindowMode()
    {
        _isCommunityBoardOpen = false;

        if (_communityTopButtons != null)
            _communityTopButtons.style.display = DisplayStyle.None;

        if (_scrollView != null)
        {
            _scrollView.verticalScrollerVisibility = ScrollerVisibility.AlwaysVisible;
            _scrollView.horizontalScrollerVisibility = ScrollerVisibility.Hidden;

            _scrollView.style.top = Length.Percent(13);
            _scrollView.style.height = Length.Percent(84);
            _scrollView.style.width = Length.Percent(93);
            _scrollView.style.left = StyleKeyword.Auto;
        }

        if (_windowEle != null)
        {
            _windowEle.style.width = 340;
            _windowEle.style.height = 400;
            _windowEle.style.maxWidth = 340;
            _windowEle.style.maxHeight = StyleKeyword.None;

            _windowEle.style.bottom = StyleKeyword.Auto;
            _windowEle.style.translate = new Translate(0, 0);

            if (!_normalWindowPositionInitialized && _root != null)
            {
                OnCenterScreen(_root);
                _normalWindowPositionInitialized = true;
            }
        }
    }

    private void ApplyCommunityBoardMode()
    {
        _isCommunityBoardOpen = true;

        if (_communityTopButtons != null)
            _communityTopButtons.style.display = DisplayStyle.Flex;

        if (_scrollView != null)
        {
            _scrollView.verticalScrollerVisibility = ScrollerVisibility.Hidden;
            _scrollView.horizontalScrollerVisibility = ScrollerVisibility.Hidden;

            _scrollView.style.top = 85;
            _scrollView.style.height = Length.Percent(74);
            _scrollView.style.width = Length.Percent(93);
            _scrollView.style.left = Length.Percent(3.5f);
        }

        if (_windowEle != null)
        {
            _windowEle.style.width = 850;
            _windowEle.style.height = 650;
            _windowEle.style.maxWidth = 850;
            _windowEle.style.maxHeight = 650;

            _windowEle.style.bottom = StyleKeyword.Auto;
            _windowEle.style.translate = new Translate(0, 0);

            if (!_communityBoardPositionInitialized && _root != null)
            {
                OnCenterScreen(_root);
                _communityBoardPositionInitialized = true;
            }
        }

        SetWindowTitle("Community Board");
    }

}
