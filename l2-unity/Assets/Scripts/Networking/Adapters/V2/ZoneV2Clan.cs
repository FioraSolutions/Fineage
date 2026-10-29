using System;
using System.Collections.Generic;

/// <summary>
/// Clan snapshot for the zone wire. Layout matches Fineage-Backend <c>ClanProtocol</c>
/// (opcodes 32–38 and 1050–1053). Unity EditMode cannot reference this assembly;
/// <c>tools/client-zone-v2.Tests</c> compiles this file directly.
/// </summary>
public sealed class ZoneV2ClanBook
{
    public const int InviteTimeoutSeconds = 15;
    public const int MainPledge = 0;

    public static readonly ZoneV2ClanBook Shared = new ZoneV2ClanBook();

    readonly Dictionary<int, string> _names = new Dictionary<int, string>();
    ZoneV2PledgeMember[] _members = Array.Empty<ZoneV2PledgeMember>();

    public int ClanId { get; private set; }
    public string Name { get; private set; } = "";
    public int Level { get; private set; }
    public int LeaderObjectId { get; private set; }
    public int CrestId { get; private set; }
    public int MemberCount { get; private set; }
    public long DissolvingExpiryUnixMs { get; private set; }
    public long CharPenaltyExpiryUnixMs { get; private set; }
    public ZoneV2PledgeMember[] Members => _members;

    public bool HasClan => ClanId != 0;

    public bool Dissolving => DissolvingExpiryUnixMs > 0;

    public void Reset()
    {
        ClearMembership();
        _names.Clear();
    }

    public void ApplyStatus(ZoneV2PledgeStatus status)
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

    public void ApplyMembers(ZoneV2PledgeMemberList list)
    {
        if (list.ClanId == 0)
        {
            ClearMembership();
            return;
        }

        ClanId = list.ClanId;
        LeaderObjectId = list.LeaderObjectId;
        ZoneV2PledgeMember[] rows = list.Members ?? Array.Empty<ZoneV2PledgeMember>();
        var copy = new ZoneV2PledgeMember[rows.Length];
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

    public bool IsLeader(int objectId)
    {
        return objectId > 0 && HasClan && objectId == LeaderObjectId;
    }

    public string LeaderName()
    {
        ZoneV2PledgeMember leader;
        if (TryMember(LeaderObjectId, out leader) && !string.IsNullOrEmpty(leader.Name))
            return leader.Name;
        return "";
    }

    public bool TryMember(int objectId, out ZoneV2PledgeMember member)
    {
        for (int i = 0; i < _members.Length; i++)
        {
            if (_members[i].ObjectId == objectId)
            {
                member = _members[i];
                return true;
            }
        }

        member = default;
        return false;
    }

    public bool TryMemberByName(string name, out ZoneV2PledgeMember member)
    {
        if (!string.IsNullOrEmpty(name))
        {
            for (int i = 0; i < _members.Length; i++)
            {
                if (string.Equals(_members[i].Name, name, StringComparison.Ordinal))
                {
                    member = _members[i];
                    return true;
                }
            }
        }

        member = default;
        return false;
    }

    public int OnlineCount()
    {
        int count = 0;
        for (int i = 0; i < _members.Length; i++)
        {
            if (_members[i].Online)
                count++;
        }

        return count;
    }

    void ClearMembership()
    {
        ClanId = 0;
        Name = "";
        Level = 0;
        LeaderObjectId = 0;
        CrestId = 0;
        MemberCount = 0;
        DissolvingExpiryUnixMs = 0;
        CharPenaltyExpiryUnixMs = 0;
        _members = Array.Empty<ZoneV2PledgeMember>();
    }
}

/// <summary>Readable lines for clan <c>Error</c> 1099 codes. A full server sentence wins.</summary>
public static class ZoneV2ClanNotices
{
    public static string LineOrEquip(string code, string message)
    {
        string clan;
        if (TryLine(code, message, out clan))
            return clan;
        return ZoneV2EquipNotices.Line(code, message);
    }

    public static bool TryLine(string code, string message, out string line)
    {
        line = null;
        if (string.IsNullOrEmpty(code) || !code.StartsWith("clan_", StringComparison.Ordinal))
            return false;
        if (IsSentence(message))
        {
            line = message.Trim();
            return true;
        }

        line = Map(code);
        return true;
    }

    public static string Map(string code)
    {
        switch (code)
        {
            case "clan_level":
                return "You must be level 10 to create a clan.";
            case "clan_member":
                return "You are already in a clan.";
            case "clan_create_penalty":
                return "You must wait before creating another clan.";
            case "clan_name":
                return "Clan names may use letters and digits only.";
            case "clan_name_length":
                return "A clan name must be 2 to 16 characters.";
            case "clan_name_taken":
                return "That clan name is already taken.";
            case "clan_none":
                return "You are not in a clan.";
            case "clan_leader":
                return "Only the clan leader can do that.";
            case "clan_leader_leave":
                return "The clan leader cannot leave. Dissolve the clan instead.";
            case "clan_pledge":
                return "Only the main clan can be invited to.";
            case "clan_self":
                return "You cannot choose yourself.";
            case "clan_target":
                return "Select a nearby player.";
            case "clan_target_clan":
                return "That player is already in a clan.";
            case "clan_busy":
                return "That player is busy.";
            case "clan_waiting":
                return "That invitation is already pending.";
            case "clan_recruit_penalty":
                return "The clan must wait before recruiting.";
            case "clan_join_penalty":
                return "You must wait before joining a clan.";
            case "clan_full":
                return "The clan is full.";
            case "clan_invite":
                return "That clan invitation has expired.";
            case "clan_combat":
                return "You cannot do that while in combat.";
            case "clan_dismiss_self":
                return "You cannot dismiss yourself.";
            case "clan_dissolving":
                return "The clan is already dissolving.";
            case "clan_not_dissolving":
                return "The clan is not dissolving.";
            case "clan_store":
                return "The clan could not be saved.";
            default:
                return "The clan request was refused.";
        }
    }

    static bool IsSentence(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            return false;
        string text = message.Trim();
        if (text.StartsWith("clan_", StringComparison.Ordinal))
            return false;
        return text.IndexOf(' ') >= 0;
    }
}
