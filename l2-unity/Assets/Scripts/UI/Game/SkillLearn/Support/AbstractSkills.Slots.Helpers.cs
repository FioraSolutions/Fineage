using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using static L2Slot;
using UnityEngine;

public partial class AbstractSkills
{
    private int GetCountPanel(int totalSkills , int skillsPerPanel)
    {
        if (totalSkills == 0)
        {
            return  1;
        }
        else
        {
            return  (int)Math.Ceiling((double)totalSkills / skillsPerPanel);
        }
    }

    private void AddSlot(SkillSlot skillSlot)
    {
        if (!_allSlots.ContainsKey(skillSlot.Id))
        {
            _allSlots.Add(skillSlot.Id, skillSlot);
        }
        else
        {
            // Debug.Log("AbstractSkills->AddSlot: Dublication id slot");
        }

    }

    protected int CalculatePanelCount(List<SkillInstance> list)
    {
        //int skillsPerPanel = 7;
        int totalSkills = list.Count;
        int panelCount = (int)Math.Ceiling((double)totalSkills / skillsPerPanel);
        return panelCount;
    }

    protected void ResolveSkillsPerPanel(VisualElement widthSource, int sizeCellOverride)
    {
        skillsPerPanel = L2SkillsPanelLayout.ResolveGridColumns(widthSource, sizeCellOverride);
    }

    public void SetWidthBoxPanel(VisualElement boxPanel, int columns)
    {
        if (boxPanel == null)
        {
            Debug.LogError("[Skills] SetWidthBoxPanel: boxPanel is null.");
            return;
        }

        boxPanel.style.width = L2SkillsPanelLayout.ComputePanelWidth(columns);
        boxPanel.style.maxWidth = L2SkillsPanelLayout.ComputePanelWidth(columns);
    }

    protected void ApplySkillsRowChrome(VisualElement rowRoot, int columns)
    {
        if (rowRoot == null)
            return;

        rowRoot.AddToClassList("skills-grid-row");
        rowRoot.style.height = L2SkillsPanelLayout.RowHeight;
        rowRoot.style.minHeight = L2SkillsPanelLayout.RowHeight;
        rowRoot.style.maxHeight = L2SkillsPanelLayout.RowHeight;

        string framePath = L2SkillsPanelLayout.RowFrameResourceForColumns(columns);
        Texture2D frame = L2UiResources.LoadTexture(framePath);
        if (frame != null)
            rowRoot.style.backgroundImage = new StyleBackground(frame);
    }
}
