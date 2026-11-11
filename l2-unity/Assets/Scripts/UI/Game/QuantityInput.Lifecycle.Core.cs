using System;
using System.Linq;
using static L2Slot;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class QuantityInput : L2PopupWindow
{
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            WireQuantityInputPacketServices();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireQuantityInputPacketServices();
    }

    private void WireQuantityInputPacketServices()
    {
        QuantityInputPacketServices.Bind(window: this);
    }

    protected override void LoadAssets()
    {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/Calculator/QuantityInput");
    }
}
