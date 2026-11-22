using UnityEngine;

/// <summary>
/// Ground height and spawn containers (#131 S35).
/// Register and animation members stay on <c>IWorldSpawnRegistration</c> in Assembly-CSharp.
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
