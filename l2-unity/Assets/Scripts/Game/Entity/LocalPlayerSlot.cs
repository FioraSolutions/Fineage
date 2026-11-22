using System;
using UnityEngine;

/// <summary>
/// Holds the spawned local <see cref="PlayerEntity"/>. Empty until <c>PlayerEntity</c> attaches.
/// </summary>
public sealed class LocalPlayerSlot : ILocalPlayerSlot
{
    PlayerEntity _player;

    public void Attach(IWorldEntity player)
    {
        if (player is PlayerEntity entity)
        {
            _player = entity;
            return;
        }

        throw new InvalidOperationException(
            "[LocalPlayerSlot] ILocalPlayerSlot attach is not a PlayerEntity — PlayerEntity.Awake passed a different type, so the local player was not published.");
    }

    public bool IsLocalId(int objectId)
    {
        return _player != null && _player.Identity != null && _player.Identity.Id == objectId;
    }

    public bool IsLocal(IWorldEntity entity)
    {
        return _player != null && entity is PlayerEntity player && _player == player;
    }

    public void SetRunning(bool running)
    {
        if (_player == null)
        {
            throw new InvalidOperationException(
                "[LocalPlayerSlot] ILocalPlayerSlot missing — PlayerEntity.Awake did not attach, so running was not set.");
        }

        _player.Running = running;
    }

    public bool TryGetPosition(out Vector3 position)
    {
        if (_player == null)
        {
            position = default;
            return false;
        }

        position = _player.transform.position;
        return true;
    }
}
