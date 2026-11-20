using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using VContainer;

public partial class ClanWindow : L2TwoPanels
{
    public void EnsureAskJoinWindow()
    {
        if (GetComponent<ClanAskJoinWindow>() == null)
            gameObject.AddComponent<ClanAskJoinWindow>();
    }

    public void ApplyZoneBook()
    {
        if (_windowEle == null || _dropdown == null)
            return;

        NetClanBook book = NetClanBook.Shared;
        if (!book.HasClan)
        {
            _packet = null;
            _selectedMemberId = 0;
            ShowNoClan();
            TuneZoneButtons(book);
            return;
        }

        var members = new List<ClanMember>();
        NetPledgeMember[] rows = book.Members;
        for (int i = 0; i < rows.Length; i++)
        {
            NetPledgeMember row = rows[i];
            members.Add(new ClanMember(row.Name, row.Level, row.ClassId, 0, 0, row.Online ? 1 : 0, row.PledgeType));
        }

        var snapshot = new PledgeShowMemberListAllDto();
        snapshot.ApplyZoneSnapshot(book, members);
        AddClanData(snapshot);
        Label peace = _windowEle.Q<Label>("labelAtPeace");
        if (peace != null)
            peace.text = book.Dissolving ? "Dissolving" : (book.AtWar ? "At War" : "At Peace");
        Label baseLabel = _windowEle.Q<Label>("None");
        if (baseLabel != null)
        {
            string hall = book.HallId > 0 ? "Clan Hall" : (book.CastleId > 0 ? "Castle" : "None");
            baseLabel.text = hall;
        }
        TuneZoneButtons(book);
    }

    public void UpdateDetailedInfo(object packet)
    {
        _detailedClan.UpdateDetailedInfo(packet , _detailedInfoElement , _packet);
        ShowDetailedInfo();
    }

    public void ShowGradeInfo(object packet)
    {
        _detailedClan.UpdateDetailedInfo(packet, _detailedInfoElement, _packet);
        ShowDetailedInfo();
    }

    private void OnDestroy()
    {
        _instance = null; 
        _creatorTableWindows = null;
        _masterClan = null;
        _detailedClan = null;
        _dataProviderClanInfo = null;
    }
}
