using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class PartyInvitationWindow : L2PopupWindow
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

    private void WirePartyPacketServices()
    {
        PartyPacketServices.Bind(partyInvite: this);
    }

    public void AddData(AskJoinPartyDto request)
    {
        _inviterName = request.RequestorName;
        _lootDistribution = request.DistributionType.ToString();
        _requestorObjectId = request.RequestorObjectId;
    }
}
