using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CreatorPanelCheckBoxWindows
{
    public VisualElement CreateCheckBox(VisualTreeAsset template)
    {
        return ToolTipsUtils.CloneOne(template);
    }

    private void AutoCreateCheckBox(VisualElement panel, List<SettingCheckBox> listSettings, VisualTreeAsset checkboxTemplate)
    {
        for (int i = 0; i < listSettings.Count; i++)
        {

            SettingCheckBox setting = listSettings[i];

            if (setting.IsHeader())
            {
                // Debug.Log("Detected Header " + setting.GetName());
                continue;
            }

            VisualElement checkbox = CreateCheckBox(checkboxTemplate);
            setting.SetElement(checkbox);
            SetSetting(checkbox , setting);
            AddClickEvent(checkbox, setting);
            panel.Add(checkbox);
        }
    }

    private VisualElement SetSetting(VisualElement checkbox , SettingCheckBox setting)
    {
        if (checkbox == null) return null;

        VisualElement uncheked = checkbox.Q<VisualElement>("imageUnchecked");
        VisualElement cheked = checkbox.Q<VisualElement>("imageChecked");
        VisualElement checkedDisabled = checkbox.Q<VisualElement>("imageCheckedDisabled");
        VisualElement uncheckdDisabled = checkbox.Q<VisualElement>("imageUnCheckedDisabled");
        Label label = checkbox.Q<Label>("labelCheckBox");

        setting.SetElementChecked(cheked);
        setting.SetElementUnChecked(uncheked);

        SetChecked(setting.IsChecked() , uncheked, cheked);
        SetDisabled(checkbox, setting.IsDisabled(), setting.IsChecked(), uncheked, cheked, checkedDisabled , uncheckdDisabled);
        SetText(label, setting.GetName());
        SetColorText(label, setting.IsDisabled());

        return checkbox;

    }

    private void AddClickEvent(VisualElement checkbox , SettingCheckBox setting)
    {
        if (!setting.IsDisabled())
        {
            VisualElement uncheked = checkbox.Q<VisualElement>("imageUnchecked");
            VisualElement cheked = checkbox.Q<VisualElement>("imageChecked");

            EventCallback<ClickEvent> callback = evt => OnClick(evt, uncheked, cheked, setting);
            checkbox.RegisterCallback(callback);
            setting.SetCallback(callback);
            _registeredCallbacks.Add(setting);
        }
    }

    private void AddClickHeaderEvent(VisualElement headerCheckBox, SettingCheckBox setting , List<SettingCheckBox> left, List<SettingCheckBox> right)
    {
        if (!setting.IsDisabled())
        {
            EventCallback<ClickEvent> callback = evt => OnClickHeader(evt, left, right, setting);
            headerCheckBox.RegisterCallback(callback);
            setting.SetCallback(callback);
            _registeredCallbacks.Add(setting);
        }
    }

    private void OnClick(ClickEvent evt, VisualElement uncheked , VisualElement cheked , SettingCheckBox setting)
    {
        if (setting.IsChecked())
        {
            SetChecked(false, uncheked, cheked);
            setting.SetChecked(false);
        }
        else
        {
            SetChecked(true, uncheked, cheked);
            setting.SetChecked(true);
        }
    }
}
