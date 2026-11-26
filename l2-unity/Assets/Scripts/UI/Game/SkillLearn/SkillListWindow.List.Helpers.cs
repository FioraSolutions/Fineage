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

    bool EnsureSingletonOwnerForSkillListApply(string phase, int skillCount)
    {
        if (_instance == this && _allSkills != null)
            return true;

        string ownerLabel = DescribeSkillListSingletonOwner();
        Debug.LogError(
            "[SkillList] " + phase + ": apply rejected — '" + gameObject.name + "' (id=" + GetInstanceID() +
            ") is not the SkillListWindow singleton; skillCount=" + skillCount + ". Owner=" + ownerLabel + ".");
        return false;
    }

    static string DescribeSkillListSingletonOwner()
    {
        if (_instance == null)
            return "none";
        return "'" + _instance.gameObject.name + "' (id=" + _instance.GetInstanceID() + ")";
    }

    bool TryCopyInDictionry(List<SkillInstance> list, string phase)
    {
        if (_allSkills == null)
        {
            int count = list != null ? list.Count : 0;
            Debug.LogError(
                "[SkillList] " + phase + ": dictionary missing on '" + gameObject.name + "' (id=" + GetInstanceID() +
                "); skillCount=" + count + ". Owner=" + DescribeSkillListSingletonOwner() + ".");
            return false;
        }

        _allSkills = list.ToDictionary(si => si.SkillID);
        return true;
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
