/// <summary>
/// Clan UI access to game services with [Inject] wiring and live fallback (#142).
/// Zone pledge snapshots land here so the clan window does not need a new locator member.
/// </summary>
public static class ClanPacketServices
{
    private static ClanWindow _clan;
    private static ClanAskJoinWindow _ask;

    public static void Bind(ClanWindow clan = null, ClanAskJoinWindow ask = null)
    {
        if (clan != null)
            _clan = clan;
        if (ask != null)
            _ask = ask;
    }

    public static ClanWindow Clan =>
        _clan != null ? _clan : ClanWindow.Instance;

    public static ClanAskJoinWindow Ask =>
        _ask != null ? _ask : ClanAskJoinWindow.Instance;

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

    /// <summary>
    /// Mobius <c>JoinPledge</c>. The window is only refreshed, never forced open: an accept
    /// arrives while the player is already looking at the invite popup, and a decline just
    /// needs to stop the wait.
    /// </summary>
    public static void ApplyJoinResult(int response)
    {
        if (response == 0)
        {
            NetChatFeed.NoteSystem("Your request to join the clan was declined.");
            return;
        }

        NetChatFeed.NoteSystem("You have joined the clan.");
        RefreshWindow();
    }

    public static void ApplyAskJoin(NetClanAskJoin ask)
    {
        NetClanBook.Shared.NoteName(ask.ClanId, ask.ClanName);
        ClanWindow clan = Clan;
        if (clan != null)
            clan.EnsureAskJoinWindow();
        ClanAskJoinWindow window = Ask;
        if (window != null)
            window.Present(ask);
    }

    public static void RefreshWindow()
    {
        ClanWindow clan = Clan;
        if (clan != null)
            clan.ApplyZoneBook();
    }

    static void NoteLocalClan(int clanId)
    {
        IZoneTransport transport = ZoneCommands.Transport;
        if (transport != null && transport.PlayerInfo.Identity != null)
            transport.PlayerInfo.Identity.ClanId = clanId;
        PlayerEntity player = DomainPacketServices.LocalPlayer;
        if (player != null && player.Identity != null)
            player.Identity.ClanId = clanId;
    }
}
