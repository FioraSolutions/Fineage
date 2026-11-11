using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

using UnityEngine.UIElements;

public partial class SystemMessageWindow : L2PopupWindow
{
    private void ClickEventClosed(ClickEvent evt)
    {
        OnButtonClosed?.Invoke(); 
        base.HideWindow();
    }

    public void ShowWindow(string messageText)
    {
        if (_windowEle.name == _dropDownTemplateName)
        {
            ReplaceWindow(_windowTemplate);
            ReRegisterCallBack();
        }

        OnCenterScreen(_root);
        _cancelButton.style.display = DisplayStyle.None;
        _okButton.style.display = DisplayStyle.Flex;
        _textLabel.text = messageText;
        base.ShowWindow();
    }

    public void ShowWindowDialogYesOrNot(string messageText)
    {
        if (_windowEle.name == _dropDownTemplateName)
        {
            ReplaceWindow(_windowTemplate);
            ReRegisterCallBack();
        }

        OnCenterScreen(_root);
        _cancelButton.style.display = DisplayStyle.Flex;
        _okButton.style.display = DisplayStyle.Flex;
        _textLabel.text = messageText;
        base.ShowWindow();
    }
}
