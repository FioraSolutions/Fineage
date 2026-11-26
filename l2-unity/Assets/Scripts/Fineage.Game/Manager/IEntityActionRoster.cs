/// <summary>
/// Action roster <c>World</c> injects and clears when an actor leaves (#131 S38e, S38k).
/// <c>GameLifetimeScope</c> registers <c>EntityActionMachine</c> as this interface.
/// </summary>
public interface IEntityActionRoster
{
    void Remove(IWorldEntity entity);
}
