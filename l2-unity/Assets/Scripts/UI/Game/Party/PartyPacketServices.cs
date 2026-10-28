/// <summary>
/// Party UI access to game services with [Inject] wiring and IncomingPacketActions fallback (#142 sub-slice 15).
/// </summary>
public static class PartyPacketServices
{
    private static GameClient _game;
    private static PartyInvitationWindow _partyInvite;

    public static void Bind(GameClient game = null, PartyInvitationWindow partyInvite = null)
    {
        if (game != null)
            _game = game;
        if (partyInvite != null)
            _partyInvite = partyInvite;
    }

    public static GameClient Game =>
        _game != null ? _game : IncomingPacketActions.Game;

    public static PartyInvitationWindow PartyInvite =>
        _partyInvite != null ? _partyInvite : IncomingPacketActions.PartyInvite;
}
