using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SkillListWindow : L2PopupWindow
{
    public SkillInstance GetSkillInstanceBySkillId(int skillId)
    {
        if (_allSkills == null)
            return null;

        _allSkills.TryGetValue(skillId, out var skill);
        return skill; 
    }

    public void ShowCooldown(int skillId, int reuseDelayMs)
    {
        if (reuseDelayMs <= 0 || !gameObject.activeInHierarchy)
            return;
        SkillSlot slot = _supportActiveSkills != null ? _supportActiveSkills.GetSlot(skillId) : null;
        if (slot == null || slot.GetReuseElement() == null)
            return;
        StartCoroutine(_cooldownAnimationService.CooldownCoroutine(
            slot.GetReuseElement(), slot.GetRechargeMaskElement(), reuseDelayMs));
    }

    private void CopyInDictionry(List<SkillInstance> list)
    {
        if (_allSkills == null)
            _allSkills = new Dictionary<int, SkillInstance>();

        _allSkills = list.ToDictionary(si => si.SkillID);
    }

    private void OnSwitchEventOut(ITab tab, bool isTrade)
    {
        var tabName = tab.GetTabName();
        var element = _builderTabs.GetActiveContent();
        switch (tabName)
        {
            case _activeName:
                _supportActiveSkills.GetOrCreateTab(element);
                FlushPendingSkillList("OnSwitchEventOut.Active");
                break;

            case _passiveName:
                _supportPassiveSkills.GetOrCreateTab(element);
                FlushPendingSkillList("OnSwitchEventOut.Passive");
                break;
        }


    }
}
