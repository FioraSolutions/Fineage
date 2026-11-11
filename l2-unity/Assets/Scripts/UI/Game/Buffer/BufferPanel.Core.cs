using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;
using static UnityEngine.Rendering.DebugUI;

public partial class BufferPanel : L2Window
{
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _dictElement = new Dictionary<int, DataCell>();
            _filterData = new FilterData(_dictElement);
            _bLink = new BLink(this);
            WireAuxiliaryGameUIPacketServices();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireAuxiliaryGameUIPacketServices();
    }

    private void WireAuxiliaryGameUIPacketServices()
    {
        AuxiliaryGameUIPacketServices.Bind(buffer: this);
        // EnterWorld may have stashed AbnormalStatusUpdate before this tray existed.
        ApplyServices.Status?.FlushPendingSelfAbnormals();
    }
}
