/// <summary>
/// Spawn registration still names Assembly-CSharp entity and animation types (#131 S35).
/// <see cref="World"/> implements this. It extends <see cref="IWorldSpawnContext"/>.
/// </summary>
public interface IWorldSpawnRegistration : IWorldSpawnContext
{
    IAnimationManager Animations { get; }
    void RegisterPlayer(PlayerEntity player);
    void RegisterUser(Entity user);
    void RegisterNpc(Entity npc);
    void RegisterItem(ItemEntity item);
}
