using UnityEngine;

[IncomingGamePacket(GameServerPacketType.RecipeBookItemList)]
public sealed class RecipeBookItemListIncoming : IncomingWirePacket<RecipeBookItemListDto>
{
    public override void Apply(RecipeBookItemListDto packet)
    {
        UiIncomingPacketServices.Queue(() =>
        {
            UiIncomingPacketServices.RecipeBook.AddData(packet);
            UiIncomingPacketServices.RecipeBook.ShowWindow();
        });
    }
}
