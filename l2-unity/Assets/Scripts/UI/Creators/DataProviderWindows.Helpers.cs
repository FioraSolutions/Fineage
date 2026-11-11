using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

public partial class DataProviderWindows :AbstractDataProvider
{
    public void AddRequiredSkillInfo(List<RequiredSkillInfo> requiredSkillInfo, VisualElement container)
    {
        if(requiredSkillInfo.Count == 0)
        {
            HideRequiredPanel(container);
            return;
        }

        for(int i =0; i < requiredSkillInfo.Count; i++)
        {
            RequiredSkillInfo itemInfo = requiredSkillInfo[i];
            EtcItemgrp item = TablePacketServices.EtcItemGrps.GetEtcItem(itemInfo.GetItemId());

            if (item != null)
            {
                ItemName itemName = TablePacketServices.ItemNames.GetItemName(itemInfo.GetItemId());
                Label itemNamelabel = container.Q<Label>("itemNameLabel");
                AddElementIfNotEmpty(itemNamelabel, itemNamelabel, itemName.Name +" X " + itemInfo.GetCount());

                //set icon
                VisualElement groupBoxIcon = container.Q<VisualElement>("RequiredItem");
                VisualElement icon = container.Q<VisualElement>("iconItem");
                AddNamedIconIfPresent(groupBoxIcon, icon, item.Icon);
            }
            else
            {

                HideRequiredPanel(container);
            }
            
        }
    }

    private void HideRequiredPanel(VisualElement container)
    {
        //RequiredItem all Required element
        VisualElement groupBoxIcon = container.Q<VisualElement>("RequiredItem");
        VisualElement icon = container.Q<VisualElement>("iconItem");
        AddElementIfNotNull(groupBoxIcon, icon, null);
    }
}
