public partial class AbstractSkills
{
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
