using System;

using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public partial class TargetWindow : L2Window
{
    private void Awake() {
        if (_instance == null) {
            _instance = this;
            WireHudCorePacketServices();
        } else {
            Destroy(this);
        }
    }

    private void OnDestroy() {
        _instance = null;
    }

    private void Start()
    {
        WireHudCorePacketServices();
    }

    private void WireHudCorePacketServices()
    {
        HudCorePacketServices.Bind(target: this);
    }

    protected override void LoadAssets() {
        _windowTemplate = LoadAsset("Data/UI/_Elements/Game/TargetWindow");
    }
}
