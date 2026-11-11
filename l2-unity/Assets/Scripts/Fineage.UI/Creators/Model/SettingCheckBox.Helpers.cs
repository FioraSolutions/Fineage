using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public partial class SettingCheckBox
{
    public SettingCheckBox GetMyHeaderSetting()
    {
        return _headerSetting;
    }

    public void SetChecked(bool click_checked)
    {
        _checked = click_checked;
    }

    public void SetDisabled(bool disabled)
    {
        _disabled = disabled;
    }

    public bool IsChecked()
    {
        return _checked;
    }

    public bool IsDisabled()
    {
        return _disabled;
    }

    public string GetName()
    {
        return _name;
    }

    public void SetCallback(EventCallback<ClickEvent> callback)
    {
        _callback = callback;
    }

    public EventCallback<ClickEvent> GetCallBack()
    {
        return _callback;
    }

    public void SetData(int data)
    {
        _data = data;
    }

    public int GetData()
    {
        return _data;
    }
}
