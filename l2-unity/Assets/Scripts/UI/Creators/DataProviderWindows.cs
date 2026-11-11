
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;

public partial class DataProviderWindows :AbstractDataProvider
{

   public void AddLearnSkill(int skillId , int correctCost , int level , VisualElement container)
   {
        Skillgrp skill = TablePacketServices.SkillGrps.GetSkill(skillId , level);
        SkillNameData skillName = TablePacketServices.SkillGrps.GetSkillName(skillId, level);

        //set icon
        VisualElement groupBoxIcon = container.Q<VisualElement>("GrowIcon");
        VisualElement icon = container.Q<VisualElement>("icon");
        AddNamedIconIfPresent(groupBoxIcon, icon, skill != null ? skill.Icon : null);

        Label nameLabel = container.Q<Label>("nameLabel");
        Label levelLabel = container.Q<Label>("settingLevel");
        Label spLabel = container.Q<Label>("settingSp");


        AddElementIfNotEmpty(nameLabel, nameLabel, skillName._name);
        AddElementIfNotEmpty(levelLabel, levelLabel, level.ToString());
        AddElementIfNotEmpty(spLabel, spLabel, correctCost.ToString());
    }




 
}
