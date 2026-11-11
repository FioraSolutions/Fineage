using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public partial class L2GameUI : L2UI
{
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DiBootstrap.EnsureGameScope();
            WireSystemUIPacketServices();
        }
        else
        {
            Destroy(this);
        }

        _openedWindows = new List<L2PopupWindow>();
    }

    private void Start()
    {
        WireSystemUIPacketServices();
    }

    private void WireSystemUIPacketServices()
    {
        SystemUIPacketServices.Bind(ui: this);
    }
}
