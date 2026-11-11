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

    public void SetWidthBoxPanel(VisualElement _templateBoxPanel , int sizeCell)
    {
        float  width = (sizeCell == 6) ? _default6CellWidth : _default7CellWidth;

        if (_templateBoxPanel != null)
        {
           
            _templateBoxPanel.style.width = width;
        }
        else
        {
            // Debug.LogWarning("ActiveSkillsHide > _templateBoxPanel is null, cannot set width");
        }
    }
}
