using NUnit.Framework;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using UnityEngine.UIElements;
using UnityEngine;

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
        _boxPanelsPhysical = new List<VisualElement>();
        _boxPanelsMagic = new List<VisualElement>();

        _rowPhysical = _activeSkillPanel.Q(_rowNamePhysical);
        _rowMagical = _activeSkillPanel.Q(_rowNameMagical);
        ResolveSkillsPerPanel(sizeCell);

        var activeSkills = list.Where(s => !s.IsMagic()).ToList();
        var magicSkills = list.Where(s => s.IsMagic()).ToList();

        SetAllSlots(new Dictionary<int, SkillSlot>());

        var allContentPhysical = _activeSkillPanel.Q(_allContentPhysical);
        var allContentMagical = _activeSkillPanel.Q(_allContentMagical);

        ShowPanelIfCount1(activeSkills, _rowPhysical, allContentPhysical);
        ShowPanelIfCount1(magicSkills, _rowMagical, allContentMagical);

        if (_allSlots != null)
            _allSlots.Clear();
        CreatePhysicalSlots(activeSkills, _rowPhysical);
        CreateMagicalSlots(magicSkills, _rowMagical);

        EnsureSectionRowsExpanded(_rowPhysical, _rowMagical);

        HidePanelIfCount0(activeSkills, _rowPhysical, allContentPhysical);
        HidePanelIfCount0(magicSkills, _rowMagical, allContentMagical);

        if (activeSkills.Count > 0 && _rowPhysical != null)
            _rowPhysical.style.display = DisplayStyle.Flex;
        if (magicSkills.Count > 0 && _rowMagical != null)
            _rowMagical.style.display = DisplayStyle.Flex;

        _tempList = list;

    }

    public void UpdateSlots(List<SkillInstance> list)
    {

        var allContentPhysical = _activeSkillPanel.Q(_allContentPhysical);
        var allContentMagical = _activeSkillPanel.Q(_allContentMagical);

        if (list == null || list.Count == 0)
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


        var gridRowsPhysical = CollectGridRows(_boxPanelsPhysical);
        var gridRowsMagic = CollectGridRows(_boxPanelsMagic);

        ShowActiveSkillsPanels(list, allContentPhysical);
        ShowMagicSkillsPanels(list, allContentMagical);

        VisualElement boxPhysical = _boxPanelsPhysical.Count > 0 ? _boxPanelsPhysical[0] : null;
        VisualElement boxMagic = _boxPanelsMagic.Count > 0 ? _boxPanelsMagic[0] : null;

        if (boxPhysical != null)
            UpdateSkills(addActiveSkills, removed, list, gridRowsPhysical, _templateSlotSkill, _templatePanel8x1, boxPhysical);
        if (boxMagic != null)
            UpdateSkills(addMagicSkills, removed, list, gridRowsMagic, _templateSlotSkill, _templatePanel8x1, boxMagic);
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

        if (list == null || list.Count == 0)
        {
            allContentPhysical.style.display = DisplayStyle.None;
            allContentMagical.style.display = DisplayStyle.None;
        }
    }
}
