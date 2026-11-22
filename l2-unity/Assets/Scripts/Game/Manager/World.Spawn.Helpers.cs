using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;
using System.Security.Principal;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements.Experimental;

public partial class World
{
    public void UpdateUser(Entity entity, CharInfoDto info)
    {
        if (_userSpawner != null)
            _userSpawner.UpdateInfo(entity, info);
    }

    public void RegisterNpc(Entity npc)
    {
        if (npc == null || npc.Identity == null)
            return;

        int id = npc.Identity.Id;
        _entities.RegisterNpc(id, npc);
        _entityWait.Notify(id, npc);
        if (npc is MonsterEntity or NpcEntity)
        {
            L2ActorPointLights.TryRegisterActorRoot(npc.transform);
            _visibility.Register(id, npc.transform);
        }
    }

    public void RegisterItem(ItemEntity item)
    {
        if (item == null || item.Identity == null)
            return;

        int id = item.Identity.Id;
        _entities.RegisterItem(id, item);
        _entityWait.Notify(id, item);
    }

    public void SpawnOrUpdateGroundItem(
        int objectId,
        int itemId,
        int count,
        bool stackable,
        Vector3 unityPos,
        int charObjId)
    {
        if (!IsWorldSpawnReady())
            return;

        Entity existing = GetEntityNoLockSync(objectId);
        if (existing is ItemEntity item)
        {
            var failCtx = new WorldPlacement.PlacementFailContext(objectId, itemId);
            Vector3 grounded = WorldPlacement.Resolve(
                WorldPlacement.PlacementKind.ItemDrop,
                unityPos,
                _groundMask,
                failCtx);
            item.Identity?.SetServerPosition(unityPos);
            item.UpdateGround(grounded, count, stackable);
            return;
        }

        if (existing != null)
            RemoveObject(objectId);

        if (_itemSpawner == null)
        {
            Debug.LogError("[World] ItemSpawner missing — cannot spawn ground item objectId=" + objectId);
            return;
        }

        _itemSpawner.Spawn(objectId, itemId, count, stackable, unityPos, charObjId, this);
    }
}
