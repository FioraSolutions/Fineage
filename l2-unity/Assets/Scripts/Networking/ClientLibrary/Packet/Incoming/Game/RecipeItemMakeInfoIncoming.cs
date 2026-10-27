using UnityEngine;

[IncomingGamePacket(GameServerPacketType.RecipeItemMakeInfo)]
public sealed class RecipeItemMakeInfoIncoming : IncomingWirePacket<RecipeItemMakeInfoDto>
{
    public override void Apply(RecipeItemMakeInfoDto packet)
    {
        IncomingPacketServices.Queue(() =>
        {
            IncomingPacketServices.RecipeBook.HideWindow();
            IncomingPacketServices.Crafting.AddData(packet);
            IncomingPacketServices.Crafting.ShowWindow();
        });
    }
}
