/// <summary>
/// Clan UI access to game services with [Inject] wiring and IncomingPacketActions fallback (#142 sub-slice 16).
/// Zone pledge snapshots land here so the clan window does not need a new locator member.
/// </summary>
public static class ClanPacketServices
{
    private static GameClient _game;
    private static ClanWindow _clan;
    private static ClanAskJoinWindow _ask;

    public static void Bind(GameClient game = null, ClanWindow clan = null, ClanAskJoinWindow ask = null)
    {
        if (game != null)
            _game = game;
        if (clan != null)
            _clan = clan;
        if (ask != null)
            _ask = ask;
    }

    public static GameClient Game =>
        _game != null ? _game : IncomingPacketActions.Game;

    public static ClanWindow Clan =>
        _clan != null ? _clan : IncomingPacketActions.Clan;

    public static ClanAskJoinWindow Ask =>
        _ask != null ? _ask : ClanAskJoinWindow.Instance;

    public static void ApplyStatus(ZoneV2PledgeStatus status)
    {
        ZoneV2ClanBook.Shared.ApplyStatus(status);
        NoteLocalClan(ZoneV2ClanBook.Shared.HasClan ? ZoneV2ClanBook.Shared.ClanId : 0);
        RefreshWindow();
    }

    public static void ApplyMembers(ZoneV2PledgeMemberList list)
    {
        ZoneV2ClanBook.Shared.ApplyMembers(list);
        NoteLocalClan(ZoneV2ClanBook.Shared.HasClan ? ZoneV2ClanBook.Shared.ClanId : 0);
        RefreshWindow();
    }

    public static void ApplyAskJoin(ZoneV2ClanAskJoin ask)
    {
        ZoneV2ClanBook.Shared.NoteName(ask.ClanId, ask.ClanName);
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
        GameClient game = Game;
        if (game != null && game.PlayerInfo.Identity != null)
            game.PlayerInfo.Identity.ClanId = clanId;
        PlayerEntity player = PlayerEntity.Instance;
        if (player != null && player.Identity != null)
            player.Identity.ClanId = clanId;
    }
}
