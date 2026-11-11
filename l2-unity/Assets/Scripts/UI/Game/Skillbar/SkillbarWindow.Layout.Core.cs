using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;
using VContainer;

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
}
