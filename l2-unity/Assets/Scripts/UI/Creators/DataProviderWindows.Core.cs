
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public partial class DataProviderWindows :AbstractDataProvider
{
    public void AddRewardItem(string demoIconName, string demoDescName, string demoNameTexture, VisualElement container)
    {
        //set icon
        VisualElement groupBoxIcon = container.Q<VisualElement>("GrowIcon");
        VisualElement icon = container.Q<VisualElement>("icon");
        AddNamedIconIfPresent(groupBoxIcon, icon, demoNameTexture);

        VisualElement groupDesc = container.Q<VisualElement>("GrowDesc");
        Label descLabel = container.Q<Label>("descLabel");
        AddElementIfNotEmpty(groupDesc, descLabel, demoIconName);

        VisualElement groupName = container.Q<VisualElement>("GrowName");
        Label nameLabel = container.Q<Label>("nameLabel");
        AddElementIfNotEmpty(groupName, nameLabel, demoDescName);
    }

    public void AddDescriptionSkill(int skillId, int correctCost, int level, VisualElement container)
    {
        Skillgrp skill = TablePacketServices.SkillGrps.GetSkill(skillId, level);
        SkillNameData skillName = TablePacketServices.SkillGrps.GetSkillName(skillId, level);

        //set icon
        VisualElement groupBoxIcon = container.Q<VisualElement>("GrowIcon");
        VisualElement icon = container.Q<VisualElement>("icon");
        AddNamedIconIfPresent(groupBoxIcon, icon, skill != null ? skill.Icon : null);

        Label skillNameLabel = container.Q<Label>("skillNameLabel");
        Label skillLevelLabel = container.Q<Label>("skillSettingLevel");
        Label spLabel = container.Q<Label>("spLabel");
        Label activeSkillLabel = container.Q<Label>("activeSkillLabel");
        
        AddElementIfNotEmpty(skillNameLabel, skillNameLabel, skillName._name);
        AddElementIfNot0(skillLevelLabel, skillLevelLabel, level);
        AddElementIfNot0(spLabel, spLabel, correctCost);
        AddElementIfNotEmpty(skillNameLabel, skillNameLabel, skillName._name);
        AddElementIfNotEmpty(activeSkillLabel, activeSkillLabel, skill.GetActiveSkillNameOrNot());


        VisualElement groupBoxSkillMp = container.Q<VisualElement>("Requirements");
        Label skillMpLabel = container.Q<Label>("skillMpLabel");
        AddElementIfNotNullVisibleHide(groupBoxSkillMp, skillMpLabel, skill.MpConsume);

        VisualElement groupBoxRang = container.Q<VisualElement>("Range");
        Label skillRangeLabel = container.Q<Label>("skillRangeLabel");
        AddElementIfNotNullVisibleHide(groupBoxRang, skillRangeLabel, skill.CastRange);

        Label skillDescription = container.Q<Label>("skillDescription");
        AddElementIfNotEmpty(skillDescription, skillDescription, skillName.Desc);


    }
}
