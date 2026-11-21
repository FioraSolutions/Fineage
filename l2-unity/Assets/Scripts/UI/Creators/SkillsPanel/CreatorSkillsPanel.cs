using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CreatorSkillsPanel : ICreatorSkillsPanel
{
    private const string _activeSkillName = "Data/UI/_Elements/Game/SkillLearn/OnlySkillPanel";

    private VisualTreeAsset _templateSkillsRow8x1;
    private VisualTreeAsset _templateSlotSkill;
    private VisualTreeAsset _templateActiveSkill;
    private VisualTreeAsset _templateBoxPanel;

    private ActiveSkillsHide _skillPanel;

    public CreatorSkillsPanel()
    {
        _skillPanel = new ActiveSkillsHide();
    }
    public void LoadAsset(Func<string, VisualTreeAsset> loaderFunc)
    {
        _templateSkillsRow8x1 = loaderFunc(L2SkillsUiAssets.SkillsGridRow);
        _templateSlotSkill = loaderFunc(L2SkillsUiAssets.SkillsSlot);
        _templateActiveSkill = loaderFunc(_activeSkillName);
        _templateBoxPanel = loaderFunc(L2SkillsUiAssets.SkillsBoxRow);

        _skillPanel.SetActiveSkillTemplate(_templateActiveSkill, _templateBoxPanel, _templateSkillsRow8x1, _templateSlotSkill);
    }

    public void RefreshData(VisualElement container)
    {
        _skillPanel.GetOrCreateTab(container);
    }

    public void CreateSlots(List<SkillInstance> list , int sizeCell)
    {
        _skillPanel.CreateSlots(list , sizeCell);
    }
}
