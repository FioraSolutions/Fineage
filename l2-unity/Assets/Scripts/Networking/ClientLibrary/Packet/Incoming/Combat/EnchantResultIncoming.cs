[IncomingGamePacket(GameServerPacketType.EnchantResult)]
public sealed class EnchantResultIncoming : IncomingWirePacket<EnchantResultDto>
{
    public override void Apply(EnchantResultDto packet)
    {
        CombatIncomingPacketServices.ApplyEnchantResult(packet);
    }
}
