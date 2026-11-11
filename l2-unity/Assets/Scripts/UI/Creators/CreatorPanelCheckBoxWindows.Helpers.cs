using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CreatePanelCheckBoxWindows : ICreatorPanelCheckBox
{
    private void UnRegisterAllClick()
    {
        if (_registeredCallbacks != null)
        {
            foreach (var callbackInfo in _registeredCallbacks)
            {
                callbackInfo.GetElement().UnregisterCallback<ClickEvent>(callbackInfo.GetCallBack());
            }
            _registeredCallbacks.Clear();
        }
    }

    private VisualElement AddTwoPanels(VisualElement panel)
    {


        if(panel != null)
        {
            VisualElement twoPanels = CreateTwoPanels(_listTemplate[0]);
            panel.Add(twoPanels);
            return twoPanels;
        }
        else
        {
            // Debug.LogError("CreatePanelCheckBoxWindows-> Not Found Panel");
            return null;
        }

    }

    private void SetCheckBoxHeaderPanel(VisualElement panel, VisualTreeAsset checkboxTemplate , List<SettingCheckBox> left , List<SettingCheckBox> right)
    {
        SettingCheckBox headerSetting = left[0];
        VisualElement checkbox = CreateCheckBox(checkboxTemplate);
        headerSetting.SetElement(checkbox);
        VisualElement header = panel.Q<VisualElement>("GroupHeaderTwoPanels");
        SetSetting(checkbox, headerSetting);
        AddClickHeaderEvent(checkbox, headerSetting ,  left,  right);
        header.Add(checkbox);
    }

    private void SetCheckBoxLeftPanel(VisualElement panel, VisualTreeAsset checkboxTemplate, List<SettingCheckBox> listSetting)
    {
        VisualElement subLeftPanel = panel.Q<VisualElement>("GroupLeftListCheckBox");
        AutoCreateCheckBox(subLeftPanel, listSetting, checkboxTemplate);
    }

    public void SetCheckBoxRightPanel(VisualElement panel, VisualTreeAsset checkboxTemplate, List<SettingCheckBox> listSetting)
    {
        VisualElement subRightPanel = panel.Q<VisualElement>("GroupRightListCheckBox");
        AutoCreateCheckBox(subRightPanel, listSetting, checkboxTemplate);
    }

    public VisualElement CreateTwoPanels(VisualTreeAsset template)
    {
        return ToolTipsUtils.CloneOne(template);
    }
}
