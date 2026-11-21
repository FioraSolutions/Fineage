/// <summary>Interlude action 51 — common-recipe manufacture shop.</summary>
public sealed class GeneralManufactureAction : L2Action
{
    public override void UseAction()
    {
        if (PlayerStatePacketServices.PlayerState != null &&
            PlayerStatePacketServices.PlayerState.State == PlayerState.DEAD)
            return;

        RecipeShopManageWindow manage = ManufacturePacketServices.Manage;
        if (manage == null)
        {
            FineageLog.Error(FineageLogCategory.GameNet, "GeneralManufacture: RecipeShopManageWindow is not bound.");
            return;
        }

        manage.OpenForCommonManufacture();
    }
}