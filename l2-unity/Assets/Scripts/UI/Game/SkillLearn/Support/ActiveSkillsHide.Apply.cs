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
}
