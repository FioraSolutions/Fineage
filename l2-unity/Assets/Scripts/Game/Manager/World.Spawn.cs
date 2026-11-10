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

    public void PickupGroundItem(int itemObjectId, int pickerCharObjId, Vector3 unityPos)
    {
        if (_itemDropPresentation != null)
            _itemDropPresentation.PlayPickup(itemObjectId, pickerCharObjId, unityPos);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
        // DropMeshLifetimeLog.NotifyRemove(itemObjectId, "GetItem_pickup");
#endif
        RemoveObject(itemObjectId);

        // Let authored PicItem finish before forcing gait (do not stretch the clip).
        if (pickerCharObjId != 0 && MoveAllCharactersPacketServices.Moves != null &&
            MoveAllCharactersPacketServices.Moves.IsMoving(pickerCharObjId))
        {
            StartCoroutine(ResumeGaitAfterPickup(pickerCharObjId));
        }
    }

    System.Collections.IEnumerator ResumeGaitAfterPickup(int pickerCharObjId)
    {
        yield return new WaitForSeconds(AnimPlayback.PickupFallbackSeconds);
        if (MoveAllCharactersPacketServices.Moves == null ||
            !MoveAllCharactersPacketServices.Moves.IsMoving(pickerCharObjId))
            yield break;

        MovementData move = MoveAllCharactersPacketServices.Moves.GetMove(pickerCharObjId);
        if (move == null || move.GetEntity() == null)
            yield break;

        move.ForceReplayUserGait();
    }

    public void SpawnPlayer(EntityIdentity identity, PlayerStatus status, PlayerStats stats, PlayerAppearance appearance)
    {
        if (_playerSpawner != null)
            _playerSpawner.Spawn(identity, status, stats, appearance, this);
    }

    public void SpawnNpc(EntityIdentity identity, NpcStatusInterlude status, Stats stats)
    {
        SpawnNpc(identity, status, stats, out _);
    }

    /// <summary>
    /// Spawns an NPC and reports whether the failure was permanent. A missing npcgrp mesh or an
    /// absent prefab can never succeed on retry, while a spawner that is not wired yet can, so the
    /// caller needs to tell those apart before dropping a knownlist row.
    /// </summary>
    public void SpawnNpc(
        EntityIdentity identity,
        NpcStatusInterlude status,
        Stats stats,
        out bool unresolvable)
    {
        unresolvable = false;
        if (identity == null || ContainsNpc(identity.Id))
            return;

        if (identity.NpcId == 20481)
            Debug.Log(" object NpcInfo 5 " + identity.Id);

        NpcSpawnRequest request;
        if (!EntitySpawnShared.TryResolveNpc(identity, _npcGrps, _npcNames, _models, out request))
        {
            unresolvable = true;
            return;
        }

        request.Status = status;
        request.Stats = stats;
        // Debug.Log("Name NPC " + request.NpcName.Name);

        if (identity.EntityType == EntityType.NPC)
        {
            if (_npcSpawner != null)
                _npcSpawner.Spawn(request, this);
        }
        else if (_monsterSpawner != null)
        {
            _monsterSpawner.Spawn(request, this);
        }
    }

    public void UpdateNpc(Entity entity , NpcInfoDto npcInfo)
    {
        if (_monsterSpawner != null)
            _monsterSpawner.UpdateInfo(entity, npcInfo);
    }

    public void UpdateUserInfo(Entity entity, UserInfoDto userInfo)
    {
        if (_playerSpawner != null)
            _playerSpawner.UpdateInfo(entity, userInfo);
    }

    public async Task DeleteObject(int objectId)
    {
        Entity entity = await GetEntityNoLock(objectId);
        if (entity == null)
        {
            // Debug.LogWarning(
                // $"[DeleteObject] HANDLER id={objectId} → entity NOT in World._objects " +
                // "(packet arrived but client has no entity — GO may be orphaned in Hierarchy)");
            return;
        }

        string typeName = entity.GetType().Name;
        // Debug.Log(
            // $"[DeleteObject] HANDLER id={objectId} found type={typeName} name={entity.name} " +
            // $"activeInHierarchy={entity.gameObject.activeInHierarchy}");

        if (entity.GetType() == typeof(MonsterEntity))
        {
            if (entity.IsDead())
            {
                if (_deadManager != null)
                    _deadManager.AddDeadAndRemove(objectId, new DeadData(entity));
                // Debug.Log($"[DeleteObject] HANDLER id={objectId} Monster DEAD → DeadManager");
            }
            else
            {
                RemoveObject(objectId);
                // Debug.Log($"[DeleteObject] HANDLER id={objectId} Monster → RemoveObject");
            }
        }
        else if (entity is ItemEntity)
        {
            if (_itemDropPresentation != null)
                _itemDropPresentation.StopItemPresentation(objectId);
#if UNITY_EDITOR || DEVELOPMENT_BUILD
            // DropMeshLifetimeLog.NotifyRemove(objectId, "DeleteObject_packet");
#endif
            RemoveObject(objectId);
            // Debug.Log($"[DeleteObject] HANDLER id={objectId} ItemEntity → RemoveObject");
        }
        else if (entity is NpcEntity)
        {
            if (entity.IsDead() && _deadManager != null)
            {
                _deadManager.AddDeadAndRemove(objectId, new DeadData(entity));
                // Debug.Log($"[DeleteObject] HANDLER id={objectId} NpcEntity DEAD → DeadManager");
            }
            else
            {
                BeginActorDisappear(objectId, entity, FinishDisappearedNpc);
                // Debug.Log($"[DeleteObject] HANDLER id={objectId} NpcEntity → disappear fade");
            }
        }
        else if (entity is UserEntity)
        {
            Vector3 l2 = VectorUtils.ConvertPosUnityToL2j(entity.transform.position);
            string nick = entity.Identity != null ? entity.Identity.Name : entity.name;
            // Debug.Log("[MOVE_SNAP] DESPAWN nick=" + nick +
                // " l2=" + l2.x.ToString("F0") + "," + l2.y.ToString("F0") + "," + l2.z.ToString("F0"));
            BeginActorDisappear(objectId, entity, DestroyDisappearedUser);
            // Debug.Log($"[DeleteObject] HANDLER id={objectId} UserEntity → disappear fade");
        }
        else
        {
            // Same as historical catch-all: anything else still must leave the world.
            // Debug.LogWarning(
                // $"[DeleteObject] HANDLER id={objectId} unexpected type={typeName} → RemoveObject");
            RemoveObject(objectId);
        }
    }
}
