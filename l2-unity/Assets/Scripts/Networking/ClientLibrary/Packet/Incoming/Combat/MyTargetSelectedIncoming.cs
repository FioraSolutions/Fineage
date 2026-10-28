[IncomingGamePacket(GameServerPacketType.MyTargetSelected)]
public sealed class MyTargetSelectedIncoming : IncomingWirePacket<MyTargetSelectDto>
{
    public override void Apply(MyTargetSelectDto packet) =>
        MyTargetSelectedApplyIncomingPacketServices.ApplyMyTargetSelected(packet);
}
