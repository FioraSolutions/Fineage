public static partial class ClanPacketServices
{
    public static void Bind(ClanWindow clan = null, ClanAskJoinWindow ask = null)
    {
        if (clan != null)
            _clan = clan;
        if (ask != null)
            _ask = ask;
    }

    public static void ApplyStatus(NetPledgeStatus status)
    {
        NetClanBook.Shared.ApplyStatus(status);
        NoteLocalClan(NetClanBook.Shared.HasClan ? NetClanBook.Shared.ClanId : 0);
        RefreshWindow();
    }

    public static void ApplyMembers(NetPledgeMemberList list)
    {
        NetClanBook.Shared.ApplyMembers(list);
        NoteLocalClan(NetClanBook.Shared.HasClan ? NetClanBook.Shared.ClanId : 0);
        RefreshWindow();
    }
}
