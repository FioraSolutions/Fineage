using System;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class SeedInfoWindow : L2PopupWindow
{
    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/Manor/SeedInfoWindow");
        _tabTemplate = LoadAsset(L2InventoryUiAssets.Tab);
        _buttonTemplate = LoadAsset("Data/UI/_Elements/Template/Elements/Buttons/DefaultButton");
        _tabHeaderTemplate = LoadAsset(L2InventoryUiAssets.TabHeader);
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _creatorWindow =  new CreatorTabsWindows();
            _creatorTable = new CreatorTableWindows();
            WireAuxiliaryGameUIPacketServices();
        }
        else
        {
            Destroy(this);
        }
    }
}
