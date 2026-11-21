using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine.UIElements;
using UnityEngine;

public partial class ActiveSkillsHide
{
    private void HidePanelActiveSkillsPanel(List<SkillInstance> list,  VisualElement allContent)
    {
        var activeSkills = list.Where(s => !s.IsMagic()).ToList();

        if (activeSkills.Count == 0)
        {
            allContent.style.display = DisplayStyle.None;
        }
    }

    private void HidePanelMagicSkillsPanel(List<SkillInstance> list, VisualElement allContent)
    {
        var magicSkills = list.Where(s => s.IsMagic()).ToList();

        if (magicSkills.Count == 0)
        {
            allContent.style.display = DisplayStyle.None;
        }
    }

    public void CreatePhysicalSlots(List<SkillInstance> list , VisualElement rowPhysical)
    {
        if (rowPhysical != null)
        {
            var boxPanel = ToolTipsUtils.CloneOne(_templateBoxPanel);
            SetWidthBoxPanel(boxPanel, skillsPerPanel);
            boxPanel.style.alignSelf = Align.FlexStart;
            var panels = base.CreateSlots(list, _templatePanel8x1, _templateSlotSkill, boxPanel);

            _boxPanelsPhysical.Add(panels);
            rowPhysical.Add(panels);
        }
        else
        {
            // Debug.LogWarning("ActiveSkillsHide > not found root panels ");
        }
    }

    public void CreateMagicalSlots(List<SkillInstance> list, VisualElement rowMagical)
    {
        if (rowMagical != null)
        {
            var boxPanel = ToolTipsUtils.CloneOne(_templateBoxPanel);
            SetWidthBoxPanel(boxPanel, skillsPerPanel);
            boxPanel.style.alignSelf = Align.FlexStart;
            var panels = base.CreateSlots(list, _templatePanel8x1, _templateSlotSkill, boxPanel);
            _boxPanelsMagic.Add(panels);
            rowMagical.Add(panels);
        }
        else
        {
            // Debug.LogWarning("ActiveSkillsHide > not found root panels ");
        }
    }

    private void RegisterClickButtonPhysical(VisualElement rootElement)
    {
        rootElement.Q<Button>("DF_Button")?.RegisterCallback<ClickEvent>(evt => ClickDfPhysical((Button)evt.target, _arrDfActiveSelect));
    }

    private void RegisterClickButtonMagic(VisualElement rootElement)
    {
        rootElement.Q<Button>("DF_Button_Magic")?.RegisterCallback<ClickEvent>(evt => ClickDfMagic((Button)evt.target, _arrDfActiveSelect));
    }

    public void ClickDfPhysical(Button btn, int[] arrDfSelect)
    {
        _rowPhysical ??= _activeSkillPanel.Q(_rowNamePhysical);
        ClickDf(btn, arrDfSelect, _rowPhysical, 0);
    }

    public void ClickDfMagic(Button btn, int[] arrDfSelect)
    {
        _rowMagical ??= _activeSkillPanel.Q(_rowNameMagical);
        ClickDf(btn, arrDfSelect, _rowMagical, 1);
    }

    static List<VisualElement> CollectGridRows(List<VisualElement> boxPanels)
    {
        var rows = new List<VisualElement>();
        if (boxPanels == null)
            return rows;

        for (int i = 0; i < boxPanels.Count; i++)
        {
            VisualElement box = boxPanels[i];
            if (box == null)
                continue;
            for (int c = 0; c < box.childCount; c++)
                rows.Add(box[c]);
        }

        return rows;
    }

    void EnsureSectionRowsExpanded(VisualElement rowPhysical, VisualElement rowMagical)
    {
        if (rowPhysical != null)
            rowPhysical.style.display = DisplayStyle.Flex;
        if (rowMagical != null)
            rowMagical.style.display = DisplayStyle.Flex;
        if (_arrDfActiveSelect != null)
        {
            if (_arrDfActiveSelect.Length > 0)
                _arrDfActiveSelect[0] = 1;
            if (_arrDfActiveSelect.Length > 1)
                _arrDfActiveSelect[1] = 1;
        }

        SyncExpandButton(_activeSkillPanel?.Q<Button>("DF_Button"), 0);
        SyncExpandButton(_activeSkillPanel?.Q<Button>("DF_Button_Magic"), 1);
    }

    void SyncExpandButton(Button button, int index)
    {
        if (button == null || _arrDfActiveSelect == null || index < 0 || index >= _arrDfActiveSelect.Length)
            return;
        bool expanded = _arrDfActiveSelect[index] != 0;
        ChangeDfBox(button, L2ExpandCollapseUiAssets.CollapseButton);
        if (!expanded)
            ChangeDfBox(button, L2ExpandCollapseUiAssets.ExpandButton);
    }
}
