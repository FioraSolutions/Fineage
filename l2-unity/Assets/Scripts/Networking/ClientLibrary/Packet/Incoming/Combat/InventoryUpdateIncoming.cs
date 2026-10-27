[IncomingGamePacket(GameServerPacketType.InventoryUpdate)]
public sealed class InventoryUpdateIncoming : IncomingWirePacket<InventoryUpdateDto>
{
    public override void Apply(InventoryUpdateDto packet)
    {
        CombatIncomingPacketServices.ApplyInventoryUpdate(packet);
    }
}
