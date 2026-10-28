
[IncomingGamePacket(GameServerPacketType.AskJoinParty)]
public sealed class AskJoinPartyIncoming : IncomingWirePacket<AskJoinPartyDto>
{
    public override void Apply(AskJoinPartyDto packet) => PartyIncomingPacketServices.ApplyAskJoinParty(packet);
}
