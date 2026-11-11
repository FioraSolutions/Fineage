using System;
using System.Collections.Generic;

public sealed partial class NetClanBook
{
    public const int InviteTimeoutSeconds = 15;
    public const int MainPledge = 0;

    public static readonly NetClanBook Shared = new NetClanBook();

    readonly Dictionary<int, string> _names = new Dictionary<int, string>();
    NetPledgeMember[] _members = Array.Empty<NetPledgeMember>();

    public int ClanId { get; private set; }
    public string Name { get; private set; } = "";
    public int Level { get; private set; }
    public int LeaderObjectId { get; private set; }
    public int CrestId { get; private set; }
    public int MemberCount { get; private set; }
    public long DissolvingExpiryUnixMs { get; private set; }
    public long CharPenaltyExpiryUnixMs { get; private set; }
    public NetPledgeMember[] Members => _members;

    public bool HasClan => ClanId != 0;

    public bool Dissolving => DissolvingExpiryUnixMs > 0;

}
