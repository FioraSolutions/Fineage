using FMOD;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using VContainer;

using UnityEngine.UIElements;

public partial class ShowListWindow : L2PopupWindow
{
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            WireGameWindowPacketServices();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireGameWindowPacketServices();
    }

    private void WireGameWindowPacketServices()
    {
        GameWindowPacketServices.Bind(showList: this);
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/ShowList/ShowListWindow");

    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);
        yield return new WaitForEndOfFrame();

        _СЃancelButton = _windowEle.Q<Button>("CancelButton");
        _okButton = _windowEle.Q<Button>("StartButton");
        var content = _windowEle.Q<VisualElement>("content");

        _dropdown = content.Q<DropdownField>("comboBox");

        RegisterCloseWindowEvent("btn-close-frame");
        var dragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);
        DisableEventOnOver(_dropdown);

        _dropdown.RegisterValueChangedCallback(OnDropdownValueChanged);

        _okButton.RegisterCallback<ClickEvent>((evt) => OnClick(evt));
        _СЃancelButton.RegisterCallback<ClickEvent>((evt) => OnCancel(evt));

        RegisterClickWindowEvent(_windowEle, null);
        OnCenterScreen(_root);
    }
}
