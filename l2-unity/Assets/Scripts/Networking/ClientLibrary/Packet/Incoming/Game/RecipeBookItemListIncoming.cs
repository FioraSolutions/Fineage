using UnityEngine;

[IncomingGamePacket(GameServerPacketType.RecipeBookItemList)]
public sealed class RecipeBookItemListIncoming : IncomingWirePacket<RecipeBookItemListDto>
{
    public override void Apply(RecipeBookItemListDto packet)
    {
        IncomingPacketServices.Queue(() =>
        {
            IncomingPacketServices.RecipeBook.AddData(packet);
            IncomingPacketServices.RecipeBook.ShowWindow();
        });
    }
}
