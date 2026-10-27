using UnityEngine;

[IncomingGamePacket(GameServerPacketType.RecipeItemMakeInfo)]
public sealed class RecipeItemMakeInfoIncoming : IncomingWirePacket<RecipeItemMakeInfoDto>
{
    public override void Apply(RecipeItemMakeInfoDto packet)
    {
        UiIncomingPacketServices.Queue(() =>
        {
            UiIncomingPacketServices.RecipeBook.HideWindow();
            UiIncomingPacketServices.Crafting.AddData(packet);
            UiIncomingPacketServices.Crafting.ShowWindow();
        });
    }
}
