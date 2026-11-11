using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SkillbarMin : AbstractSkillbar
{
    public SkillbarMin(VisualElement skillbarWindowElement, int skillbarIndex, int page, bool horizontalBar)
    : base(skillbarWindowElement, skillbarIndex, page, horizontalBar)
    {
    }

    protected override void UpdateVisuals()
    {
        _windowEle.Q<VisualElement>("Footer").style.display = DisplayStyle.None;
        _windowEle.Q<VisualElement>("ExpandBtn").style.display = DisplayStyle.None;
        _windowEle.Q<VisualElement>("MinimizeBtn").style.display = DisplayStyle.None;

        if (_horizontalBar)
        {
            _windowEle.style.marginBottom = -3;
        }
        else
        {
            _windowEle.style.marginRight = -3;
        }

        HideBar();
    }
}
