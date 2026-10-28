/// <summary>
/// Party UI access to game services with [Inject] wiring and IncomingPacketActions fallback (#142 sub-slice 15).
/// </summary>
public static class PartyPacketServices
{
    private static GameClient _game;
    private static PartyInvitationWindow _partyInvite;
    private static PartyMemberListWindow _memberList;

    public static void Bind(
        GameClient game = null,
        PartyInvitationWindow partyInvite = null,
        PartyMemberListWindow memberList = null)
    {
        if (game != null)
            _game = game;
        if (partyInvite != null)
            _partyInvite = partyInvite;
        if (memberList != null)
            _memberList = memberList;
    }

    public static GameClient Game =>
        _game != null ? _game : IncomingPacketActions.Game;

    public static PartyInvitationWindow PartyInvite =>
        _partyInvite != null ? _partyInvite : IncomingPacketActions.PartyInvite;

    public static ZoneV2PartyMemberList LastMemberList { get; private set; }

    public static PartyMemberListWindow MemberList =>
        _memberList != null ? _memberList : PartyMemberListWindow.Instance;

    public static void ApplyMemberList(ZoneV2PartyMemberList list)
    {
        LastMemberList = list;
        PartyMemberListWindow panel = MemberList;
        if (panel != null)
            panel.RefreshFromList(list);
    }
}
