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
    }

    public void SetSkillList(List<SkillInstance> list)
    {
        if (list == null) return;
        CopyInDictionry(list);
        RefreshPassiveFlags(list);

        if (!_supportActiveSkills.IsReady)
        {
            _pendingSkills = list;
            return;
        }

        var activeSkills = list.Where(s => !s.IsPassive).ToList();
        var passiveSkills = list.Where(s => s.IsPassive).ToList();
        _supportActiveSkills.CreateSlots(activeSkills, 0);
        _supportPassiveSkills.CreateSlots(passiveSkills, 0);
    }

    public void UpdateSkillList(List<SkillInstance> list)
    {
        if (list == null) return;
        CopyInDictionry(list);
        RefreshPassiveFlags(list);

        if (!_supportActiveSkills.IsReady)
        {
            _pendingSkills = list;
            return;
        }

        var activeSkills = list.Where(s => !s.IsPassive).ToList();
        var passiveSkills = list.Where(s => s.IsPassive).ToList();
        _supportActiveSkills.CreateSlots(activeSkills, 0);
        _supportPassiveSkills.CreateSlots(passiveSkills, 0);
    }

    static void RefreshPassiveFlags(List<SkillInstance> list)
    {
        for (int i = 0; i < list.Count; i++)
            SkillPassiveRules.Refresh(list[i]);
    }
}
