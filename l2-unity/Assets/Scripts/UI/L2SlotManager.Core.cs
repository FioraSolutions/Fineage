using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class L2SlotManager : L2PopupWindow
{
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            WireHudCorePacketServices();
            WireGameWindowPacketServices();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireHudCorePacketServices();
        WireGameWindowPacketServices();
    }

    private void WireHudCorePacketServices()
    {
        HudCorePacketServices.Bind(slots: this);
    }

    private void WireGameWindowPacketServices()
    {
        GameWindowPacketServices.Bind();
    }

    private void OnDestroy()
    {
        _instance = null;
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Template/DraggedSlot");
    }
}
