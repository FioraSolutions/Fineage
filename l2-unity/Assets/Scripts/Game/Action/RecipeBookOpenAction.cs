/// <summary>Opens dwarven (1) or common (0) recipe book via Net RecipeBookOpen.</summary>
public sealed class RecipeBookOpenAction : L2Action
{
    readonly int _dwarven;

    public RecipeBookOpenAction(int dwarven) => _dwarven = dwarven;

    public override void UseAction()
    {
        if (PlayerStatePacketServices.PlayerState != null &&
            PlayerStatePacketServices.PlayerState.State == PlayerState.DEAD)
            return;

        ZoneCommands.Send(new RequestRecipeBookOpenCommand(_dwarven));
    }
}