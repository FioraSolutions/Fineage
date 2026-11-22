using UnityEngine;

/// <summary>
/// Local player <c>World</c> reads after spawn (#131 S38r, S38s, S38t).
/// <c>GameLifetimeScope</c> registers <c>LocalPlayerSlot</c> as this interface.
/// <c>PlayerEntity</c> attaches itself when it becomes the live instance.
/// </summary>
public interface ILocalPlayerSlot
{
    void Attach(IWorldEntity player);

    bool IsLocalId(int objectId);

    bool IsLocal(IWorldEntity entity);

    void SetRunning(bool running);

    bool TryGetPosition(out Vector3 position);

    bool TryGetName(out string name);
}
