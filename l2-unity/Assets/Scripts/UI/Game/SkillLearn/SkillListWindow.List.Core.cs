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

        if (!EnsureSkillListTemplatesLoaded("BuildWindow"))
            yield break;

        FinishSkillListTabChrome(tabView, "BuildWindow");
        TryApplyOrphanedSkillList();
    }

    public void SetSkillList(List<SkillInstance> list) => RequestSkillListApply(list, "SetSkillList");

    public void UpdateSkillList(List<SkillInstance> list) => RequestSkillListApply(list, "UpdateSkillList");

    void RequestSkillListApply(List<SkillInstance> list, string phase)
    {
        if (list == null)
        {
            Debug.LogError("[SkillList] " + phase + ": null list.");
            return;
        }

        if (!EnsureSingletonOwnerForSkillListApply(phase, list.Count))
            return;

        if (!TryCopyInDictionry(list, phase))
            return;

        RefreshPassiveFlags(list);

        if (!CanApplySkillListToPanels(out string deferReason))
        {
            _pendingSkills = list;
            LogSkillListDeferOnce(deferReason, list.Count, phase);
            return;
        }

        ApplySkillListToPanels(list, phase);
    }

    bool EnsureSkillListTemplatesLoaded(string phase)
    {
        if (_templateActiveSkill == null || _templatePassiveSkill == null || _templateBoxPanel == null ||
            _templateSkillsRow8x1 == null || _templateSlotSkill == null)
        {
            Debug.LogError(
                "[SkillList] " + phase + " aborted: missing template active=" + (_templateActiveSkill == null) +
                " passive=" + (_templatePassiveSkill == null) + " box=" + (_templateBoxPanel == null) +
                " row8x1=" + (_templateSkillsRow8x1 == null) + " slot=" + (_templateSlotSkill == null));
            return false;
        }

        _supportActiveSkills.SetActiveSkillTemplate(
            _templateActiveSkill, _templateBoxPanel, _templateSkillsRow8x1, _templateSlotSkill);
        _supportPassiveSkills.SetPassiveSkillTemplate(
            _templatePassiveSkill, _templateBoxPanel, _templateSkillsRow8x1, _templateSlotSkill);
        return true;
    }

    void FinishSkillListTabChrome(VisualElement tabView, string phase)
    {
        _builderTabs.InitContentTabs(new string[2] { _activeName, _passiveName });
        _builderTabs.CreateTabs(tabView, _tabTemplate, _tabHeaderTemplate);
        _builderTabs.EventSwitchOut += OnSwitchEventOut;

        VisualElement element = _builderTabs.GetActiveContent();
        _supportActiveSkills.GetOrCreateTab(element);

        RegisterCloseWindowEvent("btn-close-frame");
        RegisterCloseWindowEventByName("CloseButton");
        RegisterClickWindowEvent(_windowEle, GetElementByClass("drag-area"));
        OnCenterScreen(_root);

        MarkSkillListTabsReady();
        FlushPendingSkillList(phase);
        EnsureSkillSlotsVisibleOnOpen(phase);
    }

    void MarkSkillListTabsReady()
    {
        _skillListTabsReady = true;
        _loggedDeferReason = null;
    }

    bool CanApplySkillListToPanels(out string deferReason)
    {
        if (!_skillListTabsReady)
        {
            deferReason = "tabs not built";
            return false;
        }

        if (_templateSlotSkill == null)
        {
            deferReason = "slot template missing";
            return false;
        }

        if (_builderTabs == null)
        {
            deferReason = "tab builder missing";
            return false;
        }

        if (_builderTabs.GetTabContent(_activeName) == null)
        {
            deferReason = "Active tab host missing";
            return false;
        }

        deferReason = null;
        return true;
    }

    void LogSkillListDeferOnce(string deferReason, int count, string phase)
    {
        if (string.IsNullOrEmpty(deferReason) || deferReason == _loggedDeferReason)
            return;

        _loggedDeferReason = deferReason;
        Debug.LogError(
            "[SkillList] " + phase + " deferred " + count + " skills: " + deferReason + ".");
    }

    void ApplySkillListToPanels(List<SkillInstance> list, string phase)
    {
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
        {
            _pendingSkills = list;
            Debug.LogError(
                "[SkillList] " + phase + " re-deferred " + list.Count +
                " skills: slots created but none visible in hierarchy.");
        }
    }

    void FlushPendingSkillList(string phase)
    {
        TryFlushPendingSkillList(phase);
    }

    void TryFlushPendingSkillList(string phase = "TryFlushPendingSkillList")
    {
        if (_pendingSkills == null)
            return;

        if (!CanApplySkillListToPanels(out string deferReason))
        {
            LogSkillListDeferOnce(deferReason, _pendingSkills.Count, phase);
            return;
        }

        List<SkillInstance> pending = _pendingSkills;
        _pendingSkills = null;
        ApplySkillListToPanels(pending, phase);
    }

    public void EnsureSkillSlotsVisibleOnOpen(string phase)
    {
        if (_pendingSkills != null)
        {
            FlushPendingSkillList(phase);
            return;
        }

        if (_allSkills == null || _allSkills.Count == 0)
            return;

        if (!CanApplySkillListToPanels(out _))
            return;

        int activeVisible = _supportActiveSkills.CountVisibleSkillSlots();
        int passiveVisible = _supportPassiveSkills.CountVisibleSkillSlots();
        if (activeVisible + passiveVisible > 0)
            return;

        ApplySkillListToPanels(_allSkills.Values.ToList(), phase + ".RemountOnOpen");
    }

    static void RefreshPassiveFlags(List<SkillInstance> list)
    {
        for (int i = 0; i < list.Count; i++)
            SkillPassiveRules.Refresh(list[i]);
    }

#if UNITY_EDITOR
    public int PendingSkillCountForEditModeTests =>
        _pendingSkills != null ? _pendingSkills.Count : 0;

    public int AppliedSkillCountForEditModeTests =>
        _allSkills != null ? _allSkills.Count : 0;

    public void EditorFinishTabBuildForTests(VisualElement tabView)
    {
        if (!EnsureSkillListTemplatesLoaded("EditorFinishTabBuildForTests"))
            return;
        FinishSkillListTabChrome(tabView, "EditorFinishTabBuildForTests");
    }

    public int ActiveVisibleSkillSlotsForEditModeTests =>
        _supportActiveSkills != null ? _supportActiveSkills.CountVisibleSkillSlots() : 0;

    public int PassiveVisibleSkillSlotsForEditModeTests =>
        _supportPassiveSkills != null ? _supportPassiveSkills.CountVisibleSkillSlots() : 0;
#endif
}