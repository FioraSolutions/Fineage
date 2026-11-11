public static partial class ClanPacketServices
{
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
