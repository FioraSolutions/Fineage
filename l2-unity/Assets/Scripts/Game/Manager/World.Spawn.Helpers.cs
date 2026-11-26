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

    public void RegisterNpc(IWorldEntity npc)
    {
        Entity entity = npc as Entity;
        if (entity == null)
        {
            if (npc == null || npc is Entity)
                return;

            throw new InvalidOperationException(
                "[World] IWorldSpawnRegistration.RegisterNpc is not an Entity — the spawner passed a different type, so the npc was not registered.");
        }

        if (entity.Identity == null)
            return;

        int id = entity.Identity.Id;
        _entities.RegisterNpc(id, entity);
        _entityWait.Notify(id, entity);
        if (entity is MonsterEntity or NpcEntity)
        {
            L2ActorPointLights.TryRegisterActorRoot(entity.transform);
            _visibility.Register(id, entity.transform);
        }
    }

    public void RegisterItem(IWorldEntity item)
    {
        ItemEntity entity = item as ItemEntity;
        if (entity == null)
        {
            if (item == null || item is ItemEntity)
                return;

            throw new InvalidOperationException(
                "[World] IWorldSpawnRegistration.RegisterItem is not an ItemEntity — the spawner passed a different type, so the item was not registered.");
        }

        if (entity.Identity == null)
            return;

        int id = entity.Identity.Id;
        _entities.RegisterItem(id, entity);
        _entityWait.Notify(id, entity);
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
