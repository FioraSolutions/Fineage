using UnityEngine;

[IncomingGamePacket(GameServerPacketType.EnchantResult)]
public sealed class EnchantResultIncoming : IncomingWirePacket<EnchantResultDto>
{
    public override void Apply(EnchantResultDto packet)
    {
        IncomingPacketServices.Queue(() => IncomingPacketServices.Enchant.EnchantResult(packet.Result));
    }
}
