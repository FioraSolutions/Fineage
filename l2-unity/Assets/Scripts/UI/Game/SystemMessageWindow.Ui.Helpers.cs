using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

using UnityEngine.UIElements;

public partial class SystemMessageWindow : L2PopupWindow
{
    public void ShowWindowDialogDropdownYesOrNot(string headerText , List<string> list)
    {
        ReplaceWindow(_windowTemplateDropdown);

        UnityEngine.UIElements.Label headerName = _windowEle.Q<UnityEngine.UIElements.Label>("labelText");
        DropdownField dropdown = _windowEle.Q<DropdownField>("comboBox");
        headerName.text = headerText;

        SetDropdownList(dropdown, list);
        dropdown.RegisterCallback<PointerDownEvent>(OnDropdownPointer, TrickleDown.TrickleDown);

        ReRegisterCallBack();

        OnCenterScreen(_root);
        base.ShowWindow();


    }

    private void ReRegisterCallBack()
    {
        _cancelButton = _windowEle.Q<Button>(_defaultNameCancel);
        _okButton = _windowEle.Q<Button>(_defaultNameOk);
        _cancelButton.RegisterCallback<ClickEvent>(ClickEventClosed);
        _okButton.RegisterCallback<ClickEvent>(ClickEventOk);
    }

    public void SetDropdownList(DropdownField dropdown, List<string> list)
    {
        if (list == null || list.Count == 0)
        {
            var empty = new List<string>();
            dropdown.choices = empty;
            dropdown.SetValueWithoutNotify(string.Empty);
            _listDropDown = empty;
        }
        else
        {
            dropdown.value = list[0];
            dropdown.choices = list;
            _listDropDown = list;
        }

    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }
}
