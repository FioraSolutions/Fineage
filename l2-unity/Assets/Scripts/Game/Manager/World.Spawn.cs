using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using VContainer;

public partial class World
{
    public void RegisterPlayer(PlayerEntity player)
    {
        if (player == null || player.Identity == null)
            return;

        int id = player.Identity.Id;
        _players[id] = player;
        _objects[id] = player;
    }

    public void RegisterUser(Entity user)
    {
        if (user == null || user.Identity == null)
            return;

        int id = user.Identity.Id;
        if (_objects.ContainsKey(id))
            return;

        _players[id] = user;
        _objects[id] = user;
    }

    public void SpawnUser(CharInfoDto info)
    {
        if (info == null || info.Identity == null || _userSpawner == null)
        {
            // GearFlowLog.Warn("SpawnUser abort info/spawner null");
            return;
        }

        int id = info.Identity.Id;
        if (_objects.ContainsKey(id))
        {
            // GearFlowLog.Info("SpawnUser SKIP already in world id=" + id +
                // " type=" + _objects[id].GetType().Name);
            return;
        }

        PlayerEntity local = DomainPacketServices.LocalPlayer;
        if (local != null && local.Identity != null && local.Identity.Id == id)
        {
            // GearFlowLog.Info("SpawnUser SKIP local PlayerEntity id=" + id);
            return;
        }

        // GearFlowLog.Info("SpawnUser CREATE UserEntity id=" + id +
            // " nick=" + info.Identity.Name + " " + GearFlowLog.Paperdoll(info.Appearance));
        _userSpawner.Spawn(info, this);
    }

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
