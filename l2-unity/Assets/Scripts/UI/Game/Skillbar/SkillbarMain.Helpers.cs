
using UnityEngine.UIElements;

public partial class SkillbarMain : AbstractSkillbar
{
    public void ToggleDisableTooltip()
    {
        if (!SkillbarPacketServices.Skillbar.TooltipDisabled)
        {
            if (_tooltipButton.ClassListContains("min"))
            {
                _tooltipButton.RemoveFromClassList("min");

            }
        }
        else
        {
            if (!_tooltipButton.ClassListContains("min"))
            {

                _tooltipButton.AddToClassList("min");
            }
        }
    }

    private void HandleLockClick()
    {
        SkillbarPacketServices.Skillbar.ToggleLockSkillBar();
    }

    public void ToggleLockSkillBar()
    {
        if (!SkillbarPacketServices.Skillbar.Locked)
        {
            if (_lockButton.ClassListContains("locked"))
            {
                _lockButton.RemoveFromClassList("locked");
            }
        }
        else
        {
            if (!_lockButton.ClassListContains("locked"))
            {
                _lockButton.AddToClassList("locked");
            }
        }
    }

    private void HandleRotateClick()
    {
        SkillbarPacketServices.Skillbar.ToggleRotate();
    }
}
