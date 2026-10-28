
[IncomingGamePacket(GameServerPacketType.RecipeBookItemList)]
public sealed class RecipeBookItemListIncoming : IncomingWirePacket<RecipeBookItemListDto>
{
    public override void Apply(RecipeBookItemListDto packet) =>
        RecipeBookItemListApplyIncomingPacketServices.ApplyRecipeBookItemList(packet);
}
