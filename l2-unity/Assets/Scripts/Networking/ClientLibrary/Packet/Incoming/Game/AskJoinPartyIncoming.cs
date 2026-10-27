using UnityEngine;

[IncomingGamePacket(GameServerPacketType.AskJoinParty)]
public sealed class AskJoinPartyIncoming : IncomingWirePacket<AskJoinPartyDto>
{
    public override void Apply(AskJoinPartyDto packet)
    {
        IncomingPacketServices.Queue(() =>
        {
            IncomingPacketServices.PartyInvite.AddData(packet);
            IncomingPacketServices.PartyInvite.ShowWindow();
        });
    }
}
