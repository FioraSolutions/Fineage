using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CreatePanelCheckBoxWindows : ICreatorPanelCheckBox
{
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
}
