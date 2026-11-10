

public partial class AbstractSkills
{
    public SkillSlot GetSlot(int skillId)
    {
        if (_allSlots != null && _allSlots.TryGetValue(skillId, out SkillSlot slot))
            return slot;
        return null;
    }

    public void SetAllSlots(Dictionary<int, SkillSlot> allSlots)
    {
        if (allSlots != null) _allSlots = null;
        _allSlots = allSlots;
    }

    protected VisualElement GetSkillBar(VisualElement content)
    {
        var childreb = content.Children();
        int i = 0;
        foreach (VisualElement item in childreb)
        {
            if (i > 0)
            {
                return item;
            }
            i++;
        }

        return null;
    }

    protected VisualElement CreateSlots(List<SkillInstance> skillList, VisualTreeAsset templatePanel8x1, VisualTreeAsset templateSlotsSkill, VisualElement boxPanel)
    {
       
        int totalSkills = skillList.Count;
        int panelCount = GetCountPanel(totalSkills, skillsPerPanel);

        int skillIndex = 0;

       // if(_allSlots != null) _allSlots.Clear();

        for (int i = 0; i < panelCount; i++)
        {
            var panel = ToolTipsUtils.CloneOne(templatePanel8x1);
            var rowNameVirtual = panel.Q(_rowNameInnerPanel);

            for (int j = 0; j < skillsPerPanel && skillIndex < totalSkills; j++, skillIndex++)
            {
                SkillInstance instance = skillList[skillIndex];

                if(instance.SkillID != -1)
                {
                    var slotElement = ToolTipsUtils.CloneOne(templateSlotsSkill);
                    var skillSlot = new SkillSlot(slotElement, instance.SkillID, SlotType.SkillWindow);
                    skillSlot.AssignSkill(instance);
                    rowNameVirtual.Add(slotElement);
                    AddSlot(skillSlot);
                }

            }

            boxPanel.Add(panel);
 
        }

        return boxPanel;
    }

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
