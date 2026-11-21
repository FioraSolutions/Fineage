using System;

public sealed partial class NetClanBook
{
    public int Reputation { get; private set; }
    public int LargeCrestId { get; private set; }
    public int AllyId { get; private set; }
    public string AllyName { get; private set; } = "";
    public int AllyCrestId { get; private set; }
    public int CastleId { get; private set; }
    public int HallId { get; private set; }
    public bool AtWar { get; private set; }
    public int NewLeaderObjectId { get; private set; }
    public NetSubPledge[] SubPledges { get; private set; } = Array.Empty<NetSubPledge>();
    public int[] RankPrivileges { get; private set; } = Array.Empty<int>();
    public NetClanSkill[] Skills { get; private set; } = Array.Empty<NetClanSkill>();
    public string Notice { get; private set; } = "";
    public NetPledgeWarEntry[] WarsDeclared { get; private set; } = Array.Empty<NetPledgeWarEntry>();
    public NetPledgeWarEntry[] WarsUnderAttack { get; private set; } = Array.Empty<NetPledgeWarEntry>();

    public void ApplyPledgeInfo(NetPledgeInfo info)
    {
        if (info.ClanId <= 0)
            return;
        NoteName(info.ClanId, info.Name);
        if (info.ClanId == ClanId)
        {
            Name = info.Name ?? "";
            CrestId = info.CrestId;
            AllyId = info.AllyId;
            AllyName = info.AllyName ?? "";
            AllyCrestId = info.AllyCrestId;
        }
    }

    public void ApplyWarList(NetPledgeWarList list)
    {
        WarsDeclared = list.Declared ?? Array.Empty<NetPledgeWarEntry>();
        WarsUnderAttack = list.UnderAttack ?? Array.Empty<NetPledgeWarEntry>();
    }

    public void SetNotice(string notice)
    {
        Notice = notice ?? "";
    }

    void ApplyExtendedStatus(NetPledgeStatus status)
    {
        Reputation = status.Reputation;
        LargeCrestId = status.LargeCrestId;
        AllyId = status.AllyId;
        AllyName = status.AllyName ?? "";
        AllyCrestId = status.AllyCrestId;
        CastleId = status.CastleId;
        HallId = status.HallId;
        AtWar = status.AtWar;
        NewLeaderObjectId = status.NewLeaderObjectId;
        SubPledges = status.SubPledges ?? Array.Empty<NetSubPledge>();
        RankPrivileges = status.RankPrivileges ?? Array.Empty<int>();
        Skills = status.Skills ?? Array.Empty<NetClanSkill>();
    }

    void ClearExtended()
    {
        Reputation = 0;
        LargeCrestId = 0;
        AllyId = 0;
        AllyName = "";
        AllyCrestId = 0;
        CastleId = 0;
        HallId = 0;
        AtWar = false;
        NewLeaderObjectId = 0;
        SubPledges = Array.Empty<NetSubPledge>();
        RankPrivileges = Array.Empty<int>();
        Skills = Array.Empty<NetClanSkill>();
        Notice = "";
        WarsDeclared = Array.Empty<NetPledgeWarEntry>();
        WarsUnderAttack = Array.Empty<NetPledgeWarEntry>();
    }
}