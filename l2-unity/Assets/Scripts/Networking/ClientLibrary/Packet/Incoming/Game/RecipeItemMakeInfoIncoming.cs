
[IncomingGamePacket(GameServerPacketType.RecipeItemMakeInfo)]
public sealed class RecipeItemMakeInfoIncoming : IncomingWirePacket<RecipeItemMakeInfoDto>
{
    public override void Apply(RecipeItemMakeInfoDto packet) =>
        RecipeItemMakeInfoIncomingPacketServices.ApplyRecipeItemMakeInfo(packet);
}
