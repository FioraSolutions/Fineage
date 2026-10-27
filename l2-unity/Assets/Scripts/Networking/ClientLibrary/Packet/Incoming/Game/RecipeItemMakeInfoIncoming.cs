using UnityEngine;

[IncomingGamePacket(GameServerPacketType.RecipeItemMakeInfo)]
public sealed class RecipeItemMakeInfoIncoming : IncomingWirePacket<RecipeItemMakeInfoDto>
{
    public override void Apply(RecipeItemMakeInfoDto packet) =>
        CraftIncomingPacketServices.ApplyRecipeItemMakeInfo(packet);
}
