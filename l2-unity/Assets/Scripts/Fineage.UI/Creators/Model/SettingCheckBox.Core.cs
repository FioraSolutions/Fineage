using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;
using System.Collections.Generic;

public partial class SettingCheckBox
{
    public SettingCheckBox(string name, bool isChecked = false, bool isDisabled = false)
    {
        _name = name;
        _checked = isChecked;
        _disabled = isDisabled;
    }

    public SettingCheckBox(string name, bool isChecked = false, bool isDisabled = false , int data  = 0 , SettingCheckBox headerSetting = null)
    {
        _name = name;
        _checked = isChecked;
        _disabled = isDisabled;
        _data = data;
        _headerSetting = headerSetting;
    }

    public bool IsHeader()
    {
        return _headerSetting == null;
    }

    public void SetElement(VisualElement element)
    {
        _element = element;
    }

    public VisualElement GetElement()
    {
        return _element;
    }

    public void SetElementChecked(VisualElement element)
    {
        _elementChecked = element;
    }

    public VisualElement GetElementChecked()
    {
        return _elementChecked;
    }

    public void SetElementUnChecked(VisualElement element)
    {
        _elementUnchecked = element;
    }

    public VisualElement GetElementUnChecked()
    {
        return _elementUnchecked;
    }
}
