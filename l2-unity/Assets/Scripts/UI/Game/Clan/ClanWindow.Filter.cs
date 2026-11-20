using System.Collections.Generic;
using UnityEngine.UIElements;

public partial class ClanWindow
{
    void OnSubPledgeFilterChanged(ChangeEvent<string> evt)
    {
        FilterMembersBySubPledge(evt.newValue);
    }

    void FilterMembersBySubPledge(string _)
    {
        if (_packet == null || _packet.Members == null || _masterClan == null || _creatorTableWindows == null)
            return;

        int pledgeType = SelectedPledgeType();
        var filtered = new List<ClanMember>();
        for (int i = 0; i < _packet.Members.Count; i++)
        {
            ClanMember row = _packet.Members[i];
            if (row.Sponsor == pledgeType)
                filtered.Add(row);
        }

        if (filtered.Count == 0)
            filtered = _packet.Members;
        _masterClan.CreateMembersTable(filtered, _creatorTableWindows);
    }
}