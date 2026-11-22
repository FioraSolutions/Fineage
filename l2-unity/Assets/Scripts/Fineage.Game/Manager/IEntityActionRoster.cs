/// <summary>
/// Action roster <c>World.UnregisterObject</c> clears when an actor leaves (#131 S38e).
/// </summary>
public interface IEntityActionRoster
{
    void Remove(IWorldEntity entity);
}
