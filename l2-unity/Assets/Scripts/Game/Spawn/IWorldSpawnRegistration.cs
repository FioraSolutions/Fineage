/// <summary>
/// Spawn registration (#131 S35, S35c). Register members take <see cref="IWorldEntity"/>.
/// <see cref="Animations"/> still names Assembly-CSharp <see cref="IAnimationManager"/>.
/// <see cref="World"/> implements this. It extends <see cref="IWorldSpawnContext"/>.
/// </summary>
public interface IWorldSpawnRegistration : IWorldSpawnContext
{
    IAnimationManager Animations { get; }
    void RegisterPlayer(IWorldEntity player);
    void RegisterUser(IWorldEntity user);
    void RegisterNpc(IWorldEntity npc);
    void RegisterItem(IWorldEntity item);
}
