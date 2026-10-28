[IncomingGamePacket(GameServerPacketType.MagicSkillLaunched)]
public sealed class MagicSkillLaunchedIncoming : IncomingWirePacket<MagicSkillLaunchedDto>
{
    public override void Apply(MagicSkillLaunchedDto packet) =>
        CombatIncomingPacketServices.ApplyMagicSkillLaunched(packet);
}
