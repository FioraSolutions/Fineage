using System;

/// <summary>
/// Party apply set (invitation, join result, roster), constructed by the container instead
/// of reached through static facades (#142 slice 3).
/// </summary>
public sealed class PartyApplyService
{
    public void ApplyAskJoinParty(AskJoinPartyDto packet) =>
        QueueApplyIncomingPacketServices.Queue(() =>
        {
            LocatorApplyIncomingPacketServices.PartyInvite.AddData(packet);
            LocatorApplyIncomingPacketServices.PartyInvite.ShowWindow();
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
            string summary = NetDomainMap.FormatPartyMemberList(list);
            UnityEngine.Debug.Log("[Party] " + summary);
            ChatTypeData data = ChatTypes.GetById(0);
            if (data != null)
            {
                CreatureMessage message = new CreatureMessage("Party", summary, data);
                ChatApplyIncomingPacketServices.ApplyMessage(apply => apply.CreatureSay(message));
            }
        });
}
