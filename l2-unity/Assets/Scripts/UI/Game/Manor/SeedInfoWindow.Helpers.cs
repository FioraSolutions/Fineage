using System;
using System.Collections;
using UnityEngine.UIElements;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;
using System.Collections.Generic;
using UnityEngine;
using VContainer;

public partial class SeedInfoWindow : L2PopupWindow
{
    private void Start()
    {
        WireAuxiliaryGameUIPacketServices();
    }

    private void WireAuxiliaryGameUIPacketServices()
    {
        AuxiliaryGameUIPacketServices.Bind(seedInfo: this);
    }

    protected override IEnumerator BuildWindow(VisualElement root)
    {
        InitWindow(root);
        yield return new WaitForEndOfFrame();

        var dragArea = GetElementByClass("drag-area");
        DragManipulator drag = new DragManipulator(dragArea, _windowEle);
        dragArea.AddManipulator(drag);
        var content = (VisualElement)GetElementById("content");

        _footerContainer = content.Q<VisualElement>("footerContent");
        _creatorTable.LoadAsset(LoadAsset);

        _defaultColumns0 = GetColumnWindowTab0();
        _defaultColumns1 = GetColumnWindowTab1();
        _defaultColumns2 = GetColumnWindowTab2();

        _creatorWindow.InitContentTabs(_tabsName);
        _creatorWindow.CreateTabs(content, _tabTemplate, _tabHeaderTemplate);
        _creatorWindow.InsertTablesIntoContent(_creatorTable, _defaultColumns0, true);
        _creatorWindow.EventSwitchTabByIndexOfTab += OnSwitchTab;
        AddContent0Footer(_creatorWindow , _footerContainer);



        RegisterCloseWindowEvent("btn-close-frame");
        RegisterClickWindowEvent(_windowEle, dragArea);
        OnCenterScreen(_root);

    }
}
