using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class SkillListWindow : L2PopupWindow
{
    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);


        yield return new WaitForEndOfFrame();

        var dragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);

        var content = (VisualElement)GetElementById("content");
        content?.AddToClassList("skills-panel");
        VisualElement tabView = content?.Q("InventoryTabView") ?? content;

        _builderTabs.InitContentTabs(new string[3] { _activeName, _passiveName , _learnName });
        _builderTabs.CreateTabs(tabView, _tabTemplate, _tabHeaderTemplate);

        _supportActiveSkills.SetActiveSkillTemplate(_templateActiveSkill , _templateBoxPanel , _templateSkillsRow8x1 , _templateSlotSkill);
        _supportPassiveSkills.SetPassiveSkillTemplate(_templatePassiveSkill, _templateBoxPanel, _templateSkillsRow8x1, _templateSlotSkill);

        _builderTabs.EventSwitchOut += OnSwitchEventOut;

        VisualElement element = _builderTabs.GetActiveContent();
        _supportActiveSkills.GetOrCreateTab(element);

        RegisterCloseWindowEvent("btn-close-frame");
        RegisterCloseWindowEventByName("CloseButton");
        RegisterClickWindowEvent(_windowEle, dragArea);
        OnCenterScreen(_root);

        if (_pendingSkills != null)
        {
            SetSkillList(_pendingSkills);
            _pendingSkills = null;
        }
    }

    public void SetSkillList(List<SkillInstance> list)
    {
        if (list == null) return;
        CopyInDictionry(list);

        if (!_supportActiveSkills.IsReady)
        {
            _pendingSkills = list;
            return;
        }

        var activeSkills = list.Where(s => !s.IsPassive).ToList();
        var passiveSkills = list.Where(s => s.IsPassive).ToList();
        // MainWnd SkillsHost is 256px — 6 Interlude-sized slots fit; 7 forces a horizontal scroller.
        _supportActiveSkills.CreateSlots(activeSkills, 6);
        _supportPassiveSkills.CreateSlots(passiveSkills, 6);
    }

    public void UpdateSkillList(List<SkillInstance> list)
    {
        if (list == null) return;
        CopyInDictionry(list);

        if (!_supportActiveSkills.IsReady)
        {
            _pendingSkills = list;
            return;
        }

        var activeSkills = list.Where(s => !s.IsPassive).ToList();
        var passiveSkills = list.Where(s => s.IsPassive).ToList();
        if (_supportActiveSkills.HasSlots)
        {
            _supportActiveSkills.UpdateSlots(activeSkills);
        }
        else
        {
            _supportActiveSkills.CreateSlots(activeSkills, 6);
        }

        _supportPassiveSkills.CreateSlots(passiveSkills, 6);
    }

    public SkillInstance GetSkillInstanceBySkillId(int skillId)
    {
        if (_allSkills == null)
            return null;

        _allSkills.TryGetValue(skillId, out var skill);
        return skill; 
    }

    readonly CooldownAnimationService _cooldownAnimationService = new CooldownAnimationService();

    /// <summary>Reuse overlay on the skills window slot (same frames as the skillbar).</summary>
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
        if (_allSkills.Count == 0) _allSkills.Clear();

        _allSkills = list.ToDictionary(si => si.SkillID);
    }


    // }

    // _activeName = "Active";
    //_passiveName = "Passive";
    // _learnName = "Learn Skill";
    private void OnSwitchEventOut(ITab tab, bool isTrade)
    {
        var tabName = tab.GetTabName();
        var element = _builderTabs.GetActiveContent();
        switch (tabName)
        {
            case _activeName:
                _supportActiveSkills.GetOrCreateTab(element);
                break;

            case _passiveName:
                _supportPassiveSkills.GetOrCreateTab(element);
                break;

            case _learnName:

                break;
        }


    }



 


}
