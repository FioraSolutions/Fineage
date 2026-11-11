public partial class ActiveSkillsHide
{
    public ActiveSkillsHide()
    {

        _arrDfActiveSelect = new int[5] { 0, 0, 0, 0, 0 };
    }

    public void SetActiveSkillTemplate(VisualTreeAsset templateActiveSkill , VisualTreeAsset templateBoxPanel ,  VisualTreeAsset templatePanel8x1 , VisualTreeAsset templateSlotSkill)
    {
        _activeSkillPanel = ToolTipsUtils.CloneOne(templateActiveSkill);
        _templateBoxPanel = templateBoxPanel;
        _templatePanel8x1 = templatePanel8x1;
        _templateSlotSkill = templateSlotSkill;
        _boxPanelsPhysical = new List<VisualElement>();
        _boxPanelsMagic = new List<VisualElement>();
        RegisterClickButtonPhysical(_activeSkillPanel);
        RegisterClickButtonMagic(_activeSkillPanel);
    }

    public VisualElement GetOrCreateTab(VisualElement content)
    {
        if(_activeSkillPanel != null)
        {
            content.Clear();
            content.Add(_activeSkillPanel);
        }

        return _activeSkillPanel;
    }

    public void CreateSlots(List<SkillInstance> list , int sizeCell)
    {
        int panelCount = CalculatePanelCount(list);

        skillsPerPanel = sizeCell;
    
        var activeSkills = list.Where(s => !s.IsMagic()).ToList();
        var magicSkills = list.Where(s => s.IsMagic()).ToList();

        SetAllSlots(new Dictionary<int, SkillSlot>());

        _rowPhysical = _activeSkillPanel.Q(_rowNamePhysical);
        _rowMagical = _activeSkillPanel.Q(_rowNameMagical);

        var allContentPhysical = _activeSkillPanel.Q(_allContentPhysical);
        var allContentMagical = _activeSkillPanel.Q(_allContentMagical);

        ShowPanelIfCount1(activeSkills, _rowPhysical, allContentPhysical);
        ShowPanelIfCount1(magicSkills, _rowMagical, allContentMagical);

         if(_allSlots != null) _allSlots.Clear();
        CreatePhysicalSlots(activeSkills, _rowPhysical);
        CreateMagicalSlots(magicSkills, _rowMagical);

        HidePanelIfCount0(activeSkills, _rowPhysical, allContentPhysical);
        HidePanelIfCount0(magicSkills, _rowMagical, allContentMagical);

        _tempList = list;

    }

    public void UpdateSlots(List<SkillInstance> list)
    {

        var allContentPhysical = _activeSkillPanel.Q(_allContentPhysical);
        var allContentMagical = _activeSkillPanel.Q(_allContentMagical);

        if (list == null | list.Count == 0)
        {
            //var rowPhysical_hide = _activeSkillPanel.Q(_rowNamePhysical);
            //var rowMagical_hide = _activeSkillPanel.Q(_rowNameMagical);

            HidePanels(list, allContentPhysical, allContentMagical);
            return;
        }


     
        List<SkillInstance> added;
        List<SkillInstance> removed;

        CompareSkillLists(list, _tempList, out added, out removed);

        var rowPhysical = _activeSkillPanel.Q(_rowNamePhysical);
        var rowMagical = _activeSkillPanel.Q(_rowNameMagical);



        var addActiveSkills = added.Where(s => !s.IsMagic()).ToList();
        var addMagicSkills = added.Where(s => s.IsMagic()).ToList();


        var rowsVirtualPhysical = rowPhysical.Query<VisualElement>(name: "RowsVirtual").ToList();
        var rowsVirtualMagic = rowMagical.Query<VisualElement>(name: "RowsVirtual").ToList();


        ShowActiveSkillsPanels(list, allContentPhysical);
        ShowMagicSkillsPanels(list, allContentMagical);


        UpdateSkills(addActiveSkills, removed, list, rowsVirtualPhysical, _templateSlotSkill , _templatePanel8x1 , _boxPanelsPhysical[0]);
        UpdateSkills(addMagicSkills, removed, list, rowsVirtualMagic, _templateSlotSkill, _templatePanel8x1, _boxPanelsMagic[0]);
        _tempList = list;

        HidePanelActiveSkillsPanel(list, allContentPhysical);
        HidePanelMagicSkillsPanel(list, allContentMagical);
    }

    private void ShowActiveSkillsPanels(List<SkillInstance> list, VisualElement allContentMagical)
    {
        var activeSkills = list.Where(s => !s.IsMagic()).ToList();


        if (activeSkills.Count > 0)
        {
            allContentMagical.style.display = DisplayStyle.Flex;
        }
    }

    private void ShowMagicSkillsPanels(List<SkillInstance> list, VisualElement allContentMagical)
    {

        var magicSkills = list.Where(s => s.IsMagic()).ToList();

        if (magicSkills.Count > 0)
        {
            allContentMagical.style.display = DisplayStyle.Flex;
        }
    }

    private void HidePanels(List<SkillInstance> list, VisualElement allContentMagical, VisualElement allContentPhysical)
    {

        if (list == null | list.Count == 0)
        {
            allContentPhysical.style.display = DisplayStyle.None;
            allContentMagical.style.display = DisplayStyle.None;
        }
    }
}
