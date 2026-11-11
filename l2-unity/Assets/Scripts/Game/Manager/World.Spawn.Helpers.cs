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
        _npcs.Add(id, npc);
        _objects.Add(id, npc);
    }

    public void RegisterItem(ItemEntity item)
    {
        if (item == null || item.Identity == null)
            return;

        _objects[item.Identity.Id] = item;
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
            float groundY = GetGroundHeight(unityPos);
            unityPos.y = groundY;
            item.UpdateGround(unityPos, count, stackable);
            return;
        }

        if (existing != null)
            RemoveObject(objectId);

        if (_itemSpawner == null)
        {
            // Debug.LogWarning("[World] ItemSpawner missing — cannot spawn ground item " + objectId);
            return;
        }

        _itemSpawner.Spawn(objectId, itemId, count, stackable, unityPos, charObjId, this);
    }
}
