using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UIElements;
using static L2Slot;
using UnityEngine;

public partial class AbstractSkills
{
    protected void UpdateSkills(List<SkillInstance> added, List<SkillInstance> removed, List<SkillInstance> allList , List<VisualElement> rowPanels, VisualTreeAsset templateSlotSkill , VisualTreeAsset templatePanel8x1, VisualElement boxPanel)
    {
        UpdateSkillInfo(allList);
        RemoveSkill(removed);

        AddSkill(added, rowPanels, templateSlotSkill, templatePanel8x1 , boxPanel);
        //RemoveEmptyPanels(boxPanel);
        RearrangeSkillSlots(rowPanels,  boxPanel);
        RemoveEmptyPanels(boxPanel);
    }

    private void UpdateSkillInfo(List<SkillInstance> allList)
    {
        foreach (SkillInstance skill in allList)
        {
            SkillSlot slot;

            _allSlots.TryGetValue(skill.SkillID, out slot);
            if (slot != null)
            {
                slot.UpdateData(skill);
            }
        }
    }

    private void RemoveSkill(List<SkillInstance> removed)
    {
       
        if (removed != null && removed.Count > 0)
        {
            for (int i = 0; i < removed.Count; i++)
            {
                int skillId = removed[i].SkillID;
                SkillSlot slot;
                _allSlots.TryGetValue(skillId, out slot);
                
                if (slot != null)
                {
                    slot.AssignDestroy();
                }
                else
                {
                    // Debug.Log("Not found slot for remove skill id " + skillId);
                }

                _allSlots.Remove(skillId);

            }
        }
    }

    private void RemoveEmptyPanels(VisualElement boxPanel)
    {
        List<VisualElement> delete = new List<VisualElement>();
        for (int i = boxPanel.childCount - 1; i >= 0; i--)
        {
            var panel = boxPanel[i];
            var rowNameVirtual = panel.Q(_rowNameInnerPanel);

            if (rowNameVirtual == null || rowNameVirtual.childCount == 0)
            {
                //ignore Background and Border Panel design elements
                if (panel.name != "Background" && panel.name != "Border")
                {
                    delete.Add(panel);
                }

            }

        }

        foreach (VisualElement item in delete)
        {
            // Debug.Log("Delete box panel " + item.name);
            boxPanel.Remove(item);
        }
    }
}
