using UnityEngine;

/// <summary>
/// Position, identity, and transform reads <c>World</c> makes on a spawned actor (#131 S37).
/// <c>Entity</c> stays Assembly-CSharp and implements this.
/// </summary>
public interface IWorldEntity
{
    EntityIdentity Identity { get; }
    Transform Transform { get; }
    Vector3 Position { get; }
}
