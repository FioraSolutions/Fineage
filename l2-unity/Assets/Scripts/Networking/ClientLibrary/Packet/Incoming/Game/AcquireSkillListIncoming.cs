
[IncomingGamePacket(GameServerPacketType.AcquireSkillList)]
public sealed class AcquireSkillListIncoming : IncomingWirePacket<AcquireSkillListDto>
{
    public override void Apply(AcquireSkillListDto packet) =>
        AcquireSkillListApplyIncomingPacketServices.ApplyAcquireSkillList(packet);
}
