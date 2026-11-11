using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public partial class ClanAskJoinWindow : L2PopupWindow
{
    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            ClanPacketServices.Bind(ask: this);
        }
        else
        {
            Destroy(this);
        }
    }

    void Start()
    {
        ClanPacketServices.Bind(ask: this);
    }

    public void Present(NetClanAskJoin ask)
    {
        _requestorObjectId = ask.RequestorObjectId;
        string who = string.IsNullOrEmpty(ask.RequestorName) ? "A player" : ask.RequestorName;
        string clan = string.IsNullOrEmpty(ask.ClanName) ? "a clan" : ask.ClanName;
        if (_invitationText == null)
        {
            _pendingShow = true;
            _pendingText = who + " invites you to join the clan " + clan + ".";
            return;
        }

        _invitationText.text = who + " invites you to join the clan " + clan + ".";
        ShowWindow();
    }
}
