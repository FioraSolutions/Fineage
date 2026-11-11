using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public partial class CharacterInfoWindow : L2PopupWindow
{
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            WireHudCorePacketServices();
        }
        else
        {
            Destroy(this);
        }
    }

    void OnDestroy()
    {
        _instance = null;
    }

    void Start()
    {
        WireHudCorePacketServices();
    }
}
