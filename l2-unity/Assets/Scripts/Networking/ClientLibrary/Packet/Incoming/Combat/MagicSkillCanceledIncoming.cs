[IncomingGamePacket(GameServerPacketType.MagicSkillCanceled)]
public sealed class MagicSkillCanceledIncoming : IncomingWirePacket<MagicSkillCanceledDto>
{
    public override void Apply(MagicSkillCanceledDto packet) =>
        MagicSkillCanceledIncomingPacketServices.ApplyMagicSkillCanceled(packet);
}
