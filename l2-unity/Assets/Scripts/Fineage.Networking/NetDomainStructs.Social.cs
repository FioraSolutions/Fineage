using System;


public struct NetPartyAskJoin
{
    public int RequestorObjectId;
    public string RequestorName;
    public int ItemDistribution;
}

public struct NetPartyMemberEntry
{
    public int ObjectId;
    public string Name;
    public int ClassId;
    public int Level;
    public int CurHp;
    public int MaxHp;
    public int CurMp;
    public int MaxMp;
}

public struct NetPartyMemberList
{
    public int LeaderObjectId;
    public int PartyId;
    public NetPartyMemberEntry[] Members;
}

public struct NetTradeAsk
{
    public int RequestorObjectId;
    public string RequestorName;
}

public struct NetTradeStart
{
    public int PartnerObjectId;
    public string PartnerName;
}

public struct NetTradeOfferUpdate
{
    public int IsOwn;
    public int InventoryObjectId;
    public int ItemId;
    public int Count;
    public int EnchantLevel;
}

public struct NetTradeConfirmState
{
    public int SelfConfirmed;
    public int PartnerConfirmed;
}

public struct NetClanAskJoin
{
    public int RequestorObjectId;
    public string RequestorName;
    public int ClanId;
    public string ClanName;
    public int PledgeType;
}

public struct NetPledgeStatus
{
    public int ClanId;
    public string Name;
    public int Level;
    public int LeaderObjectId;
    public int CrestId;
    public int MemberCount;
    public long DissolvingExpiryUnixMs;
    public long CharPenaltyExpiryUnixMs;
    public int Reputation;
    public int LargeCrestId;
    public int AllyId;
    public string AllyName;
    public int AllyCrestId;
    public int CastleId;
    public int HallId;
    public bool AtWar;
    public int NewLeaderObjectId;
    public NetSubPledge[] SubPledges;
    public int[] RankPrivileges;
    public NetClanSkill[] Skills;
}

public struct NetSubPledge
{
    public int Type;
    public string Name;
    public int LeaderObjectId;
}

public struct NetClanSkill
{
    public int SkillId;
    public int Level;
}

public struct NetPledgeWarEntry
{
    public int ClanId;
    public string Name;
    public bool Mutual;
}

public struct NetPledgeWarList
{
    public NetPledgeWarEntry[] Declared;
    public NetPledgeWarEntry[] UnderAttack;
}

public struct NetPledgeInfo
{
    public int ClanId;
    public string Name;
    public int CrestId;
    public int AllyId;
    public string AllyName;
    public int AllyCrestId;
}

public struct NetPledgeMember
{
    public int ObjectId;
    public string Name;
    public int ClassId;
    public int Level;
    public bool Online;
    public int PledgeType;
}

public struct NetPledgeMemberList
{
    public int ClanId;
    public int LeaderObjectId;
    public NetPledgeMember[] Members;
}
