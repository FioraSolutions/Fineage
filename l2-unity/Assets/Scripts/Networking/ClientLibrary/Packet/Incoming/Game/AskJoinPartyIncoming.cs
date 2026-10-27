using UnityEngine;

[IncomingGamePacket(GameServerPacketType.AskJoinParty)]
public sealed class AskJoinPartyIncoming : IncomingWirePacket<AskJoinPartyDto>
{
    public override void Apply(AskJoinPartyDto packet)
    {
        UiIncomingPacketServices.Queue(() =>
        {
            UiIncomingPacketServices.PartyInvite.AddData(packet);
            UiIncomingPacketServices.PartyInvite.ShowWindow();
        });
    }
}
