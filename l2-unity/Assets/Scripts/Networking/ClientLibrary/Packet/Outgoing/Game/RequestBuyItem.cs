using System.Collections.Generic;

[OutgoingCommandPacket(typeof(RequestBuyItemCommand))]
public sealed class RequestBuyItem : OutgoingWirePacket<RequestBuyItemDto>
{
    protected override byte Opcode => (byte)GameClientPacketType.RequestBuyItem;
    protected override OutgoingBuildKind BuildKind => OutgoingBuildKind.GameNoPad;

    public RequestBuyItem(RequestBuyItemCommand command)
        : this(NetNpcSession.ShopNpcObjectId, command.ListId, command.BuyList)
    {
    }

    public RequestBuyItem(int npcObjectId, int listId, List<Product> buyList)
    {
        Dto.NpcObjectId = npcObjectId;
        Dto.ListId = listId;
        Dto.Items = buyList;
    }
}
