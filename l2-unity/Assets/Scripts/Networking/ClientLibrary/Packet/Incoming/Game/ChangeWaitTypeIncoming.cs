[IncomingGamePacket(GameServerPacketType.ChangeWaitType)]
public sealed class ChangeWaitTypeIncoming : IncomingWirePacket<ChangeWaitTypeDto>
{
    public override void Apply(ChangeWaitTypeDto packet)
    {
        WorldIncomingPacketServices.ApplyWorld(apply => apply.ChangeWaitType(packet));
    }
}
