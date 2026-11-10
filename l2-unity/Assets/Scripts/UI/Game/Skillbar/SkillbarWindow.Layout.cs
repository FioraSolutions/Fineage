using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SkillbarWindow
{
    private void InitializeSkillbars()
    {
        for (int x = 0; x < 2; x++)
        {
            for (int i = 0; i < PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT; i++)
            {
                bool horizontalBar = x == 0;
                var skillbar = CreateSkillbar(i, horizontalBar);
                StartCoroutine(BuildSkillbarWindow(skillbar, horizontalBar));
                _skillbars.Add(skillbar);
            }
        }
    }

    private AbstractSkillbar CreateSkillbar(int index, bool horizontalBar)
    {
        return index == 0 ?
            new SkillbarMain(_windowEle, index, index, horizontalBar) :
            new SkillbarMin(_windowEle, index, index, horizontalBar);
    }

    private IEnumerator BuildSkillbarWindow(AbstractSkillbar skillbar, bool horizontalBar)
    {
        var template = horizontalBar ? _skillbarHorizontalTemplate : _skillbarVerticalTemplate;
        var container = horizontalBar ? _skillbarContainerHorizontal : _skillbarContainerVertical;
        yield return skillbar.BuildWindow(template, container);
        // ShortCutInit may have already filled PlayerShortcuts while bars were still building —
        // paint now that slots exist (Mobius sends ShortCutInit during EnterWorld).
        if (IsReady)
            UpdateAllShortcuts();
    }

    public void AddSkillbar()
    {
        for (int x = 0; x < 2; x++)
        {
            if (_expandedSkillbarCount >= PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT - 2)
            {
                ((SkillbarMain)_skillbars[x * PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT]).UpdateExpandInput(1);
            }
        }

        _expandedSkillbarCount++;

        ExpandSkillbar();
    }

    public void ResetSkillbar()
    {
        _expandedSkillbarCount = 0;

        for (int x = 0; x < 2; x++)
        {
            ((SkillbarMain)_skillbars[x * PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT]).UpdateExpandInput(0);
        }

        MinimizeSkillbar();
    }

    private void ExpandSkillbar()
    {
        // Stop all skillbar minimize animations
        _minimizeCoroutines.ForEach((c) => StopCoroutine(c));
        _minimizeCoroutines.Clear();

        for (int x = 0; x < 2; x++)
        {
            // Expand the next skillbar
            SkillbarMin skillbar = (SkillbarMin)_skillbars[_expandedSkillbarCount + PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT * x];
            _expandCoroutines.Add(StartCoroutine(skillbar.Expand()));

            // Hide skillbars that are not supposed to be visible
            for (int i = _expandedSkillbarCount + 1; i < PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT; i++)
            {
                _skillbars[i + PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT * x].HideBar();
            }
        }
    }

    private void MinimizeSkillbar()
    {
        // Stop all skillbar expand animations
        _expandCoroutines.ForEach((c) => StopCoroutine(c));
        _expandCoroutines.Clear();

        // Minimize all skillbars
        for (int x = 0; x < 2; x++)
        {
            for (int i = 1; i < PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT; i++)
            {
                SkillbarMin skillbar = (SkillbarMin)_skillbars[i + PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT * x];
                _minimizeCoroutines.Add(StartCoroutine(skillbar.Minimize()));
            }
        }
    }

    public void ToggleRotate()
    {
        _vertical = !_vertical;

        if (_vertical)
        {
            _skillbarContainerHorizontal.style.display = DisplayStyle.None;
            _skillbarContainerVertical.style.display = DisplayStyle.Flex;
        }
        else
        {
            _skillbarContainerHorizontal.style.display = DisplayStyle.Flex;
            _skillbarContainerVertical.style.display = DisplayStyle.None;
        }
    }

    public void OnPageChanged(int skillbarIndex, int page)
    {
        for (int x = 0; x < 2; x++)
        {
            _skillbars[skillbarIndex + PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT * x].ChangePage(page);
        }

        Shortcuts.UpdatePageMapping(skillbarIndex, page);

        UpdateAllShortcuts();
    }

    public void ToggleLockSkillBar()
    {
        Locked = !Locked;

        for (int x = 0; x < 2; x++)
        {
            ((SkillbarMain)_skillbars[x * PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT]).ToggleLockSkillBar();
        }
    }

    public void ToggleDisableTooltip()
    {
        TooltipDisabled = !TooltipDisabled;

        for (int x = 0; x < 2; x++)
        {
            ((SkillbarMain)_skillbars[x * PlayerShortcuts.MAXIMUM_SKILLBAR_COUNT]).ToggleDisableTooltip();
        }
    }
}
