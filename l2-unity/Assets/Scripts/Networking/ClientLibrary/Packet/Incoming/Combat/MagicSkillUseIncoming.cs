[IncomingGamePacket(GameServerPacketType.MagicSkillUse)]
public sealed class MagicSkillUseIncoming : IncomingWirePacket<MagicSkillUseDto>
{
    public override void Apply(MagicSkillUseDto packet) =>
        MagicSkillUseIncomingPacketServices.ApplyMagicSkillUse(packet);
}
