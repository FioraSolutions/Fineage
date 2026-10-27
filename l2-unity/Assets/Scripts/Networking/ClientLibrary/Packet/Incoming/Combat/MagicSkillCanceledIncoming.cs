[IncomingGamePacket(GameServerPacketType.MagicSkillCanceled)]
public sealed class MagicSkillCanceledIncoming : IncomingWirePacket<MagicSkillCanceledDto>
{
    public override void Apply(MagicSkillCanceledDto packet)
    {
        IncomingPacketServices.ApplyWorld(apply => apply.MagicSkillCanceled(packet));
    }
}
