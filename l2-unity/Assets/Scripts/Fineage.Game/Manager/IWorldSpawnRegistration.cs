/// <summary>
/// Spawn registration (#131 S35, S35b, S35c). Lives in Fineage.Game.
/// <see cref="World"/> implements this. It extends <see cref="IWorldSpawnContext"/>.
/// </summary>
public interface IWorldSpawnRegistration : IWorldSpawnContext
{
    void RegisterSpawnAnimation(int objectId, ISpawnAnimator animator, IWorldEntity entity);
    void RegisterPlayer(IWorldEntity player);
    void RegisterUser(IWorldEntity user);
    void RegisterNpc(IWorldEntity npc);
    void RegisterItem(IWorldEntity item);
}
