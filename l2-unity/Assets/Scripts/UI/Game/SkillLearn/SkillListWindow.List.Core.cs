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

        if (_templateActiveSkill == null || _templatePassiveSkill == null || _templateBoxPanel == null ||
            _templateSkillsRow8x1 == null || _templateSlotSkill == null)
        {
            Debug.LogError(
                "[SkillList] BuildWindow aborted: missing template active=" + (_templateActiveSkill == null) +
                " passive=" + (_templatePassiveSkill == null) + " box=" + (_templateBoxPanel == null) +
                " row8x1=" + (_templateSkillsRow8x1 == null) + " slot=" + (_templateSlotSkill == null));
            yield break;
        }

        _builderTabs.InitContentTabs(new string[2] { _activeName, _passiveName });
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

        TryApplyOrphanedSkillList();
    }

    public void SetSkillList(List<SkillInstance> list)
    {
        if (list == null)
        {
            Debug.LogError("[SkillList] SetSkillList: null list.");
            return;
        }

        CopyInDictionry(list);
        RefreshPassiveFlags(list);

        if (!_supportActiveSkills.IsReady)
        {
            _pendingSkills = list;
            return;
        }

        ApplySkillListToPanels(list, "SetSkillList");
    }

    public void UpdateSkillList(List<SkillInstance> list)
    {
        if (list == null)
        {
            Debug.LogError("[SkillList] UpdateSkillList: null list.");
            return;
        }

        CopyInDictionry(list);
        RefreshPassiveFlags(list);

        if (!_supportActiveSkills.IsReady)
        {
            _pendingSkills = list;
            return;
        }

        ApplySkillListToPanels(list, "UpdateSkillList");
    }

    void ApplySkillListToPanels(List<SkillInstance> list, string phase)
    {
        if (_templateSlotSkill == null)
        {
            Debug.LogError("[SkillList] " + phase + ": slot template null; cannot create skill slots.");
            _pendingSkills = list;
            return;
        }

        if (_builderTabs == null)
        {
            Debug.LogError("[SkillList] " + phase + ": tab builder null; list count=" + list.Count);
            _pendingSkills = list;
            return;
        }

        MountSkillTabPanels(phase);

        var activeSkills = list.Where(s => !s.IsPassive).ToList();
        var passiveSkills = list.Where(s => s.IsPassive).ToList();
        _supportActiveSkills.CreateSlots(activeSkills, 0);
        _supportPassiveSkills.CreateSlots(passiveSkills, 0);
        ValidateSkillSlotsAfterApply(phase, list, activeSkills, passiveSkills);
    }

    void MountSkillTabPanels(string phase)
    {
        VisualElement activeHost = _builderTabs.GetTabContent(_activeName);
        if (activeHost == null)
            Debug.LogError("[SkillList] " + phase + ": Active tab content host missing.");
        else
            _supportActiveSkills.GetOrCreateTab(activeHost);

        VisualElement passiveHost = _builderTabs.GetTabContent(_passiveName);
        if (passiveHost == null)
            Debug.LogError("[SkillList] " + phase + ": Passive tab content host missing.");
        else
            _supportPassiveSkills.GetOrCreateTab(passiveHost);
    }

    void ValidateSkillSlotsAfterApply(
        string phase,
        List<SkillInstance> list,
        List<SkillInstance> activeSkills,
        List<SkillInstance> passiveSkills)
    {
        int activeVisible = _supportActiveSkills.CountVisibleSkillSlots();
        int passiveVisible = _supportPassiveSkills.CountVisibleSkillSlots();

        if (activeSkills.Count > 0 && activeVisible == 0)
        {
            Debug.LogError(
                "[SkillList] " + phase + ": " + activeSkills.Count + " active skills but 0 visible slots; " +
                "panelMounted=" + _supportActiveSkills.IsMountedInHierarchy() + " display=" +
                _supportActiveSkills.DescribePanelDisplayChain());
        }

        if (passiveSkills.Count > 0 && passiveVisible == 0)
        {
            Debug.LogError(
                "[SkillList] " + phase + ": " + passiveSkills.Count + " passive skills but 0 visible slots; " +
                "panelMounted=" + _supportPassiveSkills.IsMountedInHierarchy());
        }

        if (list.Count > 0 && activeVisible + passiveVisible == 0)
            _pendingSkills = list;
    }

    void TryFlushPendingSkillList()
    {
        if (_pendingSkills == null)
            return;
        if (!_supportActiveSkills.IsReady)
        {
            Debug.LogError(
                "[SkillList] pending skill list (" + _pendingSkills.Count +
                ") not flushed: ActiveSkillsHide not ready (panel or templates missing).");
            return;
        }

        List<SkillInstance> pending = _pendingSkills;
        _pendingSkills = null;
        ApplySkillListToPanels(pending, "TryFlushPendingSkillList");
    }

    static void RefreshPassiveFlags(List<SkillInstance> list)
    {
        for (int i = 0; i < list.Count; i++)
            SkillPassiveRules.Refresh(list[i]);
    }
}