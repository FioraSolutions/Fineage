using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;


public class CreatePanelCheckBoxWindows : ICreatorPanelCheckBox
{

    private const string _checkBoxTemplateName = "Data/UI/_Elements/Template/Elements/Checkbox/DefaultCheckBox";
    private const string _twoPanelsTemplateName = "Data/UI/_Elements/Template/Clan/TwoPanelsCheckBox";
    private List<VisualTreeAsset> _listTemplate  = new List<VisualTreeAsset>();
    private  string DEFAULT_DISABLED_COLOR = "#7f7f7f";
    private Color _colorTextDisabled;
    private CheckBoxRootElements _tempElements;
    private List<SettingCheckBox> _registeredCallbacks = new List<SettingCheckBox>();
    public CreatePanelCheckBoxWindows()
    {
        ColorUtility.TryParseHtmlString(DEFAULT_DISABLED_COLOR, out _colorTextDisabled);
    }
    public void LoadAsset(Func<string, VisualTreeAsset> loaderFunc)
    {
        foreach (string templateName in GetTemplateElementName())
        {
            _listTemplate.Add(loaderFunc(templateName));
        }
    }

    private List<string> GetTemplateElementName()
    {
        return new List<string> { _twoPanelsTemplateName, _checkBoxTemplateName };
    }

    public void CreateTwoPanels(CheckBoxRootElements elements)
    {
        for (int i =0; i <  elements.GetRootPanels().Length; i++)
        {
            VisualElement rootPanel = elements.GetRootPanels()[i];
            VisualElement twoPanel = AddTwoPanels(rootPanel);
            VisualTreeAsset checkBoxTemplate = _listTemplate[1];

            if (twoPanel == null)
            {
                // Debug.LogError("CreatePanelCheckBoxWindows-> Not Found Template");
                return;
            }

            _tempElements = elements;

            List<SettingCheckBox> left = elements.GetLeft(i);
            List<SettingCheckBox> right = elements.GetRight(i);

            SetCheckBoxHeaderPanel(twoPanel, _listTemplate[1], left , right);
            //left.RemoveAt(0);
            SetCheckBoxLeftPanel(twoPanel, checkBoxTemplate, left);
            SetCheckBoxRightPanel(twoPanel, checkBoxTemplate, right);
        }
    
    }

    public void DestroyTempElements()
    {
        if(_tempElements != null)
        {
            UnRegisterAllClick();
            _tempElements = null;
            _registeredCallbacks.Clear();
        }
    }

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
