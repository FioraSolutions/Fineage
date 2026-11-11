using System;
using System.Collections.Generic;

public sealed partial class NetClanBook
{
    public void Reset()
    {
        ClearMembership();
        _names.Clear();
    }

    public void ApplyStatus(NetPledgeStatus status)
    {
        if (status.ClanId == 0)
        {
            ClearMembership();
            return;
        }

        ClanId = status.ClanId;
        Name = status.Name ?? "";
        Level = status.Level;
        LeaderObjectId = status.LeaderObjectId;
        CrestId = status.CrestId;
        MemberCount = status.MemberCount;
        DissolvingExpiryUnixMs = status.DissolvingExpiryUnixMs;
        CharPenaltyExpiryUnixMs = status.CharPenaltyExpiryUnixMs;
        NoteName(status.ClanId, Name);
    }

    public void ApplyMembers(NetPledgeMemberList list)
    {
        if (list.ClanId == 0)
        {
            ClearMembership();
            return;
        }

        ClanId = list.ClanId;
        LeaderObjectId = list.LeaderObjectId;
        NetPledgeMember[] rows = list.Members ?? Array.Empty<NetPledgeMember>();
        var copy = new NetPledgeMember[rows.Length];
        Array.Copy(rows, copy, rows.Length);
        _members = copy;
        if (MemberCount < copy.Length)
            MemberCount = copy.Length;
    }

    public void NoteName(int clanId, string name)
    {
        if (clanId <= 0 || string.IsNullOrEmpty(name))
            return;
        _names[clanId] = name;
        if (ClanId == clanId)
            Name = name;
    }

    public string NameFor(int clanId)
    {
        if (clanId <= 0)
            return "";
        if (clanId == ClanId && !string.IsNullOrEmpty(Name))
            return Name;
        string known;
        return _names.TryGetValue(clanId, out known) ? known ?? "" : "";
    }
}
