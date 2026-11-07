using VContainer;

/// <summary>
/// Party apply set (invitation, join result, roster), constructed by the container instead
/// of reached through static facades (#142 slice 3). <see cref="PartyInvitationWindow"/>
/// arrives through <see cref="ILateBound{T}"/> (#142 slice 4).
/// </summary>
public sealed class PartyApplyService
{
    private readonly ILateBound<PartyInvitationWindow> _partyInvite;

    [Inject]
    public PartyApplyService(ILateBound<PartyInvitationWindow> partyInvite)
    {
        _partyInvite = partyInvite;
    }

    public void ApplyAskJoinParty(AskJoinPartyDto packet) =>
        QueueApplyIncomingPacketServices.Queue(() =>
        {
            PartyInvitationWindow invite = _partyInvite != null ? _partyInvite.Value : null;
            if (invite == null)
                return;

            invite.AddData(packet);
            invite.ShowWindow();
        });

    public void ApplyPartyJoinResult(int response)
    {
        string text = response == 1
            ? "Your party invitation was accepted."
            : "Your party invitation was declined.";
        UnityEngine.Debug.Log("[Party] " + text);
        ChatTypeData data = ChatTypes.GetById(0);
        if (data == null)
            return;
        CreatureMessage message = new CreatureMessage("Party", text, data);
        ChatApplyIncomingPacketServices.ApplyMessage(apply => apply.CreatureSay(message));
    }

    public void ApplyPartyMemberList(NetPartyMemberList list) =>
        QueueApplyIncomingPacketServices.Queue(() =>
        {
            PartyPacketServices.ApplyMemberList(list);
            string summary = NetSocialMap.FormatPartyMemberList(list);
            UnityEngine.Debug.Log("[Party] " + summary);
            ChatTypeData data = ChatTypes.GetById(0);
            if (data != null)
            {
                CreatureMessage message = new CreatureMessage("Party", summary, data);
                ChatApplyIncomingPacketServices.ApplyMessage(apply => apply.CreatureSay(message));
            }
        });
}
