[IncomingGamePacket(GameServerPacketType.ChooseInventoryItem)]
public sealed class ChooseInventoryItemIncoming : IncomingWirePacket<ChooseInventoryItemDto>
{
    public override void Apply(ChooseInventoryItemDto packet) =>
        CombatIncomingPacketServices.ApplyChooseInventoryItem(packet);
}
