using System;

public partial class SystemMenuWindow : L2PopupWindow
{
    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);
        _root = root;

        // root.Add(_windowEle);

        yield return new WaitForEndOfFrame();

        VisualElement exitButton = GetElementByClass("exit-btn");
        exitButton.AddManipulator(new ButtonClickSoundManipulator(exitButton));
        exitButton.RegisterCallback<ClickEvent>((evt) => HandleExitButtonClick());
        VisualElement optionsButton = GetElementByClass("options-btn");
        if (optionsButton != null)
        {
            optionsButton.AddManipulator(new ButtonClickSoundManipulator(optionsButton));
            optionsButton.RegisterCallback<ClickEvent>((evt) => HandleOptionsButtonClick());
        }
        VisualElement restartButton = GetElementByClass("restart-btn");
        restartButton.AddManipulator(new ButtonClickSoundManipulator(restartButton));
        restartButton.RegisterCallback<ClickEvent>((evt) => HandleRestartButtonClick());

        _windowHeight = _windowEle.worldBound.height;
        RegisterClickWindowEvent(_windowEle, null);

        HideWindow();
    }

    private void HandleOptionsButtonClick()
    {
        HideWindow();
        if (_options != null && _options.parent != null) { _options.RemoveFromHierarchy(); return; }
        _options = new OptionsPanel();
        (_root.panel != null ? _root.panel.visualTree : _root).Add(_options);
    }

    private void HandleExitButtonClick()
    {
        HideWindow();
        SystemUIPacketServices.Exit.OpenWindow(true);
    }

    private void HandleRestartButtonClick()
    {
        HideWindow();
        SystemUIPacketServices.Exit.OpenWindow(false);
    }

    public override void ToggleHideWindow()
    {
        Vector2 basePosition = HudCorePacketServices.Menu.GetWindowPosition();
        _windowEle.transform.position = new Vector2(basePosition.x, basePosition.y - _windowHeight);
        base.ToggleHideWindow();
    }

    public override void ShowWindow()
    {
        base.ShowWindow();
        SystemUIPacketServices.Audio.PlayUISound("window_open");
        SystemUIPacketServices.Ui.WindowOpened(this);
    }

    public override void HideWindow()
    {
        base.HideWindow();
        SystemUIPacketServices.Audio.PlayUISound("window_close");
        SystemUIPacketServices.Ui.WindowClosed(this);
    }
}
