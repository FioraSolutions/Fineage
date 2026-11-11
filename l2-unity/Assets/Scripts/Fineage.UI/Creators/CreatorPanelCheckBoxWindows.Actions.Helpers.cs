using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CreatorPanelCheckBoxWindows
{
    private void OnClickHeader(ClickEvent evt, List<SettingCheckBox> left , List<SettingCheckBox> right , SettingCheckBox headerSetting)
    {
        if (headerSetting.IsChecked())
        {
            ForEachList(left, false);
            ForEachList(right, false);
            SetChecked(false, headerSetting.GetElementUnChecked(), headerSetting.GetElementChecked());
            headerSetting.SetChecked(false);
        }
        else
        {
            ForEachList(left, true);
            ForEachList(right, true);
            SetChecked(true, headerSetting.GetElementUnChecked(), headerSetting.GetElementChecked());
            headerSetting.SetChecked(true);
        }

    }

    private void ForEachList(List<SettingCheckBox> list , bool isCheckedHeader)
    {
        foreach (SettingCheckBox setting in list)
        {
            SetChecked(isCheckedHeader, setting.GetElementUnChecked(), setting.GetElementChecked());
            setting.SetChecked(isCheckedHeader);
        }
    }

    private void SetChecked(bool isChecked , VisualElement uncheked , VisualElement cheked)
    {

        if (isChecked)
        {
            uncheked.style.display = DisplayStyle.None;
            cheked.style.display = DisplayStyle.Flex;
        }
        else
        {
            cheked.style.display = DisplayStyle.None;
            uncheked.style.display = DisplayStyle.Flex;
        }
    }

    private void SetText(Label label, string name )
    {
        label.text = name;
    }

    private void SetColorText(Label label , bool isDisabled)
    {
        if (isDisabled)
        {
            label.style.color = _colorTextDisabled;
        }
        else
        {
            label.style.color = Color.white;
        }
    }

    private void SetDisabled(VisualElement checkbox, bool isDisabled, bool isChecked , VisualElement uncheked, VisualElement cheked , VisualElement  checkedDisabled, VisualElement unCheckedDisabled)
    {


        if (isDisabled & isChecked)
        {
            uncheked.style.display = DisplayStyle.None;
            cheked.style.display = DisplayStyle.None;
            checkedDisabled.style.display = DisplayStyle.Flex;
            unCheckedDisabled.style.display = DisplayStyle.None;
        }
        else if(isDisabled & !isChecked)
        {
            uncheked.style.display = DisplayStyle.None;
            cheked.style.display = DisplayStyle.None;
            checkedDisabled.style.display = DisplayStyle.None;
            unCheckedDisabled.style.display = DisplayStyle.Flex;
        }
    }

    public List<SettingCheckBox> GetSelectAllCheckBoxs() =>
      _registeredCallbacks.Any() ? _registeredCallbacks.Where(cb => cb.IsChecked()).ToList() : new List<SettingCheckBox>();
}
