using UnityEngine.UIElements;

public partial class SkillbarMain : AbstractSkillbar
{
    public SkillbarMain(VisualElement skillbarWindowElement, int skillbarIndex, int page, bool horizontalBar)
    : base(skillbarWindowElement, skillbarIndex, page, horizontalBar)
    {
    }

    protected override void UpdateVisuals()
    {
        _expandButton = _windowEle.Q<Button>("ExpandBtn");
        _expandButton.AddManipulator(new ButtonClickSoundManipulator(_expandButton));
        _expandButton.RegisterCallback<ClickEvent>((evt) => SkillbarPacketServices.Skillbar.AddSkillbar());

        _minimizeButton = _windowEle.Q<Button>("MinimizeBtn");
        _minimizeButton.AddManipulator(new ButtonClickSoundManipulator(_minimizeButton));
        _minimizeButton.RegisterCallback<ClickEvent>((evt) => SkillbarPacketServices.Skillbar.ResetSkillbar());

        _rotateButton = _windowEle.Q<Button>("RotateBtn");
        _rotateButton.AddManipulator(new ButtonClickSoundManipulator(_rotateButton));

        _lockButton = _windowEle.Q<Button>("LockBtn");
        _lockButton.AddManipulator(new ButtonClickSoundManipulator(_lockButton));

        _tooltipButton = _windowEle.Q<Button>("TooltipBtn");
        _tooltipButton.AddManipulator(new ButtonClickSoundManipulator(_tooltipButton));

        _rotateButton.RegisterCallback<ClickEvent>((evt) => HandleRotateClick());
        _lockButton.RegisterCallback<ClickEvent>((evt) => HandleLockClick());
        _tooltipButton.RegisterCallback<ClickEvent>((evt) => HandleTooltipClick());

        UpdateExpandInput(0);
    }

    public void UpdateExpandInput(int mode)
    {
        if (mode == 0)
        {
            _minimizeButton.style.display = DisplayStyle.None;
            _expandButton.style.display = DisplayStyle.Flex;
            return;
        }

        _minimizeButton.style.display = DisplayStyle.Flex;
        _expandButton.style.display = DisplayStyle.None;
    }

    private void HandleTooltipClick()
    {
        SkillbarPacketServices.Skillbar.ToggleDisableTooltip();
    }
}
