public sealed class DwarvenManufactureAction : L2Action
{
    public override void UseAction()
    {
        if (PlayerStatePacketServices.PlayerState != null &&
            PlayerStatePacketServices.PlayerState.State == PlayerState.DEAD)
            return;

        RecipeShopManageWindow manage = ManufacturePacketServices.Manage;
        if (manage == null)
        {
            FineageLog.Error(FineageLogCategory.GameNet, "DwarvenManufacture: RecipeShopManageWindow is not bound.");
            return;
        }

        manage.OpenForDwarvenManufacture();
    }
}