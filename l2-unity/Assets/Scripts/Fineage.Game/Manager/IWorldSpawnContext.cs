using UnityEngine;

/// <summary>
/// Ground height and spawn containers (#131 S35).
/// Register members live on <c>IWorldSpawnRegistration</c> in Fineage.Game.
/// </summary>
public interface IWorldSpawnContext
{
    float GetGroundHeight(Vector3 pos);
    bool OfflineMode { get; }
    Transform UsersContainer { get; }
    Transform NpcsContainer { get; }
    Transform MonstersContainer { get; }
    Transform ItemsContainer { get; }
    bool ContainsNpc(int id);
}
