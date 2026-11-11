using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public partial class SellCropListWindow : L2PopupWindow
{
    private void Awake()
    {
        if (_instance == null)
        {
            _creatorTableWindows = new CreatorTableWindows();
            _instance = this;
            WireAuxiliaryGameUIPacketServices();
        }
        else
        {
            _creatorTableWindows.DestroyTable();
            Destroy(this);
        }
    }

    private void Start()
    {
        WireAuxiliaryGameUIPacketServices();
    }

    private void WireAuxiliaryGameUIPacketServices()
    {
        AuxiliaryGameUIPacketServices.Bind(sellCrop: this);
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/Manor/SellCropListWindow");

    }
}
