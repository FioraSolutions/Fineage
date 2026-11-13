using System;
using System.Collections.Generic;
using UnityEngine;

public sealed partial class WorldPacketApply
{
    private Entity GetEntity(int objId)
    {
        return GameWorld == null ? null : GameWorld.GetEntityNoLockSync(objId);
    }

    /// <summary>
    /// Own cast can arrive before the pawn is in the lookup. A missing mob id must not
    /// become the local player — that plays their berserk on you.
    /// </summary>
    private static Entity LocalCasterOrNull(int objId)
    {
        PlayerEntity player = DomainPacketServices.LocalPlayer;
        if (player == null || player.Identity == null || player.Identity.Id != objId)
            return null;
        return player;
    }

    private bool TryResolve(Entity entity, out EntityWorldApply apply)
    {
        Type type = entity.GetType();
        while (type != null && type != typeof(Entity) && type != typeof(object))
        {
            if (_byType.TryGetValue(type, out apply))
                return true;
            type = type.BaseType;
        }

        apply = null;
        return false;
    }

    private bool IsWorldSpawnReady()
    {
        GameManager manager = _gameManager != null ? _gameManager : WorldApplyPacketServices.Manager;
        return manager != null && manager.WorldSpawnReady && GameWorld != null;
    }

    private void QueueWorldNextFrame(System.Action<WorldPacketApply> apply)
    {
        if (apply == null)
            return;
        if (_applyQueue != null)
            _applyQueue.QueueNextFrame(() => apply(this));
        else
            WorldApplyPacketServices.QueueWorldNextFrame(apply);
    }
}
