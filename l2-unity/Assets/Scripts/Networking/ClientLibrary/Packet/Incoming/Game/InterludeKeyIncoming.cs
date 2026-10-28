[IncomingGamePacket(GameServerPacketType.InterludeKeyPacket)]
public sealed class InterludeKeyIncoming : IncomingWirePacket<InterludeKeyDto>
{
    protected override void OnParsed(InterludeKeyDto packet) =>
        InterludeKeyParsedIncomingPacketServices.OnInterludeKeyParsed(packet);

    public override void Apply(InterludeKeyDto packet) =>
        InterludeKeyApplyIncomingPacketServices.ApplyInterludeKey(packet);
}
