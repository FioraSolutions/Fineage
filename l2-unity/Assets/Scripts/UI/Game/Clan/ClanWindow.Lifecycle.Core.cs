using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class ClanWindow : L2TwoPanels
{
    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _creatorTableWindows = new CreatorTableWindows();
            _masterClan = new MasterClan();

            _dataProviderClanInfo = new DataProviderClanInfo();
            _detailedClan = new ClanDetailedInfo(_dataProviderClanInfo);
            WireClanPacketServices();
            EnsureAskJoinWindow();
        }
        else
        {
            Destroy(this);
        }
    }

    private void Start()
    {
        WireClanPacketServices();
    }
}
