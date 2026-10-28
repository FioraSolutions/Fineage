
[IncomingGamePacket(GameServerPacketType.AcquireSkillInfo)]
public sealed class AcquireSkillInfoIncoming : IncomingWirePacket<AcquireSkillInfoDto>
{
    public override void Apply(AcquireSkillInfoDto packet) =>
        AcquireSkillInfoIncomingPacketServices.ApplyAcquireSkillInfo(packet);
}
