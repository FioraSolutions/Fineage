using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

using UnityEngine.UIElements;

public partial class SystemMessageWindow : L2PopupWindow
{
    public void ClearDialogHandlers()
    {
        OnButtonOk = null;
        OnButtonClosed = null;
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            WireSystemMessagePacketServices();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireSystemMessagePacketServices();
    }

    private void WireSystemMessagePacketServices()
    {
        SystemMessagePacketServices.Bind(window: this);
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/SystemMessageWindow");
        _windowTemplateDropdown = LoadAsset("Data/UI/_Elements/Game/SystemMessageDropBoxWidow");
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);
        yield return new WaitForEndOfFrame();

        _cancelButton = _windowEle.Q<Button>(_defaultNameCancel);
        _okButton = _windowEle.Q<Button>(_defaultNameOk);
        _textLabel = _windowEle.Q<UnityEngine.UIElements.Label>("labelText");
        //RegisterCloseWindowEventByName("OkButton");
        _cancelButton.RegisterCallback<ClickEvent>(ClickEventClosed);
        _okButton.RegisterCallback<ClickEvent>(ClickEventOk);

        RegisterClickWindowEvent(_windowEle, null);
        OnCenterScreen(_root);
    }

    private void ClickEventOk(ClickEvent evt)
    {
        OnButtonOk?.Invoke(); 
        base.HideWindow();
    }
}
