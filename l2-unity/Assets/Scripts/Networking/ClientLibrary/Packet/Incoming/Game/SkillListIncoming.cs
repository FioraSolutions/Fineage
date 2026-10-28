
[IncomingGamePacket(GameServerPacketType.SkillList)]
public sealed class SkillListIncoming : IncomingWirePacket<SkillListDto>
{
    public override void Apply(SkillListDto packet) =>
        SkillListApplyIncomingPacketServices.ApplySkillList(packet);
}
