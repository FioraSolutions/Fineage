/// <summary>
/// Party UI access to game services with [Inject] wiring and live fallback (#142).
/// </summary>
public static class PartyPacketServices
{
    private static PartyInvitationWindow _partyInvite;
    private static PartyMemberListWindow _memberList;

    public static void Bind(
        PartyInvitationWindow partyInvite = null,
        PartyMemberListWindow memberList = null)
    {
        if (partyInvite != null)
            _partyInvite = partyInvite;
        if (memberList != null)
            _memberList = memberList;
    }

    public static PartyInvitationWindow PartyInvite =>
        _partyInvite != null ? _partyInvite : PartyInvitationWindow.Instance;

    public static NetPartyMemberList LastMemberList { get; private set; }

    public static PartyMemberListWindow MemberList =>
        _memberList != null ? _memberList : PartyMemberListWindow.Instance;

    public static void ApplyMemberList(NetPartyMemberList list)
    {
        LastMemberList = list;
        PartyMemberListWindow panel = MemberList;
        if (panel != null)
            panel.RefreshFromList(list);
    }
}
