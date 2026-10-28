[IncomingGamePacket(GameServerPacketType.ItemList)]
public sealed class ItemListIncoming : IncomingWirePacket<ItemListDto>
{
    public override void Apply(ItemListDto packet) =>
        ItemListIncomingPacketServices.ApplyItemList(packet);
}
