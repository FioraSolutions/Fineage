
[IncomingGamePacket(GameServerPacketType.AcquireSkillList)]
public sealed class AcquireSkillListIncoming : IncomingWirePacket<AcquireSkillListDto>
{
    public override void Apply(AcquireSkillListDto packet) =>
        SkillLearnIncomingPacketServices.ApplyAcquireSkillList(packet);
}
