

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

    private void RearrangeSkillSlots(List<VisualElement> rowPanels, VisualElement boxPanel)
    {
        //const int skillsPerPanel = 7;
        List<VisualElement> toDelete = new List<VisualElement>();

        // Собираем все SkillSlot из всех панелей
        List<VisualElement> allSlots = new List<VisualElement>();
        foreach (var panel in rowPanels)
        {
            var rowNameVirtual = panel.Q(_rowNameInnerPanel);
            if (rowNameVirtual != null)
            {
                for (int i = 0; i < rowNameVirtual.childCount; i++)
                {
                    allSlots.Add(rowNameVirtual[i]);
                }
                rowNameVirtual.Clear();
            }
        }


        int slotIndex = 0;
        foreach (var panel in rowPanels)
        {
            var rowNameVirtual = panel.Q(_rowNameInnerPanel);
            if (rowNameVirtual != null)
            {
                for (int j = 0; j < skillsPerPanel && slotIndex < allSlots.Count; j++, slotIndex++)
                {
                    rowNameVirtual.Add(allSlots[slotIndex]);
                }
            }
        }

    }

    protected void AddSkill(List<SkillInstance> added, List<VisualElement> rowPanels, VisualTreeAsset templateSlotSkill, VisualTreeAsset templatePanel8x1 , VisualElement boxPanel)
    {
        //const int skillsPerPanel = 7;

        foreach (var skill in added)
        {
            VisualElement targetPanel = null;

            // Если панелей нет, создаём первую
            if (rowPanels.Count == 0)
            {
                targetPanel = ToolTipsUtils.CloneOne(templatePanel8x1);
                var rowNameVirtual1 = targetPanel.Q(_rowNameInnerPanel);
                rowPanels.Add(rowNameVirtual1);
                boxPanel.Add(targetPanel);
            }
            else
            {
                // Берём последнюю панель
                targetPanel = rowPanels[rowPanels.Count - 1];
                // Считаем количество ячеек в RowsVirtual
                var rowNameVirtual = targetPanel.Q(_rowNameInnerPanel);
                int currentCount = rowNameVirtual != null ? rowNameVirtual.childCount : 0;

                // Если панель заполнена, создаём новую
                if (currentCount >= skillsPerPanel)
                {
                    targetPanel = ToolTipsUtils.CloneOne(templatePanel8x1);
                    var rowNameVirtual1 = targetPanel.Q(_rowNameInnerPanel);
                    rowPanels.Add(rowNameVirtual1);
                    boxPanel.Add(targetPanel);
                }
            }

            // Добавляем слот в панель
            var rowNameVirtualAdd = targetPanel.Q(_rowNameInnerPanel);
            var slotElement = ToolTipsUtils.CloneOne(templateSlotSkill);
            var skillSlot = new SkillSlot(slotElement, skill.SkillID, SlotType.SkillWindow);
            skillSlot.AssignSkill(skill);
            //skillSlot.EventLeftClick += OnClickLeftEvent;
            rowNameVirtualAdd.Add(slotElement);

            if (!_allSlots.ContainsKey(skill.SkillID))
            {
                _allSlots.Add(skill.SkillID, skillSlot);
            }
            else
            {
                // Debug.Log("AddSkill dublication found " + skill.SkillID);
            }

        }
    }

    protected void CompareSkillLists(List<SkillInstance> newList, List<SkillInstance> oldList, out List<SkillInstance> added, out List<SkillInstance> removed)
    {

        var oldIds = oldList?.Select(s => s.SkillID).ToHashSet() ?? new HashSet<int>();
        var newIds = newList?.Select(s => s.SkillID).ToHashSet() ?? new HashSet<int>();

        added = newList?.Where(s => !oldIds.Contains(s.SkillID)).ToList() ?? new List<SkillInstance>();
        removed = oldList?.Where(s => !newIds.Contains(s.SkillID)).ToList() ?? new List<SkillInstance>();
    }

    protected void ShowPanelIfCount1(List<SkillInstance> list, VisualElement panel, VisualElement allContent)
    {
        if (list != null | list.Count > 0)
        {
            panel.Clear();
            allContent.style.display = DisplayStyle.Flex;
        }
    }

    protected void HidePanelIfCount0(List<SkillInstance> list, VisualElement panel, VisualElement allContent)
    {
        if (list == null | list.Count == 0)
        {
            //panel.Clear();
            allContent.style.display = DisplayStyle.None;
        }
    }

}
