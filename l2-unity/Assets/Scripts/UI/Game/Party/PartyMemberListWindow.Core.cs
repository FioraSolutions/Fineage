using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public partial class PartyMemberListWindow : L2Window
{
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            WirePartyPacketServices();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        WirePartyPacketServices();
    }

    private void OnDestroy()
    {
        if (_instance == this)
            _instance = null;
    }

    private void WirePartyPacketServices()
    {
        PartyPacketServices.Bind(memberList: this);
    }
}
